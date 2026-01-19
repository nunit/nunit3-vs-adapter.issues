using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using IssueRunner.Commands;
using IssueRunner.Gui.Views;
using IssueRunner.Models;
using IssueRunner.Services;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using System.Reactive;
using System.Text.Json;

namespace IssueRunner.Gui.ViewModels;

/// <summary>
/// Main view model for the application.
/// </summary>
public class MainViewModel : ViewModelBase
{
    private CancellationTokenSource? _testCancellationSource;
    private RunOptions? _lastRunOptions;
    private RunTestsStatusViewModel? _statusViewModel;
    private RunTestsStatusDialog? _statusDialog;
    private readonly List<int> _foldersWithoutMetadata = [];

    private readonly IServiceProvider _services;
    private readonly IEnvironmentService _environmentService;
    private IMarkerService _markerService;

    public MainViewModel(IServiceProvider services)
    {
        try
        {
            _services = services;
            _environmentService = services.GetRequiredService<IEnvironmentService>();
            _markerService = services.GetRequiredService<IMarkerService>();

            // ReactiveUI with Avalonia automatically handles thread marshaling when UseReactiveUI() is called
            BrowseRepositoryCommand = ReactiveCommand.CreateFromTask(BrowseRepositoryAsync);
            BrowseRepositoryCommand.ThrownExceptions.Subscribe(ex => AppendExceptionLog("Browse command exception:", ex));

            RunTestsCommand = ReactiveCommand.CreateFromTask(RunTestsAsync,
                this.WhenAnyValue(x => x.IsRunningTests, isRunning => !isRunning));
            RunTestsCommand.ThrownExceptions.Subscribe(ex => AppendExceptionLog("Run Tests exception:", ex));

            CancelTestsCommand = ReactiveCommand.Create(CancelTests,
                this.WhenAnyValue(x => x.IsRunningTests));

            SyncFromGitHubCommand = ReactiveCommand.CreateFromTask(SyncFromGitHubAsync);
            SyncFromGitHubCommand.ThrownExceptions.Subscribe(ex => AppendExceptionLog("Sync from GitHub exception:", ex));

            SyncToFoldersCommand = ReactiveCommand.CreateFromTask(() => SyncToFoldersAsync());
            SyncToFoldersCommand.ThrownExceptions.Subscribe(ex => AppendExceptionLog("Sync to Folders exception:", ex));

            ResetPackagesCommand = ReactiveCommand.CreateFromTask(ResetPackagesAsync);
            ResetPackagesCommand.ThrownExceptions.Subscribe(ex => AppendExceptionLog("Reset Packages exception:", ex));

            GenerateReportCommand = ReactiveCommand.CreateFromTask(GenerateReportAsync);
            GenerateReportCommand.ThrownExceptions.Subscribe(ex => AppendExceptionLog("Generate Report exception:", ex));

            CheckRegressionsCommand = ReactiveCommand.CreateFromTask(CheckRegressionsAsync);
            CheckRegressionsCommand.ThrownExceptions.Subscribe(ex => AppendExceptionLog("Check Regressions exception:", ex));

            ShowTestStatusCommand = ReactiveCommand.CreateFromTask(ShowTestStatusAsync);
            ShowTestStatusCommand.ThrownExceptions.Subscribe(ex => AppendExceptionLogToFile("Error showing test status:", "ShowTestStatus", ex));

            ShowIssueListCommand = ReactiveCommand.Create(ShowIssueList);
            ShowIssueListCommand.ThrownExceptions.Subscribe(ex => AppendExceptionLogToFile("Error showing issue list:", "ShowIssueList", ex));

            ShowOptionsCommand = ReactiveCommand.CreateFromTask(ShowOptionsAsync);
            ShowOptionsCommand.ThrownExceptions.Subscribe(ex => AppendExceptionLog("Show Options exception:", ex));

            ListFoldersWithoutMetadataCommand = ReactiveCommand.Create(ListFoldersWithoutMetadata);

            // Set initial view to issue list
            CurrentView = new IssueListView();
            CurrentViewType = "IssueList";

            // Auto-detect repository if running from a repo directory
            _ = Task.Run(InitializeRepositoryAsync);
        }
        catch (Exception ex)
        {
            // Log constructor exception to file
            LogExceptionToFile("MainViewModel constructor", ex);
            throw;
        }

    }

    private void AppendExceptionLog(string title, Exception ex)
    {
        try
        {
            AppendLog($"{title} {ex.Message}");
            AppendLog($"Exception type: {ex.GetType().Name}");
            if (ex.StackTrace != null)
            {
                AppendLog($"Stack trace: {ex.StackTrace}");
            }
        }
        catch
        {
            // If AppendLog fails, at least log to file
            LogExceptionToFile(title, ex);
        }
    }

    private void AppendExceptionLogToFile(string title, string msg, Exception ex)
    {
        AppendLog($"{title} {ex.Message}");
        LogExceptionToFile(msg, ex);
    }

    private static void LogExceptionToFile(string context, Exception ex)
    {
        try
        {
            var logPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "IssueRunner", "crash.log");
            Directory.CreateDirectory(Path.GetDirectoryName(logPath)!);
            File.AppendAllText(logPath, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {context}: {ex.Message}\n{ex.StackTrace}\n");
        }
        catch
        {
            // Ignore logging errors
        }
    }

