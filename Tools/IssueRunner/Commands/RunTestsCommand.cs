using IssueRunner.Models;
using IssueRunner.Services;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Text.RegularExpressions;


namespace IssueRunner.Commands;

/// <summary>
/// Command to run tests for issues.
/// </summary>
public sealed class RunTestsCommand
{
    private static readonly string[] NUnitPackages =
    [
        "NUnit",
        "NUnit.Analyzers",
        "NUnit3TestAdapter"
    ];

    private readonly IIssueDiscoveryService _issueDiscovery;
    private readonly IProjectAnalyzerService _projectAnalyzer;
    private readonly IFrameworkUpgradeService _frameworkUpgrade;
    private readonly IPackageUpdateService _packageUpdate;
    private readonly ITestExecutionService _testExecution;
    private readonly ILogger<RunTestsCommand> _logger;
    private readonly ILoggerFactory _loggerFactory;
    private readonly IProcessExecutor _processExecutor;
    private readonly INuGetPackageVersionService _nugetVersions;
    private readonly IEnvironmentService _environmentService;

    /// <summary>
    /// Initializes a new instance of the <see cref="RunTestsCommand"/> class.
    /// </summary>
    public RunTestsCommand(
        IIssueDiscoveryService issueDiscovery,
        IProjectAnalyzerService projectAnalyzer,
        IFrameworkUpgradeService frameworkUpgrade,
        IPackageUpdateService packageUpdate,
        ITestExecutionService testExecution,
        ILogger<RunTestsCommand> logger,
        ILoggerFactory loggerFactory,
        IProcessExecutor processExecutor,
        INuGetPackageVersionService nugetVersions,
        IEnvironmentService environmentService)
    {
        _issueDiscovery = issueDiscovery;
        _projectAnalyzer = projectAnalyzer;
        _frameworkUpgrade = frameworkUpgrade;
        _packageUpdate = packageUpdate;
        _testExecution = testExecution;
        _logger = logger;
        _loggerFactory = loggerFactory;
        _processExecutor = processExecutor;
        _nugetVersions = nugetVersions;
        _environmentService = environmentService;
    }

    /// <summary>
    /// Executes the command.
    /// </summary>
    public async Task<int> ExecuteAsync(
        string repositoryRoot,
        RunOptions options,
        CancellationToken cancellationToken)
    {
        try
        {
            Console.WriteLine($"Repository root: {repositoryRoot}");
            Console.WriteLine();

            var issueFolders = _issueDiscovery.DiscoverIssueFolders();
            var centralMetadata = await LoadCentralMetadataAsync(repositoryRoot, cancellationToken);
            var metadataDict = centralMetadata.ToDictionary(m => m.Number);

            // Inform if the user asked for specific issues that are not present locally
            if (options.IssueNumbers is { Count: > 0 })
            {
                var missing = options.IssueNumbers.Where(n => !issueFolders.ContainsKey(n)).ToList();
                var present = options.IssueNumbers.Where(issueFolders.ContainsKey).ToList();

                foreach (var missingIssue in missing)
                {
                    Console.WriteLine($"[{missingIssue}] Skipped: issue folder not found in {repositoryRoot}");
                }

                // If none of the requested issues exist locally, stop early instead of generating reports.
                if (present.Count == 0)
                {
                    Console.WriteLine("No requested issues found locally. Skipping test execution.");
                    return 0;
                }

                // Narrow discovery to the requested set that actually exists
                issueFolders = issueFolders
                    .Where(kvp => present.Contains(kvp.Key))
                    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            }

            // Check if feed changed from previous run
            await CheckChangedFeedAndReset(repositoryRoot, options, cancellationToken);

            // Step 1: Filter which issues to run (based on current frameworks)
            var issuesToRun = FilterIssues(
                issueFolders,
                metadataDict,
                options);

            //Step 2: Upgrade frameworks for filtered issues only, then process
            var results = await UpgradeFrameworks(options, cancellationToken, issuesToRun);

            await SaveResultsAsync(repositoryRoot, results, cancellationToken);

            return 0;
        }
        catch (FileNotFoundException)
        {
            // Error message already printed in LoadCentralMetadataAsync
            return 1;
        }
    }

