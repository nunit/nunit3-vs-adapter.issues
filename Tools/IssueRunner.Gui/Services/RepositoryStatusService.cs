using System.Text.Json;
using IssueRunner.Models;
using IssueRunner.Services;
using Microsoft.Extensions.Logging;

namespace IssueRunner.Gui.Services;

/// <summary>
/// Loads repository summary, metadata counts, and package versions.
/// </summary>
public sealed class RepositoryStatusService : IRepositoryStatusService
{
    private readonly IEnvironmentService _environmentService;
    private readonly IIssueDiscoveryService _issueDiscoveryService;
    private readonly IMarkerService _markerService;
    private readonly ITestResultAggregator _aggregator;
    private readonly ILogger<RepositoryStatusService> _logger;

    public RepositoryStatusService(
        IEnvironmentService environmentService,
        IIssueDiscoveryService issueDiscoveryService,
        IMarkerService markerService,
        ITestResultAggregator aggregator,
        ILogger<RepositoryStatusService> logger)
    {
        _environmentService = environmentService;
        _issueDiscoveryService = issueDiscoveryService;
        _markerService = markerService;
        _aggregator = aggregator;
        _logger = logger;
    }

    public async Task<RepositoryLoadResult> LoadAsync(string repositoryPath, Action<string> log)
    {
        var folders = new Dictionary<int, string>();
        var metadataCount = 0;
        var foldersWithoutMetadata = new List<int>();
        var metadataWithoutFolders = new List<int>();
        var baselinePackages = "Not set";
        var currentPackages = "Not set";
        var summaryText = "Invalid repository path. Please select a valid repository folder.";
        var passedCount = 0;
        var failedCount = 0;
        var skippedCount = 0;
        var notRestoredCount = 0;
        var notCompilingCount = 0;
        var notTestedCount = 0;

        try
        {
            // Discover issue folders
            _environmentService.AddRoot(repositoryPath);
            folders = _issueDiscoveryService.DiscoverIssueFolders();

            // Load results for summary counts (per-issue aggregation)
            var dataDir = _environmentService.GetDataDirectory(repositoryPath);
            var resultsPath = Path.Combine(dataDir, "results.json");
            var baselineResultsPath = Path.Combine(dataDir, "results-baseline.json");

            var currentResults = new List<IssueResult>();
            if (File.Exists(resultsPath))
            {
                try
                {
                    var resultsJson = await File.ReadAllTextAsync(resultsPath);
                    currentResults = JsonSerializer.Deserialize<List<IssueResult>>(resultsJson) ?? [];
                }
                catch (Exception ex)
                {
                    log($"Warning: Failed to load results.json for status counts: {ex.Message}");
                }
            }

            var aggregatedCurrent = _aggregator.AggregatePerIssue(folders, currentResults, _markerService, log);

            passedCount = aggregatedCurrent.Count(a => a.Status == AggregatedIssueStatus.Passed);
            failedCount = aggregatedCurrent.Count(a => a.Status == AggregatedIssueStatus.Failed);
            skippedCount = aggregatedCurrent.Count(a => a.Status == AggregatedIssueStatus.Skipped);
            notRestoredCount = aggregatedCurrent.Count(a => a.Status == AggregatedIssueStatus.NotRestored);
            notCompilingCount = aggregatedCurrent.Count(a => a.Status == AggregatedIssueStatus.NotCompiling);
            notTestedCount = aggregatedCurrent.Count(a => a.Status == AggregatedIssueStatus.NotTested);

            var baselinePassedCount = 0;
            var baselineFailedCount = 0;

            if (File.Exists(baselineResultsPath))
            {
                try
                {
                    var baselineResultsJson = await File.ReadAllTextAsync(baselineResultsPath);
                    var baselineResults = JsonSerializer.Deserialize<List<IssueResult>>(baselineResultsJson) ?? [];

                    // Baseline counts should also be per issue using the same rules
                    var aggregatedBaseline = _aggregator.AggregatePerIssue(folders, baselineResults, _markerService, log);
                    baselinePassedCount = aggregatedBaseline.Count(a => a.Status == AggregatedIssueStatus.Passed);
                    baselineFailedCount = aggregatedBaseline.Count(a => a.Status == AggregatedIssueStatus.Failed);
                }
                catch (Exception ex)
                {
                    log($"Warning: Failed to load results-baseline.json: {ex.Message}");
                }
            }

            // Load metadata counts
            var metadataPath = Path.Combine(dataDir, "issues_metadata.json");
            var metadataNumbers = new HashSet<int>();
            if (File.Exists(metadataPath))
            {
                try
                {
                    var metadataJson = await File.ReadAllTextAsync(metadataPath);
                    var metadataList = JsonSerializer.Deserialize<List<IssueMetadata>>(metadataJson) ?? [];
                    metadataNumbers = metadataList.Select(m => m.Number).Distinct().ToHashSet();
                }
                catch (Exception ex)
                {
                    log($"Warning: Failed to load issues_metadata.json: {ex.Message}");
                }
            }

            metadataCount = metadataNumbers.Count;

            // Find folders without metadata
            foldersWithoutMetadata = folders.Keys.Where(n => !metadataNumbers.Contains(n)).OrderBy(n => n).ToList();
            metadataWithoutFolders = metadataNumbers.Where(n => !folders.ContainsKey(n)).OrderBy(n => n).ToList();

            // Calculate folders with metadata (folders that have corresponding metadata entries)
            var foldersWithMetadata = folders.Count - foldersWithoutMetadata.Count;

            // Load package versions
            baselinePackages = LoadPackageVersions(Path.Combine(dataDir, "nunit-packages-baseline.json")) ?? "Not set";
            currentPackages = LoadPackageVersions(Path.Combine(dataDir, "nunit-packages-current.json")) ?? "Not set";

            // Build summary text
            var passedDiff = passedCount - baselinePassedCount;
            var failedDiff = failedCount - baselineFailedCount;
            var passedDiffText = passedDiff >= 0 ? $"+{passedDiff}" : $"{passedDiff}";
            var failedDiffText = failedDiff >= 0 ? $"+{failedDiff}" : $"{failedDiff}";

            var repoConfig = _environmentService.RepositoryConfig;
            var repoInfo = repoConfig != null ? $"{repoConfig.Owner}/{repoConfig.Name}" : "Unknown";

            var passedLine = $"Passed: {passedCount}";
            if (baselinePassedCount > 0 || baselineFailedCount > 0)
            {
                passedLine += $" ({passedDiffText})";
            }

            var failedLine = $"Failed: {failedCount}";
            if (baselinePassedCount > 0 || baselineFailedCount > 0)
            {
                failedLine += $" ({failedDiffText})";
            }

            summaryText = $"Repository: {repoInfo}\n" +
                          $"Issues: {folders.Count} folders, {foldersWithMetadata} with metadata\n" +
                          $"{passedLine}\n" +
                          $"{failedLine}\n" +
                          $"Skipped: {skippedCount}\n" +
                          $"Not Restored: {notRestoredCount}\n" +
                          $"Not Compiling: {notCompilingCount}\n" +
                          $"Not Tested: {notTestedCount}";

            log($"Loaded repository: {repositoryPath}");
            log($"Found {folders.Count} issue folders, {metadataCount} with metadata ({metadataCount - metadataWithoutFolders.Count} central, {metadataWithoutFolders.Count} local only)");
            if (foldersWithoutMetadata.Count > 0)
            {
                log($"Found {foldersWithoutMetadata.Count} folders without metadata");
            }
            if (metadataWithoutFolders.Any())
            {
                log($"Found {metadataWithoutFolders.Count} metadata entries without folders (may have been deleted): {string.Join(", ", metadataWithoutFolders)}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading repository status");
            summaryText = $"Error loading repository: {ex.Message}";
            log($"Error: {ex.Message}");
            if (ex.StackTrace != null)
            {
                log($"Stack: {ex.StackTrace}");
            }
        }

        return new RepositoryLoadResult
        {
            Folders = folders,
            MetadataCount = metadataCount,
            FoldersWithoutMetadata = foldersWithoutMetadata,
            MetadataWithoutFolders = metadataWithoutFolders,
            SummaryText = summaryText,
            BaselineNUnitPackages = baselinePackages,
            CurrentNUnitPackages = currentPackages,
            PassedCount = passedCount,
            FailedCount = failedCount,
            SkippedCount = skippedCount,
            NotRestoredCount = notRestoredCount,
            NotCompilingCount = notCompilingCount,
            NotTestedCount = notTestedCount
        };
    }

    private static string? LoadPackageVersions(string path)
    {
        if (!File.Exists(path))
        {
            return null;
        }

        try
        {
            var json = File.ReadAllText(path);
            var versions = JsonSerializer.Deserialize<NUnitPackageVersions>(json);
            return versions?.Packages is { Count: > 0 } ? FormatPackageVersions(versions.Packages) : "Not set";
        }
        catch
        {
            return "Error loading";
        }
    }

    private static string FormatPackageVersions(Dictionary<string, string> packages)
    {
        return string.Join(", ", packages.Select(kvp => $"{kvp.Key}: {kvp.Value}"));
    }
}