    private async Task InitializeRepositoryAsync()
    {
        try
        {
            // First, try to load saved repository path from AppData
            var savedPath = AppSettings.LoadRepositoryPath();
            if (savedPath != null && Directory.Exists(savedPath))
            {
                // Verify it's still a valid repository
                var hasRepoConfig = File.Exists(Path.Combine(savedPath, ".nunit", "IssueRunner", "repository.json")) ||
                                   File.Exists(Path.Combine(savedPath, "Tools", "repository.json")) ||
                                   File.Exists(Path.Combine(savedPath, "repository.json"));
                if (hasRepoConfig)
                {
                    await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
                    {
                        RepositoryPath = savedPath;
                        AppendLog($"Loaded saved repository: {savedPath}");
                    });
                    return;
                }
            }

            // Fallback: auto-detect from current directory
            var currentDir = Directory.GetCurrentDirectory();
            var detectedRoot = _environmentService.ResolveRepositoryRoot(currentDir);

            // Check if we're actually in a repository (has repository.json in new or old locations)
            var hasRepoConfig2 = File.Exists(Path.Combine(detectedRoot, ".nunit", "IssueRunner", "repository.json")) ||
                               File.Exists(Path.Combine(detectedRoot, "Tools", "repository.json")) ||
                               File.Exists(Path.Combine(detectedRoot, "repository.json"));

            if (hasRepoConfig2 && Directory.Exists(detectedRoot))
            {
                // We're in a repository, load it automatically
                await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
                {
                    RepositoryPath = detectedRoot;
                    AppendLog("Auto-detected repository from current directory");
                });
            }
        }
        catch (Exception ex)
        {
            await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
            {
                AppendLog($"Auto-detection error: {ex.Message}");
            });
        }
    }

    public string RepositoryPath
    {
        get;
        set
        {
            if (SetProperty(ref field, value))
            {
                // Save repository path to AppData when it's set
                if (!string.IsNullOrWhiteSpace(value) && Directory.Exists(value))
                {
                    AppSettings.SaveRepositoryPath(value);
                }

                LoadRepository();
            }
        }
    } = "";

    public string SummaryText
    {
        get;
        set => SetProperty(ref field, value);
    } = "Select a repository to begin.";

    public string BaselineNUnitPackages
    {
        get;
        set => SetProperty(ref field, value);
    } = "Not set";

    public string CurrentNUnitPackages
    {
        get;
        set => SetProperty(ref field, value);
    } = "Not set";

    public bool HasFoldersWithoutMetadata => _foldersWithoutMetadata.Count > 0;

    public string LogOutput
    {
        get;
        set => SetProperty(ref field, value);
    } = "";

    public double Progress
    {
        get;
        set => SetProperty(ref field, value);
    } = 0.0;

    public object? CurrentView
    {
        get;
        set => SetProperty(ref field, value);
    }

    public string CurrentViewType
    {
        get;
        private set
        {
            if (SetProperty(ref field, value))
            {
                OnPropertyChanged(nameof(IssueListButtonBackground));
                OnPropertyChanged(nameof(TestStatusButtonBackground));
            }
        }
    } = "IssueList";

    public string IssueListButtonBackground => CurrentViewType == "IssueList" ? "#0078D4" : "#666";
    public string TestStatusButtonBackground => CurrentViewType == "TestStatus" ? "#0078D4" : "#666";

    public ReactiveCommand<Unit, Unit> BrowseRepositoryCommand { get; }
    public ReactiveCommand<Unit, Unit> RunTestsCommand { get; }
    public ReactiveCommand<Unit, Unit> CancelTestsCommand { get; }
    public ReactiveCommand<Unit, Unit> SyncFromGitHubCommand { get; }
    public ReactiveCommand<Unit, Unit> SyncToFoldersCommand { get; }
    public ReactiveCommand<Unit, Unit> ResetPackagesCommand { get; }
    public ReactiveCommand<Unit, Unit> GenerateReportCommand { get; }
    public ReactiveCommand<Unit, Unit> CheckRegressionsCommand { get; }
    public ReactiveCommand<Unit, Unit> ShowTestStatusCommand { get; }
    public ReactiveCommand<Unit, Unit> ShowIssueListCommand { get; }
    public ReactiveCommand<Unit, Unit> ShowOptionsCommand { get; }
    public ReactiveCommand<Unit, Unit> ListFoldersWithoutMetadataCommand { get; }

    public int FoldersWithoutMetadataCount => _foldersWithoutMetadata.Count;

    public bool IsRunningTests
    {
        get;
        private set => SetProperty(ref field, value);
    } = false;

    private async Task BrowseRepositoryAsync()
    {
        var window = GetMainWindow();
        if (window == null)
        {
            AppendLog("Error: Could not find main window for dialog.");
            return;
        }

        // Use StorageProvider API (modern, cross-platform)
        try
        {
            var folder = await window.StorageProvider.OpenFolderPickerAsync(
                new Avalonia.Platform.Storage.FolderPickerOpenOptions
                {
                    Title = "Select Repository Folder",
                    AllowMultiple = false
                });

            if (folder.Count > 0)
            {
                var selectedFolder = folder[0];
                // Get local path from IStorageFolder
                // Path is a Uri, so we need to convert it to a local path
                var uri = selectedFolder.Path;
                string? path = null;

                if (uri != null)
                {
                    // For file:// URIs, decode the path
                    if (uri.Scheme == "file")
                    {
                        path = Uri.UnescapeDataString(uri.AbsolutePath);
                        // On Windows, remove leading slash if present
                        if (path.Length > 0 && path[0] == '/' && path.Length > 2 && path[2] == ':')
                        {
                            path = path[1..];
                        }
                    }
                    else
                    {
                        // Try AbsolutePath as fallback
                        path = uri.AbsolutePath;
                    }
                }

                if (!string.IsNullOrWhiteSpace(path))
                {
                    RepositoryPath = path;
                    return;
                }
                AppendLog("Error: Could not determine folder path from selected folder.");
            }
        }
        catch (Exception ex)
        {
            AppendLog($"Error opening folder picker: {ex.Message}");
            AppendLog($"Exception type: {ex.GetType().Name}");
            throw; // Re-throw so ReactiveCommand can handle it
        }
    }

    private Window? GetMainWindow()
    {
        if (Avalonia.Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            return desktop.MainWindow;
        }
        return null;
    }

    private void LoadRepository()
    {
        if (string.IsNullOrWhiteSpace(RepositoryPath) || !Directory.Exists(RepositoryPath))
        {
            SummaryText = "Invalid repository path. Please select a valid repository folder.";
            CurrentView = null;
            return;
        }

        try
        {
            _environmentService.AddRoot(RepositoryPath);
            var discovery = _services.GetRequiredService<IIssueDiscoveryService>();
            var folders = discovery.DiscoverIssueFolders();

            // Load test results if available
            var dataDir = _environmentService.GetDataDirectory(RepositoryPath);
            var resultsPath = Path.Combine(dataDir, "results.json");
            var baselineResultsPath = Path.Combine(dataDir, "results-baseline.json");

            var passedCount = 0;
            var failedCount = 0;

            if (File.Exists(resultsPath))
            {
                try
                {
                    var resultsJson = File.ReadAllText(resultsPath);
                    var results = JsonSerializer.Deserialize<List<IssueResult>>(resultsJson);
                    if (results != null)
                    {
                        // Count passes and fails using domain model filtering
                        passedCount = results.Count(r => r.TestResult == "success");
                        failedCount = results.Count(r => r.TestResult != null && r.TestResult != "success");
                    }
                }
                catch { }
            }

            // Load baseline for comparison
            var baselinePassedCount = 0;
            var baselineFailedCount = 0;

            if (File.Exists(baselineResultsPath))
            {
                try
                {
                    var baselineResultsJson = File.ReadAllText(baselineResultsPath);
                    var baselineResults = JsonSerializer.Deserialize<List<IssueResult>>(baselineResultsJson);
                    if (baselineResults != null)
                    {
                        // Count passes and fails using domain model filtering
                        baselinePassedCount = baselineResults.Count(r => r.TestResult == "success");
                        baselineFailedCount = baselineResults.Count(r => r.TestResult != null && r.TestResult != "success");
                    }
                }
                catch { }
            }

            var repoConfig = _environmentService.RepositoryConfig;
            var repoInfo = repoConfig != null ? $"{repoConfig.Owner}/{repoConfig.Name}" : "Unknown";

            // Count issues with metadata and identify folders without metadata
            var metadataPath = Path.Combine(dataDir, "issues_metadata.json");
            var metadataCount = 0;
            var metadataNumbers = new HashSet<int>();
            _foldersWithoutMetadata.Clear();

            // Load central metadata
            if (File.Exists(metadataPath))
            {
                try
                {
                    var metadataJson = File.ReadAllText(metadataPath);
                    var metadataList = JsonSerializer.Deserialize<List<IssueMetadata>>(metadataJson) ?? [];
                    // Handle duplicates by taking unique issue numbers
                    metadataNumbers = metadataList.Select(m => m.Number).Distinct().ToHashSet();
                }
                catch { }
            }

            // Also check for local issue_metadata.json files in folders (for issues from Discussions, etc.)
            var foldersWithLocalMetadata = new HashSet<int>();
            foreach (var (folderNumber, folderPath) in folders)
            {
                var localMetadataPath = Path.Combine(folderPath, "issue_metadata.json");
                if (File.Exists(localMetadataPath))
                {
                    try
                    {
                        var localMetadataJson = File.ReadAllText(localMetadataPath);
                        var localMetadataList = JsonSerializer.Deserialize<List<IssueProjectMetadata>>(localMetadataJson) ?? [];
                        if (localMetadataList.Any())
                        {
                            // Folder has local metadata (might be from Discussions)
                            foldersWithLocalMetadata.Add(folderNumber);
                            // Also add to metadataNumbers if not already there
                            if (!metadataNumbers.Contains(folderNumber))
                            {
                                metadataNumbers.Add(folderNumber);
                            }
                        }
                    }
                    catch { }
                }
            }

            metadataCount = metadataNumbers.Count;

            // Find folders without metadata (neither central nor local)
            _foldersWithoutMetadata.Clear(); // Ensure we start fresh
            foreach (var folderNumber in folders.Keys)
            {
                if (!metadataNumbers.Contains(folderNumber))
                {
                    _foldersWithoutMetadata.Add(folderNumber);
                }
            }
            _foldersWithoutMetadata.Sort();

            // Also find metadata entries without folders (for debugging/info)
            var metadataWithoutFolders = metadataNumbers.Where(n => !folders.ContainsKey(n)).ToList();

            // Build summary with baseline comparison
            var passedDiff = passedCount - baselinePassedCount;
            var failedDiff = failedCount - baselineFailedCount;
            var passedDiffText = passedDiff >= 0 ? $"+{passedDiff}" : $"{passedDiff}";
            var failedDiffText = failedDiff >= 0 ? $"+{failedDiff}" : $"{failedDiff}";

            // Build summary
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

            var summary = $"Repository: {repoInfo}\n" +
                         $"Issues: {folders.Count} folders, {metadataCount} with metadata\n" +
                         $"{passedLine}\n" +
                         $"{failedLine}";

            SummaryText = summary;

            // Load NUnit package versions
            LoadNUnitPackageVersions(dataDir);

            OnPropertyChanged(nameof(FoldersWithoutMetadataCount));
            OnPropertyChanged(nameof(HasFoldersWithoutMetadata));

            AppendLog($"Loaded repository: {RepositoryPath}");
            AppendLog($"Found {folders.Count} issue folders, {metadataCount} with metadata ({metadataNumbers.Count - foldersWithLocalMetadata.Count} central, {foldersWithLocalMetadata.Count} local only)");
            if (_foldersWithoutMetadata.Count > 0)
            {
                AppendLog($"Found {_foldersWithoutMetadata.Count} folders without metadata");
            }
            if (metadataWithoutFolders.Any())
            {
                AppendLog($"Found {metadataWithoutFolders.Count} metadata entries without folders (may have been deleted): {string.Join(", ", metadataWithoutFolders)}");
            }

            // Load issue list view (async, will update when ready)
            var issueListViewModel = _services.GetRequiredService<IssueListViewModel>();
            CurrentView = new IssueListView { DataContext = issueListViewModel };
            CurrentViewType = "IssueList";
            // Capture RepositoryPath to avoid timing issues with async execution
            var repoPath = RepositoryPath;
            _ = Task.Run(async () => await LoadIssuesIntoViewAsync(issueListViewModel, folders, repoPath));
        }
        catch (Exception ex)
        {
            SummaryText = $"Error loading repository: {ex.Message}";
            AppendLog($"Error: {ex.Message}");
            if (ex.StackTrace != null)
            {
                AppendLog($"Stack: {ex.StackTrace}");
            }
            CurrentView = null;
        }
    }

    private void LoadNUnitPackageVersions(string dataDir)
    {
        try
        {
            // Load baseline package versions
            var baselinePackagesPath = Path.Combine(dataDir, "nunit-packages-baseline.json");
            if (File.Exists(baselinePackagesPath))
            {
                var baselineJson = File.ReadAllText(baselinePackagesPath);
                var baselineVersions = JsonSerializer.Deserialize<NUnitPackageVersions>(baselineJson);
                BaselineNUnitPackages = baselineVersions?.Packages is { Count: > 0 } ? FormatPackageVersions(baselineVersions.Packages) : "Not set";
            }
            else
            {
                BaselineNUnitPackages = "Not set";
            }

            // Load current package versions
            var currentPackagesPath = Path.Combine(dataDir, "nunit-packages-current.json");
            if (File.Exists(currentPackagesPath))
            {
                var currentJson = File.ReadAllText(currentPackagesPath);
                var currentVersions = JsonSerializer.Deserialize<NUnitPackageVersions>(currentJson);
                CurrentNUnitPackages = currentVersions?.Packages is { Count: > 0 } ? FormatPackageVersions(currentVersions.Packages) : "Not set";
            }
            else
            {
                CurrentNUnitPackages = "Not set";
            }
        }
        catch (Exception ex)
        {
            BaselineNUnitPackages = "Error loading";
            CurrentNUnitPackages = "Error loading";
            AppendLog($"Warning: Failed to load NUnit package versions: {ex.Message}");
        }
    }

    private static string FormatPackageVersions(Dictionary<string, string> packages)
    {
        return string.Join(", ", packages.Select(kvp => $"{kvp.Key}: {kvp.Value}"));
    }

    private async Task LoadIssuesIntoViewAsync(IssueListViewModel viewModel, Dictionary<int, string> folders, string repositoryPath)
    {
        try
        {
            var issues = new List<IssueListItem>();

            // Get repository config for GitHub URL generation
            var repoConfig = _environmentService.RepositoryConfig;
            var baseUrl = !string.IsNullOrEmpty(repoConfig.Owner) && !string.IsNullOrEmpty(repoConfig.Name)
                ? $"https://github.com/{repoConfig.Owner}/{repoConfig.Name}/issues/"
                : "";

            // Load metadata
            var dataDir = _environmentService.GetDataDirectory(repositoryPath);
            var metadataPath = Path.Combine(dataDir, "issues_metadata.json");
            var metadataDict = new Dictionary<int, IssueMetadata>();

            // Debug: Log the path being checked
            AppendLog($"Debug: Checking metadata at {metadataPath}, RepositoryPath={repositoryPath}, DataDir={dataDir}, Exists={File.Exists(metadataPath)}");

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
                            AppendLog($"Warning: Duplicate metadata entries found for issue {dup.Key}. Using the last occurrence.");
                        }
                    }

                    // Use GroupBy().ToDictionary() to take the last occurrence of each duplicate
                    metadataDict = metadata
                        .GroupBy(m => m.Number)
                        .ToDictionary(g => g.Key, g => g.Last());

                    // Count how many have titles
                    var titlesCount = metadataDict.Values.Count(m => !string.IsNullOrWhiteSpace(m.Title));
                    AppendLog($"Loaded {metadataDict.Count} issue metadata entries from {metadataPath} ({titlesCount} with titles)");
                }
                catch (Exception ex)
                {
                    AppendLog($"Warning: Could not load metadata: {ex.Message}");
                }
            }
            else
            {
                AppendLog($"Warning: Metadata file not found at {metadataPath}");
            }

            // Load test results from results.json
            var resultsPath = Path.Combine(dataDir, "results.json");
            var resultsByIssue = new Dictionary<int, (string Result, string LastRun)>();
            var failedBuilds = new HashSet<int>(); // Track issues with build or restore failures from results.json

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

                            // Check if this issue has any build or restore failures
                            var hasBuildFailure = issueResults.Any(r => 
                                r.BuildResult is "fail" || 
                                r.RestoreResult is "fail");
                            if (hasBuildFailure)
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
                                bool isWorse = false;
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
                catch { }
            }

            // Load diff data
            var diffService = _services.GetRequiredService<ITestResultDiffService>();
            var diffs = await diffService.CompareResultsAsync(RepositoryPath);
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

            // Get issue discovery service to check for marker files
            var testExecution = _services.GetRequiredService<ITestExecutionService>();
            var projectAnalyzer = _services.GetRequiredService<IProjectAnalyzerService>();

            // Log how many folders were discovered
            AppendLog($"Discovered {folders.Count} issue folders to load");

            if (folders.Count == 0)
            {
                AppendLog("Warning: No issue folders found. The IssueListView will be empty.");
            }

            // Build issue list
            foreach (var kvp in folders.OrderBy(k => k.Key))
            {
                var issueNum = kvp.Key;
                var folderPath = kvp.Value;
                var metadata = metadataDict.GetValueOrDefault(issueNum);

                // Debug: Log if metadata is missing or title is empty (only in verbose mode to avoid spam)
                // Note: We'll always set a title (either from metadata or fallback), so titles should never be empty

                var result = resultsByIssue.TryGetValue(issueNum, out var r) ? r : (Result: "Not tested", LastRun: "");

                var lastRunDate = "";
                if (!string.IsNullOrEmpty(result.LastRun) &&
                    DateTime.TryParse(result.LastRun, out var dt))
                {
                    lastRunDate = dt.ToString("yyyy-MM-dd HH:mm");
                }

                // Determine TestTypes based on whether issue has custom scripts
                var hasCustomScripts = testExecution.HasCustomRunners(folderPath);
                // Column display values: "Scripts" or "DotNet test"
                var testTypes = hasCustomScripts ? "Scripts" : "DotNet test";

                // Determine Framework type (.Net or .Net Framework)
                string framework = "";
                try
                {
                    var projectFiles = projectAnalyzer.FindProjectFiles(folderPath);
                    var hasNetFx = false;
                    var hasNet = false;

                    foreach (var projectFile in projectFiles)
                    {
                        var (targetFrameworks, _) = projectAnalyzer.ParseProjectFile(projectFile);
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
                    // Check which marker file exists using the list from IssueDiscoveryService
                    var markerReason = _markerService.GetMarkerReason(folderPath);
                    detailedState = "skipped";
                    stateValue = IssueState.Skipped;
                    // Always show marker reason for skipped issues, regardless of test result
                    notTestedReason = markerReason;
                }
                else if (failedBuilds.Contains(issueNum))
                {
                    // failedBuilds is populated from results.json (checks build_result and restore_result)
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
                            // For fixed issues, show the current status (should be "success")
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

            // Check if we're already on the UI thread (or in unit test without dispatcher)
            if (Avalonia.Threading.Dispatcher.UIThread.CheckAccess())
            {
                // Already on UI thread, execute directly
                viewModel.IssueChanges = issueChanges;
                viewModel.LoadIssues(issues);
                viewModel.SetRunTestsCallback(async (issueNumbers) => await RunFilteredTestsAsync(issueNumbers));
                viewModel.SetShowOptionsCallback(async () => await ShowOptionsAsync());
                viewModel.SetSyncFromGitHubCommand(SyncFromGitHubCommand);
                viewModel.SetRepositoryBaseUrl(baseUrl);
            }
            else
            {
                // Not on UI thread, use InvokeAsync
                await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
                {
                    viewModel.IssueChanges = issueChanges;
                    viewModel.LoadIssues(issues);
                    viewModel.SetRunTestsCallback(async (issueNumbers) => await RunFilteredTestsAsync(issueNumbers));
                    viewModel.SetShowOptionsCallback(async () => await ShowOptionsAsync());
                    viewModel.SetSyncFromGitHubCommand(SyncFromGitHubCommand);
                    viewModel.SetRepositoryBaseUrl(baseUrl);
                });
            }
        }
        catch (Exception ex)
        {
            AppendLog($"Error loading issues into view: {ex.Message}");
            AppendLog($"Stack trace: {ex.StackTrace}");
            LogExceptionToFile("LoadIssuesIntoViewAsync", ex);
        }
    }

    private async Task RunFilteredTestsAsync(List<int> issueNumbers)
    {
        try
        {
            if (!ValidateRepository())
            {
                return;
            }

            // Show options dialog
            var baseOptions = _lastRunOptions ?? new RunOptions
            {
                Scope = TestScope.All,
                Feed = PackageFeed.Stable,
                TestTypes = TestTypes.All,
                Verbosity = LogVerbosity.Normal
            };

            var optionsViewModel = new RunTestsOptionsViewModel(baseOptions, _environmentService);

            var dialog = new RunTestsOptionsDialog(optionsViewModel)
            {
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            var window = GetMainWindow();
            if (window != null)
            {
                await dialog.ShowDialog(window);
            }
            else
            {
                dialog.Show();
                await Task.Delay(100);
            }

            // Check if user cancelled
            if (optionsViewModel.Result == null)
            {
                AppendLog("Run Tests cancelled.");
                return;
            }

            var options = optionsViewModel.Result;

            // Override with filtered issue numbers (user might have changed them, but we want to use the filtered list)
            options = new RunOptions
            {
                Scope = TestScope.All, // Override scope since we're using specific issue numbers
                Feed = options.Feed,
                TestTypes = options.TestTypes,
                Verbosity = options.Verbosity,
                RerunFailedTests = false, // Not relevant when running specific issues
                SkipNetFx = options.SkipNetFx,
                OnlyNetFx = options.OnlyNetFx,
                NUnitOnly = options.NUnitOnly,
                IssueNumbers = issueNumbers // Use the filtered list
            };

            // Store options for next time
            _lastRunOptions = options;

            // Create cancellation token source
            _testCancellationSource = new CancellationTokenSource();
            IsRunningTests = true;

            // Create and show status dialog
            _statusViewModel = new RunTestsStatusViewModel
            {
                TotalIssues = issueNumbers.Count,
                IsRunning = true,
                IsStableFeed = options.Feed == PackageFeed.Stable
            };
            _statusViewModel.CancelCommand = ReactiveCommand.Create(() =>
            {
                if (_testCancellationSource != null && !_testCancellationSource.Token.IsCancellationRequested)
                {
                    _statusViewModel!.CurrentStatus = "Cancelling...";
                    _testCancellationSource.Cancel();
                }
            });
            _statusViewModel.SetBaselineCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                await SetBaselineAsync();
            });
            _statusViewModel.CloseCommand = ReactiveCommand.Create(() =>
            {
                _statusDialog?.Close();
                _statusDialog = null;
                _statusViewModel = null;
            });

            _statusDialog = new RunTestsStatusDialog(_statusViewModel)
            {
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            var mainWindowForStatus = GetMainWindow();
            if (mainWindowForStatus != null)
            {
                // Show dialog without blocking - use Show() for non-modal display
                _statusDialog.Show(mainWindowForStatus);
            }

            AppendLog($"Running tests for {issueNumbers.Count} filtered issue(s)...");
            AppendLog($"Issues: {string.Join(", ", issueNumbers)}");
            AppendLog($"Feed: {options.Feed}, TestTypes: {options.TestTypes}");

            // Create a real-time console writer that updates both log and status
            var originalOut = Console.Out;
            var realTimeWriter = new RealTimeLogWriter(AppendLog, UpdateStatusFromLog);
            Console.SetOut(realTimeWriter);

            try
            {
                // Run command in background task to allow real-time updates
                var cmd = _services.GetRequiredService<RunTestsCommand>();
                var exitCode = await Task.Run(async () =>
                    await cmd.ExecuteAsync(_environmentService.Root, options, _testCancellationSource.Token));

                if (_testCancellationSource.Token.IsCancellationRequested)
                {
                    AppendLog("Test run was cancelled by user.");
                }
                else if (exitCode == 0)
                {
                    AppendLog("Test run completed successfully.");
                    // Reload repository to refresh the issue list
                    LoadRepository();
                }
                else
                {
                    AppendLog($"Test run completed with errors (exit code: {exitCode}).");
                }
            }
            catch (OperationCanceledException)
            {
                AppendLog("Test run was cancelled by user.");
            }
            finally
            {
                Console.SetOut(originalOut);
                realTimeWriter.Dispose();
                IsRunningTests = false;

                // Update status dialog
                if (_statusViewModel != null)
                {
                    _statusViewModel.IsRunning = false;
                    if (_testCancellationSource?.Token.IsCancellationRequested == true)
                    {
                        _statusViewModel.CurrentStatus = "Cancelled";
                    }
                    else
                    {
                        _statusViewModel.CurrentStatus = "Completed";
                    }
                }

                // Don't auto-close the dialog - let user close it manually
                // The dialog will show "Completed" status and user can close it or set baseline

                _testCancellationSource = null;
            }
        }
        catch (Exception ex)
        {
            AppendLog($"Error running filtered tests: {ex.Message}");
            AppendLog($"Exception type: {ex.GetType().Name}");
            if (ex.StackTrace != null)
            {
                AppendLog($"Stack trace: {ex.StackTrace}");
            }
            IsRunningTests = false;
            _testCancellationSource = null;
        }
    }

    public void AppendLog(string message)
    {
        try
        {
            var timestamp = DateTime.Now.ToString("HH:mm:ss");
            var logMessage = $"[{timestamp}] {message}\n";

            // Ensure we're on the UI thread
            if (Avalonia.Threading.Dispatcher.UIThread.CheckAccess())
            {
                LogOutput += logMessage;
                OnPropertyChanged(nameof(LogOutput));
            }
            else
            {
                Avalonia.Threading.Dispatcher.UIThread.Post(() =>
                {
                    try
                    {
                        LogOutput += logMessage;
                        OnPropertyChanged(nameof(LogOutput));
                    }
                    catch (Exception ex)
                    {
                        LogExceptionToFile("AppendLog UI thread", ex);
                    }
                });
            }
        }
        catch (Exception ex)
        {
            LogExceptionToFile("AppendLog", ex);
        }
    }

    public void ClearLog()
    {
        LogOutput = "";
        OnPropertyChanged(nameof(LogOutput));
    }

    private void ShowIssueList()
    {
        try
        {
            if (!ValidateRepository())
            {
                return;
            }

            var issueListViewModel = _services.GetRequiredService<IssueListViewModel>();
            CurrentView = new IssueListView { DataContext = issueListViewModel };
            CurrentViewType = "IssueList";

            // Set callbacks
            issueListViewModel.SetShowOptionsCallback(async () => await ShowOptionsAsync());

            // Reload issues if repository is already loaded
            if (!string.IsNullOrWhiteSpace(_environmentService.Root))
            {
                var discovery = _services.GetRequiredService<IIssueDiscoveryService>();
                var folders = discovery.DiscoverIssueFolders();
                var repoPath = RepositoryPath;
                _ = Task.Run(async () => await LoadIssuesIntoViewAsync(issueListViewModel, folders, repoPath));
            }
        }
        catch (Exception ex)
        {
            AppendLog($"Error showing issue list: {ex.Message}");
            LogExceptionToFile("ShowIssueList", ex);
        }
    }

    private async Task ShowTestStatusAsync()
    {
        try
        {
            if (!ValidateRepository())
            {
                return;
            }

            var testStatusViewModel = new TestStatusViewModel(_environmentService);
            await testStatusViewModel.LoadDataAsync(_environmentService.Root);

            var testStatusView = new TestStatusView
            {
                DataContext = testStatusViewModel
            };

            // Set view on UI thread if available, otherwise set directly (for unit tests)
            if (Avalonia.Threading.Dispatcher.UIThread.CheckAccess())
            {
                CurrentView = testStatusView;
                CurrentViewType = "TestStatus";
            }
            else
            {
                await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
                {
                    CurrentView = testStatusView;
                    CurrentViewType = "TestStatus";
                });
            }
        }
        catch (Exception ex)
        {
            AppendLog($"Error showing test status: {ex.Message}");
            LogExceptionToFile("ShowTestStatus", ex);
        }
    }

    private async Task SetBaselineAsync()
    {
        try
        {
            if (!ValidateRepository())
            {
                return;
            }

            var dataDir = _environmentService.GetDataDirectory(_environmentService.Root);
            var resultsPath = Path.Combine(dataDir, "results.json");
            var baselineResultsPath = Path.Combine(dataDir, "results-baseline.json");
            var currentPackagesPath = Path.Combine(dataDir, "nunit-packages-current.json");
            var baselinePackagesPath = Path.Combine(dataDir, "nunit-packages-baseline.json");

            if (!File.Exists(resultsPath))
            {
                AppendLog("No test results to set as baseline.");
                return;
            }

            // Copy current results.json to results-baseline.json
            File.Copy(resultsPath, baselineResultsPath, overwrite: true);

            // Copy current package versions to baseline
            if (File.Exists(currentPackagesPath))
            {
                File.Copy(currentPackagesPath, baselinePackagesPath, overwrite: true);
            }

            AppendLog("Baseline set successfully.");

            // Reload repository to update summary
            LoadRepository();

            // Update status dialog if open
            if (_statusViewModel != null)
            {
                _statusViewModel.CanSetBaseline = false; // Already set
            }
        }
        catch (Exception ex)
        {
            AppendLog($"Error setting baseline: {ex.Message}");
            LogExceptionToFile("SetBaseline", ex);
        }
    }

    private void UpdateStatusFromLog(string logLine)
    {
        if (_statusViewModel == null) return;

        try
        {
            // Parse issue number and message
            var match = System.Text.RegularExpressions.Regex.Match(logLine, @"\[(\d+)\]\s*(.+)");
            if (match.Success)
            {
                var issueNum = match.Groups[1].Value;
                var message = match.Groups[2].Value.Trim();

                _statusViewModel.CurrentIssue = $"Issue {issueNum}";

                // Update status based on message
                if (message.Contains("Updating packages", StringComparison.OrdinalIgnoreCase))
                {
                    _statusViewModel.CurrentStatus = $"Updating packages for Issue {issueNum}...";
                    _statusViewModel.CurrentPhase = "Updating packages";
                    if (int.TryParse(issueNum, out var issueNumber))
                    {
                        // Estimate progress based on issue number (assuming sequential processing)
                        _statusViewModel.CurrentPhaseProgress = issueNumber;
                        _statusViewModel.CurrentPhaseTotal = _statusViewModel.TotalIssues;
                    }
                }
                else if (message.Contains("Restore failed", StringComparison.OrdinalIgnoreCase))
                {
                    _statusViewModel.CurrentStatus = $"Restore failed for Issue {issueNum}";
                    _statusViewModel.CurrentPhase = "Restore";
                }
                else if (message.Contains("Build failed", StringComparison.OrdinalIgnoreCase))
                {
                    _statusViewModel.CurrentStatus = $"Build failed for Issue {issueNum}";
                    _statusViewModel.CurrentPhase = "Build";
                }
                else if (message.Contains("Test failed", StringComparison.OrdinalIgnoreCase))
                {
                    _statusViewModel.CurrentStatus = $"Test failed for Issue {issueNum}";
                    _statusViewModel.CurrentPhase = "Test";
                }
                else if (message.Contains("All steps succeeded", StringComparison.OrdinalIgnoreCase))
                {
                    _statusViewModel.CurrentStatus = $"All steps succeeded for Issue {issueNum}";
                    _statusViewModel.CurrentPhase = "Complete";
                }
                else if (message.Contains("Restore succeeded", StringComparison.OrdinalIgnoreCase))
                {
                    _statusViewModel.CurrentStatus = $"Restore succeeded for Issue {issueNum}";
                    _statusViewModel.CurrentPhase = "Restore";
                }
                else if (message.Contains("Restore failed", StringComparison.OrdinalIgnoreCase))
                {
                    _statusViewModel.CurrentStatus = $"Restore failed for Issue {issueNum}";
                    _statusViewModel.CurrentPhase = "Restore";
                }
                else if (message.Contains("Build succeeded", StringComparison.OrdinalIgnoreCase))
                {
                    _statusViewModel.CurrentStatus = $"Build succeeded for Issue {issueNum}";
                    _statusViewModel.CurrentPhase = "Build";
                }
                else if (message.Contains("Build failed", StringComparison.OrdinalIgnoreCase))
                {
                    _statusViewModel.CurrentStatus = $"Build failed for Issue {issueNum}";
                    _statusViewModel.CurrentPhase = "Build";
                }
                else if (message.Contains("Test failed", StringComparison.OrdinalIgnoreCase))
                {
                    _statusViewModel.CurrentStatus = $"Test failed for Issue {issueNum}";
                    _statusViewModel.CurrentPhase = "Test";
                }
                else if (message.Contains("All steps succeeded", StringComparison.OrdinalIgnoreCase))
                {
                    _statusViewModel.CurrentStatus = $"All steps succeeded for Issue {issueNum}";
                    _statusViewModel.CurrentPhase = "Complete";
                }
                else if (message.Contains("Running tests", StringComparison.OrdinalIgnoreCase))
                {
                    _statusViewModel.CurrentStatus = $"Running tests for Issue {issueNum}...";
                    _statusViewModel.CurrentPhase = "Running tests";
                    if (int.TryParse(issueNum, out var issueNumber))
                    {
                        // Estimate progress based on issue number (assuming sequential processing)
                        _statusViewModel.CurrentPhaseProgress = issueNumber;
                        _statusViewModel.CurrentPhaseTotal = _statusViewModel.TotalIssues;
                    }
                }
                // Check for failure first (more specific), then success
                // Format is: "[123] Failure: {reason}" or "[123] Success: No regression failure"
                // Must check for "Failure:" first because "Success" might appear in failure messages
                if (message.StartsWith("Failure:", StringComparison.OrdinalIgnoreCase))
                {
                    _statusViewModel.ProcessedIssues++;
                    _statusViewModel.Failed++;
                    _statusViewModel.CurrentStatus = $"Issue {issueNum} failed";
                    _statusViewModel.CurrentPhase = "";
                    _statusViewModel.CurrentPhaseProgress = 0;
                    _statusViewModel.CurrentPhaseTotal = 0;
                }
                else if (message.StartsWith("Success:", StringComparison.OrdinalIgnoreCase))
                {
                    _statusViewModel.ProcessedIssues++;
                    _statusViewModel.Succeeded++;
                    _statusViewModel.CurrentStatus = $"Issue {issueNum} completed successfully";
                    _statusViewModel.CurrentPhase = "";
                    _statusViewModel.CurrentPhaseProgress = 0;
                    _statusViewModel.CurrentPhaseTotal = 0;
                }
                else if (message.Contains("Skipped", StringComparison.OrdinalIgnoreCase))
                {
                    _statusViewModel.ProcessedIssues++;
                    _statusViewModel.Skipped++;
                    _statusViewModel.CurrentStatus = $"Issue {issueNum} skipped";
                    _statusViewModel.CurrentPhase = "";
                    _statusViewModel.CurrentPhaseProgress = 0;
                    _statusViewModel.CurrentPhaseTotal = 0;
                }
                else if (message.Contains("not compiling", StringComparison.OrdinalIgnoreCase) ||
                         message.Contains("Compilation failed", StringComparison.OrdinalIgnoreCase))
                {
                    _statusViewModel.ProcessedIssues++;
                    _statusViewModel.NotCompiling++;
                    _statusViewModel.CurrentStatus = $"Issue {issueNum} - not compiling";
                }
            }
            else
            {
                // Check for other status messages (without issue numbers)
                var lowerLine = logLine.ToLowerInvariant();
                if (lowerLine.Contains("resetting packages", StringComparison.OrdinalIgnoreCase) ||
                    lowerLine.Contains("reset to metadata", StringComparison.OrdinalIgnoreCase))
                {
                    _statusViewModel.CurrentStatus = "Resetting packages to metadata versions...";
                    _statusViewModel.CurrentPhase = "Resetting packages";
                    _statusViewModel.CurrentPhaseProgress = 0;
                    _statusViewModel.CurrentPhaseTotal = 0;
                }
                else if (lowerLine.Contains("upgrading frameworks", StringComparison.OrdinalIgnoreCase) ||
                         lowerLine.Contains("upgrade frameworks", StringComparison.OrdinalIgnoreCase))
                {
                    _statusViewModel.CurrentStatus = "Upgrading target frameworks...";
                    _statusViewModel.CurrentPhase = "Upgrading frameworks";
                    _statusViewModel.CurrentPhaseProgress = 0;
                    _statusViewModel.CurrentPhaseTotal = 0;
                }
                else if (lowerLine.Contains("filtering issues", StringComparison.OrdinalIgnoreCase))
                {
                    _statusViewModel.CurrentStatus = "Filtering issues...";
                    _statusViewModel.CurrentPhase = "Filtering issues";
                    _statusViewModel.CurrentPhaseProgress = 0;
                    _statusViewModel.CurrentPhaseTotal = 0;
                }
                else if (lowerLine.Contains("repository root", StringComparison.OrdinalIgnoreCase))
                {
                    _statusViewModel.CurrentStatus = "Initializing test run...";
                    _statusViewModel.CurrentPhase = "Initializing";
                    _statusViewModel.CurrentPhaseProgress = 0;
                    _statusViewModel.CurrentPhaseTotal = 0;
                }
                else if (lowerLine.Contains("target nunit", StringComparison.OrdinalIgnoreCase) ||
                         lowerLine.Contains("nunit version", StringComparison.OrdinalIgnoreCase))
                {
                    _statusViewModel.CurrentStatus = "Checking NUnit package versions...";
                    _statusViewModel.CurrentPhase = "Checking versions";
                    _statusViewModel.CurrentPhaseProgress = 0;
                    _statusViewModel.CurrentPhaseTotal = 0;
                }
            }
        }
        catch
        {
            // Ignore parsing errors
        }
    }

    private async Task ShowOptionsAsync()
    {
        try
        {
            if (!ValidateRepository())
            {
                return;
            }

            // Show options dialog with last used options or defaults
            var optionsViewModel = new RunTestsOptionsViewModel(_lastRunOptions, _environmentService);

            var dialog = new RunTestsOptionsDialog(optionsViewModel)
            {
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            var mainWindowForOptions = GetMainWindow();
            if (mainWindowForOptions != null)
            {
                await dialog.ShowDialog(mainWindowForOptions);
            }
            else
            {
                dialog.Show();
                await Task.Delay(100);
            }

            // Save options if user clicked OK
            if (optionsViewModel.Result != null)
            {
                _lastRunOptions = optionsViewModel.Result;
                AppendLog("Options saved.");
            }
        }
        catch (Exception ex)
        {
            AppendLog($"Error showing options: {ex.Message}");
            LogExceptionToFile("ShowOptionsAsync", ex);
        }
    }

    private async Task RunTestsAsync()
    {
        try
        {
            if (!ValidateRepository())
            {
                return;
            }

            // Show options dialog with last used options or defaults
            var optionsViewModel = new RunTestsOptionsViewModel(_lastRunOptions, _environmentService);

            var dialog = new RunTestsOptionsDialog(optionsViewModel)
            {
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            var window = GetMainWindow();
            if (window != null)
            {
                await dialog.ShowDialog(window);
            }
            else
            {
                // Fallback if we can't get main window
                dialog.Show();
                await Task.Delay(100); // Give it a moment to show
            }

            // Check if user cancelled
            if (optionsViewModel.Result == null)
            {
                AppendLog("Run Tests cancelled.");
                return;
            }

            var options = optionsViewModel.Result;

            // Store options for next time
            _lastRunOptions = options;

            // Create cancellation token source
            _testCancellationSource = new CancellationTokenSource();
            IsRunningTests = true;

            // Determine total issues
            var totalIssues = options.IssueNumbers?.Count ?? 0;
            if (totalIssues == 0)
            {
                // Need to count issues that will be run
                var issueFolders = _services.GetRequiredService<IIssueDiscoveryService>().DiscoverIssueFolders();
                totalIssues = issueFolders.Count;
            }

            // Create and show status dialog
            _statusViewModel = new RunTestsStatusViewModel
            {
                TotalIssues = totalIssues,
                IsRunning = true,
                IsStableFeed = options.Feed == PackageFeed.Stable
            };
            _statusViewModel.CancelCommand = ReactiveCommand.Create(() =>
            {
                if (_testCancellationSource != null && !_testCancellationSource.Token.IsCancellationRequested)
                {
                    _statusViewModel!.CurrentStatus = "Cancelling...";
                    _testCancellationSource.Cancel();
                }
            });
            _statusViewModel.SetBaselineCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                await SetBaselineAsync();
            });
            _statusViewModel.CloseCommand = ReactiveCommand.Create(() =>
            {
                _statusDialog?.Close();
                _statusDialog = null;
                _statusViewModel = null;
            });

            _statusDialog = new RunTestsStatusDialog(_statusViewModel)
            {
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            var mainWindowForDialog = GetMainWindow();
            if (mainWindowForDialog != null)
            {
                // Show dialog without blocking - use Show() instead of ShowDialog() for non-modal
                _statusDialog.Show(mainWindowForDialog);
            }

            AppendLog("Running tests...");
            AppendLog($"Scope: {options.Scope}, Feed: {options.Feed}, TestTypes: {options.TestTypes}");
            if (options.IssueNumbers != null && options.IssueNumbers.Count > 0)
            {
                AppendLog($"Issues: {string.Join(", ", options.IssueNumbers)}");
            }

            // Create a real-time console writer that updates both log and status
            var originalOut = Console.Out;
            var realTimeWriter = new RealTimeLogWriter(AppendLog, UpdateStatusFromLog);
            Console.SetOut(realTimeWriter);

            try
            {
                // Run command in background task to allow real-time updates
                var cmd = _services.GetRequiredService<RunTestsCommand>();
                var exitCode = await Task.Run(async () =>
                    await cmd.ExecuteAsync(_environmentService.Root, options, _testCancellationSource.Token));

                if (_testCancellationSource.Token.IsCancellationRequested)
                {
                    AppendLog("Test run was cancelled by user.");
                }
                else if (exitCode == 0)
                {
                    AppendLog("Test run completed successfully.");
                }
                else
                {
                    AppendLog($"Test run completed with errors (exit code: {exitCode}).");
                }
            }
            catch (OperationCanceledException)
            {
                AppendLog("Test run was cancelled by user.");
            }
            finally
            {
                Console.SetOut(originalOut);
                realTimeWriter.Dispose();
                IsRunningTests = false;

                // Update status dialog
                if (_statusViewModel != null)
                {
                    _statusViewModel.IsRunning = false;
                    if (_testCancellationSource?.Token.IsCancellationRequested == true)
                    {
                        _statusViewModel.CurrentStatus = "Cancelled";
                    }
                    else
                    {
                        _statusViewModel.CurrentStatus = "Completed";
                    }
                }

                // Don't auto-close the dialog - let user close it manually
                // The dialog will show "Completed" status and user can close it or set baseline

                _testCancellationSource?.Dispose();
                _testCancellationSource = null;

                // Refresh the issue list view to show updated states (marker files, test results, etc.)
                if (CurrentView is IssueListView issueListView && issueListView.DataContext is IssueListViewModel issueListViewModel)
                {
                    var issueDiscovery = _services.GetRequiredService<IIssueDiscoveryService>();
                    var folders = issueDiscovery.DiscoverIssueFolders();
                    var repoPath = RepositoryPath;
                    _ = Task.Run(async () => await LoadIssuesIntoViewAsync(issueListViewModel, folders, repoPath));
                }
            }
        }
        catch (Exception ex)
        {
            AppendLog($"Error: {ex.Message}");
            AppendLog($"Exception type: {ex.GetType().Name}");
            if (ex.InnerException != null)
            {
                AppendLog($"Inner exception: {ex.InnerException.Message}");
            }
            if (ex.StackTrace != null)
            {
                AppendLog($"Stack trace: {ex.StackTrace}");
            }
            IsRunningTests = false;
            _testCancellationSource?.Dispose();
            _testCancellationSource = null;
        }
    }

    private void CancelTests()
    {
        if (_testCancellationSource is { Token.IsCancellationRequested: false })
        {
            AppendLog("Cancelling test run... Please wait for current operations to complete.");
            _testCancellationSource.Cancel();

            // Also try to kill any running processes if possible
            // Note: This is a best-effort - processes will be killed when they check cancellation
        }
    }

    private bool ValidateRepository()
    {
        if (string.IsNullOrWhiteSpace(_environmentService.Root) || !Directory.Exists(_environmentService.Root))
        {
            AppendLog("Error: No repository loaded. Please select a repository first.");
            return false;
        }
        return true;
    }

    private async Task SyncFromGitHubAsync()
    {
        try
        {
            if (!ValidateRepository())
            {
                return;
            }

            var mainWindow = GetMainWindow();
            if (mainWindow == null)
            {
                AppendLog("Error: Could not find main window");
                return;
            }

            // Create and set up sync dialog
            var syncViewModel = new SyncFromGitHubViewModel();
            var cancellationSource = new CancellationTokenSource();
            SyncFromGitHubDialog? syncDialog = null;

            // Set up commands BEFORE creating dialog
            syncViewModel.StartCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                await StartSyncAsync(syncViewModel, cancellationSource);
            });
            syncViewModel.CancelCommand = ReactiveCommand.Create(() =>
            {
                cancellationSource.Cancel();
                syncViewModel.StatusText = "Cancelling...";
            });
            syncViewModel.SyncToFoldersCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                await SyncToFoldersAsync(syncViewModel, cancellationSource);
            });
            // ShowOptionsCommand and ShowProgressCommand are already set in constructor
            // CloseCommand is already set in constructor to call CloseDialog()

            // Create dialog after commands are set
            syncDialog = new SyncFromGitHubDialog(syncViewModel)
            {
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            // Set dialog reference in ViewModel so CloseCommand can close it
            syncViewModel.SetDialogWindow(syncDialog);

            // Show dialog
            await syncDialog.ShowDialog(mainWindow);
        }
        catch (Exception ex)
        {
            AppendLog($"Error: {ex.Message}");
            AppendLog($"Exception type: {ex.GetType().Name}");
            if (ex.InnerException != null)
            {
                AppendLog($"Inner exception: {ex.InnerException.Message}");
            }
            if (ex.StackTrace != null)
            {
                AppendLog($"Stack trace: {ex.StackTrace}");
            }
            LogExceptionToFile("SyncFromGitHubAsync", ex);
        }
    }

    private async Task StartSyncAsync(SyncFromGitHubViewModel viewModel, CancellationTokenSource cancellationSource)
    {
        try
        {
            viewModel.IsRunning = true;
            viewModel.Reset();
            viewModel.StatusText = "Starting sync...";

            // Get sync options - normalize sync mode string
            var syncMode = viewModel.SyncMode == "Missing Metadata" ? "MissingMetadata" : "All";
            var updateExisting = viewModel.UpdateExisting;

            // Get total issue count for progress
            var issueDiscovery = _services.GetRequiredService<IIssueDiscoveryService>();
            var issueFolders = issueDiscovery.DiscoverIssueFolders();
            var allIssueNumbers = issueFolders.Keys.OrderBy(n => n).ToList();

            // Load existing metadata to determine total count
            var dataDir = _environmentService.GetDataDirectory(RepositoryPath);
            var metadataPath = Path.Combine(dataDir, "issues_metadata.json");
            var existingMetadata = new Dictionary<int, IssueMetadata>();
            if (File.Exists(metadataPath))
            {
                try
                {
                    var existingJson = await File.ReadAllTextAsync(metadataPath);
                    var existingList = JsonSerializer.Deserialize<List<IssueMetadata>>(existingJson) ?? [];

                    // Handle duplicate issue numbers gracefully
                    var duplicates = existingList.GroupBy(m => m.Number).Where(g => g.Count() > 1).ToList();
                    if (duplicates.Any())
                    {
                        foreach (var dup in duplicates)
                        {
                            AppendLog($"Warning: Duplicate metadata entries found for issue {dup.Key}. Using the last occurrence.");
                        }
                    }

                    // Use GroupBy().ToDictionary() to take the last occurrence of each duplicate
                    existingMetadata = existingList
                        .GroupBy(m => m.Number)
                        .ToDictionary(g => g.Key, g => g.Last());
                }
                catch { }
            }

            // Calculate total based on sync mode
            int totalIssues;
            int totalIssueCount = allIssueNumbers.Count;
            int existingCount = existingMetadata.Count;

            if (syncMode == "MissingMetadata")
            {
                var missingCount = allIssueNumbers.Count(n => !existingMetadata.ContainsKey(n));
                totalIssues = missingCount;
                if (missingCount == 0)
                {
                    viewModel.StatusText = "No issues need syncing - all issues already have metadata.";
                    AppendLog($"Missing Metadata sync: All {totalIssueCount} issues already have metadata. Nothing to sync.");
                    viewModel.IsRunning = false;
                    viewModel.SyncCompleted = true;
                    return;
                }
                viewModel.StatusText = $"Missing Metadata sync: {missingCount} issues need metadata out of {totalIssueCount} total ({existingCount} already have metadata).";
                AppendLog($"Missing Metadata sync: Found {missingCount} issues without metadata out of {totalIssueCount} total issues.");
                AppendLog($"Syncing {missingCount} issues that are missing metadata...");
            }
            else
            {
                totalIssues = totalIssueCount;
                viewModel.StatusText = $"Syncing all {totalIssues} issues...";
                AppendLog($"All sync: Syncing all {totalIssues} issues from GitHub...");
            }

            viewModel.TotalIssues = totalIssues;

            // Create progress callback
            SyncProgressCallback progressCallback = (issueNumber, found, synced, totalProcessed, total) =>
            {
                if (Avalonia.Threading.Dispatcher.UIThread.CheckAccess())
                {
                    UpdateSyncProgress(viewModel, issueNumber, found, synced);
                }
                else
                {
                    Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
                    {
                        UpdateSyncProgress(viewModel, issueNumber, found, synced);
                    });
                }
            };

            // Redirect console output to log
            var originalOut = Console.Out;
            var logWriter = new StringWriter();
            Console.SetOut(logWriter);

            try
            {
                var cmd = _services.GetRequiredService<SyncFromGitHubCommand>();
                var exitCode = await cmd.ExecuteAsync(
                    null,
                    cancellationSource.Token,
                    progressCallback,
                    syncMode,
                    updateExisting);

                var output = logWriter.ToString();
                if (!string.IsNullOrWhiteSpace(output))
                {
                    AppendLog(output.TrimEnd());
                }

                viewModel.IsRunning = false;
                viewModel.SyncCompleted = true;

                // Build detailed completion message
                var syncModeDisplay = syncMode == "MissingMetadata" ? "Missing Metadata" : "All";
                var summary = $"{viewModel.IssuesSynced} synced, {viewModel.IssuesFound} found, {viewModel.IssuesNotFound} not found";

                if (exitCode == 0)
                {
                    viewModel.StatusText = $"{syncModeDisplay} sync completed successfully. {summary}.";
                    AppendLog($"{syncModeDisplay} sync completed successfully.");
                    AppendLog($"Summary: {summary}.");
                }
                else
                {
                    viewModel.StatusText = $"{syncModeDisplay} sync completed with errors. {summary}.";
                    AppendLog($"{syncModeDisplay} sync completed with errors (exit code: {exitCode}).");
                    AppendLog($"Summary: {summary}.");
                }

                // Reload repository to refresh issue list
                LoadRepository();
            }
            catch (OperationCanceledException)
            {
                viewModel.IsRunning = false;
                viewModel.StatusText = "Sync cancelled by user.";
                AppendLog("Sync from GitHub cancelled.");
            }
            finally
            {
                Console.SetOut(originalOut);
                logWriter.Dispose();
            }
        }
        catch (Exception ex)
        {
            viewModel.IsRunning = false;
            viewModel.StatusText = $"Error: {ex.Message}";
            AppendLog($"Error during sync: {ex.Message}");
            LogExceptionToFile("StartSyncAsync", ex);
        }
    }

    private void UpdateSyncProgress(SyncFromGitHubViewModel viewModel, int issueNumber, bool found, bool synced)
    {
        if (found)
        {
            viewModel.IssuesFound++;
            if (synced)
            {
                viewModel.IssuesSynced++;
            }
        }
        else
        {
            viewModel.IssuesNotFound++;
        }

        var progressPercent = viewModel.TotalIssues > 0
            ? (viewModel.IssuesSynced + viewModel.IssuesNotFound) * 100.0 / viewModel.TotalIssues
            : 0.0;
        viewModel.StatusText = $"Syncing issue {issueNumber}... ({viewModel.IssuesSynced}/{viewModel.TotalIssues} synced, {viewModel.IssuesFound} found, {viewModel.IssuesNotFound} not found)";
    }

    private void ListFoldersWithoutMetadata()
    {
        // Recalculate to ensure we have the latest data
        if (string.IsNullOrWhiteSpace(RepositoryPath) || !Directory.Exists(RepositoryPath))
        {
            AppendLog("Error: No repository loaded.");
            return;
        }

        try
        {
            var discovery = _services.GetRequiredService<IIssueDiscoveryService>();
            var folders = discovery.DiscoverIssueFolders();

            var dataDir = _environmentService.GetDataDirectory(RepositoryPath);
            var metadataPath = Path.Combine(dataDir, "issues_metadata.json");
            var metadataNumbers = new HashSet<int>();

            if (File.Exists(metadataPath))
            {
                try
                {
                    var metadataJson = File.ReadAllText(metadataPath);
                    var metadataList = JsonSerializer.Deserialize<List<IssueMetadata>>(metadataJson) ?? [];
                    metadataNumbers = metadataList.Select(m => m.Number).Distinct().ToHashSet();
                }
                catch (Exception ex)
                {
                    AppendLog($"Error reading metadata: {ex.Message}");
                    return;
                }
            }

            var foldersWithoutMetadata = folders.Keys.Where(n => !metadataNumbers.Contains(n)).OrderBy(n => n).ToList();
            var metadataWithoutFolders = metadataNumbers.Where(n => !folders.ContainsKey(n)).OrderBy(n => n).ToList();

            AppendLog($"Summary:");
            AppendLog($"  Total folders: {folders.Count}");
            AppendLog($"  Total metadata entries: {metadataNumbers.Count}");
            AppendLog($"  Folders without metadata: {foldersWithoutMetadata.Count}");
            if (metadataWithoutFolders.Any())
            {
                AppendLog($"  Metadata without folders: {metadataWithoutFolders.Count}");
            }
            AppendLog("");

            if (foldersWithoutMetadata.Count == 0)
            {
                AppendLog("No folders without metadata found.");
            }
            else
            {
                AppendLog($"Folders without metadata ({foldersWithoutMetadata.Count}):");
                AppendLog(string.Join(", ", foldersWithoutMetadata));
                AppendLog($"These folders exist but don't have corresponding issues in GitHub (may have been moved or deleted).");
            }

            if (metadataWithoutFolders.Any())
            {
                AppendLog("");
                AppendLog($"Metadata entries without folders ({metadataWithoutFolders.Count}):");
                AppendLog(string.Join(", ", metadataWithoutFolders));
                AppendLog($"These metadata entries exist but the folders have been deleted.");
            }

            // Show the calculation
            var expectedDifference = folders.Count - metadataNumbers.Count;
            var actualFoldersWithoutMetadata = foldersWithoutMetadata.Count;
            if (expectedDifference != actualFoldersWithoutMetadata)
            {
                AppendLog("");
                AppendLog($"Note: Simple calculation (132 - 125 = 7) doesn't match because:");
                AppendLog($"  {metadataWithoutFolders.Count} metadata entries don't have folders");
                AppendLog($"  So: {folders.Count} folders - ({metadataNumbers.Count} metadata - {metadataWithoutFolders.Count} without folders) = {folders.Count - (metadataNumbers.Count - metadataWithoutFolders.Count)}");
            }
        }
        catch (Exception ex)
        {
            AppendLog($"Error listing folders without metadata: {ex.Message}");
        }
    }

    private async Task SyncToFoldersAsync(SyncFromGitHubViewModel? viewModel = null, CancellationTokenSource? cancellationSource = null)
    {
        try
        {
            if (!ValidateRepository())
            {
                if (viewModel != null)
                {
                    viewModel.StatusText = "Error: No repository loaded.";
                }
                return;
            }

            // Check if metadata file exists
            var dataDir = _environmentService.GetDataDirectory(RepositoryPath);
            var metadataPath = Path.Combine(dataDir, "issues_metadata.json");
            if (!File.Exists(metadataPath))
            {
                if (viewModel != null)
                {
                    viewModel.StatusText = "Error: No metadata file found. Please sync from GitHub first.";
                }
                AppendLog("Error: No metadata file found. Please sync from GitHub first.");
                return;
            }

            // Update dialog status if provided - ensure UI thread updates
            if (viewModel != null)
            {
                if (Avalonia.Threading.Dispatcher.UIThread.CheckAccess())
                {
                    viewModel.IsRunning = true;
                    viewModel.ShowProgress = true;
                    viewModel.StatusText = "Syncing metadata to folders...";
                    viewModel.Progress = 0;
                    viewModel.TotalIssues = 0;
                    viewModel.IssuesSynced = 0;
                }
                else
                {
                    await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
                    {
                        viewModel.IsRunning = true;
                        viewModel.ShowProgress = true;
                        viewModel.StatusText = "Syncing metadata to folders...";
                        viewModel.Progress = 0;
                        viewModel.TotalIssues = 0;
                        viewModel.IssuesSynced = 0;
                    });
                }
            }

            AppendLog("Syncing metadata to folders...");

            // Get total issue count for progress
            var issueDiscovery = _services.GetRequiredService<IIssueDiscoveryService>();
            var issueFolders = issueDiscovery.DiscoverIssueFolders();
            var totalIssues = issueFolders.Count;

            if (viewModel != null)
            {
                if (Avalonia.Threading.Dispatcher.UIThread.CheckAccess())
                {
                    viewModel.TotalIssues = totalIssues;
                }
                else
                {
                    await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
                    {
                        viewModel.TotalIssues = totalIssues;
                    });
                }
            }

            // Redirect console output to log
            var originalOut = Console.Out;
            var logWriter = new StringWriter();
            Console.SetOut(logWriter);

            try
            {
                var cmd = _services.GetRequiredService<SyncToFoldersCommand>();
                var cancellationToken = cancellationSource?.Token ?? CancellationToken.None;
                var exitCode = await cmd.ExecuteAsync(_environmentService.Root, null, cancellationToken);

                var output = logWriter.ToString();
                if (!string.IsNullOrWhiteSpace(output))
                {
                    AppendLog(output.TrimEnd());
                }

                if (viewModel != null)
                {
                    if (Avalonia.Threading.Dispatcher.UIThread.CheckAccess())
                    {
                        viewModel.Progress = 100;
                        viewModel.IssuesSynced = totalIssues; // All issues processed
                        viewModel.IsRunning = false;

                        viewModel.StatusText = exitCode == 0 ? $"Sync to folders completed successfully. {totalIssues} issue folders updated." : $"Sync to folders completed with errors (exit code: {exitCode}).";
                        // ShowProgress stays true - user can manually switch back to options if needed
                    }
                    else
                    {
                        await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
                        {
                            viewModel.Progress = 100;
                            viewModel.IssuesSynced = totalIssues;
                            viewModel.IsRunning = false;

                            viewModel.StatusText = exitCode == 0 ? $"Sync to folders completed successfully. {totalIssues} issue folders updated." : $"Sync to folders completed with errors (exit code: {exitCode}).";
                            // ShowProgress stays true - user can manually switch back to options if needed
                        });
                    }

                    AppendLog(exitCode == 0
                        ? "Sync to folders completed successfully."
                        : $"Sync to folders completed with errors (exit code: {exitCode}).");
                }
                else
                {
                    AppendLog(exitCode == 0
                        ? "Sync to folders completed successfully."
                        : $"Sync to folders completed with errors (exit code: {exitCode}).");
                }
            }
            finally
            {
                Console.SetOut(originalOut);
                logWriter.Dispose();
            }
        }
        catch (OperationCanceledException)
        {
            if (viewModel != null)
            {
                if (Avalonia.Threading.Dispatcher.UIThread.CheckAccess())
                {
                    viewModel.IsRunning = false;
                    viewModel.StatusText = "Sync to folders cancelled by user.";
                }
                else
                {
                    await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
                    {
                        viewModel.IsRunning = false;
                        viewModel.StatusText = "Sync to folders cancelled by user.";
                    });
                }
            }
            AppendLog("Sync to folders cancelled by user.");
        }
        catch (Exception ex)
        {
            if (viewModel != null)
            {
                if (Avalonia.Threading.Dispatcher.UIThread.CheckAccess())
                {
                    viewModel.IsRunning = false;
                    viewModel.StatusText = $"Error: {ex.Message}";
                }
                else
                {
                    await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
                    {
                        viewModel.IsRunning = false;
                        viewModel.StatusText = $"Error: {ex.Message}";
                    });
                }
            }
            AppendLog($"Error: {ex.Message}");
            AppendLog($"Exception type: {ex.GetType().Name}");
            if (ex.InnerException != null)
            {
                AppendLog($"Inner exception: {ex.InnerException.Message}");
            }
            if (ex.StackTrace != null)
            {
                AppendLog($"Stack trace: {ex.StackTrace}");
            }
        }
    }

    private async Task ResetPackagesAsync()
    {
        AppendLog("Reset Packages command not yet fully implemented.");
        // TODO: Show dialog for issue selection, then execute
        await Task.CompletedTask;
    }

    private async Task GenerateReportAsync()
    {
        try
        {
            if (!ValidateRepository())
            {
                return;
            }

            AppendLog("Generating test report...");

            // Redirect console output to log
            var originalOut = Console.Out;
            var logWriter = new StringWriter();
            Console.SetOut(logWriter);

            try
            {
                var cmd = _services.GetRequiredService<GenerateReportCommand>();
                var exitCode = await cmd.ExecuteAsync(CancellationToken.None);

                var output = logWriter.ToString();
                if (!string.IsNullOrWhiteSpace(output))
                {
                    AppendLog(output.TrimEnd());
                }

                AppendLog(exitCode == 0
                    ? "Report generated successfully."
                    : $"Report generation completed with errors (exit code: {exitCode}).");
            }
            finally
            {
                Console.SetOut(originalOut);
                logWriter.Dispose();
            }
        }
        catch (Exception ex)
        {
            AppendLog($"Error: {ex.Message}");
            AppendLog($"Exception type: {ex.GetType().Name}");
            if (ex.InnerException != null)
            {
                AppendLog($"Inner exception: {ex.InnerException.Message}");
            }
            if (ex.StackTrace != null)
            {
                AppendLog($"Stack trace: {ex.StackTrace}");
            }
        }
    }

    private async Task CheckRegressionsAsync()
    {
        try
        {
            if (!ValidateRepository())
            {
                return;
            }

            AppendLog("Checking for regressions...");

            // Redirect console output to log
            var originalOut = Console.Out;
            var logWriter = new StringWriter();
            Console.SetOut(logWriter);

            try
            {
                var cmd = _services.GetRequiredService<CheckRegressionsCommand>();
                var exitCode = await cmd.ExecuteAsync(_environmentService.Root, CancellationToken.None);

                var output = logWriter.ToString();
                if (!string.IsNullOrWhiteSpace(output))
                {
                    AppendLog(output.TrimEnd());
                }

                AppendLog(exitCode == 0 ? "No regressions found." : "Regressions detected!");
            }
            finally
            {
                Console.SetOut(originalOut);
                logWriter.Dispose();
            }
        }
        catch (Exception ex)
        {
            AppendLog($"Error: {ex.Message}");
            AppendLog($"Exception type: {ex.GetType().Name}");
            if (ex.InnerException != null)
            {
                AppendLog($"Inner exception: {ex.InnerException.Message}");
            }
            if (ex.StackTrace != null)
            {
                AppendLog($"Stack trace: {ex.StackTrace}");
            }
        }
    }
}