    private async Task CheckChangedFeedAndReset(string repositoryRoot, RunOptions options,
        CancellationToken cancellationToken)
    {
        var previousResults = await LoadPreviousResultsAsync(repositoryRoot, cancellationToken);
        var feedChanged = CheckFeedChanged(previousResults, options.Feed);

        if (feedChanged)
        {
            Console.WriteLine($"Feed changed to {options.Feed} - resetting packages to metadata versions...");
            var previousFeed = GetPreviousFeed(previousResults);
            await ResetPackagesForIssuesAsync(repositoryRoot, options.IssueNumbers, previousFeed, cancellationToken);
        }
    }

    /// <summary>
    /// Step 2: Upgrade frameworks for filtered issues only, then process
    /// </summary>
    private async Task<List<IssueResult>> UpgradeFrameworks(RunOptions options, CancellationToken cancellationToken, Dictionary<int, string> issuesToRun)
    {
        var results = new List<IssueResult>();
        var isFirstIssue = true;

        foreach (var (issueNumber, folderPath) in issuesToRun)
        {
            if (_issueDiscovery.ShouldSkipIssue(folderPath))
            {
                Console.WriteLine($"[{issueNumber}] Skipped due to marker file (ignore/explicit/wip)");
                continue;
            }

            // Upgrade frameworks before processing
            _frameworkUpgrade.UpgradeAllProjectFrameworks(folderPath, issueNumber);

            var result = await ProcessIssueAsync(
                issueNumber,
                folderPath,
                options,
                isFirstIssue,
                cancellationToken);

            if (result != null)
            {
                results.Add(result);
                isFirstIssue = false;
            }
        }

        return results;
    }

    private Dictionary<int, string> FilterIssues(
        Dictionary<int, string> allIssues,
        Dictionary<int, IssueMetadata> metadata,
        RunOptions options)
    {
        var filtered = allIssues;

        if (options.IssueNumbers != null && options.IssueNumbers.Count > 0)
        {
            filtered = filtered
                .Where(kvp => options.IssueNumbers.Contains(kvp.Key))
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        filtered = options.Scope switch
        {
            TestScope.RegressionOnly => filtered
                .Where(kvp => metadata.TryGetValue(kvp.Key, out var m) && m.State == "closed")
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value),
            
            TestScope.OpenOnly => filtered
                .Where(kvp => metadata.TryGetValue(kvp.Key, out var m) && m.State == "open")
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value),
            
            _ => filtered
        };

        if (options.ExecutionMode != ExecutionMode.All)
        {
            filtered = filtered
                .Where(kvp =>
                {
                    var hasCustom = _testExecution.HasCustomRunners(kvp.Value);
                    return options.ExecutionMode == ExecutionMode.Custom
                        ? hasCustom
                        : !hasCustom;
                })
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        return filtered;
    }

