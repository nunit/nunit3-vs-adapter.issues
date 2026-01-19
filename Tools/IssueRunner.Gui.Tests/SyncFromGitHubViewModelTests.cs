using IssueRunner.Gui.ViewModels;
using NUnit.Framework;

namespace IssueRunner.Gui.Tests;

[TestFixture]
public class SyncFromGitHubViewModelTests
{
    [Test]
    public void Reset_SetsAllPropertiesToInitialState()
    {
        // Arrange
        var viewModel = new SyncFromGitHubViewModel();
        viewModel.StatusText = "Syncing...";
        viewModel.TotalIssues = 20;
        viewModel.IssuesSynced = 10;
        viewModel.IssuesFound = 8;
        viewModel.IssuesNotFound = 2;
        viewModel.IsRunning = true;
        viewModel.SyncCompleted = true;

        // Act
        viewModel.Reset();

        // Assert
        Assert.That(viewModel.StatusText, Is.EqualTo("Ready to sync..."));
        Assert.That(viewModel.IssuesSynced, Is.EqualTo(0));
        Assert.That(viewModel.IssuesFound, Is.EqualTo(0));
        Assert.That(viewModel.IssuesNotFound, Is.EqualTo(0));
        Assert.That(viewModel.Progress, Is.EqualTo(0.0));
        Assert.That(viewModel.TotalIssues, Is.EqualTo(0));
        Assert.That(viewModel.IsRunning, Is.False);
        Assert.That(viewModel.SyncCompleted, Is.False);
    }

    [Test]
    public void IssuesSynced_UpdatesProgress_WhenSet()
    {
        // Arrange
        var viewModel = new SyncFromGitHubViewModel();
        viewModel.TotalIssues = 10;

        // Act
        viewModel.IssuesSynced = 5;

        // Assert
        Assert.That(viewModel.Progress, Is.EqualTo(50.0));
        Assert.That(viewModel.ProgressText, Is.EqualTo("5 / 10 issues synced"));
    }

    [Test]
    public void TotalIssues_UpdatesProgress_WhenSet()
    {
        // Arrange
        var viewModel = new SyncFromGitHubViewModel();
        viewModel.IssuesSynced = 3;

        // Act
        viewModel.TotalIssues = 6;

        // Assert
        Assert.That(viewModel.Progress, Is.EqualTo(50.0));
        Assert.That(viewModel.ProgressText, Is.EqualTo("3 / 6 issues synced"));
    }

    [Test]
    public void IsRunning_UpdatesCanCancel_WhenSet()
    {
        // Arrange
        var viewModel = new SyncFromGitHubViewModel();
        Assert.That(viewModel.CanCancel, Is.True); // Default is true

        // Act
        viewModel.IsRunning = true;

        // Assert
        Assert.That(viewModel.CanCancel, Is.True);
        Assert.That(viewModel.CanStart, Is.False);
    }

    [Test]
    public void IsRunning_False_AllowsStart()
    {
        // Arrange
        var viewModel = new SyncFromGitHubViewModel();
        viewModel.IsRunning = true;

        // Act
        viewModel.IsRunning = false;

        // Assert
        Assert.That(viewModel.CanStart, Is.True);
    }

    [Test]
    public void SyncCompleted_UpdatesCanSyncToFolders_WhenSet()
    {
        // Arrange
        var viewModel = new SyncFromGitHubViewModel();
        viewModel.IsRunning = false;

        // Act
        viewModel.SyncCompleted = true;

        // Assert
        Assert.That(viewModel.CanSyncToFolders, Is.True);
    }

    [Test]
    public void IsRunning_True_DisablesSyncToFolders_EvenIfCompleted()
    {
        // Arrange
        var viewModel = new SyncFromGitHubViewModel();
        viewModel.SyncCompleted = true;
        viewModel.IsRunning = false;
        Assert.That(viewModel.CanSyncToFolders, Is.True);

        // Act
        viewModel.IsRunning = true;

        // Assert
        Assert.That(viewModel.CanSyncToFolders, Is.False);
    }

    [Test]
    public void ProgressText_ShowsCorrectFormat_WhenTotalIssuesIsZero()
    {
        // Arrange
        var viewModel = new SyncFromGitHubViewModel();
        viewModel.TotalIssues = 0;
        viewModel.IssuesSynced = 0;

        // Act & Assert
        Assert.That(viewModel.ProgressText, Is.EqualTo(""));

        // Act
        viewModel.IssuesSynced = 5;

        // Assert
        Assert.That(viewModel.ProgressText, Is.EqualTo("5 issues synced"));
    }

    [Test]
    public void CancelCommand_CanBeSetAndExecuted()
    {
        // Arrange
        var viewModel = new SyncFromGitHubViewModel();
        var cancelExecuted = false;
        var cancelCommand = ReactiveUI.ReactiveCommand.Create(() =>
        {
            cancelExecuted = true;
        });

        // Act
        viewModel.CancelCommand = cancelCommand;
        viewModel.CancelCommand.Execute().Subscribe();

        // Assert
        Assert.That(cancelExecuted, Is.True);
        Assert.That(viewModel.CancelCommand, Is.Not.Null);
    }

    [Test]
    public void CancelCommand_UpdatesStatusText_WhenExecuted()
    {
        // Arrange
        var viewModel = new SyncFromGitHubViewModel();
        viewModel.IsRunning = true;
        viewModel.StatusText = "Syncing...";
        
        var cancelCommand = ReactiveUI.ReactiveCommand.Create(() =>
        {
            viewModel.StatusText = "Cancelling...";
        });

        // Act
        viewModel.CancelCommand = cancelCommand;
        viewModel.CancelCommand.Execute().Subscribe();

        // Assert
        Assert.That(viewModel.StatusText, Is.EqualTo("Cancelling..."));
    }
}

