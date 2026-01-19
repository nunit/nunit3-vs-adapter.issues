using IssueRunner.Models;
using IssueRunner.Services;
using ReactiveUI;
using System.Reactive;
using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace IssueRunner.Gui.ViewModels;

/// <summary>
/// ViewModel for Run Tests options dialog.
/// </summary>
public class RunTestsOptionsViewModel : ViewModelBase
{
    private RunOptions? _result;
    private string _scopeCountsText = "";
    private string _expectedCountText = "";
    private readonly IEnvironmentService? _environmentService;

    public RunTestsOptionsViewModel(RunOptions? lastOptions = null, IEnvironmentService? environmentService = null)
    {
        _environmentService = environmentService;
        // Initialize from last options if available
        if (lastOptions != null)
        {
            _feed = lastOptions.Feed;
            _testTypes = lastOptions.TestTypes;
            _verbosity = lastOptions.Verbosity;
            _rerunFailedTests = lastOptions.RerunFailedTests;
            _skipNetFx = lastOptions.SkipNetFx;
            _onlyNetFx = lastOptions.OnlyNetFx;
            _nUnitOnly = lastOptions.NUnitOnly;
            if (lastOptions.IssueNumbers != null && lastOptions.IssueNumbers.Count > 0)
            {
                _issueNumbers = string.Join(", ", lastOptions.IssueNumbers);
            }
        }

        RunCommand = ReactiveCommand.Create(() =>
        {
            _result = ToRunOptions();
            CloseDialog();
        });
        
        CancelCommand = ReactiveCommand.Create(() =>
        {
            _result = null;
            CloseDialog();
        });
        
        // Update expected count when options change
        this.WhenAnyValue(x => x.TestTypes, x => x.IssueNumbers, x => x.RerunFailedTests)
            .Subscribe(_ => UpdateExpectedCount());
    }

    public string ScopeCountsText
    {
        get => _scopeCountsText;
        private set => SetProperty(ref _scopeCountsText, value);
    }

    public string ExpectedCountText
    {
        get => _expectedCountText;
        private set => SetProperty(ref _expectedCountText, value);
    }

    public ReactiveCommand<Unit, Unit> RunCommand { get; }
    public ReactiveCommand<Unit, Unit> CancelCommand { get; }

    public RunOptions? Result => _result;

    private void CloseDialog()
    {
        if (Avalonia.Application.Current?.ApplicationLifetime is Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop)
        {
            var window = desktop.Windows.OfType<Avalonia.Controls.Window>()
                .FirstOrDefault(w => w.DataContext == this);
            window?.Close();
        }
    }
    private PackageFeed _feed = PackageFeed.Stable;
    private TestTypes _testTypes = TestTypes.All;
    private LogVerbosity _verbosity = LogVerbosity.Normal;
    private bool _rerunFailedTests = false;
    private bool _skipNetFx = false;
    private bool _onlyNetFx = false;
    private bool _nUnitOnly = false;
    private string _issueNumbers = "";

    public PackageFeed Feed
    {
        get => _feed;
        set => SetProperty(ref _feed, value);
    }

    public TestTypes TestTypes
    {
        get => _testTypes;
        set => SetProperty(ref _testTypes, value);
    }

    public LogVerbosity Verbosity
    {
        get => _verbosity;
        set => SetProperty(ref _verbosity, value);
    }

    public bool RerunFailedTests
    {
        get => _rerunFailedTests;
        set => SetProperty(ref _rerunFailedTests, value);
    }

    public bool SkipNetFx
    {
        get => _skipNetFx;
        set => SetProperty(ref _skipNetFx, value);
    }

    public bool OnlyNetFx
    {
        get => _onlyNetFx;
        set => SetProperty(ref _onlyNetFx, value);
    }

    public bool NUnitOnly
    {
        get => _nUnitOnly;
        set => SetProperty(ref _nUnitOnly, value);
    }

    public string IssueNumbers
    {
        get => _issueNumbers;
        set => SetProperty(ref _issueNumbers, value);
    }