    private async Task<IssueResult?> ProcessIssueAsync(
        int issueNumber,
        string folderPath,
        RunOptions options,
        bool isFirstIssue,
        CancellationToken cancellationToken)
    {
        // Framework already upgraded in ExecuteAsync before this is called
        var projectFiles = GetProjectFilesForIssue(folderPath, issueNumber);
        
        if (projectFiles.Count == 0)
        {
            if (options.Verbosity == LogVerbosity.Verbose)
            {
                Console.WriteLine($"[{issueNumber}] No project files found");
            }
            return null;
        }

        var projectFile = projectFiles.First();
        
        // Re-parse after framework upgrade
        var (frameworks, packages) = _projectAnalyzer.ParseProjectFile(projectFile);

        if (ShouldSkipFramework(frameworks, folderPath, options))
        {
            Console.WriteLine($"[{issueNumber}] Skipped (not netfx-only; --only-netfx enabled)");
            return null;
        }

        var relativeProjectPath = Path.GetRelativePath(Path.GetDirectoryName(folderPath)!, projectFile);
        
        // Display target NUnit package versions once, before any package updates kick off
        if (isFirstIssue)
        {
            await PrintTargetNUnitVersionsAsync(options.Feed, cancellationToken);
        }

        // Step 2: Update packages for all relevant projects in this issue folder
        var updateOutputs = new List<string>();
        var updateErrors = new List<string>();
        var updateSuccess = true;

        foreach (var proj in projectFiles)
        {
            var rel = Path.GetRelativePath(Path.GetDirectoryName(folderPath)!, proj);
            Console.WriteLine($"[{issueNumber}] Updating packages in {rel}");

            var (success, output, error) =
                await _packageUpdate.UpdatePackagesAsync(
                    proj,
                    options.NUnitOnly,
                    options.Feed,
                    options.TimeoutSeconds,
                    cancellationToken);

            updateSuccess &= success;

            if (!string.IsNullOrWhiteSpace(output))
            {
                updateOutputs.Add($"[{rel}]\n{output}");
            }

            if (!string.IsNullOrWhiteSpace(error))
            {
                updateErrors.Add($"[{rel}]\n{error}");
            }
        }

        var updateOutput = string.Join("\n\n", updateOutputs);
        var updateError = string.Join("\n\n", updateErrors);

        if (options.Verbosity == LogVerbosity.Verbose && !string.IsNullOrWhiteSpace(updateOutput))
        {
            Console.WriteLine(updateOutput);
        }

        // Run tests for all project files in this issue (aggregate results)
        var testOutputs = new List<string>();
        var testErrors = new List<string>();
        var allTestsSucceeded = true;
        string? runSettings = null;
        List<string>? scripts = null;

        foreach (var proj in projectFiles)
        {
            var rel = Path.GetRelativePath(Path.GetDirectoryName(folderPath)!, proj);
            var (projTestSuccess, projTestOutput, projTestError, projRunSettings, projScripts) =
                await _testExecution.ExecuteTestsAsync(
                    proj,
                    folderPath,
                    options.TimeoutSeconds,
                    cancellationToken);

            allTestsSucceeded &= projTestSuccess;
            runSettings ??= projRunSettings;
            scripts ??= projScripts;

            testOutputs.Add($"=== {rel} ===\n{projTestOutput}");
            if (!string.IsNullOrWhiteSpace(projTestError))
            {
                testErrors.Add($"=== {rel} ===\n{projTestError}");
            }

            var executionMethod = projScripts is { Count: > 0 }
                ? $"custom scripts: {string.Join(", ", projScripts.Select(Path.GetFileName))}"
                : "dotnet test";

            Console.WriteLine($"[{issueNumber}] Running tests in {rel} ({executionMethod})");

            if (options.Verbosity == LogVerbosity.Verbose)
            {
                if (!string.IsNullOrWhiteSpace(projTestOutput))
                {
                    Console.WriteLine(projTestOutput);
                }
                if (!string.IsNullOrWhiteSpace(projTestError))
                {
                    Console.WriteLine("--- Test stderr ---");
                    Console.WriteLine(projTestError);
                }
            }
        }

        var testOutput = string.Join("\n\n", testOutputs);
        var testError = string.Join("\n\n", testErrors);
        var testSuccess = allTestsSucceeded;

        // Show result
        var conclusion = BuildConclusion(testSuccess, updateSuccess, testOutput, testError);
        
        Console.WriteLine($"[{issueNumber}] {conclusion}");

        if (!testSuccess && options.Verbosity == LogVerbosity.Normal && !string.IsNullOrWhiteSpace(testError))
        {
            Console.WriteLine(testError);
        }

        var result = new IssueResult
        {
            Number = issueNumber,
            ProjectPath = Path.GetRelativePath(folderPath, projectFile),
            ProjectStyle = _projectAnalyzer.GetProjectStyle(projectFile),
            TargetFrameworks = frameworks,
            Packages = packages.Select(p => $"{p.Name}={p.Version}").ToList(),
            UpdateResult = updateSuccess ? "success" : "fail",
            UpdateOutput = updateOutput,
            UpdateError = updateError,
            TestResult = testSuccess ? "success" : "fail",
            TestOutput = testOutput,
            TestError = testError,
            TestConclusion = conclusion,
            RunSettings = runSettings,
            RunnerScripts = scripts,
            Feed = options.Feed.ToString()
        };

        return result;
    }

