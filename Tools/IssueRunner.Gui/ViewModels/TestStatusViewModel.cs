using IssueRunner.Models;
using IssueRunner.Services;
using IssueRunner.Gui.Services;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace IssueRunner.Gui.ViewModels;

/// <summary>
/// ViewModel for the Test Status page showing baseline comparison.
/// </summary>
public class TestStatusViewModel : ViewModelBase
{
    private int _currentPassed = 0;
    private int _currentFailed = 0;
    private int _baselinePassed = 0;
    private int _baselineFailed = 0;
    private int _passedDiff = 0;
    private int _failedDiff = 0;
    private string _baselineDate = "Not set";
    private string _repositoryRoot = "";
    private readonly IEnvironmentService? _environmentService;
    private readonly IIssueDiscoveryService? _issueDiscoveryService;
    private readonly IMarkerService? _markerService;
    private readonly ITestResultAggregator? _aggregator;

    public TestStatusViewModel(
        IEnvironmentService? environmentService = null,
        IIssueDiscoveryService? issueDiscoveryService = null,
        IMarkerService? markerService = null,
        ITestResultAggregator? aggregator = null)
    {
        _environmentService = environmentService;
        _issueDiscoveryService = issueDiscoveryService;
        _markerService = markerService;
        _aggregator = aggregator;
        CurrentResults = new ObservableCollection<TestResultEntry>();
        BaselineResults = new ObservableCollection<TestResultEntry>();
        NewPasses = new ObservableCollection<TestResultEntry>();
        NewFails = new ObservableCollection<TestResultEntry>();
        Regressions = new ObservableCollection<TestResultEntry>();
        Fixed = new ObservableCollection<TestResultEntry>();
    }

    public int CurrentPassed
    {
        get => _currentPassed;
        set
        {
            if (SetProperty(ref _currentPassed, value))
            {
                UpdateDiffs();
            }
        }
    }

    public int CurrentFailed
    {
        get => _currentFailed;
        set
        {
            if (SetProperty(ref _currentFailed, value))
            {
                UpdateDiffs();
            }
        }
    }

    public int BaselinePassed
    {
        get => _baselinePassed;
        set
        {
            if (SetProperty(ref _baselinePassed, value))
            {
                UpdateDiffs();
            }
        }
    }

    public int BaselineFailed
    {
        get => _baselineFailed;
        set
        {
            if (SetProperty(ref _baselineFailed, value))
            {
                UpdateDiffs();
            }
        }
    }

    public int PassedDiff
    {
        get => _passedDiff;
        private set => SetProperty(ref _passedDiff, value);
    }

    public int FailedDiff
    {
        get => _failedDiff;
        private set => SetProperty(ref _failedDiff, value);
    }

    public string BaselineDate
    {
        get => _baselineDate;
        set => SetProperty(ref _baselineDate, value);
    }

    public string PassedDiffText => PassedDiff >= 0 ? $"+{PassedDiff}" : $"{PassedDiff}";
    public string FailedDiffText => FailedDiff >= 0 ? $"+{FailedDiff}" : $"{FailedDiff}";
    public string PassedDiffColor => PassedDiff > 0 ? "#4CAF50" : PassedDiff < 0 ? "#F44336" : "#666";
    public string FailedDiffColor => FailedDiff < 0 ? "#4CAF50" : FailedDiff > 0 ? "#F44336" : "#666";

    public ObservableCollection<TestResultEntry> CurrentResults { get; }
    public ObservableCollection<TestResultEntry> BaselineResults { get; }
    public ObservableCollection<TestResultEntry> NewPasses { get; }
    public ObservableCollection<TestResultEntry> NewFails { get; }
    public ObservableCollection<TestResultEntry> Regressions { get; }
    public ObservableCollection<TestResultEntry> Fixed { get; }

    private void UpdateDiffs()
    {
        PassedDiff = CurrentPassed - BaselinePassed;
        FailedDiff = CurrentFailed - BaselineFailed;
        OnPropertyChanged(nameof(PassedDiffText));
        OnPropertyChanged(nameof(FailedDiffText));
        OnPropertyChanged(nameof(PassedDiffColor));
        OnPropertyChanged(nameof(FailedDiffColor));
    }

