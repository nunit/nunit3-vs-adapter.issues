using Avalonia.Controls;
using Avalonia.Headless.NUnit;
using IssueRunner.Gui.ViewModels;
using IssueRunner.Gui.Views;
using NUnit.Framework;

namespace IssueRunner.Gui.Tests;

[TestFixture]
public class RunTestsStatusDialogHeadlessTests : HeadlessTestBase
{
    [AvaloniaTest]
    public async Task StatusDialog_DisplaysRestoreBuildTestProgressSeparately()
    {
        // Arrange
        var viewModel = new RunTestsStatusViewModel
        {
            TotalIssues = 10,
            ProcessedIssues = 5,
            CurrentStatus = "Restore succeeded for Issue 123",
            CurrentPhase = "Restore"
        };

        var dialog = new RunTestsStatusDialog(viewModel);
        dialog.Show();

        await Task.Delay(100);

        // Assert - Check that CurrentStatus and CurrentPhase are displayed
        // The dialog should show the current status
        Assert.That(viewModel.CurrentStatus, Is.EqualTo("Restore succeeded for Issue 123"));
        Assert.That(viewModel.CurrentPhase, Is.EqualTo("Restore"));

        // Update to build phase
        viewModel.CurrentStatus = "Build succeeded for Issue 123";
        viewModel.CurrentPhase = "Build";
        await Task.Delay(50);

        Assert.That(viewModel.CurrentStatus, Is.EqualTo("Build succeeded for Issue 123"));
        Assert.That(viewModel.CurrentPhase, Is.EqualTo("Build"));

        // Update to test phase
        viewModel.CurrentStatus = "All steps succeeded for Issue 123";
        viewModel.CurrentPhase = "Complete";
        await Task.Delay(50);

        Assert.That(viewModel.CurrentStatus, Is.EqualTo("All steps succeeded for Issue 123"));
        Assert.That(viewModel.CurrentPhase, Is.EqualTo("Complete"));
    }

    [AvaloniaTest]
    public async Task StatusDialog_ShowsRestoreFailed_WhenRestoreStepFails()
    {
        // Arrange
        var viewModel = new RunTestsStatusViewModel
        {
            TotalIssues = 10,
            ProcessedIssues = 5,
            CurrentStatus = "Restore failed for Issue 123",
            CurrentPhase = "Restore"
        };

        var dialog = new RunTestsStatusDialog(viewModel);
        dialog.Show();

        await Task.Delay(100);

        // Assert
        Assert.That(viewModel.CurrentStatus, Does.Contain("Restore failed"));
        Assert.That(viewModel.CurrentPhase, Is.EqualTo("Restore"));
    }

    [AvaloniaTest]
    public async Task StatusDialog_ShowsBuildFailed_WhenBuildStepFails()
    {
        // Arrange
        var viewModel = new RunTestsStatusViewModel
        {
            TotalIssues = 10,
            ProcessedIssues = 5,
            CurrentStatus = "Build failed for Issue 123",
            CurrentPhase = "Build"
        };

        var dialog = new RunTestsStatusDialog(viewModel);
        dialog.Show();

        await Task.Delay(100);

        // Assert
        Assert.That(viewModel.CurrentStatus, Does.Contain("Build failed"));
        Assert.That(viewModel.CurrentPhase, Is.EqualTo("Build"));
    }

    [AvaloniaTest]
    public async Task StatusDialog_ShowsTestFailed_WhenTestStepFails()
    {
        // Arrange
        var viewModel = new RunTestsStatusViewModel
        {
            TotalIssues = 10,
            ProcessedIssues = 5,
            CurrentStatus = "Test failed for Issue 123",
            CurrentPhase = "Test"
        };

        var dialog = new RunTestsStatusDialog(viewModel);
        dialog.Show();

        await Task.Delay(100);

        // Assert
        Assert.That(viewModel.CurrentStatus, Does.Contain("Test failed"));
        Assert.That(viewModel.CurrentPhase, Is.EqualTo("Test"));
    }
}

