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

    /// <summary>
    /// Initializes a new instance of the <see cref="ReportGeneratorService"/> class.
    /// </summary>
    public ReportGeneratorService(ILogger<ReportGeneratorService> logger)
    {
        _logger = logger;
    }

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
        AppendRegressionTests(sb, results, metadataDict);
        AppendOpenIssues(sb, results, metadataDict);

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
        var packages = results
            .SelectMany(r => r.Packages)
            .Select(p => p.Split('='))
            .Where(parts => parts.Length == 2)
            .GroupBy(parts => parts[0])
            .Select(g => new { Name = g.Key, Versions = g.Select(p => p[1]).Distinct().ToList() })
            .OrderBy(p => p.Name)
            .ToList();

        foreach (var package in packages)
        {
            // If only one version, show it simply
            if (package.Versions.Count == 1)
            {
                sb.AppendLine($"- {package.Name}={package.Versions[0]}");
            }
            else
            {
                sb.AppendLine($"- {package.Name}: {string.Join(", ", package.Versions)}");
            }
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
            sb.AppendLine($"| {status} #{result.Number} | {result.TestResult} | {conclusion} |");
        }

        sb.AppendLine();

        var failed = closedResults.Where(r => r.TestResult == "fail").ToList();
        if (failed.Count > 0)
        {
            sb.AppendLine("### Closed failures (details)");
            sb.AppendLine();
            sb.AppendLine("| Issue | Conclusion | Details |");
            sb.AppendLine("| --- | --- | --- |");

            foreach (var result in failed)
            {
                if (!metadata.TryGetValue(result.Number, out var meta))
                {
                    continue;
                }

                var conclusion = "Failure: Regression failure.";
                var details = FormatDetailsForTable(result.TestError, result.TestOutput);
                sb.AppendLine($"| #{result.Number} https://github.com/nunit/nunit3-vs-adapter/issues/{result.Number} | {conclusion} | {details} |");
            }

            sb.AppendLine();
        }
    }

    private string FormatDetailsForTable(string? error, string? output)
    {
        var combined = new StringBuilder();
        
        if (!string.IsNullOrWhiteSpace(error))
        {
            combined.Append(error.Trim());
        }
        
        if (!string.IsNullOrWhiteSpace(output))
        {
            if (combined.Length > 0)
            {
                combined.Append(" ");
            }
            combined.Append(output.Trim());
        }

        var result = combined.ToString();
        
        // Replace line breaks with <br/> for markdown table cells
        result = result.Replace("\r\n", "<br/>").Replace("\n", "<br/>").Replace("\r", "<br/>");
        
        return result;
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

        if (openResults.Count == 0)
        {
            sb.AppendLine("*No open issues tested*");
            sb.AppendLine();
            return;
        }

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
                    sb.AppendLine($"| #{result.Number} https://github.com/nunit/nunit3-vs-adapter/issues/{result.Number} | Open issue, but test succeeds. |");
                }
            }

            sb.AppendLine();
        }

        if (failed.Count > 0)
        {
            sb.AppendLine("### Failing (confirmed repros)");
            sb.AppendLine();
            sb.AppendLine("| Issue | Conclusion | Details |");
            sb.AppendLine("| --- | --- | --- |");

            foreach (var result in failed)
            {
                if (metadata.TryGetValue(result.Number, out var meta))
                {
                    var conclusion = "Failure: Open issue, repro fails.";
                    var details = FormatDetailsForTable(result.TestError, result.TestOutput);
                    sb.AppendLine($"| #{result.Number} https://github.com/nunit/nunit3-vs-adapter/issues/{result.Number} | {conclusion} | {details} |");
                }
            }

            sb.AppendLine();
        }
    }
}
