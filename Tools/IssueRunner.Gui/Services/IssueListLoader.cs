using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using IssueRunner.Gui.ViewModels;
using IssueRunner.Models;
using IssueRunner.Services;

namespace IssueRunner.Gui.Services;

/// <summary>
/// Default implementation of <see cref="IIssueListLoader"/> that builds issue list items
/// from metadata, results, diffs, and marker files.
/// </summary>
public sealed class IssueListLoader : IIssueListLoader
{
    private readonly IEnvironmentService _environmentService;
    private readonly ITestExecutionService _testExecutionService;
    private readonly IProjectAnalyzerService _projectAnalyzerService;
    private readonly ITestResultDiffService _diffService;
    private readonly IMarkerService _markerService;

    public IssueListLoader(
        IEnvironmentService environmentService,
        ITestExecutionService testExecutionService,
        IProjectAnalyzerService projectAnalyzerService,
        ITestResultDiffService diffService,
        IMarkerService markerService)
    {
        _environmentService = environmentService;
        _testExecutionService = testExecutionService;
        _projectAnalyzerService = projectAnalyzerService;
        _diffService = diffService;
        _markerService = markerService;
    }

    public async Task<IssueListLoadResult> LoadIssuesAsync(
        string repositoryRoot,
        Dictionary<int, string> folders,
        Action<string>? log = null)
    {
        var issues = new List<IssueListItem>();

        // Get repository config for GitHub URL generation
        var repoConfig = _environmentService.RepositoryConfig;
        var baseUrl = !string.IsNullOrEmpty(repoConfig.Owner) && !string.IsNullOrEmpty(repoConfig.Name)
            ? $"https://github.com/{repoConfig.Owner}/{repoConfig.Name}/issues/"
            : "";

        // Load metadata
        var dataDir = _environmentService.GetDataDirectory(repositoryRoot);
        var metadataPath = Path.Combine(dataDir, "issues_metadata.json");
        var metadataDict = new Dictionary<int, IssueMetadata>();

        log?.Invoke(
            $"Debug: Checking metadata at {metadataPath}, RepositoryPath={repositoryRoot}, DataDir={dataDir}, Exists={File.Exists(metadataPath)}");

        if (File.Exists(metadataPath))
        {
            try
            {
                var metadataJson = await File.ReadAllTextAsync(metadataPath);
                var metadata = JsonSerializer.Deserialize<List<IssueMetadata>>(metadataJson) ?? [];

                // Handle duplicate issue numbers gracefully
                var duplicates = metadata.GroupBy(m => m.Number).Where(g => g.Count() > 1).ToList();
                if (duplicates.Any())
                {
                    foreach (var dup in duplicates)
                    {
                        log?.Invoke(
                            $"Warning: Duplicate metadata entries found for issue {dup.Key}. Using the last occurrence.");
                    }
                }

                // Use GroupBy().ToDictionary() to take the last occurrence of each duplicate
                metadataDict = metadata
                    .GroupBy(m => m.Number)
                    .ToDictionary(g => g.Key, g => g.Last());

                // Count how many have titles
                var titlesCount = metadataDict.Values.Count(m => !string.IsNullOrWhiteSpace(m.Title));
                log?.Invoke(
                    $"Loaded {metadataDict.Count} issue metadata entries from {metadataPath} ({titlesCount} with titles)");
            }
            catch (Exception ex)
            {
                log?.Invoke($"Warning: Could not load metadata: {ex.Message}");
            }
        }
        else
        {
            log?.Invoke($"Warning: Metadata file not found at {metadataPath}");
        }

        // Load test results from results.json
        var resultsPath = Path.Combine(dataDir, "results.json");
        var resultsByIssue = new Dictionary<int, (string Result, string LastRun)>();
        var failedRestores = new HashSet<int>(); // Track issues with restore failures from results.json
        var failedBuilds = new HashSet<int>(); // Track issues with build failures from results.json
        var restoreErrors = new Dictionary<int, string>(); // Track restore error messages by issue number

        if (File.Exists(resultsPath))
        {
            try
            {
                var resultsJson = await File.ReadAllTextAsync(resultsPath);
                var allResults = JsonSerializer.Deserialize<List<IssueResult>>(resultsJson);
                if (allResults != null)
                {
                    // Group results by issue number
                    var resultsByIssueNumber = allResults
                        .GroupBy(r => r.Number)
                        .ToDictionary(g => g.Key, g => g.ToList());

                    // For each issue, determine the worst result (fail > not run > success)
                    foreach (var kvp in resultsByIssueNumber)
                    {
                        var issueNum = kvp.Key;
                        var issueResults = kvp.Value;

                        // Check if this issue has any restore failures
                        // A restore failure is indicated by RestoreResult == "fail" OR RestoreError having content
                        var restoreFailure = issueResults.FirstOrDefault(r =>
                            r.RestoreResult is "fail" ||
                            !string.IsNullOrWhiteSpace(r.RestoreError));
                        if (restoreFailure != null)
                        {
                            failedRestores.Add(issueNum);
                            // Store the restore error message if available
                            if (!string.IsNullOrWhiteSpace(restoreFailure.RestoreError))
                            {
                                restoreErrors[issueNum] = restoreFailure.RestoreError;
                            }
                        }
                        // Check for build failures (only if restore didn't fail)
                        else if (issueResults.Any(r => r.BuildResult is "fail"))
                        {
                            failedBuilds.Add(issueNum);
                        }

                        // Determine worst result and most recent timestamp
                        IssueResult? worstResult = null;
                        string? worstStatus = null;
                        string? lastRun = null;

                        foreach (var result in issueResults)
                        {
                            var status = result.TestResult ?? "not run";
                            var resultLastRun = result.LastRun;

                            // Determine priority: fail > not run > success
                            var isWorse = false;
                            if (worstStatus == null || (status == "fail" && worstStatus != "fail"))
                            {
                                isWorse = true;
                            }
                            else if (status == "not run" && worstStatus == "success")
                            {
                                isWorse = true;
                            }
                            // If same priority, use most recent
                            else if (status == worstStatus &&
                                     !string.IsNullOrEmpty(resultLastRun) &&
                                     !string.IsNullOrEmpty(lastRun) &&
                                     string.Compare(resultLastRun, lastRun, StringComparison.Ordinal) > 0)
                            {
                                isWorse = true;
                            }

                            if (isWorse)
                            {
                                worstResult = result;
                                worstStatus = status;
                                lastRun = resultLastRun;
                            }
                        }

                        if (worstResult != null)
                        {
                            resultsByIssue[issueNum] = (worstStatus ?? "not run", lastRun ?? "");
                        }
                    }
                }
            }
            catch
            {
                // Ignore result loading errors for now
            }
        }

        // Load diff data
        var diffs = await _diffService.CompareResultsAsync(repositoryRoot);
        var issueChanges = new Dictionary<string, ChangeType>();
        var issueStatusDisplay = new Dictionary<int, string>();

        foreach (var diff in diffs)
        {
            var key = $"Issue{diff.IssueNumber}|{diff.ProjectPath}";
            issueChanges[key] = diff.ChangeType;

            issueStatusDisplay[diff.IssueNumber] = diff.ChangeType switch
            {
                // Set StatusDisplay for regressions (show "=> fail")
                ChangeType.Regression => "=> fail",
                ChangeType.Fixed or ChangeType.CompileToFail or ChangeType.Other => diff.CurrentStatus,
                _ => issueStatusDisplay[diff.IssueNumber]
            };
        }

        log?.Invoke($"Discovered {folders.Count} issue folders to load");

        if (folders.Count == 0)
        {
            log?.Invoke("Warning: No issue folders found. The IssueListView will be empty.");
        }

        // Build issue list
        foreach (var kvp in folders.OrderBy(k => k.Key))
        {
            var issueNum = kvp.Key;
            var folderPath = kvp.Value;
            var metadata = metadataDict.GetValueOrDefault(issueNum);

            var result = resultsByIssue.TryGetValue(issueNum, out var r) ? r : (Result: "Not tested", LastRun: "");

            var lastRunDate = "";
            if (!string.IsNullOrEmpty(result.LastRun) &&
                DateTime.TryParse(result.LastRun, out var dt))
            {
                lastRunDate = dt.ToString("yyyy-MM-dd HH:mm");
            }

            // Determine TestTypes based on whether issue has custom scripts
            var hasCustomScripts = _testExecutionService.HasCustomRunners(folderPath);
            // Column display values: "Scripts" or "DotNet test"
            var testTypes = hasCustomScripts ? "Scripts" : "DotNet test";

            // Determine Framework type (.Net or .Net Framework)
            var framework = "";
            try
            {
                var projectFiles = _projectAnalyzerService.FindProjectFiles(folderPath);
                var hasNetFx = false;
                var hasNet = false;

                foreach (var projectFile in projectFiles)
                {
                    var (targetFrameworks, _) = _projectAnalyzerService.ParseProjectFile(projectFile);
                    foreach (var tfm in targetFrameworks)
                    {
                        // Check if it's .NET Framework (net35, net40, net45, net451, net452, net46, net461, net462, net47, net471, net472, net48, net481)
                        if (tfm.StartsWith("net", StringComparison.OrdinalIgnoreCase) &&
                            !tfm.StartsWith("netcoreapp", StringComparison.OrdinalIgnoreCase) &&
                            !tfm.StartsWith("netstandard", StringComparison.OrdinalIgnoreCase))
                        {
                            // Check if it's a numeric framework (net35, net40, etc.) or net4xx
                            var tfmLower = tfm.ToLowerInvariant();
                            if (tfmLower == "net35" || tfmLower == "net40" ||
                                tfmLower.StartsWith("net4") || tfmLower.StartsWith("net3"))
                            {
                                hasNetFx = true;
                            }
                            else if (tfmLower.StartsWith("net5") || tfmLower.StartsWith("net6") ||
                                     tfmLower.StartsWith("net7") || tfmLower.StartsWith("net8") ||
                                     tfmLower.StartsWith("net9") || tfmLower.StartsWith("net10"))
                            {
                                hasNet = true;
                            }
                        }
                    }
                }

                // Prioritize .NET Framework if both are present
                if (hasNetFx)
                {
                    framework = ".Net Framework";
                }
                else if (hasNet)
                {
                    framework = ".Net";
                }
                // If no projects found or no frameworks detected, leave empty (will show in "All")
            }
            catch
            {
                // If framework detection fails, leave empty
            }

            // Determine state value and detailed state
            // State logic: New (metadata only, just arrived from GitHub), Synced (has metadata and folder),
            // Failed restore/compile (from results.json build_result or restore_result),
            // Runnable (ready to run), Skipped (has marker file)
            IssueState stateValue;
            var detailedState = metadata?.State ?? "Unknown";
            string? notTestedReason = null;

            if (_markerService.ShouldSkipIssue(folderPath))
            {
                var markerReason = _markerService.GetMarkerReason(folderPath);
                detailedState = "skipped";
                stateValue = IssueState.Skipped;
                // Always show marker reason for skipped issues, regardless of test result
                notTestedReason = markerReason;
            }
            else if (failedRestores.Contains(issueNum))
            {
                // failedRestores is populated from results.json (checks restore_result == "fail" or RestoreError)
                stateValue = IssueState.FailedRestore;
                detailedState = "not restored";
                // Include restore error message if available
                if (restoreErrors.TryGetValue(issueNum, out var restoreError))
                {
                    notTestedReason = $"Restore failed: {restoreError.Trim()}";
                }
                else
                {
                    notTestedReason = "Restore failed";
                }
            }
            else if (failedBuilds.Contains(issueNum))
            {
                // failedBuilds is populated from results.json (checks build_result == "fail")
                stateValue = IssueState.FailedCompile;
                detailedState = "not compiling";
                if (result.Result == "Not tested" || string.IsNullOrEmpty(result.Result))
                {
                    notTestedReason = "Not compiling";
                }
            }
            else if (metadata != null)
            {
                // No test results - could be New (just synced) or Synced (ready to process)
                // For now, if it has metadata, consider it Synced (has been synced with GitHub and folders)
                // Has metadata - determine state based on test results
                // New: just synced from GitHub, no test results yet
                // Synced: has metadata and folder, ready to process
                // Runnable: has been tested (has test result)
                stateValue = string.IsNullOrEmpty(result.Result) || result.Result == "Not tested"
                    ? IssueState.Synced
                    : IssueState.Runnable;
            }
            else
            {
                // No metadata - created before, synced with GitHub and folders
                stateValue = IssueState.Synced;
            }

            // Get title from metadata (metadata always has titles)
            var issueTitle = metadata?.Title ?? $"Issue {issueNum}";

            // Check if this issue has a change
            var changeType = ChangeType.None;
            string? statusDisplay = null; // Null by default, will use TestResult via TargetNullValue, or set if there's a change

            // Find matching diff entry (match by issue number)
            // Note: We match by issue number only since we don't track project path in IssueListItem
            var matchingDiff = diffs.FirstOrDefault(d => d.IssueNumber == issueNum);
            if (matchingDiff != null)
            {
                changeType = matchingDiff.ChangeType;
                statusDisplay = matchingDiff.ChangeType switch
                {
                    ChangeType.Regression => "=> fail",
                    // For other changes, show the current status
                    ChangeType.Fixed or ChangeType.CompileToFail or ChangeType.Other =>
                        matchingDiff.CurrentStatus,
                    _ => statusDisplay
                };
            }

            issues.Add(new IssueListItem
            {
                Number = issueNum,
                Title = issueTitle,
                State = metadata?.State ?? "Unknown",
                StateValue = stateValue,
                DetailedState = detailedState,
                TestResult = result.Result,
                LastRun = lastRunDate,
                NotTestedReason = notTestedReason,
                TestTypes = testTypes,
                GitHubUrl = !string.IsNullOrEmpty(baseUrl) ? $"{baseUrl}{issueNum}" : "",
                Framework = framework,
                ChangeType = changeType,
                StatusDisplay = statusDisplay
            });
        }

        return new IssueListLoadResult
        {
            Issues = issues,
            IssueChanges = issueChanges,
            RepositoryBaseUrl = baseUrl
        };
    }
}

