using IssueRunner.Models;
using IssueRunner.Services;
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

    public TestStatusViewModel(IEnvironmentService? environmentService = null)
    {
        _environmentService = environmentService;
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
        
        // Convert to TestResultEntry format for display, split by pass/fail
        var currentPasses = allCurrentResults
            .Where(r => r.TestResult == "success")
            .Select(r => new TestResultEntry
            {
                Issue = $"Issue{r.Number}",
                Project = r.ProjectPath,
                LastRun = r.LastRun ?? "",
                TestResult = r.TestResult ?? "success"
            })
            .ToList();
        
        var currentFails = allCurrentResults
            .Where(r => r.TestResult != null && r.TestResult != "success")
            .Select(r => new TestResultEntry
            {
                Issue = $"Issue{r.Number}",
                Project = r.ProjectPath,
                LastRun = r.LastRun ?? "",
                TestResult = r.TestResult ?? "fail"
            })
            .ToList();
        
        CurrentPassed = currentPasses.Count;
        CurrentFailed = currentFails.Count;
        
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
        
        // Convert to TestResultEntry format for display, split by pass/fail
        var baselinePasses = allBaselineResults
            .Where(r => r.TestResult == "success")
            .Select(r => new TestResultEntry
            {
                Issue = $"Issue{r.Number}",
                Project = r.ProjectPath,
                LastRun = r.LastRun ?? "",
                TestResult = r.TestResult ?? "success"
            })
            .ToList();
        
        var baselineFails = allBaselineResults
            .Where(r => r.TestResult != null && r.TestResult != "success")
            .Select(r => new TestResultEntry
            {
                Issue = $"Issue{r.Number}",
                Project = r.ProjectPath,
                LastRun = r.LastRun ?? "",
                TestResult = r.TestResult ?? "fail"
            })
            .ToList();
        
        BaselinePassed = baselinePasses.Count;
        BaselineFailed = baselineFails.Count;
        
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
        
        // Calculate differences
        var currentPassKeys = currentPasses.Select(e => $"{e.Issue}/{e.Project}").ToHashSet();
        var currentFailKeys = currentFails.Select(e => $"{e.Issue}/{e.Project}").ToHashSet();
        var baselinePassKeys = baselinePasses.Select(e => $"{e.Issue}/{e.Project}").ToHashSet();
        var baselineFailKeys = baselineFails.Select(e => $"{e.Issue}/{e.Project}").ToHashSet();
        
        // New passes: in current passes but not in baseline passes
        NewPasses.Clear();
        foreach (var entry in currentPasses)
        {
            var key = $"{entry.Issue}/{entry.Project}";
            if (!baselinePassKeys.Contains(key))
            {
                NewPasses.Add(entry);
            }
        }
        
        // New fails: in current fails but not in baseline fails
        NewFails.Clear();
        foreach (var entry in currentFails)
        {
            var key = $"{entry.Issue}/{entry.Project}";
            if (!baselineFailKeys.Contains(key))
            {
                NewFails.Add(entry);
            }
        }
        
        // Regressions: in baseline passes but now in current fails
        Regressions.Clear();
        foreach (var entry in baselinePasses)
        {
            var key = $"{entry.Issue}/{entry.Project}";
            if (currentFailKeys.Contains(key))
            {
                Regressions.Add(entry);
            }
        }
        
        // Fixed: in baseline fails but now in current passes
        Fixed.Clear();
        foreach (var entry in baselineFails)
        {
            var key = $"{entry.Issue}/{entry.Project}";
            if (currentPassKeys.Contains(key))
            {
                Fixed.Add(entry);
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





