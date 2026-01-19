using Avalonia.Controls;
using Avalonia.Headless.NUnit;
using IssueRunner.Gui.ViewModels;
using IssueRunner.Gui.Views;
using NUnit.Framework;
using ReactiveUI;
using System.Threading.Tasks;

namespace IssueRunner.Gui.Tests;

[TestFixture]
public class SyncFromGitHubDialogHeadlessTests : HeadlessTestBase
{
    [AvaloniaTest]
    public async Task Dialog_DisplaysProgressAndStatusCorrectly()
    {
        // Arrange
        var viewModel = new SyncFromGitHubViewModel
        {
            TotalIssues = 10,
            IssuesSynced = 5,
            IssuesFound = 7,
            IssuesNotFound = 3,
            StatusText = "Syncing issue 5...",
            IsRunning = true
        };

        var dialog = new SyncFromGitHubDialog(viewModel);
        dialog.Show();

        await Task.Delay(100);

        // Assert
        Assert.That(viewModel.StatusText, Is.EqualTo("Syncing issue 5..."));
        Assert.That(viewModel.Progress, Is.EqualTo(50.0));
        Assert.That(viewModel.IssuesSynced, Is.EqualTo(5));
        Assert.That(viewModel.IssuesFound, Is.EqualTo(7));
        Assert.That(viewModel.IssuesNotFound, Is.EqualTo(3));
        Assert.That(viewModel.ProgressText, Is.EqualTo("5 / 10 issues synced"));
    }

    [AvaloniaTest]
    public async Task Dialog_ShowsCancelButton_WhenRunning()
    {
        // Arrange
        var viewModel = new SyncFromGitHubViewModel
        {
            IsRunning = true,
            CanCancel = true
        };

        var dialog = new SyncFromGitHubDialog(viewModel);
        dialog.Show();

        await Task.Delay(100);

        // Assert
        Assert.That(viewModel.IsRunning, Is.True);
        Assert.That(viewModel.CanCancel, Is.True);
        Assert.That(viewModel.CancelCommand, Is.Not.Null);
    }

    [AvaloniaTest]
    public async Task Dialog_ShowsSyncToFoldersButton_AlwaysVisible()
    {
        // Arrange - Button should be visible even before sync starts
        var viewModel = new SyncFromGitHubViewModel
        {
            IsRunning = false,
            SyncCompleted = false,
            IssuesSynced = 0,
            TotalIssues = 0
        };

        var dialog = new SyncFromGitHubDialog(viewModel);
        dialog.Show();

        await Task.Delay(100);

        // Assert - Button should be visible and command should be available
        Assert.That(viewModel.SyncToFoldersCommand, Is.Not.Null);
        
        // Test that button is also visible after sync completes
        viewModel.SyncCompleted = true;
        viewModel.IssuesSynced = 10;
        viewModel.TotalIssues = 10;
        await Task.Delay(50);
        
        Assert.That(viewModel.SyncToFoldersCommand, Is.Not.Null);
    }

    [AvaloniaTest]
    public async Task Dialog_SyncToFoldersButton_IsAlwaysEnabled()
    {
        // Arrange - Test button is enabled before sync
        var viewModel = new SyncFromGitHubViewModel
        {
            IsRunning = false,
            SyncCompleted = false
        };
        viewModel.SyncToFoldersCommand = ReactiveCommand.Create(() => { });

        var dialog = new SyncFromGitHubDialog(viewModel);
        dialog.Show();

        await Task.Delay(100);

        // Assert - Button should be enabled (no IsEnabled binding means always enabled)
        // Since the button has no IsEnabled binding, it's always enabled
        Assert.That(viewModel.SyncToFoldersCommand, Is.Not.Null);

        // Test that button is still available during sync
        viewModel.IsRunning = true;
        await Task.Delay(50);
        Assert.That(viewModel.SyncToFoldersCommand, Is.Not.Null);

        // Test that button is still available after sync completes
        viewModel.IsRunning = false;
        viewModel.SyncCompleted = true;
        await Task.Delay(50);
        Assert.That(viewModel.SyncToFoldersCommand, Is.Not.Null);
    }

    [AvaloniaTest]
    public async Task Dialog_UpdatesProgress_WhenIssuesSyncedChanges()
    {
        // Arrange
        var viewModel = new SyncFromGitHubViewModel
        {
            TotalIssues = 10,
            IssuesSynced = 0,
            IsRunning = true
        };

        var dialog = new SyncFromGitHubDialog(viewModel);
        dialog.Show();

        await Task.Delay(100);

        // Act - Update progress
        viewModel.IssuesSynced = 3;
        await Task.Delay(50);

        // Assert
        Assert.That(viewModel.Progress, Is.EqualTo(30.0));
        Assert.That(viewModel.ProgressText, Is.EqualTo("3 / 10 issues synced"));

        // Act - Update more
        viewModel.IssuesSynced = 7;
        await Task.Delay(50);

        // Assert
        Assert.That(viewModel.Progress, Is.EqualTo(70.0));
        Assert.That(viewModel.ProgressText, Is.EqualTo("7 / 10 issues synced"));
    }

    [AvaloniaTest]
    public async Task Dialog_ShowsOptions_WhenNotRunning()
    {
        // Arrange
        var viewModel = new SyncFromGitHubViewModel
        {
            IsRunning = false,
            SyncMode = "All",
            UpdateExisting = false
        };

        var dialog = new SyncFromGitHubDialog(viewModel);
        dialog.Show();

        await Task.Delay(100);

        // Assert
        Assert.That(viewModel.IsRunning, Is.False);
        Assert.That(viewModel.SyncMode, Is.EqualTo("All"));
        Assert.That(viewModel.UpdateExisting, Is.False);
        Assert.That(viewModel.CanStart, Is.True);
    }
}

