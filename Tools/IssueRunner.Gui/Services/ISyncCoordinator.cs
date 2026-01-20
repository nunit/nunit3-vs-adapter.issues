using Avalonia.Controls;
using IssueRunner.Gui.ViewModels;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace IssueRunner.Gui.Services;

/// <summary>
/// Coordinates synchronization operations with GitHub and syncing metadata to folders.
/// </summary>
public interface ISyncCoordinator
{
    /// <summary>
    /// Shows the Sync From GitHub dialog and runs a sync operation.
    /// </summary>
    Task SyncFromGitHubAsync(
        Func<bool> validateRepository,
        Func<Window?> getMainWindow,
        Action<string> log,
        Func<Task> reloadRepository,
        string repositoryPath);

    /// <summary>
    /// Runs Sync To Folders directly (used by separate command/button).
    /// </summary>
    Task SyncToFoldersAsync(
        Func<bool> validateRepository,
        Action<string> log,
        string repositoryPath,
        CancellationToken cancellationToken = default);
}

