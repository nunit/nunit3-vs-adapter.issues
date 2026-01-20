using Avalonia.Controls;
using IssueRunner.Commands;
using IssueRunner.Gui.ViewModels;
using IssueRunner.Gui.Views;
using IssueRunner.Models;
using IssueRunner.Services;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace IssueRunner.Gui.Services;

/// <summary>
/// Service responsible for orchestrating test runs, managing status dialogs, and handling baseline operations.
/// </summary>
public sealed class TestRunOrchestrator : ITestRunOrchestrator
{
    private readonly IServiceProvider _services;
    private readonly IEnvironmentService _environmentService;
    private readonly IIssueDiscoveryService _issueDiscovery;
    private CancellationTokenSource? _cancellationSource;
    private RunTestsStatusViewModel? _statusViewModel;
    private RunTestsStatusDialog? _statusDialog;

    public TestRunOrchestrator(
        IServiceProvider services,
        IEnvironmentService environmentService,
        IIssueDiscoveryService issueDiscovery)
    {
        _services = services;
        _environmentService = environmentService;
        _issueDiscovery = issueDiscovery;
    }

    public bool IsRunning => _cancellationSource != null && !_cancellationSource.Token.IsCancellationRequested;

    public async Task<bool> RunTestsAsync(
        RunOptions? lastRunOptions,
        Action<string> log,
        Action onRepositoryReload,
        Func<IssueListViewModel, Dictionary<int, string>, string, Task> onIssuesReload,
        Func<Window?> getMainWindow,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (!ValidateRepository(log))
            {
                return false;
            }

            // Show options dialog with last used options or defaults
            var optionsViewModel = new RunTestsOptionsViewModel(lastRunOptions, _environmentService);

            var dialog = new RunTestsOptionsDialog(optionsViewModel)
            {
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            var window = getMainWindow();
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
                log("Run Tests cancelled.");
                return false;
            }

            var options = optionsViewModel.Result;

            // Determine total issues
            var totalIssues = options.IssueNumbers?.Count ?? 0;
            if (totalIssues == 0)
            {
                // Need to count issues that will be run
                var issueFolders = _issueDiscovery.DiscoverIssueFolders();
                totalIssues = issueFolders.Count;
            }

            return await ExecuteTestRunAsync(
                options,
                totalIssues,
                log,
                onRepositoryReload,
                onIssuesReload,
                getMainWindow,
                cancellationToken);
        }
        catch (Exception ex)
        {
            log($"Error: {ex.Message}");
            log($"Exception type: {ex.GetType().Name}");
            if (ex.InnerException != null)
            {
                log($"Inner exception: {ex.InnerException.Message}");
            }
            if (ex.StackTrace != null)
            {
                log($"Stack trace: {ex.StackTrace}");
            }
            return false;
        }
    }

    public async Task<bool> RunFilteredTestsAsync(
        List<int> issueNumbers,
        RunOptions? lastRunOptions,
        Action<string> log,
        Action onRepositoryReload,
        Func<IssueListViewModel, Dictionary<int, string>, string, Task> onIssuesReload,
        Func<Window?> getMainWindow,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (!ValidateRepository(log))
            {
                return false;
            }

            // Show options dialog
            var baseOptions = lastRunOptions ?? new RunOptions
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

            var window = getMainWindow();
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
                log("Run Tests cancelled.");
                return false;
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

            return await ExecuteTestRunAsync(
                options,
                issueNumbers.Count,
                log,
                onRepositoryReload,
                onIssuesReload,
                getMainWindow,
                cancellationToken);
        }
        catch (Exception ex)
        {
            log($"Error running filtered tests: {ex.Message}");
            log($"Exception type: {ex.GetType().Name}");
            if (ex.StackTrace != null)
            {
                log($"Stack trace: {ex.StackTrace}");
            }
            return false;
        }
    }

    private async Task<bool> ExecuteTestRunAsync(
        RunOptions options,
        int totalIssues,
        Action<string> log,
        Action onRepositoryReload,
        Func<IssueListViewModel, Dictionary<int, string>, string, Task> onIssuesReload,
        Func<Window?> getMainWindow,
        CancellationToken cancellationToken)
    {
        // Create cancellation token source
        _cancellationSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        // Create and show status dialog
        _statusViewModel = new RunTestsStatusViewModel
        {
            TotalIssues = totalIssues,
            IsRunning = true,
            IsStableFeed = options.Feed == PackageFeed.Stable,
            StartTime = DateTime.Now
        };
        _statusViewModel.CancelCommand = ReactiveCommand.Create(() =>
        {
            if (_cancellationSource != null && !_cancellationSource.Token.IsCancellationRequested)
            {
                _statusViewModel!.CurrentStatus = "Cancelling...";
                _cancellationSource.Cancel();
            }
        });
        _statusViewModel.SetBaselineCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            await SetBaselineAsync(log, onRepositoryReload);
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

        var mainWindowForDialog = getMainWindow();
        if (mainWindowForDialog != null)
        {
            // Show dialog without blocking - use Show() instead of ShowDialog() for non-modal
            _statusDialog.Show(mainWindowForDialog);
        }

        log("Running tests...");
        log($"Scope: {options.Scope}, Feed: {options.Feed}, TestTypes: {options.TestTypes}");
        if (options.IssueNumbers != null && options.IssueNumbers.Count > 0)
        {
            log($"Issues: {string.Join(", ", options.IssueNumbers)}");
        }

        // Create a real-time console writer that updates both log and status
        var originalOut = Console.Out;
        var realTimeWriter = new RealTimeLogWriter(log, UpdateStatusFromLog);
        Console.SetOut(realTimeWriter);

        try
        {
            // Run command in background task to allow real-time updates
            var cmd = _services.GetRequiredService<RunTestsCommand>();
            var exitCode = await Task.Run(async () =>
                await cmd.ExecuteAsync(_environmentService.Root, options, _cancellationSource.Token));

            if (_cancellationSource.Token.IsCancellationRequested)
            {
                log("Test run was cancelled by user.");
            }
            else if (exitCode == 0)
            {
                log("Test run completed successfully.");
                // Reload repository to refresh the issue list
                onRepositoryReload();
            }
            else
            {
                log($"Test run completed with errors (exit code: {exitCode}).");
            }

            return true;
        }
        catch (OperationCanceledException)
        {
            log("Test run was cancelled by user.");
            return false;
        }
        finally
        {
            Console.SetOut(originalOut);
            realTimeWriter.Dispose();

            // Update status dialog
            if (_statusViewModel != null)
            {
                _statusViewModel.EndTime = DateTime.Now;
                _statusViewModel.IsRunning = false;
                if (_cancellationSource?.Token.IsCancellationRequested == true)
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

            _cancellationSource?.Dispose();
            _cancellationSource = null;

            // Refresh the issue list view to show updated states (marker files, test results, etc.)
            var mainWindow = getMainWindow();
            if (mainWindow?.DataContext is MainViewModel mainViewModel)
            {
                if (mainViewModel.CurrentView is IssueListView issueListView &&
                    issueListView.DataContext is IssueListViewModel issueListViewModel)
                {
                    var folders = _issueDiscovery.DiscoverIssueFolders();
                    var repoPath = _environmentService.Root;
                    _ = Task.Run(async () => await onIssuesReload(issueListViewModel, folders, repoPath));
                }
            }
        }
    }

    public async Task SetBaselineAsync(
        Action<string> log,
        Action onRepositoryReload)
    {
        try
        {
            if (!ValidateRepository(log))
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
                log("No test results to set as baseline.");
                return;
            }

            // Copy current results.json to results-baseline.json
            File.Copy(resultsPath, baselineResultsPath, overwrite: true);

            // Copy current package versions to baseline
            if (File.Exists(currentPackagesPath))
            {
                File.Copy(currentPackagesPath, baselinePackagesPath, overwrite: true);
            }

            log("Baseline set successfully.");

            // Reload repository to update summary
            onRepositoryReload();

            // Update status dialog if open
            if (_statusViewModel != null)
            {
                _statusViewModel.CanSetBaseline = false; // Already set
            }
        }
        catch (Exception ex)
        {
            log($"Error setting baseline: {ex.Message}");
        }
    }

    public void CancelCurrentRun()
    {
        if (_cancellationSource is { Token.IsCancellationRequested: false })
        {
            _cancellationSource.Cancel();
            _statusViewModel?.CurrentStatus = "Cancelling...";
        }
    }

    private bool ValidateRepository(Action<string> log)
    {
        if (string.IsNullOrWhiteSpace(_environmentService.Root) || !Directory.Exists(_environmentService.Root))
        {
            log("Error: No repository loaded. Please select a repository first.");
            return false;
        }
        return true;
    }

    private void UpdateStatusFromLog(string logLine)
    {
        if (_statusViewModel == null) return;

        try
        {
            // Parse issue number and message
            var match = Regex.Match(logLine, @"\[(\d+)\]\s*(.+)");
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
            }
        }
        catch
        {
            // Ignore errors in status updates
        }
    }
}
