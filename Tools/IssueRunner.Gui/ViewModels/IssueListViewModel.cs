using System.Collections.ObjectModel;
using IssueRunner.Models;
using ReactiveUI;
using System.Reactive;
using System.Reactive.Linq;
using System.Diagnostics;

namespace IssueRunner.Gui.ViewModels;

/// <summary>
/// View model for the issue list view.
/// </summary>
public class IssueListViewModel : ViewModelBase
{
    private readonly ObservableCollection<IssueListItem> _allIssues = new();
    private readonly ObservableCollection<IssueListItem> _filteredIssues = new();
    private Func<List<int>, Task>? _runTestsCallback;
    private Func<Task>? _showOptionsCallback;
    private string _repositoryBaseUrl = "";
    private Dictionary<string, ChangeType> _issueChanges = new();

    public IssueListViewModel()
    {
        AllIssues = new ReadOnlyObservableCollection<IssueListItem>(_allIssues);
        Issues = new ReadOnlyObservableCollection<IssueListItem>(_filteredIssues);
        
        // Update filtered list when filters change
        this.WhenAnyValue(x => x.SelectedScope, x => x.SelectedState, x => x.SelectedTestResult, x => x.SelectedTestTypes, x => x.SelectedFramework, x => x.ShowDiffOnly)
            .Subscribe(_ => ApplyFilters());
        
        ToggleDiffCommand = ReactiveCommand.Create(() =>
        {
            ShowDiffOnly = !ShowDiffOnly;
        });
        
        // RunTestsCommand can execute whenever there is at least one issue in the filtered list or issue numbers are specified.
        RunTestsCommand = ReactiveCommand.CreateFromTask(
            RunTestsAsync,
            this.WhenAnyValue(x => x.FilteredIssueCount, x => x.IssueNumbers)
                .Select(t => t.Item1 > 0 || !string.IsNullOrWhiteSpace(t.Item2)),
            RxApp.MainThreadScheduler);
        
        ShowOptionsCommand = ReactiveCommand.CreateFromTask(ShowOptionsAsync);
        
        // Command to open GitHub issue URL
        OpenIssueCommand = ReactiveCommand.Create<int>(OpenIssueUrl);
        
        // Command to clear issue numbers
        ClearIssueNumbersCommand = ReactiveCommand.Create(() =>
        {
            IssueNumbers = "";
        });
    }

    public ReactiveCommand<Unit, Unit> RunTestsCommand { get; }
    public ReactiveCommand<Unit, Unit> ShowOptionsCommand { get; }
    public ReactiveCommand<int, Unit> OpenIssueCommand { get; }
    public ReactiveCommand<Unit, Unit> ClearIssueNumbersCommand { get; }
    public ReactiveCommand<Unit, Unit> ToggleDiffCommand { get; }

    public ReactiveCommand<Unit, Unit>? SyncFromGitHubCommand
    {
        get;
        set => SetProperty(ref field, value);
    }

    public ReadOnlyObservableCollection<IssueListItem> AllIssues { get; }
    public ReadOnlyObservableCollection<IssueListItem> Issues { get; }

    public TestScope SelectedScope
    {
        get;
        set => SetProperty(ref field, value);
    } = TestScope.All;

    public string SelectedState
    {
        get;
        set => SetProperty(ref field, value);
    } = "All";

    public string SelectedTestResult
    {
        get;
        set => SetProperty(ref field, value);
    } = "All";

    public string SelectedTestTypes
    {
        get;
        set => SetProperty(ref field, value);
    } = "All";

    public string SelectedFramework
    {
        get;
        set => SetProperty(ref field, value);
    } = "All";

    public IEnumerable<TestScope> AvailableScopes => Enum.GetValues<TestScope>();
    public IEnumerable<string> AvailableStates => new[] { "All", "New", "Synced", "Failed restore", "Failed compile", "Runnable", "Skipped" };
    public IEnumerable<string> AvailableTestResults => new[] { "All", "Success", "Fail", "Not Tested" };
    public IEnumerable<string> AvailableTestTypes => new[] { "All", "Scripts only", "dotnet test only" };
    public IEnumerable<string> AvailableFrameworks => new[] { "All", ".Net", ".Net Framework" };

    public bool HasActiveFilters 
        => SelectedScope != TestScope.All 
           || SelectedState != "All" 
           || SelectedTestResult != "All" 
           || SelectedTestTypes != "All" 
           || SelectedFramework != "All";

    public int FilteredIssueCount => _filteredIssues.Count;

    public string IssueNumbers
    {
        get;
        set => SetProperty(ref field, value);
    } = "";

    public bool ShowDiffOnly
    {
        get;
        set => SetProperty(ref field, value);
    } = false;

    public Dictionary<string, ChangeType> IssueChanges
    {
        get => _issueChanges;
        set => SetProperty(ref _issueChanges, value);
    }

    public void SetRunTestsCallback(Func<List<int>, Task> callback)
    {
        _runTestsCallback = callback;
    }

    public void SetShowOptionsCallback(Func<Task> callback)
    {
        _showOptionsCallback = callback;
    }

    public void SetSyncFromGitHubCommand(ReactiveCommand<Unit, Unit> command)
    {
        SyncFromGitHubCommand = command;
    }

    public void SetRepositoryBaseUrl(string baseUrl)
    {
        _repositoryBaseUrl = baseUrl;
    }

