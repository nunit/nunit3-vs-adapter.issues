using IssueRunner.Models;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace IssueRunner.Services;

/// <summary>
/// Service for comparing test results between baseline and current.
/// </summary>
public sealed class TestResultDiffService : ITestResultDiffService
{
    private readonly IEnvironmentService _environmentService;
    private readonly ILogger<TestResultDiffService> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="TestResultDiffService"/> class.
    /// </summary>
    public TestResultDiffService(
        IEnvironmentService environmentService,
        ILogger<TestResultDiffService> logger)
    {
        _environmentService = environmentService;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<List<TestResultDiff>> CompareResultsAsync(string repositoryRoot)
    {
        var diffs = new List<TestResultDiff>();
        var dataDir = _environmentService.GetDataDirectory(repositoryRoot);

        try
        {
            // Load current results from results.json
            var currentResults = await LoadResultsAsync(dataDir, "results.json");

            // Load baseline results from results-baseline.json
            var baselineResults = await LoadResultsAsync(dataDir, "results-baseline.json");

            // Create dictionaries for easy lookup (key: "Number|ProjectPath")
            // Normalize project path to lowercase for case-insensitive matching
            var currentDict = currentResults
                .ToDictionary(r => $"{r.Number}|{r.ProjectPath.ToLowerInvariant()}", r => r, StringComparer.OrdinalIgnoreCase);

            var baselineDict = baselineResults
                .ToDictionary(r => $"{r.Number}|{r.ProjectPath.ToLowerInvariant()}", r => r, StringComparer.OrdinalIgnoreCase);

            // Get all unique keys from both current and baseline
            var allKeys = currentDict.Keys
                .Union(baselineDict.Keys)
                .Distinct()
                .ToList();

            foreach (var key in allKeys)
            {
                var parts = key.Split('|');
                if (parts.Length != 2)
                {
                    continue;
                }

                if (!int.TryParse(parts[0], out var issueNumber))
                {
                    continue;
                }

                // Project path is already normalized to lowercase in the dictionary key
                var projectPath = parts[1];

                // Determine baseline status from IssueResult.TestResult
                string baselineStatus;
                if (baselineDict.TryGetValue(key, out var baselineResult))
                {
                    baselineStatus = baselineResult.TestResult ?? "not run";
                }
                else
                {
                    baselineStatus = "not run";
                }

                // Determine current status from IssueResult.TestResult
                string currentStatus;
                if (currentDict.TryGetValue(key, out var currentResult))
                {
                    currentStatus = currentResult.TestResult ?? "not run";
                }
                else
                {
                    currentStatus = "not run";
                }

                // Normalize status values for comparison
                baselineStatus = NormalizeStatus(baselineStatus);
                currentStatus = NormalizeStatus(currentStatus);

                // Skip if no change
                if (baselineStatus == currentStatus)
                {
                    continue;
                }

                // Determine change type
                var changeType = DetermineChangeType(baselineStatus, currentStatus);

                // Skip if change type is Skipped (fail -> skipped)
                if (changeType == ChangeType.Skipped)
                {
                    continue;
                }

                diffs.Add(new TestResultDiff
                {
                    IssueNumber = issueNumber,
                    ProjectPath = projectPath,
                    BaselineStatus = baselineStatus,
                    CurrentStatus = currentStatus,
                    ChangeType = changeType
                });
            }

            return diffs;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error comparing test results");
            return diffs;
        }
    }

    /// <summary>
    /// Normalizes test result status for comparison (maps various status values to standard ones).
    /// </summary>
    private static string NormalizeStatus(string status)
    {
        if (string.IsNullOrEmpty(status))
        {
            return "not run";
        }

        // Normalize to standard status values
        var normalized = status.ToLowerInvariant();
        if (normalized == "success" || normalized == "pass")
        {
            return "success";
        }
        if (normalized == "fail" || normalized == "failed")
        {
            return "fail";
        }
        if (normalized == "not run" || normalized == "notrun")
        {
            return "not run";
        }
        if (normalized.Contains("not compiling") || normalized.Contains("compile"))
        {
            return "not compile";
        }

        // For any other status, return as-is (will be treated as "Other" in change detection)
        return normalized;
    }

    private static ChangeType DetermineChangeType(string baselineStatus, string currentStatus)
    {
        // Fixed: Was non-success, now success (Green)
        if (baselineStatus != "success" && currentStatus == "success")
        {
            return ChangeType.Fixed;
        }

        // Regression: Was success, now fail (Red)
        if (baselineStatus == "success" && currentStatus == "fail")
        {
            return ChangeType.Regression;
        }

        // CompileToFail: Was not compile/restore fail, now test fail (Orange)
        // Note: We don't have "not compile" status in test-passes/test-fails, so this might need results.json
        // For now, we'll treat "not run" -> "fail" as CompileToFail
        if ((baselineStatus == "not run" || baselineStatus == "not compile") && currentStatus == "fail")
        {
            return ChangeType.CompileToFail;
        }

        // Skipped: Was fail, now skipped (exclude from list)
        if (baselineStatus == "fail" && currentStatus == "skipped")
        {
            return ChangeType.Skipped;
        }

        // Other: Any other status change (Grey)
        return ChangeType.Other;
    }

    /// <summary>
    /// Loads IssueResult list from results.json or results-baseline.json.
    /// </summary>
    private async Task<List<IssueResult>> LoadResultsAsync(string dataDir, string fileName)
    {
        var filePath = Path.Combine(dataDir, fileName);
        if (!File.Exists(filePath))
        {
            return new List<IssueResult>();
        }

        try
        {
            var json = await File.ReadAllTextAsync(filePath);
            var results = JsonSerializer.Deserialize<List<IssueResult>>(json);
            return results ?? new List<IssueResult>();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to load results from {Path}", filePath);
            return new List<IssueResult>();
        }
    }
}