    public async Task LoadDataAsync(string repositoryRoot)
    {
        _repositoryRoot = repositoryRoot;
        
        // Get data directory (creates it if it doesn't exist)
        var dataDir = _environmentService?.GetDataDirectory(repositoryRoot);
        if (dataDir == null)
        {
            dataDir = Path.Combine(repositoryRoot, ".nunit", "IssueRunner");
            if (!Directory.Exists(dataDir))
            {
                Directory.CreateDirectory(dataDir);
            }
        }
        
        // Load metadata to get issue titles
        var metadataPath = Path.Combine(dataDir, "issues_metadata.json");
        var metadataDict = new Dictionary<int, IssueMetadata>();
        if (File.Exists(metadataPath))
        {
            try
            {
                var metadataJson = await File.ReadAllTextAsync(metadataPath);
                var metadata = JsonSerializer.Deserialize<List<IssueMetadata>>(metadataJson) ?? [];
                // Handle duplicates by taking the last occurrence
                metadataDict = metadata
                    .GroupBy(m => m.Number)
                    .ToDictionary(g => g.Key, g => g.Last());
            }
            catch { /* Ignore metadata loading errors */ }
        }
        
        // Helper function to get issue display name from metadata or fallback to "Issue{Number}"
        string GetIssueDisplayName(int issueNumber)
        {
            if (metadataDict.TryGetValue(issueNumber, out var metadata) && 
                !string.IsNullOrWhiteSpace(metadata.Title))
            {
                return metadata.Title;
            }
            return $"Issue{issueNumber}";
        }
        
        // Helper to extract issue number from Issue string (either "Issue{Number}" or title)
        int ExtractIssueNumber(string issueDisplay)
        {
            // Try to extract from "Issue{Number}" format first
            if (issueDisplay.StartsWith("Issue", StringComparison.OrdinalIgnoreCase))
            {
                var numberStr = issueDisplay.Substring(5); // "Issue".Length
                if (int.TryParse(numberStr, out var num))
                {
                    return num;
                }
            }
            // If it's a title, look it up in metadata
            var metadataEntry = metadataDict.FirstOrDefault(kvp => kvp.Value.Title == issueDisplay);
            if (metadataEntry.Key != 0)
            {
                return metadataEntry.Key;
            }
            return 0;
        }
        
        // Load current results from results.json
        var resultsPath = Path.Combine(dataDir, "results.json");
        var allCurrentResults = new List<IssueResult>();
        
        if (File.Exists(resultsPath))
        {
            try
            {
                var json = await File.ReadAllTextAsync(resultsPath);
                var results = JsonSerializer.Deserialize<List<IssueResult>>(json);
                if (results != null)
                {
                    allCurrentResults = results;
                }
            }
            catch { }
        }
        
        // Load baseline results from results-baseline.json
        var baselineResultsPath = Path.Combine(dataDir, "results-baseline.json");
        var allBaselineResults = new List<IssueResult>();
        
        if (File.Exists(baselineResultsPath))
        {
            try
            {
                var json = await File.ReadAllTextAsync(baselineResultsPath);
                var results = JsonSerializer.Deserialize<List<IssueResult>>(json);
                if (results != null)
                {
                    allBaselineResults = results;
                }
            }
            catch { }
        }
        
        List<TestResultEntry> currentPasses;
        List<TestResultEntry> currentFails;
        List<TestResultEntry> baselinePasses;
        List<TestResultEntry> baselineFails;
        Dictionary<int, TestResultEntry> currentPassDict = new();
        Dictionary<int, TestResultEntry> currentFailDict = new();
        Dictionary<int, TestResultEntry> baselinePassDict = new();
        Dictionary<int, TestResultEntry> baselineFailDict = new();

        // Prefer per-issue aggregation when all services are available
        if (_aggregator != null && _issueDiscoveryService != null && _markerService != null)
        {
            var folders = _issueDiscoveryService.DiscoverIssueFolders();

            var aggregatedCurrent = _aggregator.AggregatePerIssue(
                folders,
                allCurrentResults,
                _markerService);

            var aggregatedBaseline = _aggregator.AggregatePerIssue(
                folders,
                allBaselineResults,
                _markerService);

            CurrentPassed = aggregatedCurrent.Count(a => a.Status == AggregatedIssueStatus.Passed);
            CurrentFailed = aggregatedCurrent.Count(a => a.Status == AggregatedIssueStatus.Failed);

            BaselinePassed = aggregatedBaseline.Count(a => a.Status == AggregatedIssueStatus.Passed);
            BaselineFailed = aggregatedBaseline.Count(a => a.Status == AggregatedIssueStatus.Failed);

            // Map aggregated results to TestResultEntry for display (per issue)
            static string MapStatusToResult(AggregatedIssueStatus status) =>
                status switch
                {
                    AggregatedIssueStatus.Passed => "success",
                    AggregatedIssueStatus.Failed => "fail",
                    AggregatedIssueStatus.Skipped => "skipped",
                    AggregatedIssueStatus.NotRestored => "not restored",
                    AggregatedIssueStatus.NotCompiling => "not compiling",
                    _ => "Not tested"
                };

            var currentPassesWithNumbers = aggregatedCurrent
                .Where(a => a.Status == AggregatedIssueStatus.Passed)
                .Select(a => new { Number = a.Number, Entry = new TestResultEntry
                {
                    Issue = GetIssueDisplayName(a.Number),
                    Project = "",
                    LastRun = a.LastRun ?? "",
                    TestResult = "success"
                }})
                .ToList();
            currentPassDict = currentPassesWithNumbers.ToDictionary(x => x.Number, x => x.Entry);
            currentPasses = currentPassesWithNumbers.Select(x => x.Entry).ToList();

            var currentFailsWithNumbers = aggregatedCurrent
                .Where(a => a.Status != AggregatedIssueStatus.Passed)
                .Select(a => new { Number = a.Number, Entry = new TestResultEntry
                {
                    Issue = GetIssueDisplayName(a.Number),
                    Project = "",
                    LastRun = a.LastRun ?? "",
                    TestResult = MapStatusToResult(a.Status)
                }})
                .ToList();
            currentFailDict = currentFailsWithNumbers.ToDictionary(x => x.Number, x => x.Entry);
            currentFails = currentFailsWithNumbers.Select(x => x.Entry).ToList();

            var baselinePassesWithNumbers = aggregatedBaseline
                .Where(a => a.Status == AggregatedIssueStatus.Passed)
                .Select(a => new { Number = a.Number, Entry = new TestResultEntry
                {
                    Issue = GetIssueDisplayName(a.Number),
                    Project = "",
                    LastRun = a.LastRun ?? "",
                    TestResult = "success"
                }})
                .ToList();
            baselinePassDict = baselinePassesWithNumbers.ToDictionary(x => x.Number, x => x.Entry);
            baselinePasses = baselinePassesWithNumbers.Select(x => x.Entry).ToList();

            var baselineFailsWithNumbers = aggregatedBaseline
                .Where(a => a.Status != AggregatedIssueStatus.Passed)
                .Select(a => new { Number = a.Number, Entry = new TestResultEntry
                {
                    Issue = GetIssueDisplayName(a.Number),
                    Project = "",
                    LastRun = a.LastRun ?? "",
                    TestResult = MapStatusToResult(a.Status)
                }})
                .ToList();
            baselineFailDict = baselineFailsWithNumbers.ToDictionary(x => x.Number, x => x.Entry);
            baselineFails = baselineFailsWithNumbers.Select(x => x.Entry).ToList();
        }
        else
        {
            // Fallback: per-row counting (legacy behavior, used in tests without DI)
            currentPasses = allCurrentResults
                .Where(r => r.TestResult == "success")
                .Select(r => new TestResultEntry
                {
                    Issue = GetIssueDisplayName(r.Number),
                    Project = r.ProjectPath,
                    LastRun = r.LastRun ?? "",
                    TestResult = r.TestResult ?? "success"
                })
                .ToList();

            currentFails = allCurrentResults
                .Where(r => r.TestResult != null && r.TestResult != "success")
                .Select(r => new TestResultEntry
                {
                    Issue = GetIssueDisplayName(r.Number),
                    Project = r.ProjectPath,
                    LastRun = r.LastRun ?? "",
                    TestResult = r.TestResult ?? "fail"
                })
                .ToList();

            CurrentPassed = currentPasses.Count;
            CurrentFailed = currentFails.Count;

            baselinePasses = allBaselineResults
                .Where(r => r.TestResult == "success")
                .Select(r => new TestResultEntry
                {
                    Issue = GetIssueDisplayName(r.Number),
                    Project = r.ProjectPath,
                    LastRun = r.LastRun ?? "",
                    TestResult = r.TestResult ?? "success"
                })
                .ToList();

            baselineFails = allBaselineResults
                .Where(r => r.TestResult != null && r.TestResult != "success")
                .Select(r => new TestResultEntry
                {
                    Issue = GetIssueDisplayName(r.Number),
                    Project = r.ProjectPath,
                    LastRun = r.LastRun ?? "",
                    TestResult = r.TestResult ?? "fail"
                })
                .ToList();

            BaselinePassed = baselinePasses.Count;
            BaselineFailed = baselineFails.Count;
            
            // Create dictionaries for fallback path too
            currentPassDict = currentPasses
                .Select(e => new { Number = ExtractIssueNumber(e.Issue), Entry = e })
                .Where(x => x.Number > 0)
                .ToDictionary(x => x.Number, x => x.Entry);
            currentFailDict = currentFails
                .Select(e => new { Number = ExtractIssueNumber(e.Issue), Entry = e })
                .Where(x => x.Number > 0)
                .ToDictionary(x => x.Number, x => x.Entry);
            baselinePassDict = baselinePasses
                .Select(e => new { Number = ExtractIssueNumber(e.Issue), Entry = e })
                .Where(x => x.Number > 0)
                .ToDictionary(x => x.Number, x => x.Entry);
            baselineFailDict = baselineFails
                .Select(e => new { Number = ExtractIssueNumber(e.Issue), Entry = e })
                .Where(x => x.Number > 0)
                .ToDictionary(x => x.Number, x => x.Entry);
        }
        
        // Get baseline date from file modification time or from first result's LastRun
        if (File.Exists(baselineResultsPath))
        {
            var fileInfo = new FileInfo(baselineResultsPath);
            BaselineDate = fileInfo.LastWriteTime.ToString("yyyy-MM-dd HH:mm");
        }
        else if (allBaselineResults.Count > 0 && !string.IsNullOrEmpty(allBaselineResults[0].LastRun))
        {
            if (DateTime.TryParse(allBaselineResults[0].LastRun, out var dt))
            {
                BaselineDate = dt.ToString("yyyy-MM-dd HH:mm");
            }
            else
            {
                BaselineDate = "Not set";
            }
        }
        else
        {
            BaselineDate = "Not set";
        }
        
        // Calculate differences (per issue; project is informational only)
        // Use issue numbers for comparison (from aggregated results)
        var currentPassNumbers = currentPassDict.Keys.ToHashSet();
        var currentFailNumbers = currentFailDict.Keys.ToHashSet();
        var baselinePassNumbers = baselinePassDict.Keys.ToHashSet();
        var baselineFailNumbers = baselineFailDict.Keys.ToHashSet();
        
        // New passes: in current passes but not in baseline passes
        NewPasses.Clear();
        foreach (var kvp in currentPassDict)
        {
            if (!baselinePassNumbers.Contains(kvp.Key))
            {
                NewPasses.Add(kvp.Value);
            }
        }
        
        // New fails: in current fails but not in baseline fails
        NewFails.Clear();
        foreach (var kvp in currentFailDict)
        {
            if (!baselineFailNumbers.Contains(kvp.Key))
            {
                NewFails.Add(kvp.Value);
            }
        }
        
        // Regressions: in baseline passes but now in current fails
        Regressions.Clear();
        foreach (var kvp in baselinePassDict)
        {
            if (currentFailNumbers.Contains(kvp.Key))
            {
                Regressions.Add(kvp.Value);
            }
        }
        
        // Fixed: in baseline fails but now in current passes
        Fixed.Clear();
        foreach (var kvp in baselineFailDict)
        {
            if (currentPassNumbers.Contains(kvp.Key))
            {
                Fixed.Add(kvp.Value);
            }
        }
        
        // Update collections
        CurrentResults.Clear();
        foreach (var entry in currentPasses.Concat(currentFails).OrderBy(e => e.Issue))
        {
            CurrentResults.Add(entry);
        }
        
        BaselineResults.Clear();
        foreach (var entry in baselinePasses.Concat(baselineFails).OrderBy(e => e.Issue))
        {
            BaselineResults.Add(entry);
        }
    }

    public async Task SetBaselineAsync()
    {
        if (string.IsNullOrEmpty(_repositoryRoot)) return;
        
        // Get data directory (creates it if it doesn't exist)
        var dataDir = _environmentService?.GetDataDirectory(_repositoryRoot);
        if (dataDir == null)
        {
            dataDir = Path.Combine(_repositoryRoot, ".nunit", "IssueRunner");
            if (!Directory.Exists(dataDir))
            {
                Directory.CreateDirectory(dataDir);
            }
        }
        
        var resultsPath = Path.Combine(dataDir, "results.json");
        var baselineResultsPath = Path.Combine(dataDir, "results-baseline.json");
        
        try
        {
            // Copy current results.json to results-baseline.json
            if (File.Exists(resultsPath))
            {
                File.Copy(resultsPath, baselineResultsPath, overwrite: true);
            }
            else
            {
                throw new Exception("No test results found to set as baseline.");
            }
            
            // Reload data to update baseline info
            await LoadDataAsync(_repositoryRoot);
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to set baseline: {ex.Message}", ex);
        }
    }
}