    private void OpenIssueUrl(int issueNumber)
    {
        var issue = _allIssues.FirstOrDefault(i => i.Number == issueNumber);
        if (issue != null && !string.IsNullOrEmpty(issue.GitHubUrl))
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = issue.GitHubUrl,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                // Log error but don't crash - could use a logger if available
                System.Diagnostics.Debug.WriteLine($"Failed to open URL {issue.GitHubUrl}: {ex.Message}");
            }
        }
        else if (!string.IsNullOrEmpty(_repositoryBaseUrl))
        {
            // Fallback: construct URL from base URL
            var url = $"{_repositoryBaseUrl}{issueNumber}";
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to open URL {url}: {ex.Message}");
            }
        }
    }

    private async Task RunTestsAsync()
    {
        if (_runTestsCallback == null)
        {
            return;
        }

        List<int> issueNumbers;
        
        // If IssueNumbers is specified, use that; otherwise use filtered issues
        if (!string.IsNullOrWhiteSpace(IssueNumbers))
        {
            // Parse issue numbers from the text box
            var numbers = new List<int>();
            var parts = IssueNumbers.Split(new[] { ',', ' ', ';' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var part in parts)
            {
                if (int.TryParse(part.Trim(), out var num))
                {
                    numbers.Add(num);
                }
            }
            issueNumbers = numbers;
            
            if (issueNumbers.Count == 0)
            {
                // Invalid input, fall back to filtered issues
                issueNumbers = _filteredIssues.Select(i => i.Number).ToList();
            }
        }
        else
        {
            // Use filtered issues
            issueNumbers = _filteredIssues.Select(i => i.Number).ToList();
        }
        
        if (issueNumbers.Count == 0)
        {
            return;
        }

        await _runTestsCallback(issueNumbers);
    }

    private async Task ShowOptionsAsync()
    {
        if (_showOptionsCallback == null)
        {
            return;
        }

        await _showOptionsCallback();
    }

    public void LoadIssues(IEnumerable<IssueListItem> items)
    {
        _allIssues.Clear();
        foreach (var item in items)
        {
            _allIssues.Add(item);
        }
        ApplyFilters();
    }

    private void ApplyFilters()
    {
        _filteredIssues.Clear();
        
        var filtered = _allIssues.AsEnumerable();
        
        // Apply scope filter
        if (SelectedScope != TestScope.All)
        {
            filtered = SelectedScope switch
            {
                TestScope.Regression => filtered.Where(i => 
                    i.State == "closed"),
                TestScope.Open => filtered.Where(i => 
                    i.State == "open"),
                _ => filtered
            };
        }
        
        // Apply state filter
        if (SelectedState != "All")
        {
            filtered = SelectedState switch
            {
                "New" => filtered.Where(i => i.StateValue == IssueState.New),
                "Synced" => filtered.Where(i => i.StateValue == IssueState.Synced),
                "Failed restore" => filtered.Where(i => i.StateValue == IssueState.FailedRestore),
                "Failed compile" => filtered.Where(i => i.StateValue == IssueState.FailedCompile),
                "Runnable" => filtered.Where(i => i.StateValue == IssueState.Runnable),
                "Skipped" => filtered.Where(i => i.StateValue == IssueState.Skipped),
                _ => filtered
            };
        }
        
        // Apply test result filter
        if (SelectedTestResult != "All")
        {
            filtered = SelectedTestResult switch
            {
                "Success" => filtered.Where(i => i.TestResult == "success"),
                "Fail" => filtered.Where(i => i.TestResult == "fail"),
                "Not Tested" => filtered.Where(i => i.TestResult == "Not tested" || string.IsNullOrEmpty(i.TestResult)),
                _ => filtered
            };
        }
        
        // Apply TestTypes filter
        if (SelectedTestTypes != "All")
        {
            // Display values in the column are "Scripts" or "DotNet test",
            // while filter options are "Scripts only" / "dotnet test only".
            filtered = SelectedTestTypes switch
            {
                "Scripts only" => filtered.Where(i => i.TestTypes == "Scripts"),
                "dotnet test only" => filtered.Where(i => i.TestTypes == "DotNet test"),
                _ => filtered
            };
        }
        
        // Apply Framework filter
        if (SelectedFramework != "All")
        {
            filtered = SelectedFramework switch
            {
                ".Net" => filtered.Where(i => i.Framework == ".Net"),
                ".Net Framework" => filtered.Where(i => i.Framework == ".Net Framework"),
                _ => filtered
            };
        }
        
        // Apply Diff filter (show only changed issues)
        if (ShowDiffOnly && _issueChanges.Count > 0)
        {
            filtered = filtered.Where(i =>
            {
                // Check if this issue has a change (match by issue number)
                // The key format is "Issue{Number}|{ProjectPath}"
                return _issueChanges.Keys.Any(key =>
                {
                    var parts = key.Split('|');
                    if (parts.Length >= 1 && int.TryParse(parts[0].Replace("Issue", "", StringComparison.OrdinalIgnoreCase), out var issueNum))
                    {
                        return issueNum == i.Number;
                    }
                    return false;
                });
            });
        }
        
        foreach (var item in filtered)
        {
            _filteredIssues.Add(item);
        }
        
        // Notify that filtered count and active filters changed
        OnPropertyChanged(nameof(FilteredIssueCount));
        OnPropertyChanged(nameof(HasActiveFilters));
    }
}