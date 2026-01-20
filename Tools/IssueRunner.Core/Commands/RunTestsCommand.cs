using IssueRunner.Models;
using IssueRunner.Services;
using Microsoft.Extensions.Logging;
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
    private readonly IMarkerService _markerService;

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
        IEnvironmentService environmentService,
        IMarkerService markerService)
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
        _markerService = markerService;
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
            
            // Handle duplicate issue numbers gracefully
            var duplicates = centralMetadata.GroupBy(m => m.Number).Where(g => g.Count() > 1).ToList();
            if (duplicates.Count != 0)
            {
                foreach (var dup in duplicates)
                {
                    Console.WriteLine($"Warning: Duplicate metadata entries found for issue {dup.Key}. Using the last occurrence.");
                    _logger.LogWarning("Duplicate metadata entries found for issue {IssueNumber}. Using the last occurrence.", dup.Key);
                }
            }
            
            // Use GroupBy().ToDictionary() to take the last occurrence of each duplicate
            var metadataDict = centralMetadata
                .GroupBy(m => m.Number)
                .ToDictionary(g => g.Key, g => g.Last());

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
                    DeleteResultsFile(repositoryRoot);
                    return 1;
                }

                // Narrow discovery to the requested set that actually exists
                issueFolders = issueFolders
                    .Where(kvp => present.Contains(kvp.Key))
                    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            }

            // Check if feed changed from previous run
            await CheckChangedFeedAndReset(repositoryRoot, options, cancellationToken);

            // Step 1: Filter which issues to run (based on current frameworks)
            var issuesToRun = await FilterIssuesAsync(
                issueFolders,
                metadataDict,
                options,
                repositoryRoot,
                cancellationToken);

            //Step 2: Upgrade frameworks for filtered issues only, then process
            List<IssueResult> results;
            try
            {
                results = await UpgradeFrameworks(repositoryRoot, options, cancellationToken, issuesToRun, metadataDict);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Test execution was cancelled by user.");
                return 1;
            }

            // Add entries for skipped and not-compiling issues that were filtered out
            var skippedAndNotCompilingResults = await CreateSkippedAndNotCompilingResultsAsync(
                issueFolders,
                issuesToRun,
                metadataDict,
                repositoryRoot,
                options,
                cancellationToken);
            results.AddRange(skippedAndNotCompilingResults);

            if (results.Count == 0)
            {
                Console.WriteLine("No tests were executed. Skipping results and report generation.");
                DeleteResultsFile(repositoryRoot);
                return 1;
            }

            // Check cancellation before saving results
            cancellationToken.ThrowIfCancellationRequested();
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
            await ResetPackagesForIssuesAsync(repositoryRoot, options.IssueNumbers, previousFeed, options.Verbosity, cancellationToken);
        }
    }

    /// <summary>
    /// Step 2: Upgrade frameworks for filtered issues only, then process
    /// </summary>
    private async Task<List<IssueResult>> UpgradeFrameworks(
        string repositoryRoot, 
        RunOptions options, 
        CancellationToken cancellationToken, 
        Dictionary<int, string> issuesToRun,
        Dictionary<int, IssueMetadata> metadataDict)
    {
        var results = new List<IssueResult>();
        var isFirstIssue = true;

        foreach (var (issueNumber, folderPath) in issuesToRun)
        {
            // Check for cancellation before processing each issue
            cancellationToken.ThrowIfCancellationRequested();

            if (_markerService.ShouldSkipIssue(folderPath))
            {
                Console.WriteLine($"[{issueNumber}] Skipped due to marker file");
                // Create a result entry for skipped issue
                var skippedResult = CreateSkippedIssueResult(issueNumber, folderPath, metadataDict, options);
                if (skippedResult != null)
                {
                    results.Add(skippedResult);
                }
                continue;
            }

            // Upgrade frameworks before processing
            _frameworkUpgrade.UpgradeAllProjectFrameworks(folderPath, issueNumber);

            var result = await ProcessIssueAsync(
                issueNumber,
                folderPath,
                repositoryRoot,
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

    private async Task<Dictionary<int, string>> FilterIssuesAsync(
        Dictionary<int, string> allIssues,
        Dictionary<int, IssueMetadata> metadata,
        RunOptions options,
        string repositoryRoot,
        CancellationToken cancellationToken)
    {
        var filtered = allIssues;

        // Handle rerun-failed option
        if (options.RerunFailedTests)
        {
            var allResults = await LoadPreviousResultsAsync(repositoryRoot, cancellationToken);
            if (allResults.Count == 0)
            {
                Console.WriteLine("ERROR: results.json not found or empty. Cannot rerun failed tests.");
                Console.WriteLine("Hint: Run tests first to generate results.json");
                return new();
            }

            // Filter to only failed tests (TestResult != "success")
            var failedResults = allResults
                .Where(r => r.TestResult != null && r.TestResult != "success")
                .ToList();

            if (failedResults.Count == 0)
            {
                Console.WriteLine("No failed tests found in results.json.");
                return new Dictionary<int, string>();
            }

            // Extract unique issue numbers from failed tests
            var failedIssueNumbers = failedResults
                .Select(r => r.Number)
                .Distinct()
                .ToHashSet();

            filtered = filtered
                .Where(kvp => failedIssueNumbers.Contains(kvp.Key))
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            Console.WriteLine($"Rerunning {filtered.Count} failed test(s) from results.json");
        }
        else if (options.IssueNumbers != null && options.IssueNumbers.Count > 0)
        {
            filtered = filtered
                .Where(kvp => options.IssueNumbers.Contains(kvp.Key))
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        // Exclude non-compiling issues (unless rerunning failed tests)
        if (!options.RerunFailedTests)
        {
            var failedBuilds = await LoadFailedBuildsAsync(repositoryRoot, cancellationToken);
            if (failedBuilds.Count > 0)
            {
                var excludedCount = filtered.Count(kvp => failedBuilds.Contains(kvp.Key));
                if (excludedCount > 0)
                {
                    Console.WriteLine($"Excluding {excludedCount} non-compiling issue(s) from results.json");
                    filtered = filtered
                        .Where(kvp => !failedBuilds.Contains(kvp.Key))
                        .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
                }
            }
        }

        // Apply scope filter
        filtered = options.Scope switch
        {
            TestScope.Regression => filtered
                .Where(kvp => metadata.TryGetValue(kvp.Key, out var m) && m.State == "closed")
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value),
            
            TestScope.Open => filtered
                .Where(kvp => metadata.TryGetValue(kvp.Key, out var m) && m.State == "open")
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value),
            
            _ => filtered
        };

        if (options.TestTypes != TestTypes.All)
        {
            filtered = filtered
                .Where(kvp =>
                {
                    var hasCustom = _testExecution.HasCustomRunners(kvp.Value);
                    return options.TestTypes == TestTypes.Custom
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
        string repositoryRoot,
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
            if (options.Verbosity == LogVerbosity.Verbose)
            {
                Console.WriteLine($"[{issueNumber}] Skipped (not netfx-only; --only-netfx enabled)");
            }
            return null;
        }

        var relativeProjectPath = Path.GetRelativePath(Path.GetDirectoryName(folderPath)!, projectFile);
        
        // Display target NUnit package versions once, before any package updates kick off
        if (isFirstIssue)
        {
            await PrintTargetNUnitVersionsAsync(repositoryRoot, options.Feed, cancellationToken);
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
        string? failedStep = null; // Track which step failed: "restore", "build", or "test"
        TestExecutionResult? lastTestResult = null; // Track the last test result for IssueResult

        foreach (var proj in projectFiles)
        {
            // Check for cancellation before each test execution
            cancellationToken.ThrowIfCancellationRequested();

            var rel = Path.GetRelativePath(Path.GetDirectoryName(folderPath)!, proj);
            var testResult = await _testExecution.ExecuteTestsAsync(
                proj,
                folderPath,
                options.TimeoutSeconds,
                cancellationToken);

            lastTestResult = testResult; // Track for IssueResult
            allTestsSucceeded &= testResult.OverallSuccess;
            runSettings ??= testResult.RunSettings;
            scripts ??= testResult.Scripts;
            
            // Track which step failed (first failure wins)
            if (failedStep == null)
            {
                if (testResult.RestoreResult.Status == StepStatus.Failed)
                {
                    failedStep = "restore";
                }
                else if (testResult.BuildResult.Status == StepStatus.Failed)
                {
                    failedStep = "build";
                }
                else if (testResult.TestResult.Status == StepStatus.Failed)
                {
                    failedStep = "test";
                }
            }

            // Collect outputs from all steps that ran
            var stepOutputs = new List<string>();
            var stepErrors = new List<string>();
            
            if (testResult.RestoreResult.Status != StepStatus.NotRun)
            {
                stepOutputs.Add($"=== Restore ===\n{testResult.RestoreResult.Output}");
                if (!string.IsNullOrWhiteSpace(testResult.RestoreResult.Error))
                {
                    stepErrors.Add($"=== Restore Error ===\n{testResult.RestoreResult.Error}");
                }
            }
            
            if (testResult.BuildResult.Status != StepStatus.NotRun)
            {
                stepOutputs.Add($"=== Build ===\n{testResult.BuildResult.Output}");
                if (!string.IsNullOrWhiteSpace(testResult.BuildResult.Error))
                {
                    stepErrors.Add($"=== Build Error ===\n{testResult.BuildResult.Error}");
                }
            }
            
            if (testResult.TestResult.Status != StepStatus.NotRun)
            {
                stepOutputs.Add($"=== Test ===\n{testResult.TestResult.Output}");
                if (!string.IsNullOrWhiteSpace(testResult.TestResult.Error))
                {
                    stepErrors.Add($"=== Test Error ===\n{testResult.TestResult.Error}");
                }
            }
            
            testOutputs.Add($"=== {rel} ===\n{string.Join("\n", stepOutputs)}");
            if (stepErrors.Count > 0)
            {
                testErrors.Add($"=== {rel} ===\n{string.Join("\n", stepErrors)}");
            }

            var executionMethod = testResult.Scripts is { Count: > 0 }
                ? $"custom scripts: {string.Join(", ", testResult.Scripts.Select(Path.GetFileName))}"
                : "dotnet test";

            Console.WriteLine($"[{issueNumber}] Running tests in {rel} ({executionMethod})");
            
            // Report step status with progress updates
            if (testResult.RestoreResult.Status == StepStatus.Failed)
            {
                Console.WriteLine($"[{issueNumber}] Restore failed");
            }
            else if (testResult.RestoreResult.Status == StepStatus.Success)
            {
                Console.WriteLine($"[{issueNumber}] Restore succeeded");
                if (testResult.BuildResult.Status == StepStatus.Failed)
                {
                    Console.WriteLine($"[{issueNumber}] Build failed");
                }
                else if (testResult.BuildResult.Status == StepStatus.Success)
                {
                    Console.WriteLine($"[{issueNumber}] Build succeeded");
                    if (testResult.TestResult.Status == StepStatus.Failed)
                    {
                        Console.WriteLine($"[{issueNumber}] Test failed");
                    }
                    else if (testResult.TestResult.Status == StepStatus.Success)
                    {
                        Console.WriteLine($"[{issueNumber}] All steps succeeded");
                    }
                }
            }
            
            if (options.Verbosity == LogVerbosity.Verbose)
            {
                if (testResult.RestoreResult.Status != StepStatus.NotRun && !string.IsNullOrWhiteSpace(testResult.RestoreResult.Output))
                {
                    Console.WriteLine("--- Restore output ---");
                    Console.WriteLine(testResult.RestoreResult.Output);
                }
                if (testResult.BuildResult.Status != StepStatus.NotRun && !string.IsNullOrWhiteSpace(testResult.BuildResult.Output))
                {
                    Console.WriteLine("--- Build output ---");
                    Console.WriteLine(testResult.BuildResult.Output);
                }
                if (testResult.TestResult.Status != StepStatus.NotRun && !string.IsNullOrWhiteSpace(testResult.TestResult.Output))
                {
                    Console.WriteLine("--- Test output ---");
                    Console.WriteLine(testResult.TestResult.Output);
                }
                if (stepErrors.Count > 0)
                {
                    Console.WriteLine("--- Error output ---");
                    Console.WriteLine(string.Join("\n", stepErrors));
                }
            }
        }

        var testOutput = string.Join("\n\n", testOutputs);
        var testError = string.Join("\n\n", testErrors);
        var testSuccess = allTestsSucceeded;

        // Aggregate step results from all projects (use the last project's results)
        var restoreResult = StepStatus.NotRun;
        var buildResult = StepStatus.NotRun;
        var testResultStatus = StepStatus.NotRun;
        string? restoreOutput = null;
        string? restoreError = null;
        string? buildOutput = null;
        string? buildError = null;
        
        // Collect step results from the last project
        if (lastTestResult != null)
        {
            restoreResult = lastTestResult.RestoreResult.Status;
            restoreOutput = lastTestResult.RestoreResult.Output;
            restoreError = lastTestResult.RestoreResult.Error;
            buildResult = lastTestResult.BuildResult.Status;
            buildOutput = lastTestResult.BuildResult.Output;
            buildError = lastTestResult.BuildResult.Error;
            testResultStatus = lastTestResult.TestResult.Status;
        }

        // Show result
        var conclusion = BuildConclusion(testSuccess, updateSuccess, testOutput, testError, failedStep);
        
        Console.WriteLine($"[{issueNumber}] {conclusion}");

        if (!testSuccess && options.Verbosity == LogVerbosity.Normal && !string.IsNullOrWhiteSpace(testError))
        {
            Console.WriteLine(testError);
        }

        var now = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");
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
            RestoreResult = restoreResult == StepStatus.NotRun ? "not run" : (restoreResult == StepStatus.Success ? "success" : "fail"),
            RestoreOutput = restoreOutput,
            RestoreError = restoreError,
            BuildResult = buildResult == StepStatus.NotRun ? "not run" : (buildResult == StepStatus.Success ? "success" : "fail"),
            BuildOutput = buildOutput,
            BuildError = buildError,
            TestResult = testResultStatus == StepStatus.NotRun ? "not run" : (testResultStatus == StepStatus.Success ? "success" : "fail"),
            TestOutput = testOutput,
            TestError = testError,
            TestConclusion = conclusion,
            RunSettings = runSettings,
            RunnerScripts = scripts,
            Feed = options.Feed.ToString(),
            LastRun = now
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

    private async Task PrintTargetNUnitVersionsAsync(
        string repositoryRoot,
        PackageFeed feed,
        CancellationToken cancellationToken)
    {
        try
        {
            var latest = await _nugetVersions.GetLatestVersionsAsync(NUnitPackages, feed, cancellationToken);
            if (latest.Count == 0)
            {
                return;
            }

            Console.WriteLine("\nTarget NUnit package versions:");
            var packageVersions = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var pkg in NUnitPackages)
            {
                if (latest.TryGetValue(pkg, out var version))
                {
                    var versionString = version.ToNormalizedString();
                    Console.WriteLine($"  {pkg} = {versionString}");
                    packageVersions[pkg] = versionString;
                }
            }
            Console.WriteLine();

            // Save current package versions to file immediately
            await SaveCurrentPackageVersionsAsync(repositoryRoot, packageVersions, feed, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "Failed to fetch target NUnit package versions for feed {Feed}", feed);
        }
    }

    private async Task SaveCurrentPackageVersionsAsync(
        string repositoryRoot,
        Dictionary<string, string> packageVersions,
        PackageFeed feed,
        CancellationToken cancellationToken)
    {
        try
        {
            var dataDir = _environmentService.GetDataDirectory(repositoryRoot);
            var filePath = Path.Combine(dataDir, "nunit-packages-current.json");

            var versions = new NUnitPackageVersions
            {
                Packages = packageVersions,
                Timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                Feed = feed.ToString()
            };

            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            var json = JsonSerializer.Serialize(versions, options);
            await File.WriteAllTextAsync(filePath, json, cancellationToken);
            _logger.LogDebug("Saved current NUnit package versions to {Path}", filePath);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to save current NUnit package versions");
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

    private static string BuildConclusion(bool testSuccess, bool updateSuccess, string testOutput, string testError, string? failedStep)
    {
        var counts = FormatTestCounts(testOutput, testError);

        if (testSuccess)
        {
            return string.IsNullOrEmpty(counts)
                ? "Success: No regression failure"
                : $"Success: No regression failure ({counts})";
        }

        // Determine failure reason based on which step failed
        string failureReason;
        if (failedStep == "restore")
        {
            failureReason = "Restore failed";
        }
        else if (failedStep == "build")
        {
            failureReason = "Build failed";
        }
        else if (failedStep == "test")
        {
            failureReason = DetermineFailureReason(updateSuccess, testOutput, testError);
        }
        else
        {
            // Fallback to old logic if failedStep is not set
            failureReason = DetermineFailureReason(updateSuccess, testOutput, testError);
        }
        
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
            return $"{passValue} passed, {failed.Value} failed";
        }

        return $"{passValue} test(s) passed";
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
        var dataDir = _environmentService.GetDataDirectory(repositoryRoot);
        var path = Path.Combine(dataDir, "issues_metadata.json");
        
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
        var dataDir = _environmentService.GetDataDirectory(repositoryRoot);
        var resultsPath = Path.Combine(dataDir, "results.json");
        
        // Load existing results
        var existingResults = await LoadPreviousResultsAsync(repositoryRoot, cancellationToken);
        
        // Create dictionary keyed by Number|ProjectPath for efficient lookup and update
        var resultsDict = existingResults
            .ToDictionary(r => $"{r.Number}|{r.ProjectPath}", r => r, StringComparer.OrdinalIgnoreCase);
        
        // Update with new results (overwrite existing entries for same issue+project)
        foreach (var result in results)
        {
            var key = $"{result.Number}|{result.ProjectPath}";
            resultsDict[key] = result;
        }
        
        // Convert back to list and sort for consistency
        var mergedResults = resultsDict.Values
            .OrderBy(r => r.Number)
            .ThenBy(r => r.ProjectPath)
            .ToList();
        
        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };
        
        var json = JsonSerializer.Serialize(mergedResults, options);
        await File.WriteAllTextAsync(resultsPath, json, cancellationToken);
        
        // Write per-issue result files (issue_results.json in each issue folder)
        await WritePerIssueResultFilesAsync(repositoryRoot, results, cancellationToken);
    }

    private async Task<List<IssueResult>> LoadPreviousResultsAsync(
        string repositoryRoot,
        CancellationToken cancellationToken)
    {
        var dataDir = _environmentService.GetDataDirectory(repositoryRoot);
        var resultsPath = Path.Combine(dataDir, "results.json");

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

    private void DeleteResultsFile(string repositoryRoot)
    {
        var dataDir = _environmentService.GetDataDirectory(repositoryRoot);
        var resultsPath = Path.Combine(dataDir, "results.json");
        if (File.Exists(resultsPath))
        {
            try
            {
                File.Delete(resultsPath);
            }
            catch
            {
                // Best-effort cleanup
            }
        }
    }

    /// <summary>
    /// Creates IssueResult entries for skipped and not-compiling issues that were filtered out.
    /// Preserves existing entries from results.json for issues with build/restore failures.
    /// </summary>
    private async Task<List<IssueResult>> CreateSkippedAndNotCompilingResultsAsync(
        Dictionary<int, string> allIssueFolders,
        Dictionary<int, string> issuesToRun,
        Dictionary<int, IssueMetadata> metadataDict,
        string repositoryRoot,
        RunOptions options,
        CancellationToken cancellationToken)
    {
        var results = new List<IssueResult>();
        
        // Find issues that were filtered out (not in issuesToRun but in allIssueFolders)
        var filteredOutIssues = allIssueFolders
            .Where(kvp => !issuesToRun.ContainsKey(kvp.Key))
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        
        if (filteredOutIssues.Count == 0)
        {
            return results;
        }
        
        // Load existing results to preserve entries for issues with build/restore failures
        var existingResults = await LoadPreviousResultsAsync(repositoryRoot, cancellationToken);
        var existingResultsByIssue = existingResults
            .GroupBy(r => r.Number)
            .ToDictionary(g => g.Key, g => g.ToList());
        
        var now = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");
        
        foreach (var (issueNumber, folderPath) in filteredOutIssues)
        {
            // Skip if it's a skipped issue (those are handled in UpgradeFrameworks and should not be in results.json)
            if (_markerService.ShouldSkipIssue(folderPath))
            {
                continue;
            }
            
            // Check if this issue has existing results with build or restore failures
            if (existingResultsByIssue.TryGetValue(issueNumber, out var issueResults))
            {
                // Check if any result for this issue has build or restore failure
                var hasBuildOrRestoreFailure = issueResults.Any(r => 
                    (r.BuildResult != null && r.BuildResult == "fail") || 
                    (r.RestoreResult != null && r.RestoreResult == "fail"));
                
                if (hasBuildOrRestoreFailure)
                {
                    // Preserve existing entries (they will be kept in results.json via SaveResultsAsync merge)
                    // Update LastRun timestamp and Feed to current run values
                    foreach (var existingResult in issueResults)
                    {
                        // Create a copy with updated LastRun timestamp and Feed
                        var preservedResult = new IssueResult
                        {
                            Number = existingResult.Number,
                            ProjectPath = existingResult.ProjectPath,
                            ProjectStyle = existingResult.ProjectStyle,
                            TargetFrameworks = existingResult.TargetFrameworks,
                            Packages = existingResult.Packages,
                            UpdateResult = existingResult.UpdateResult,
                            UpdateOutput = existingResult.UpdateOutput,
                            UpdateError = existingResult.UpdateError,
                            RestoreResult = existingResult.RestoreResult,
                            RestoreOutput = existingResult.RestoreOutput,
                            RestoreError = existingResult.RestoreError,
                            BuildResult = existingResult.BuildResult,
                            BuildOutput = existingResult.BuildOutput,
                            BuildError = existingResult.BuildError,
                            TestResult = existingResult.TestResult,
                            TestOutput = existingResult.TestOutput,
                            TestError = existingResult.TestError,
                            TestConclusion = existingResult.TestConclusion,
                            RunSettings = existingResult.RunSettings,
                            RunnerScripts = existingResult.RunnerScripts,
                            Feed = options.Feed.ToString(), // Update feed to current feed
                            RunnerExpectations = existingResult.RunnerExpectations,
                            LastRun = now // Update timestamp to indicate we checked this issue in this run
                        };
                        results.Add(preservedResult);
                    }
                }
            }
        }
        
        return results;
    }
    
    /// <summary>
    /// Creates an IssueResult for a skipped issue (has marker file).
    /// </summary>
    private IssueResult? CreateSkippedIssueResult(
        int issueNumber,
        string folderPath,
        Dictionary<int, IssueMetadata> metadataDict,
        RunOptions options)
    {
        // Get project files to determine project path
        var projectFiles = GetProjectFilesForIssue(folderPath, issueNumber);
        if (projectFiles.Count == 0)
        {
            return null;
        }
        
        var projectFile = projectFiles[0]; // Use first project
        var now = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");
        var (frameworks, _) = _projectAnalyzer.ParseProjectFile(projectFile);
        
        return new IssueResult
        {
            Number = issueNumber,
            ProjectPath = Path.GetRelativePath(folderPath, projectFile),
            ProjectStyle = _projectAnalyzer.GetProjectStyle(projectFile),
            TargetFrameworks = frameworks,
            Packages = new List<string>(), // No packages for skipped issues
            UpdateResult = "not run",
            RestoreResult = "not run",
            BuildResult = "not run",
            TestResult = "skipped",
            Feed = options.Feed.ToString(),
            LastRun = now
        };
    }
    
    /// <summary>
    /// Creates an IssueResult for a not-compiling issue.
    /// </summary>
    private IssueResult? CreateNotCompilingIssueResult(
        int issueNumber,
        string folderPath,
        Dictionary<int, IssueMetadata> metadataDict,
        RunOptions options)
    {
        // Get project files to determine project path
        var projectFiles = GetProjectFilesForIssue(folderPath, issueNumber);
        if (projectFiles.Count == 0)
        {
            return null;
        }
        
        var projectFile = projectFiles[0]; // Use first project
        var now = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");
        var (frameworks, _) = _projectAnalyzer.ParseProjectFile(projectFile);
        
        return new IssueResult
        {
            Number = issueNumber,
            ProjectPath = Path.GetRelativePath(folderPath, projectFile),
            ProjectStyle = _projectAnalyzer.GetProjectStyle(projectFile),
            TargetFrameworks = frameworks,
            Packages = new List<string>(), // Packages may not be relevant for not-compiling issues
            UpdateResult = "not run",
            RestoreResult = "not run",
            BuildResult = "not compile",
            TestResult = "not compiling",
            Feed = options.Feed.ToString(),
            LastRun = now
        };
    }

    /// <summary>
    /// Loads the list of non-compiling issues from results.json (checks build_result and restore_result).
    /// </summary>
    private async Task<HashSet<int>> LoadFailedBuildsAsync(
        string repositoryRoot,
        CancellationToken cancellationToken)
    {
        var dataDir = _environmentService.GetDataDirectory(repositoryRoot);
        var resultsPath = Path.Combine(dataDir, "results.json");
        
        if (!File.Exists(resultsPath))
        {
            // No results.json means no test runs yet, so we don't know which issues fail to compile
            return new HashSet<int>();
        }

        try
        {
            var json = await File.ReadAllTextAsync(resultsPath, cancellationToken);
            var allResults = JsonSerializer.Deserialize<List<IssueResult>>(json);
            
            if (allResults == null)
            {
                return new HashSet<int>();
            }

            // Group results by issue number and check for build/restore failures
            var failedIssues = new HashSet<int>();
            var resultsByIssue = allResults
                .GroupBy(r => r.Number)
                .ToDictionary(g => g.Key, g => g.ToList());

            foreach (var kvp in resultsByIssue)
            {
                var issueNum = kvp.Key;
                var issueResults = kvp.Value;

                // Check if any result for this issue has build or restore failure
                var hasBuildFailure = issueResults.Any(r => 
                    (r.BuildResult != null && r.BuildResult == "fail") || 
                    (r.RestoreResult != null && r.RestoreResult == "fail"));
                
                if (hasBuildFailure)
                {
                    failedIssues.Add(issueNum);
                }
            }

            return failedIssues;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error loading results.json to determine failed builds, continuing with all issues");
            return new HashSet<int>();
        }
    }

    /// <summary>
    /// Loads test results from a JSON file (test-passes.json or test-fails.json).
    /// </summary>
    private async Task<TestResultList?> LoadTestResultsAsync(
        string repositoryRoot,
        string filename,
        CancellationToken cancellationToken)
    {
        var dataDir = _environmentService.GetDataDirectory(repositoryRoot);
        var filePath = Path.Combine(dataDir, filename);
        
        if (!File.Exists(filePath))
        {
            return null;
        }

        try
        {
            var json = await File.ReadAllTextAsync(filePath, cancellationToken);
            return JsonSerializer.Deserialize<TestResultList>(json);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error loading {File}, returning empty list", filename);
            return new TestResultList { TestResults = new List<TestResultEntry>() };
        }
    }

    /// <summary>
    /// DEPRECATED: No longer used. Test results are now stored in results.json only.
    /// Saves test results to test-passes.json and test-fails.json.
    /// </summary>
    [Obsolete("Test results are now stored in results.json only. This method is kept for reference but is no longer called.")]
    private async Task SaveTestResultsAsync(
        string repositoryRoot,
        List<IssueResult> results,
        CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");
        var dataDir = _environmentService.GetDataDirectory(repositoryRoot);
        var passesPath = Path.Combine(dataDir, "test-passes.json");
        var failsPath = Path.Combine(dataDir, "test-fails.json");

        // Load existing results
        var existingPasses = await LoadTestResultsAsync(repositoryRoot, "test-passes.json", cancellationToken);
        var existingFails = await LoadTestResultsAsync(repositoryRoot, "test-fails.json", cancellationToken);

        var passesDict = existingPasses?.TestResults
            .ToDictionary(r => $"{r.Issue}|{r.Project}", r => r) 
            ?? new Dictionary<string, TestResultEntry>();

        var failsDict = existingFails?.TestResults
            .ToDictionary(r => $"{r.Issue}|{r.Project}", r => r)
            ?? new Dictionary<string, TestResultEntry>();

        // Track how many tests were processed in this run
        var processedPasses = 0;
        var processedFails = 0;

        // Update with new results
        foreach (var result in results)
        {
            var issueName = $"Issue{result.Number}";
            var key = $"{issueName}|{result.ProjectPath}";
            
            var entry = new TestResultEntry
            {
                Issue = issueName,
                Project = result.ProjectPath,
                LastRun = now,
                TestResult = result.TestResult ?? "unknown"
            };

            if (result.TestResult == "success")
            {
                passesDict[key] = entry;
                // Remove from fails if it was there
                failsDict.Remove(key);
                processedPasses++;
            }
            else
            {
                failsDict[key] = entry;
                // Remove from passes if it was there
                passesDict.Remove(key);
                processedFails++;
            }
        }

        // Save updated lists
        var passesList = new TestResultList
        {
            TestResults = passesDict.Values.OrderBy(r => r.Issue).ThenBy(r => r.Project).ToList()
        };

        var failsList = new TestResultList
        {
            TestResults = failsDict.Values.OrderBy(r => r.Issue).ThenBy(r => r.Project).ToList()
        };

        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        var passesJson = JsonSerializer.Serialize(passesList, options);
        var failsJson = JsonSerializer.Serialize(failsList, options);

        await File.WriteAllTextAsync(passesPath, passesJson, cancellationToken);
        await File.WriteAllTextAsync(failsPath, failsJson, cancellationToken);

        // Note: WritePerIssueResultFilesAsync is now called from SaveResultsAsync

        // Show both the tests processed in this run and the total counts
        if (processedPasses > 0 || processedFails > 0)
        {
            Console.WriteLine($"Processed {processedPasses} passing and {processedFails} failing test(s) in this run");
        }
        Console.WriteLine($"Total: {passesList.TestResults.Count} passing and {failsList.TestResults.Count} failing test(s) in test-passes.json and test-fails.json");
    }

    /// <summary>
    /// Writes per-issue result files (issue_results.json) in each issue folder.
    /// </summary>
    private async Task WritePerIssueResultFilesAsync(
        string repositoryRoot,
        List<IssueResult> results,
        CancellationToken cancellationToken)
    {
        if (results.Count == 0)
        {
            return;
        }

        // Get issue folders
        var issueFolders = _issueDiscovery.DiscoverIssueFolders();

        // Group results by issue number
        var resultsByIssue = results.GroupBy(r => r.Number).ToDictionary(g => g.Key, g => g.ToList());

        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        // Write issue_results.json for each issue that has results
        foreach (var kvp in resultsByIssue)
        {
            var issueNumber = kvp.Key;
            var issueResults = kvp.Value;

            if (issueFolders.TryGetValue(issueNumber, out var issueFolderPath))
            {
                var issueResultsPath = Path.Combine(issueFolderPath, "issue_results.json");
                try
                {
                    var json = JsonSerializer.Serialize(issueResults, options);
                    await File.WriteAllTextAsync(issueResultsPath, json, cancellationToken);
                    _logger.LogDebug("Wrote issue_results.json for Issue{IssueNumber} at {Path}", issueNumber, issueResultsPath);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to write issue_results.json for Issue{IssueNumber} at {Path}", issueNumber, issueResultsPath);
                }
            }
        }
    }

    /// <summary>
    /// DEPRECATED: No longer used. Test results are now stored in results.json only.
    /// Promotes successful tests from test-fails.json to test-passes.json.
    /// </summary>
    [Obsolete("Test results are now stored in results.json only. This method is kept for reference but is no longer called.")]
    private async Task PromoteResultsAsync(
        string repositoryRoot,
        List<IssueResult> results,
        CancellationToken cancellationToken)
    {
        var failsList = await LoadTestResultsAsync(repositoryRoot, "test-fails.json", cancellationToken);
        if (failsList == null || failsList.TestResults.Count == 0)
        {
            return;
        }

        var promoted = new List<TestResultEntry>();
        var remainingFails = new List<TestResultEntry>();

        foreach (var failEntry in failsList.TestResults)
        {
            // Check if this issue/project combination now passes
            var matchingResult = results.FirstOrDefault(r => 
                $"Issue{r.Number}" == failEntry.Issue && 
                r.ProjectPath == failEntry.Project &&
                r.TestResult == "success");

            if (matchingResult != null)
            {
                promoted.Add(failEntry);
            }
            else
            {
                remainingFails.Add(failEntry);
            }
        }

        if (promoted.Count > 0)
        {
            Console.WriteLine($"Promoted {promoted.Count} test(s) from test-fails.json to test-passes.json:");
            foreach (var entry in promoted)
            {
                Console.WriteLine($"  - {entry.Issue}: {entry.Project}");
            }

            // Update fails list (remove promoted)
            var dataDir = _environmentService.GetDataDirectory(repositoryRoot);
            var updatedFails = new TestResultList { TestResults = remainingFails };
            var failsPath = Path.Combine(dataDir, "test-fails.json");
            var options = new JsonSerializerOptions { WriteIndented = true };
            var failsJson = JsonSerializer.Serialize(updatedFails, options);
            await File.WriteAllTextAsync(failsPath, failsJson, cancellationToken);

            // Add to passes list
            var passesList = await LoadTestResultsAsync(repositoryRoot, "test-passes.json", cancellationToken);
            var passesDict = passesList?.TestResults
                .ToDictionary(r => $"{r.Issue}|{r.Project}", r => r)
                ?? new Dictionary<string, TestResultEntry>();

            var now = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");
            foreach (var entry in promoted)
            {
                var key = $"{entry.Issue}|{entry.Project}";
                passesDict[key] = new TestResultEntry
                {
                    Issue = entry.Issue,
                    Project = entry.Project,
                    LastRun = now,
                    TestResult = "success"
                };
            }

            var updatedPasses = new TestResultList
            {
                TestResults = passesDict.Values.OrderBy(r => r.Issue).ThenBy(r => r.Project).ToList()
            };

            var passesPath = Path.Combine(dataDir, "test-passes.json");
            var passesJson = JsonSerializer.Serialize(updatedPasses, options);
            await File.WriteAllTextAsync(passesPath, passesJson, cancellationToken);
        }
    }

    private async Task ResetPackagesForIssuesAsync(
        string repositoryRoot,
        List<int>? issueNumbers,
        string? previousFeed,
        LogVerbosity verbosity,
        CancellationToken cancellationToken)
    {
        var resetLogger = _loggerFactory.CreateLogger<ResetPackagesCommand>();
        var resetCommand = new ResetPackagesCommand(
            _issueDiscovery,
            _projectAnalyzer,
            resetLogger,
            _markerService);

        await resetCommand.ExecuteAsync(repositoryRoot, issueNumbers, verbosity, cancellationToken);

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

