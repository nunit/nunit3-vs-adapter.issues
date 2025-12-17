using IssueRunner.Models;
using IssueRunner.Services;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace IssueRunner.Commands;

/// <summary>
/// Command to run tests for issues.
/// </summary>
public sealed class RunTestsCommand
{
    private readonly IIssueDiscoveryService _issueDiscovery;
    private readonly IProjectAnalyzerService _projectAnalyzer;
    private readonly IFrameworkUpgradeService _frameworkUpgrade;
    private readonly IPackageUpdateService _packageUpdate;
    private readonly ITestExecutionService _testExecution;
    private readonly ILogger<RunTestsCommand> _logger;
    private readonly ILoggerFactory _loggerFactory;
    private readonly ProcessExecutor _processExecutor;

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
        ProcessExecutor processExecutor)
    {
        _issueDiscovery = issueDiscovery;
        _projectAnalyzer = projectAnalyzer;
        _frameworkUpgrade = frameworkUpgrade;
        _packageUpdate = packageUpdate;
        _testExecution = testExecution;
        _logger = logger;
        _loggerFactory = loggerFactory;
        _processExecutor = processExecutor;
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
            var issueFolders = _issueDiscovery.DiscoverIssueFolders(repositoryRoot);
            var centralMetadata = await LoadCentralMetadataAsync(repositoryRoot, cancellationToken);
            var metadataDict = centralMetadata.ToDictionary(m => m.Number);

            // Check if feed changed from previous run
            var previousResults = await LoadPreviousResultsAsync(repositoryRoot, cancellationToken);
            var feedChanged = CheckFeedChanged(previousResults, options.Feed);

            if (feedChanged)
            {
                Console.WriteLine($"Feed changed to {options.Feed} - resetting packages to metadata versions...");
                var previousFeed = GetPreviousFeed(previousResults);
                await ResetPackagesForIssuesAsync(repositoryRoot, options.IssueNumbers, previousFeed, cancellationToken);
            }

            // Step 1: Filter which issues to run (based on current frameworks)
            var issuesToRun = FilterIssues(
                issueFolders,
                metadataDict,
                options);

            var results = new List<IssueResult>();
            var updateLogs = new List<PackageUpdateLog>();
            var isFirstIssue = true;

            // Step 2: Upgrade frameworks for filtered issues only, then process
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

            await SaveResultsAsync(repositoryRoot, results, cancellationToken);

            return 0;
        }
        catch (FileNotFoundException)
        {
            // Error message already printed in LoadCentralMetadataAsync
            return 1;
        }
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
        var projectFiles = _projectAnalyzer.FindProjectFiles(folderPath);
        
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
        
        // Step 2: Update packages
        Console.WriteLine($"[{issueNumber}] Updating packages in {relativeProjectPath}");
        
        var (updateSuccess, updateOutput, updateError) =
            await _packageUpdate.UpdatePackagesAsync(
                projectFile,
                options.NUnitOnly,
                options.Feed,
                options.TimeoutSeconds,
                cancellationToken);

        // Display NUnit package versions for the first issue
        if (isFirstIssue)
        {
            // Re-parse project to get current versions
            var (_, updatedPackages) = _projectAnalyzer.ParseProjectFile(projectFile);
            var nunitPackages = updatedPackages
                .Where(p => p.Name.StartsWith("NUnit", StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (nunitPackages.Count > 0)
            {
                Console.WriteLine("\nTarget NUnit package versions:");
                foreach (var pkg in nunitPackages)
                {
                    Console.WriteLine($"  {pkg.Name} = {pkg.Version}");
                }
                Console.WriteLine();
            }
        }

        if (options.Verbosity == LogVerbosity.Verbose && !string.IsNullOrWhiteSpace(updateOutput))
        {
            Console.WriteLine(updateOutput);
        }

        // Run tests
        var (testSuccess, testOutput, testError, runSettings, scripts) =
            await _testExecution.ExecuteTestsAsync(
                projectFile,
                folderPath,
                options.TimeoutSeconds,
                cancellationToken);

        var executionMethod = scripts != null && scripts.Count > 0
            ? $"custom scripts: {string.Join(", ", scripts.Select(Path.GetFileName))}"
            : "dotnet test";
        
        Console.WriteLine($"[{issueNumber}] Running tests in {relativeProjectPath} ({executionMethod})");

        if (options.Verbosity == LogVerbosity.Verbose)
        {
            if (!string.IsNullOrWhiteSpace(testOutput))
            {
                Console.WriteLine(testOutput);
            }
            if (!string.IsNullOrWhiteSpace(testError))
            {
                Console.WriteLine("--- Test stderr ---");
                Console.WriteLine(testError);
            }
        }

        // Show result
        string conclusion;
        if (testSuccess)
        {
            conclusion = "Success: No regression failure";
        }
        else
        {
            var failureReason = DetermineFailureReason(updateSuccess, testOutput, testError);
            conclusion = $"Failure: {failureReason}";
        }
        
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

    private static bool ShouldSkipFramework(
        List<string> frameworks,
        string issueFolderPath,
        RunOptions options)
    {
        if (!options.SkipNetFx && !options.OnlyNetFx)
        {
            return false;
        }

        // Check for Windows marker file (case insensitive)
        var hasWindowsMarker = Directory.GetFiles(issueFolderPath)
            .Select(Path.GetFileName)
            .Any(f => f != null && 
                (f.Equals("windows", StringComparison.OrdinalIgnoreCase) || 
                 f.Equals("windows.md", StringComparison.OrdinalIgnoreCase)));

        var hasNetFx = frameworks.Any(f =>
            f.StartsWith("net4") || f.StartsWith("net3") || f.StartsWith("net2"));

        // Treat Windows marker as if it's netfx for workflow filtering purposes
        var treatAsNetFx = hasNetFx || hasWindowsMarker;

        if (options.SkipNetFx && treatAsNetFx)
        {
            return true;
        }

        if (options.OnlyNetFx && !treatAsNetFx)
        {
            return true;
        }

        return false;
    }

    private async Task<List<IssueMetadata>> LoadCentralMetadataAsync(
        string repositoryRoot,
        CancellationToken cancellationToken)
    {
        var path = Path.Combine(repositoryRoot, "Tools", "issues_metadata.json");
        
        if (!File.Exists(path))
        {
            Console.WriteLine($"ERROR: Metadata file not found: {path}");
            Console.WriteLine("Please run: IssueRunner metadata sync-from-github");
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
