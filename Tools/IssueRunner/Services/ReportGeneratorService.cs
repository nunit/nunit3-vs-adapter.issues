using IssueRunner.Models;
using Microsoft.Extensions.Logging;
using System.Text;

namespace IssueRunner.Services;

/// <summary>
/// Service for generating test reports.
/// </summary>
public sealed class ReportGeneratorService
{
    private readonly ILogger<ReportGeneratorService> _logger;
    private readonly IEnvironmentService _environmentService;
    private readonly RepositoryConfig repositoryConfig;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReportGeneratorService"/> class.
    /// </summary>
    public ReportGeneratorService(ILogger<ReportGeneratorService> logger, IEnvironmentService environmentService)
    {
        _logger = logger;
        _environmentService = environmentService;
        repositoryConfig = _environmentService.RepositoryConfig;
    }

    private string IssueLink(int number) => $"[#{number}](https://github.com/{repositoryConfig}/issues/{number})";

    /// <summary>
    /// Generates a markdown test report.
    /// </summary>
    public string GenerateReport(
        List<IssueResult> results,
        List<IssueMetadata> metadata)
    {
        var sb = new StringBuilder();
        var metadataDict = metadata.ToDictionary(m => m.Number);

        sb.AppendLine("# Test Report");
        sb.AppendLine();
        sb.AppendLine("## Summary");
        sb.AppendLine();
        
        AppendSummary(sb, results, metadataDict);
        sb.AppendLine("## What we are testing");
        sb.AppendLine();
        sb.AppendLine("Package versions under test:");
        sb.AppendLine();
        AppendPackageVersions(sb, results);
        
        // Only show regression section if there are actually closed issues tested
        var hasClosedIssues = results.Any(r => metadataDict.TryGetValue(r.Number, out var m) && m.State == "closed");
        if (hasClosedIssues)
        {
            AppendRegressionTests(sb, results, metadataDict);
        }
        
        // Only show open issues section if there are actually open issues tested
        var hasOpenIssues = results.Any(r => metadataDict.TryGetValue(r.Number, out var m) && m.State == "open");
        if (hasOpenIssues)
        {
            AppendOpenIssues(sb, results, metadataDict);
        }

        return sb.ToString();
    }

    private void AppendSummary(
        StringBuilder sb,
        List<IssueResult> results,
        Dictionary<int, IssueMetadata> metadata)
    {
        var closedIssues = results
            .Where(r => metadata.TryGetValue(r.Number, out var m) && m.State == "closed")
            .ToList();
        var closedPassed = closedIssues.Count(r => r.TestResult == "success");
        var closedFailed = closedIssues.Count(r => r.TestResult == "fail");

        var openIssues = results
            .Where(r => metadata.TryGetValue(r.Number, out var m) && m.State == "open")
            .ToList();
        var openPassed = openIssues.Count(r => r.TestResult == "success");
        var openFailed = openIssues.Count(r => r.TestResult == "fail");

        sb.AppendLine($"- Regression tests: total {closedIssues.Count}, success {closedPassed}, fail {closedFailed}");
        sb.AppendLine($"- Open issues: total {openIssues.Count}, success {openPassed}, fail {openFailed}");
        sb.AppendLine();
    }

    private static void AppendPackageVersions(
        StringBuilder sb,
        List<IssueResult> results)
    {
        // Only show the packages we're actually testing: NUnit, NUnit.Analyzers, and NUnit3TestAdapter
        var relevantPackages = new[] { "NUnit", "NUnit.Analyzers", "NUnit3TestAdapter" };
        
        var packages = results
            .SelectMany(r => r.Packages)
            .Select(p => p.Split('='))
            .Where(parts => parts.Length == 2 && relevantPackages.Contains(parts[0]))
            .GroupBy(parts => parts[0])
            .Select(g => new { Name = g.Key, Version = g.Select(p => p[1]).Max() })
            .OrderBy(p => p.Name)
            .ToList();

        foreach (var package in packages)
        {
            sb.AppendLine($"- {package.Name}: {package.Version}");
        }
        
        sb.AppendLine();
    }