    private static string DetermineFailureReason(bool updateSuccess, string testOutput, string testError)
    {
        // Check if package update failed
        if (!updateSuccess)
        {
            return "Package update failed";
        }

        var combinedOutput = testOutput + "\n" + testError;
        var lowerOutput = combinedOutput.ToLowerInvariant();

        // Check for compilation errors
        if (lowerOutput.Contains("error cs") || 
            lowerOutput.Contains("build failed") ||
            lowerOutput.Contains("compilation failed"))
        {
            return "Compilation failed";
        }

        // Check for no tests found
        if (lowerOutput.Contains("no test is available") ||
            lowerOutput.Contains("no tests found") ||
            lowerOutput.Contains("test run aborted"))
        {
            return "No tests found";
        }

        // Check for test count mismatches
        var expectedMatch = System.Text.RegularExpressions.Regex.Match(combinedOutput, @"expected.*?(\d+)\s+tests?", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        var foundMatch = System.Text.RegularExpressions.Regex.Match(combinedOutput, @"found.*?(\d+)\s+tests?", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        if (expectedMatch.Success && foundMatch.Success)
        {
            return $"Expected {expectedMatch.Groups[1].Value} tests, found {foundMatch.Groups[1].Value}";
        }

        // Check for assertion failures
        if (lowerOutput.Contains("failed!") || 
            lowerOutput.Contains("assertion") ||
            lowerOutput.Contains("expected:") && lowerOutput.Contains("actual:"))
        {
            return "Test assertions failed";
        }

        // Check for exceptions
        if (lowerOutput.Contains("exception:") || 
            lowerOutput.Contains("stacktrace") ||
            lowerOutput.Contains("at system."))
        {
            return "Tests threw exception";
        }

        // Generic test failure
        return "Tests failed";
    }

    private async Task PrintTargetNUnitVersionsAsync(PackageFeed feed, CancellationToken cancellationToken)
    {
        try
        {
            var latest = await _nugetVersions.GetLatestVersionsAsync(NUnitPackages, feed, cancellationToken);
            if (latest.Count == 0)
            {
                return;
            }

            Console.WriteLine("\nTarget NUnit package versions:");
            foreach (var pkg in NUnitPackages)
            {
                if (latest.TryGetValue(pkg, out var version))
                {
                    Console.WriteLine($"  {pkg} = {version}");
                }
            }
            Console.WriteLine();
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "Failed to fetch target NUnit package versions for feed {Feed}", feed);
        }
    }

    private List<string> GetProjectFilesForIssue(string issueFolderPath, int issueNumber)
    {
        var metadataPath = Path.Combine(issueFolderPath, "issue_metadata.json");
        var projectFiles = new List<string>();

        if (File.Exists(metadataPath))
        {
            try
            {
                var json = File.ReadAllText(metadataPath);
                using var doc = JsonDocument.Parse(json);

                foreach (var element in doc.RootElement.EnumerateArray())
                {
                    if (!element.TryGetProperty("number", out var numProp) ||
                        numProp.GetInt32() != issueNumber)
                    {
                        continue;
                    }

                    if (!element.TryGetProperty("project_path", out var pathProp))
                    {
                        continue;
                    }

                    var projectPath = pathProp.GetString();
                    if (string.IsNullOrWhiteSpace(projectPath))
                    {
                        continue;
                    }

                    var fullPath = Path.Combine(
                        issueFolderPath,
                        projectPath.Replace('/', Path.DirectorySeparatorChar));

                    if (File.Exists(fullPath))
                    {
                        projectFiles.Add(fullPath);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "[{Issue}] Failed to parse issue_metadata.json for project paths", issueNumber);
            }
        }

        if (projectFiles.Count > 0)
        {
            return projectFiles.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
        }

        return _projectAnalyzer.FindProjectFiles(issueFolderPath);
    }

    private static string BuildConclusion(bool testSuccess, bool updateSuccess, string testOutput, string testError)
    {
        var counts = FormatTestCounts(testOutput, testError);

        if (testSuccess)
        {
            return string.IsNullOrEmpty(counts)
                ? "Success: No regression failure"
                : $"Success: No regression failure ({counts})";
        }

        var failureReason = DetermineFailureReason(updateSuccess, testOutput, testError);
        return string.IsNullOrEmpty(counts)
            ? $"Failure: {failureReason}"
            : $"Failure: {failureReason} ({counts})";
    }

    private static string FormatTestCounts(string output, string error)
    {
        var (passed, failed) = ExtractCounts(output, error);

        if (!passed.HasValue && !failed.HasValue)
        {
            return string.Empty;
        }

        var passValue = passed ?? 0;

        if (failed.HasValue && failed.Value > 0)
        {
            return $"Pass {passValue}, Fail {failed.Value}";
        }

        return $"Pass {passValue}";
    }

    private static (int? Passed, int? Failed) ExtractCounts(string output, string error)
    {
        var combined = string.Join("\n", output ?? string.Empty, error ?? string.Empty);
        if (string.IsNullOrWhiteSpace(combined))
        {
            return (null, null);
        }

        int? SumMatches(params string[] patterns)
        {
            foreach (var pattern in patterns)
            {
                var matches = Regex.Matches(combined, pattern, RegexOptions.IgnoreCase);
                if (matches.Count == 0)
                {
                    continue;
                }
                return matches.Select(m => int.Parse(m.Groups[1].Value)).Sum();
            }
            return null;
        }

        // New MTP summary: "Test run summary: Passed!  total: 6  failed: 0  succeeded: 6"
        var passed = SumMatches(@"Passed:\s*(\d+)", @"Succeeded:\s*(\d+)", @"Tests run:\s*\d+.*?Passed:\s*(\d+)");
        // Old vstest summary: "Passed!  - Failed: 0, Passed: 3, Skipped: 0, Total: 3"
        passed ??= SumMatches(@"Total tests:\s*\d+.*?Passed:\s*(\d+)");

        var failed = SumMatches(
            @"Failed:\s*(\d+)",
            @"Failures:\s*(\d+)",
            @"Tests run:\s*\d+.*?Failed:\s*(\d+)");

        var total = SumMatches(
            @"total:\s*(\d+)",
            @"Total\s+tests:\s*(\d+)",
            @"Tests\s+run:\s*(\d+)");

        // If we don't have failed but have total and passed, derive it
        if (!failed.HasValue && total.HasValue && passed.HasValue)
        {
            failed = Math.Max(total.Value - passed.Value, 0);
        }

        return (passed, failed);
    }

    private static bool ShouldSkipFramework(
        List<string> frameworks,
        string issueFolderPath,
        RunOptions options)
    {
        if (options is { SkipNetFx: false, OnlyNetFx: false })
        {
            return false;
        }

        // Check for Windows marker file (case insensitive)
        var hasWindowsMarker = HasWindowsMarker(issueFolderPath);

        var hasNetFx = HasNetFx(frameworks);

        // Treat Windows marker as if it's netfx for workflow filtering purposes
        var treatAsNetFx = hasNetFx || hasWindowsMarker;

        if (options.SkipNetFx && treatAsNetFx)
        {
            return true;
        }

        return options.OnlyNetFx && !treatAsNetFx;
    }

    private static bool HasNetFx(List<string> frameworks)
    {
        return frameworks.Any(f =>
            f.StartsWith("net4") || f.StartsWith("net3") || f.StartsWith("net2"));
    }

    private static bool HasWindowsMarker(string issueFolderPath)
    {
        var hasWindowsMarker = Directory.GetFiles(issueFolderPath)
            .Select(Path.GetFileName)
            .Any(f => f != null && 
                      (f.Equals("windows", StringComparison.OrdinalIgnoreCase) || 
                       f.Equals("windows.md", StringComparison.OrdinalIgnoreCase)));
        return hasWindowsMarker;
    }

    private async Task<List<IssueMetadata>> LoadCentralMetadataAsync(
        string repositoryRoot,
        CancellationToken cancellationToken)
    {
        var path = Path.Combine(repositoryRoot, "Tools", "issues_metadata.json");
        
        if (!File.Exists(path))
        {
            Console.WriteLine($"ERROR: Metadata file not found: {path}");
            Console.WriteLine();
            Console.WriteLine("Please run the sync scripts to create metadata:");
            Console.WriteLine("  Windows:    ..\\nunit3-vs-adapter.issues\\Tools\\sync-from-github.cmd");
            Console.WriteLine("  Linux/macOS: ../nunit3-vs-adapter.issues/Tools/sync-from-github.sh");
            throw new FileNotFoundException($"Metadata file not found: {path}", path);
        }

        var json = await File.ReadAllTextAsync(path, cancellationToken);
        return JsonSerializer.Deserialize<List<IssueMetadata>>(json) ?? [];
    }

    private async Task SaveResultsAsync(
        string repositoryRoot,
        List<IssueResult> results,
        CancellationToken cancellationToken)
    {
        var resultsPath = Path.Combine(repositoryRoot, "results.json");
        
        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        var json = JsonSerializer.Serialize(results, options);
        await File.WriteAllTextAsync(resultsPath, json, cancellationToken);
    }

    private async Task<List<IssueResult>> LoadPreviousResultsAsync(
        string repositoryRoot,
        CancellationToken cancellationToken)
    {
        var resultsPath = Path.Combine(repositoryRoot, "results.json");

        if (!File.Exists(resultsPath))
        {
            return [];
        }

        try
        {
            var json = await File.ReadAllTextAsync(resultsPath, cancellationToken);
            return JsonSerializer.Deserialize<List<IssueResult>>(json) ?? [];
        }
        catch
        {
            return [];
        }
    }

    private bool CheckFeedChanged(List<IssueResult> previousResults, PackageFeed currentFeed)
    {
        if (previousResults.Count == 0)
        {
            return false;
        }

        var previousFeed = previousResults.FirstOrDefault()?.Feed;
        if (string.IsNullOrEmpty(previousFeed))
        {
            return false;
        }

        return previousFeed != currentFeed.ToString();
    }

    private string? GetPreviousFeed(List<IssueResult> previousResults)
    {
        return previousResults.FirstOrDefault()?.Feed;
    }

    private async Task ResetPackagesForIssuesAsync(
        string repositoryRoot,
        List<int>? issueNumbers,
        string? previousFeed,
        CancellationToken cancellationToken)
    {
        var resetLogger = _loggerFactory.CreateLogger<ResetPackagesCommand>();
        var resetCommand = new ResetPackagesCommand(
            _issueDiscovery,
            _projectAnalyzer,
            resetLogger);

        await resetCommand.ExecuteAsync(repositoryRoot, issueNumbers, cancellationToken);

        // Remove MyGet source if previous feed was Alpha
        if (previousFeed == PackageFeed.Alpha.ToString())
        {
            await RemoveMyGetSourceAsync(cancellationToken);
        }
    }

    private async Task RemoveMyGetSourceAsync(CancellationToken cancellationToken)
    {
        try
        {
            // Check if MyGet source exists
            var listResult = await _processExecutor.ExecuteAsync(
                "dotnet",
                "nuget list source",
                Environment.CurrentDirectory,
                30,
                cancellationToken);

            if (listResult.Output.Contains("nunit-myget", StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogInformation("Removing MyGet source");
                await _processExecutor.ExecuteAsync(
                    "dotnet",
                    "nuget remove source nunit-myget",
                    Environment.CurrentDirectory,
                    30,
                    cancellationToken);
                Console.WriteLine("Removed MyGet source from nuget.config");
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to remove MyGet source");
        }
    }
}
