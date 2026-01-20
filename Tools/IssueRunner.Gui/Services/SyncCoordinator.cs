using Avalonia.Controls;
using IssueRunner.Commands;
using IssueRunner.Gui.ViewModels;
using IssueRunner.Gui.Views;
using IssueRunner.Models;
using IssueRunner.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace IssueRunner.Gui.Services;

/// <summary>
/// Default implementation of <see cref="ISyncCoordinator"/> that owns all sync-related logic.
/// </summary>
public sealed class SyncCoordinator : ISyncCoordinator
{
    private readonly IServiceProvider _services;
    private readonly IEnvironmentService _environmentService;
    private readonly IIssueDiscoveryService _issueDiscovery;

    public SyncCoordinator(
        IServiceProvider services,
        IEnvironmentService environmentService,
        IIssueDiscoveryService issueDiscovery)
    {
        _services = services;
        _environmentService = environmentService;
        _issueDiscovery = issueDiscovery;
    }

    public async Task SyncFromGitHubAsync(
        Func<bool> validateRepository,
        Func<Window?> getMainWindow,
        Action<string> log,
        Func<Task> reloadRepository,
        string repositoryPath)
    {
        if (!validateRepository())
        {
            return;
        }

        var mainWindow = getMainWindow();
        if (mainWindow == null)
        {
            log("Error: Could not find main window");
            return;
        }

        // Create and set up sync dialog
        var syncViewModel = new SyncFromGitHubViewModel();
        var cancellationSource = new CancellationTokenSource();
        SyncFromGitHubDialog? syncDialog = null;

        // Set up commands BEFORE creating dialog
        syncViewModel.StartCommand = ReactiveUI.ReactiveCommand.CreateFromTask(async () =>
        {
            await StartSyncAsync(syncViewModel, cancellationSource, log, reloadRepository, repositoryPath);
        });
        syncViewModel.CancelCommand = ReactiveUI.ReactiveCommand.Create(() =>
        {
            cancellationSource.Cancel();
            syncViewModel.StatusText = "Cancelling...";
        });
        syncViewModel.SyncToFoldersCommand = ReactiveUI.ReactiveCommand.CreateFromTask(async () =>
        {
            await SyncToFoldersInternalAsync(syncViewModel, cancellationSource, validateRepository, log, repositoryPath);
        });

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

    public async Task SyncToFoldersAsync(
        Func<bool> validateRepository,
        Action<string> log,
        string repositoryPath,
        CancellationToken cancellationToken = default)
    {
        await SyncToFoldersInternalAsync(
            viewModel: null,
            cancellationSource: CancellationTokenSource.CreateLinkedTokenSource(cancellationToken),
            validateRepository,
            log,
            repositoryPath);
    }

    private async Task StartSyncAsync(
        SyncFromGitHubViewModel viewModel,
        CancellationTokenSource cancellationSource,
        Action<string> log,
        Func<Task> reloadRepository,
        string repositoryPath)
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
            var issueFolders = _issueDiscovery.DiscoverIssueFolders();
            var allIssueNumbers = issueFolders.Keys.OrderBy(n => n).ToList();

            // Load existing metadata to determine total count
            var dataDir = _environmentService.GetDataDirectory(repositoryPath);
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
                            log($"Warning: Duplicate metadata entries found for issue {dup.Key}. Using the last occurrence.");
                        }
                    }

                    // Use GroupBy().ToDictionary() to take the last occurrence of each duplicate
                    existingMetadata = existingList
                        .GroupBy(m => m.Number)
                        .ToDictionary(g => g.Key, g => g.Last());
                }
                catch
                {
                    // Ignore metadata load errors - sync can still proceed
                }
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
                    log($"Missing Metadata sync: All {totalIssueCount} issues already have metadata. Nothing to sync.");
                    viewModel.IsRunning = false;
                    viewModel.SyncCompleted = true;
                    return;
                }
                viewModel.StatusText = $"Missing Metadata sync: {missingCount} issues need metadata out of {totalIssueCount} total ({existingCount} already have metadata).";
                log($"Missing Metadata sync: Found {missingCount} issues without metadata out of {totalIssueCount} total issues.");
                log($"Syncing {missingCount} issues that are missing metadata...");
            }
            else
            {
                totalIssues = totalIssueCount;
                viewModel.StatusText = $"Syncing all {totalIssues} issues...";
                log($"All sync: Syncing all {totalIssues} issues from GitHub...");
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
                    log(output.TrimEnd());
                }

                viewModel.IsRunning = false;
                viewModel.SyncCompleted = true;

                // Build detailed completion message
                var syncModeDisplay = syncMode == "MissingMetadata" ? "Missing Metadata" : "All";
                var summary = $"{viewModel.IssuesSynced} synced, {viewModel.IssuesFound} found, {viewModel.IssuesNotFound} not found";

                if (exitCode == 0)
                {
                    viewModel.StatusText = $"{syncModeDisplay} sync completed successfully. {summary}.";
                    log($"{syncModeDisplay} sync completed successfully.");
                    log($"Summary: {summary}.");
                }
                else
                {
                    viewModel.StatusText = $"{syncModeDisplay} sync completed with errors. {summary}.";
                    log($"{syncModeDisplay} sync completed with errors (exit code: {exitCode}).");
                    log($"Summary: {summary}.");
                }

                // Reload repository to refresh issue list
                await reloadRepository();
            }
            catch (OperationCanceledException)
            {
                viewModel.IsRunning = false;
                viewModel.StatusText = "Sync cancelled by user.";
                log("Sync from GitHub cancelled.");
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
            log($"Error during sync: {ex.Message}");
        }
    }

    private static void UpdateSyncProgress(
        SyncFromGitHubViewModel viewModel,
        int issueNumber,
        bool found,
        bool synced)
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

        // We currently don't expose ProgressPercent on the view model, but we can update status text
        viewModel.StatusText = $"Syncing issue {issueNumber}... ({viewModel.IssuesSynced}/{viewModel.TotalIssues} synced, {viewModel.IssuesFound} found, {viewModel.IssuesNotFound} not found)";
    }

    private async Task SyncToFoldersInternalAsync(
        SyncFromGitHubViewModel? viewModel,
        CancellationTokenSource cancellationSource,
        Func<bool> validateRepository,
        Action<string> log,
        string repositoryPath)
    {
        try
        {
            if (!validateRepository())
            {
                if (viewModel != null)
                {
                    viewModel.StatusText = "Error: No repository loaded.";
                }
                return;
            }

            // Check if metadata file exists
            var dataDir = _environmentService.GetDataDirectory(repositoryPath);
            var metadataPath = Path.Combine(dataDir, "issues_metadata.json");
            if (!File.Exists(metadataPath))
            {
                viewModel?.StatusText = "Error: No metadata file found. Please sync from GitHub first.";
                log("Error: No metadata file found. Please sync from GitHub first.");
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

            log("Syncing metadata to folders...");

            // Get total issue count for progress
            var issueFolders = _issueDiscovery.DiscoverIssueFolders();
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
                var cancellationToken = cancellationSource.Token;
                var exitCode = await cmd.ExecuteAsync(_environmentService.Root, null, cancellationToken);

                var output = logWriter.ToString();
                if (!string.IsNullOrWhiteSpace(output))
                {
                    log(output.TrimEnd());
                }

                if (viewModel != null)
                {
                    if (Avalonia.Threading.Dispatcher.UIThread.CheckAccess())
                    {
                        viewModel.Progress = 100;
                        viewModel.IssuesSynced = totalIssues; // All issues processed
                        viewModel.IsRunning = false;

                        viewModel.StatusText = exitCode == 0
                            ? $"Sync to folders completed successfully. {totalIssues} issue folders updated."
                            : $"Sync to folders completed with errors (exit code: {exitCode}).";
                    }
                    else
                    {
                        await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
                        {
                            viewModel.Progress = 100;
                            viewModel.IssuesSynced = totalIssues;
                            viewModel.IsRunning = false;

                            viewModel.StatusText = exitCode == 0
                                ? $"Sync to folders completed successfully. {totalIssues} issue folders updated."
                                : $"Sync to folders completed with errors (exit code: {exitCode}).";
                        });
                    }

                    log(exitCode == 0
                        ? "Sync to folders completed successfully."
                        : $"Sync to folders completed with errors (exit code: {exitCode}).");
                }
                else
                {
                    log(exitCode == 0
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
            log("Sync to folders cancelled by user.");
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
            log($"Error: {ex.Message}");
        }
    }
}