    public RunOptions ToRunOptions()
    {
        // Parse issue numbers if provided
        List<int>? issueNumbersList = null;
        if (!string.IsNullOrWhiteSpace(IssueNumbers))
        {
            var numbers = new List<int>();
            var parts = IssueNumbers.Split(new[] { ',', ' ', ';' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var part in parts)
            {
                if (int.TryParse(part.Trim(), out var num))
                {
                    numbers.Add(num);
                }
            }
            if (numbers.Count > 0)
            {
                issueNumbersList = numbers;
            }
        }

        var options = new RunOptions
        {
            Scope = TestScope.All, // Scope is now set in the filter section of IssueListView
            Feed = Feed,
            TestTypes = TestTypes,
            Verbosity = Verbosity,
            RerunFailedTests = RerunFailedTests,
            SkipNetFx = SkipNetFx,
            OnlyNetFx = OnlyNetFx,
            NUnitOnly = NUnitOnly,
            IssueNumbers = issueNumbersList
        };

        return options;
    }

    public async Task CalculateScopeCountsAsync(string repositoryRoot, IServiceProvider services)
    {
        try
        {
            var issueDiscovery = services.GetRequiredService<IIssueDiscoveryService>();
            var allIssues = issueDiscovery.DiscoverIssueFolders();
            var totalCount = allIssues.Count;

            // Load metadata - get data directory (creates it if it doesn't exist)
            var dataDir = _environmentService?.GetDataDirectory(repositoryRoot);
            if (dataDir == null)
            {
                dataDir = Path.Combine(repositoryRoot, ".nunit", "IssueRunner");
                if (!Directory.Exists(dataDir))
                {
                    Directory.CreateDirectory(dataDir);
                }
            }
            var metadataPath = Path.Combine(dataDir, "issues_metadata.json");
            var metadata = new Dictionary<int, IssueMetadata>();
            if (File.Exists(metadataPath))
            {
                var metadataJson = await File.ReadAllTextAsync(metadataPath);
                var metadataList = JsonSerializer.Deserialize<List<IssueMetadata>>(metadataJson) ?? [];
                metadata = metadataList.ToDictionary(m => m.Number);
            }

            // Load previous results
            var resultsPath = Path.Combine(dataDir, "results.json");
            var resultsByIssue = new Dictionary<int, string?>();
            if (File.Exists(resultsPath))
            {
                var resultsJson = await File.ReadAllTextAsync(resultsPath);
                var results = JsonSerializer.Deserialize<List<IssueResult>>(resultsJson) ?? [];
                resultsByIssue = results
                    .GroupBy(r => r.Number)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Any(r => r.TestResult == "fail") 
                            ? "fail" 
                            : g.FirstOrDefault()?.TestResult);
            }

            // Calculate counts for each scope (Scope is now in filter section, but we keep this for reference)
            var allCount = totalCount;
            var regressionCount = allIssues.Count(kvp => metadata.TryGetValue(kvp.Key, out var m) && m.State == "closed");
            var openCount = allIssues.Count(kvp => metadata.TryGetValue(kvp.Key, out var m) && m.State == "open");

            ScopeCountsText = $"All: {allCount} | Regression: {regressionCount} | Open: {openCount}";
            
            UpdateExpectedCount();
        }
        catch (Exception ex)
        {
            ScopeCountsText = $"Error calculating counts: {ex.Message}";
        }
    }

    private void UpdateExpectedCount()
    {
        // Scope is now handled in the filter section, so we don't show scope count here
        if (!string.IsNullOrWhiteSpace(IssueNumbers))
        {
            var parts = IssueNumbers.Split(new[] { ',', ' ', ';' }, StringSplitOptions.RemoveEmptyEntries);
            var issueCount = parts.Count(p => int.TryParse(p.Trim(), out _));
            ExpectedCountText = $"Expected: {issueCount} issue(s) (from specified list)";
        }
        else if (RerunFailedTests)
        {
            ExpectedCountText = $"Expected: Issues from test-fails.json";
        }
        else
        {
            var modeText = TestTypes == TestTypes.All ? "" : $" ({TestTypes} execution only)";
            ExpectedCountText = $"Expected: Issues based on filters{modeText}";
        }
    }

    private static string ExtractCount(string text, string prefix)
    {
        var index = text.IndexOf(prefix);
        if (index >= 0)
        {
            var start = index + prefix.Length;
            var end = text.IndexOf(' ', start);
            if (end < 0) end = text.IndexOf('|', start);
            if (end < 0) end = text.Length;
            return text.Substring(start, end - start).Trim();
        }
        return "?";
    }
}