    private void AppendRegressionTests(
        StringBuilder sb,
        List<IssueResult> results,
        Dictionary<int, IssueMetadata> metadata)
    {
        sb.AppendLine("## Regression tests (closed issues)");
        sb.AppendLine();

        var closedResults = results
            .Where(r => metadata.TryGetValue(r.Number, out var m) && m.State == "closed")
            .OrderBy(r => r.Number)
            .ToList();

        if (closedResults.Count == 0)
        {
            sb.AppendLine("*No closed issues tested*");
            sb.AppendLine();
            return;
        }

        var passed = closedResults.Count(r => r.TestResult == "success");
        var failedCount = closedResults.Count(r => r.TestResult == "fail");
        
        sb.AppendLine($"- Total: {closedResults.Count}, Success: {passed}, Fail: {failedCount}");
        sb.AppendLine();

        sb.AppendLine("| Issue | Test | Conclusion |");
        sb.AppendLine("| --- | --- | --- |");

        foreach (var result in closedResults)
        {
            if (!metadata.TryGetValue(result.Number, out var meta))
            {
                continue;
            }

            var status = result.TestResult == "success" ? "✅" : "❗";
            var conclusion = result.TestResult == "success"
                ? "Success: No regression failure"
                : "Failure: Regression failure.";
            sb.AppendLine($"| {status} {IssueLink(result.Number)} | {result.TestResult} | {conclusion} |");
        }

        sb.AppendLine();

        var failed = closedResults.Where(r => r.TestResult == "fail").ToList();
        if (failed.Count > 0)
        {
            sb.AppendLine("### Closed failures (details)");
            sb.AppendLine();
            

            foreach (var result in failed)
            {
                if (!metadata.TryGetValue(result.Number, out var meta))
                {
                    continue;
                }

                sb.AppendLine($"#### Issue #{result.Number}: {meta.Title}");
                sb.AppendLine();
                sb.AppendLine($"**Link**: {IssueLink(result.Number)}");
                sb.AppendLine();
                if (meta.Labels is { Count: > 0 })
                {
                    sb.AppendLine($"**Labels**: {string.Join(", ", meta.Labels)}");
                    sb.AppendLine();
                }
                sb.AppendLine($"**Conclusion**: Failure: Regression failure.");
                sb.AppendLine();
                sb.AppendLine("**Details**:");
                sb.AppendLine();
                sb.AppendLine("```");
                
                if (!string.IsNullOrWhiteSpace(result.TestError))
                {
                    sb.AppendLine(result.TestError.Trim());
                }
                
                if (!string.IsNullOrWhiteSpace(result.TestOutput))
                {
                    if (!string.IsNullOrWhiteSpace(result.TestError))
                    {
                        sb.AppendLine();
                    }
                    sb.AppendLine(result.TestOutput.Trim());
                }
                
                sb.AppendLine("```");
                sb.AppendLine();
            }
        }
    }

    private void AppendOpenIssues(
        StringBuilder sb,
        List<IssueResult> results,
        Dictionary<int, IssueMetadata> metadata)
    {
        sb.AppendLine("## Open issues");
        sb.AppendLine();

        var openResults = results
            .Where(r => metadata.TryGetValue(r.Number, out var m) && m.State == "open")
            .OrderBy(r => r.Number)
            .ToList();

        var succeeded = openResults
            .Where(r => r.TestResult == "success")
            .ToList();

        var failed = openResults
            .Where(r => r.TestResult == "fail")
            .ToList();

        sb.AppendLine($"- Total: {openResults.Count}, Success: {succeeded.Count}, Fail: {failed.Count}");
        sb.AppendLine();

        if (succeeded.Count > 0)
        {
            sb.AppendLine("### Succeeded (candidates to close)");
            sb.AppendLine();
            sb.AppendLine("| Issue | Conclusion |");
            sb.AppendLine("| --- | --- |");

            foreach (var result in succeeded)
            {
                if (metadata.TryGetValue(result.Number, out var meta))
                {
                    sb.AppendLine($"| {IssueLink(result.Number)} | Open issue, but test succeeds. |");
                }
            }

            sb.AppendLine();
        }

        if (failed.Count > 0)
        {
            sb.AppendLine("### Failing (confirmed repros)");
            sb.AppendLine();

            foreach (var result in failed)
            {
                if (metadata.TryGetValue(result.Number, out var meta))
                {
                    sb.AppendLine($"#### Issue #{result.Number}: {meta.Title}");
                    sb.AppendLine();
                    sb.AppendLine($"**Link**: {IssueLink(result.Number)}");
                    sb.AppendLine();
                    if (meta.Labels is { Count: > 0 })
                    {
                        sb.AppendLine($"**Labels**: {string.Join(", ", meta.Labels)}");
                        sb.AppendLine();
                    }
                    sb.AppendLine($"**Conclusion**: Failure: Open issue, repro fails.");
                    sb.AppendLine();
                    sb.AppendLine("**Details**:");
                    sb.AppendLine();
                    sb.AppendLine("```");
                    
                    if (!string.IsNullOrWhiteSpace(result.TestError))
                    {
                        sb.AppendLine(result.TestError.Trim());
                    }
                    
                    if (!string.IsNullOrWhiteSpace(result.TestOutput))
                    {
                        if (!string.IsNullOrWhiteSpace(result.TestError))
                        {
                            sb.AppendLine();
                        }
                        sb.AppendLine(result.TestOutput.Trim());
                    }
                    
                    sb.AppendLine("```");
                    sb.AppendLine();
                }
            }
        }
    }
}
