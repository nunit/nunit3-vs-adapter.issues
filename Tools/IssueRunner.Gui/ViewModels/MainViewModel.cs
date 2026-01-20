using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using IssueRunner.Commands;
using IssueRunner.Gui.Services;
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
    private RunOptions? _lastRunOptions;
    private readonly List<int> _foldersWithoutMetadata = [];

    private readonly IServiceProvider _services;
    private readonly IEnvironmentService _environmentService;
    private readonly IRepositoryStatusService _repositoryStatusService;
    private readonly ITestRunOrchestrator _testRunOrchestrator;
    private readonly ISyncCoordinator _syncCoordinator;

    public MainViewModel(IServiceProvider services)
    {
        try
        {
            _services = services;
            _environmentService = services.GetRequiredService<IEnvironmentService>();
            _repositoryStatusService = services.GetRequiredService<IRepositoryStatusService>();
            _testRunOrchestrator = services.GetRequiredService<ITestRunOrchestrator>();
            _syncCoordinator = services.GetRequiredService<ISyncCoordinator>();

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

            ClearLogCommand = ReactiveCommand.Create(ClearLog);

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

    private async Task ExecuteOnUIThreadAsync(Action action)
    {
        if (Avalonia.Threading.Dispatcher.UIThread.CheckAccess())
        {
            action();
        }
        else
        {
            await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(action);
        }
    }

    private async Task ExecuteCommandWithConsoleCaptureAsync(Func<Task<int>> command, Action<int> onComplete)
    {
        var originalOut = Console.Out;
        var logWriter = new StringWriter();
        Console.SetOut(logWriter);

        try
        {
            var exitCode = await command();

            var output = logWriter.ToString();
            if (!string.IsNullOrWhiteSpace(output))
            {
                AppendLog(output.TrimEnd());
            }

            onComplete(exitCode);
        }
        finally
        {
            Console.SetOut(originalOut);
            logWriter.Dispose();
        }
    }

    private void LogDetailedException(string context, Exception ex)
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
        LogExceptionToFile(context, ex);
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
                else
                {
                    // Saved path exists but is not a valid repository - log warning but continue to auto-detect
                    await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
                    {
                        AppendLog($"Warning: Saved repository path '{savedPath}' is not a valid repository. Attempting auto-detect...");
                    });
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
    public ReactiveCommand<Unit, Unit> ClearLogCommand { get; }

    public int FoldersWithoutMetadataCount => _foldersWithoutMetadata.Count;

    public bool IsRunningTests => _testRunOrchestrator.IsRunning;

    public int PassedCount
    {
        get;
        private set => SetProperty(ref field, value);
    }

    public int FailedCount
    {
        get;
        private set => SetProperty(ref field, value);
    }

    public int SkippedCount
    {
        get;
        private set => SetProperty(ref field, value);
    }

    public int NotRestoredCount
    {
        get;
        private set => SetProperty(ref field, value);
    }

    public int NotCompilingCount
    {
        get;
        private set => SetProperty(ref field, value);
    }

    public int NotTestedCount
    {
        get;
        private set => SetProperty(ref field, value);
    }

    private async Task BrowseRepositoryAsync()
    {
        var window = GetMainWindow();
        if (window == null)
        {
            AppendLog("Error: Could not find main window for dialog.");
            return;
        }

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
                var path = ExtractPathFromStorageFolder(folder[0]);
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

    private static string? ExtractPathFromStorageFolder(Avalonia.Platform.Storage.IStorageFolder folder)
    {
        var uri = folder.Path;
        if (uri == null)
        {
        return null;
    }

        if (uri.Scheme == "file")
        {
            var path = Uri.UnescapeDataString(uri.AbsolutePath);
            // On Windows, remove leading slash if present
            if (path.Length > 0 && path[0] == '/' && path.Length > 2 && path[2] == ':')
            {
                path = path[1..];
            }
            return path;
        }

        return uri.AbsolutePath;
    }

    private Window? GetMainWindow()
    {
        if (Avalonia.Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            return desktop.MainWindow;
        }
        return null;
    }

    private async void LoadRepository()
    {
        if (string.IsNullOrWhiteSpace(RepositoryPath) || !Directory.Exists(RepositoryPath))
        {
            SummaryText = "Invalid repository path. Please select a valid repository folder.";
            CurrentView = null;
            return;
        }

        try
        {
            var status = await _repositoryStatusService.LoadAsync(RepositoryPath, AppendLog);

            SummaryText = status.SummaryText;
            BaselineNUnitPackages = status.BaselineNUnitPackages;
            CurrentNUnitPackages = status.CurrentNUnitPackages;

            PassedCount = status.PassedCount;
            FailedCount = status.FailedCount;
            SkippedCount = status.SkippedCount;
            NotRestoredCount = status.NotRestoredCount;
            NotCompilingCount = status.NotCompilingCount;
            NotTestedCount = status.NotTestedCount;

            _foldersWithoutMetadata.Clear();
            _foldersWithoutMetadata.AddRange(status.FoldersWithoutMetadata);

            OnPropertyChanged(nameof(FoldersWithoutMetadataCount));
            OnPropertyChanged(nameof(HasFoldersWithoutMetadata));

            // Load issue list view (async, will update when ready)
            var issueListViewModel = _services.GetRequiredService<IssueListViewModel>();
            CurrentView = new IssueListView { DataContext = issueListViewModel };
            CurrentViewType = "IssueList";
            // Capture RepositoryPath to avoid timing issues with async execution
            var repoPath = RepositoryPath;
            _ = Task.Run(async () => await LoadIssuesIntoViewAsync(issueListViewModel, status.Folders, repoPath));
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

    private async Task LoadIssuesIntoViewAsync(IssueListViewModel viewModel, Dictionary<int, string> folders, string repositoryPath)
    {
        try
        {
            var loader = _services.GetRequiredService<IIssueListLoader>();
            var result = await loader.LoadIssuesAsync(repositoryPath, folders, AppendLog);

            await ExecuteOnUIThreadAsync(() =>
            {
                viewModel.IssueChanges = result.IssueChanges;
                viewModel.LoadIssues(result.Issues);
                viewModel.SetRunTestsCallback(async (issueNumbers) => await RunFilteredTestsAsync(issueNumbers));
                viewModel.SetShowOptionsCallback(async () => await ShowOptionsAsync());
                viewModel.SetSyncFromGitHubCommand(SyncFromGitHubCommand);
                viewModel.SetRepositoryBaseUrl(result.RepositoryBaseUrl);
            });
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
        var result = await _testRunOrchestrator.RunFilteredTestsAsync(
            issueNumbers,
            _lastRunOptions,
            AppendLog,
            LoadRepository,
            LoadIssuesIntoViewAsync,
            GetMainWindow);
        
        // Store last run options if tests were run
        // Note: The orchestrator doesn't return the options, so we keep the last ones
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

            var testStatusViewModel = new TestStatusViewModel(
                _environmentService,
                _services.GetRequiredService<IIssueDiscoveryService>(),
                _services.GetRequiredService<IMarkerService>(),
                _services.GetRequiredService<ITestResultAggregator>());
            await testStatusViewModel.LoadDataAsync(_environmentService.Root);

            var testStatusView = new TestStatusView
            {
                DataContext = testStatusViewModel
            };

            await ExecuteOnUIThreadAsync(() =>
                {
                    CurrentView = testStatusView;
                    CurrentViewType = "TestStatus";
                });
        }
        catch (Exception ex)
        {
            AppendLog($"Error showing test status: {ex.Message}");
            LogExceptionToFile("ShowTestStatus", ex);
        }
    }

    private async Task SetBaselineAsync()
    {
        await _testRunOrchestrator.SetBaselineAsync(AppendLog, LoadRepository);
    }

    // UpdateStatusFromLog has been moved to TestRunOrchestrator

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
        var result = await _testRunOrchestrator.RunTestsAsync(
            _lastRunOptions,
            AppendLog,
            LoadRepository,
            LoadIssuesIntoViewAsync,
            GetMainWindow);
        
        // Note: LastRunOptions is managed by the orchestrator internally
        // We may need to update this if we want to persist options across runs
    }


    private void CancelTests()
    {
        _testRunOrchestrator.CancelCurrentRun();
            AppendLog("Cancelling test run... Please wait for current operations to complete.");
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
            await _syncCoordinator.SyncFromGitHubAsync(
                ValidateRepository,
                GetMainWindow,
                AppendLog,
                async () => LoadRepository(),
                RepositoryPath);
        }
        catch (Exception ex)
        {
            LogDetailedException("SyncFromGitHubAsync", ex);
        }
    }

    // Sync details are delegated to ISyncCoordinator; MainViewModel only validates and passes callbacks.

    private void ListFoldersWithoutMetadata()
    {
        if (string.IsNullOrWhiteSpace(RepositoryPath) || !Directory.Exists(RepositoryPath))
        {
            AppendLog("Error: No repository loaded.");
                    return;
        }

        try
        {
            var discovery = _services.GetRequiredService<IIssueDiscoveryService>();
            var folders = discovery.DiscoverIssueFolders();
            var metadataNumbers = LoadMetadataNumbers();

            var foldersWithoutMetadata = folders.Keys.Where(n => !metadataNumbers.Contains(n)).OrderBy(n => n).ToList();
            var metadataWithoutFolders = metadataNumbers.Where(n => !folders.ContainsKey(n)).OrderBy(n => n).ToList();

            LogSummary(folders.Count, metadataNumbers.Count, foldersWithoutMetadata.Count, metadataWithoutFolders.Count);
            LogFoldersWithoutMetadata(foldersWithoutMetadata);
            LogMetadataWithoutFolders(metadataWithoutFolders);
            LogCalculationNote(folders.Count, metadataNumbers.Count, foldersWithoutMetadata.Count, metadataWithoutFolders.Count);
        }
        catch (Exception ex)
        {
            AppendLog($"Error listing folders without metadata: {ex.Message}");
        }
    }

    private HashSet<int> LoadMetadataNumbers()
    {
        var dataDir = _environmentService.GetDataDirectory(RepositoryPath);
        var metadataPath = Path.Combine(dataDir, "issues_metadata.json");

        if (!File.Exists(metadataPath))
        {
            return new HashSet<int>();
        }

                try
                {
                    var metadataJson = File.ReadAllText(metadataPath);
                    var metadataList = JsonSerializer.Deserialize<List<IssueMetadata>>(metadataJson) ?? [];
            return metadataList.Select(m => m.Number).Distinct().ToHashSet();
                }
                catch (Exception ex)
                {
                    AppendLog($"Error reading metadata: {ex.Message}");
            return new HashSet<int>();
        }
    }

    private void LogSummary(int folderCount, int metadataCount, int foldersWithoutMetadataCount, int metadataWithoutFoldersCount)
    {
        AppendLog("Summary:");
        AppendLog($"  Total folders: {folderCount}");
        AppendLog($"  Total metadata entries: {metadataCount}");
        AppendLog($"  Folders without metadata: {foldersWithoutMetadataCount}");
        if (metadataWithoutFoldersCount > 0)
        {
            AppendLog($"  Metadata without folders: {metadataWithoutFoldersCount}");
            }
            AppendLog("");
    }

    private void LogFoldersWithoutMetadata(List<int> foldersWithoutMetadata)
    {
            if (foldersWithoutMetadata.Count == 0)
            {
                AppendLog("No folders without metadata found.");
            }
            else
            {
                AppendLog($"Folders without metadata ({foldersWithoutMetadata.Count}):");
                AppendLog(string.Join(", ", foldersWithoutMetadata));
            AppendLog("These folders exist but don't have corresponding issues in GitHub (may have been moved or deleted).");
        }
            }

    private void LogMetadataWithoutFolders(List<int> metadataWithoutFolders)
    {
            if (metadataWithoutFolders.Any())
            {
                AppendLog("");
                AppendLog($"Metadata entries without folders ({metadataWithoutFolders.Count}):");
                AppendLog(string.Join(", ", metadataWithoutFolders));
            AppendLog("These metadata entries exist but the folders have been deleted.");
        }
    }

    private void LogCalculationNote(int folderCount, int metadataCount, int foldersWithoutMetadataCount, int metadataWithoutFoldersCount)
    {
        var expectedDifference = folderCount - metadataCount;
        if (expectedDifference != foldersWithoutMetadataCount)
            {
                AppendLog("");
            AppendLog("Note: Simple calculation doesn't match because:");
            AppendLog($"  {metadataWithoutFoldersCount} metadata entries don't have folders");
            AppendLog($"  So: {folderCount} folders - ({metadataCount} metadata - {metadataWithoutFoldersCount} without folders) = {folderCount - (metadataCount - metadataWithoutFoldersCount)}");
        }
    }

    private async Task SyncToFoldersAsync(SyncFromGitHubViewModel? viewModel = null, CancellationTokenSource? cancellationSource = null)
    {
        try
        {
            var token = cancellationSource?.Token ?? CancellationToken.None;
            await _syncCoordinator.SyncToFoldersAsync(
                ValidateRepository,
                AppendLog,
                RepositoryPath,
                token);
        }
        catch (Exception ex)
        {
            LogDetailedException("SyncToFoldersAsync", ex);
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

            await ExecuteCommandWithConsoleCaptureAsync(async () =>
            {
                var cmd = _services.GetRequiredService<GenerateReportCommand>();
                return await cmd.ExecuteAsync(CancellationToken.None);
            }, 
            exitCode => AppendLog(exitCode == 0
                    ? "Report generated successfully."
                : $"Report generation completed with errors (exit code: {exitCode})."));
        }
        catch (Exception ex)
        {
            LogDetailedException("GenerateReportAsync", ex);
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

            await ExecuteCommandWithConsoleCaptureAsync(async () =>
            {
                var cmd = _services.GetRequiredService<CheckRegressionsCommand>();
                return await cmd.ExecuteAsync(_environmentService.Root, CancellationToken.None);
            },
            exitCode => AppendLog(exitCode == 0 ? "No regressions found." : "Regressions detected!"));
        }
        catch (Exception ex)
        {
            LogDetailedException("CheckRegressionsAsync", ex);
        }
    }
}