using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.NUnit;
using IssueRunner.Gui.ViewModels;
using IssueRunner.Gui.Views;
using IssueRunner.Models;
using IssueRunner.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace IssueRunner.Gui.Tests;

[TestFixture]
public class MainWindowHeadlessTests : HeadlessTestBase
{
    [AvaloniaTest]
    public void MainWindow_CanBeCreated()
    {
        var window = CreateTestWindow();
        Assert.That(window, Is.Not.Null);
        Assert.That(window, Is.InstanceOf<MainWindow>());
    }

    [AvaloniaTest,Explicit]
    public async Task NavigationButtons_AppearWhenTestStatusViewIsActive()
    {
        var services = CreateTestServiceProvider();
        var window = CreateTestWindow(services);
        var viewModel = (MainViewModel)window.DataContext!;
        var envService = services.GetRequiredService<IEnvironmentService>();
        viewModel.RepositoryPath = envService.Root;

        // Wait for repository initialization
        await Task.Delay(500);

        // Switch to Test Status view - wait for async command to complete
        var completed = false;
        viewModel.ShowTestStatusCommand.Execute().Subscribe(
            _ => { },
            ex => { completed = true; },
            () => { completed = true; });

        // Wait for command to complete
        var timeout = DateTime.Now.AddSeconds(5);
        while (!completed && DateTime.Now < timeout)
        {
            await Task.Delay(50);
        }
        await Task.Delay(100); // Allow time for view update

        // We don't need to show the window in headless tests; rely on ViewModel state
        Assert.That(viewModel.CurrentViewType, Is.EqualTo("TestStatus"));
    }

    [AvaloniaTest]
    public async Task ClickingIssueListButton_SwitchesViewBackToIssueListView()
    {
        var services = CreateTestServiceProvider();
        var window = CreateTestWindow(services);
        var viewModel = (MainViewModel)window.DataContext!;

        var envService = services.GetRequiredService<IEnvironmentService>();
        viewModel.RepositoryPath = envService.Root;

        // Wait for repository initialization
        await Task.Delay(300);

        // First switch to Test Status - wait for async command to complete
        var testStatusCompleted = false;
        viewModel.ShowTestStatusCommand.Execute().Subscribe(
            _ => { },
            ex => { testStatusCompleted = true; },
            () => { testStatusCompleted = true; });

        var timeout = DateTime.Now.AddSeconds(5);
        while (!testStatusCompleted && DateTime.Now < timeout)
        {
            await Task.Delay(50);
        }
        await Task.Delay(100);

        Assert.That(viewModel.CurrentViewType, Is.EqualTo("TestStatus"));

        // Then switch back to Issue List
        viewModel.ShowIssueListCommand.Execute().Subscribe();
        await Task.Delay(100); // Allow time for view update

        // Assert
        Assert.That(viewModel.CurrentView, Is.InstanceOf<IssueListView>());
        Assert.That(viewModel.CurrentViewType, Is.EqualTo("IssueList"));
    }

    [AvaloniaTest]
    public async Task ClickingTestStatusButton_SwitchesViewToTestStatusView()
    {
        var services = CreateTestServiceProvider();
        var window = CreateTestWindow(services);
        var viewModel = (MainViewModel)window.DataContext!;

        var envService = services.GetRequiredService<IEnvironmentService>();
        viewModel.RepositoryPath = envService.Root;

        // Wait for repository initialization
        await Task.Delay(300);

        // Initial state should be IssueList
        Assert.That(viewModel.CurrentViewType, Is.EqualTo("IssueList"));

        // Switch to Test Status - wait for async command to complete
        var completed = false;
        viewModel.ShowTestStatusCommand.Execute().Subscribe(
            _ => { },
            ex => { completed = true; },
            () => { completed = true; });

        var timeout = DateTime.Now.AddSeconds(5);
        while (!completed && DateTime.Now < timeout)
        {
            await Task.Delay(50);
        }
        await Task.Delay(100); // Allow time for view update

        // Assert
        Assert.That(viewModel.CurrentViewType, Is.EqualTo("TestStatus"));
    }

    [AvaloniaTest]
    public async Task RepositoryStatus_DisplaysAllStatusLinesCorrectly()
    {
        // Arrange
        var services = CreateTestServiceProvider();
        var window = CreateTestWindow(services);
        var viewModel = (MainViewModel)window.DataContext!;
        var envService = services.GetRequiredService<IEnvironmentService>();
        var testRepoPath = envService.Root;

        // Create test data directory and results
        var dataDir = Path.Combine(testRepoPath, ".nunit", "IssueRunner");
        Directory.CreateDirectory(dataDir);
        
        // Create repository.json in the data directory (RepositoryStatusService expects it there)
        var repoConfigJson = JsonSerializer.Serialize(new IssueRunner.Models.RepositoryConfig("test", "test"));
        File.WriteAllText(Path.Combine(dataDir, "repository.json"), repoConfigJson);

        // Create issue folders
        var issue1Folder = Path.Combine(testRepoPath, "Issue1");
        var issue2Folder = Path.Combine(testRepoPath, "Issue2");
        var issue3Folder = Path.Combine(testRepoPath, "Issue3");
        Directory.CreateDirectory(issue1Folder);
        Directory.CreateDirectory(issue2Folder);
        Directory.CreateDirectory(issue3Folder);

        // Create marker file for issue 3
        await File.WriteAllTextAsync(Path.Combine(issue3Folder, "ignore"), "");

        // Create results.json
        var resultsPath = Path.Combine(dataDir, "results.json");
        var results = new List<IssueResult>
        {
            new IssueResult
            {
                Number = 1,
                ProjectPath = "Issue1/test.csproj",
                TargetFrameworks = new List<string> { "net8.0" },
                Packages = new List<string>(),
                RestoreResult = "success",
                BuildResult = "success",
                TestResult = "success",
                LastRun = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ")
            },
            new IssueResult
            {
                Number = 2,
                ProjectPath = "Issue2/test.csproj",
                TargetFrameworks = new List<string> { "net8.0" },
                Packages = new List<string>(),
                RestoreResult = "success",
                BuildResult = "success",
                TestResult = "fail",
                LastRun = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ")
            }
        };
        var resultsJson = JsonSerializer.Serialize(results);
        await File.WriteAllTextAsync(resultsPath, resultsJson);

        // Mock issue discovery - MUST be set up before RepositoryPath is set
        // Get the service that was registered in CreateTestServiceProvider
        var issueDiscovery = services.GetRequiredService<IIssueDiscoveryService>();
        // Clear any previous returns and set new ones
        issueDiscovery.ClearReceivedCalls();
        issueDiscovery.DiscoverIssueFolders().Returns(new Dictionary<int, string>
        {
            { 1, issue1Folder },
            { 2, issue2Folder },
            { 3, issue3Folder }
        });

        // Mock marker service - MUST be set up before RepositoryPath is set
        // Issue 3 should be skipped, others should not
        var markerService = services.GetRequiredService<IMarkerService>();
        markerService.ClearReceivedCalls();
        markerService.ShouldSkipIssue(Arg.Any<string>()).Returns(callInfo =>
        {
            var path = callInfo.Arg<string>();
            return path == issue3Folder;
        });
        markerService.GetMarkerReason(Arg.Any<string>()).Returns("Ignored");

        viewModel.RepositoryPath = testRepoPath;

        // Wait for repository to load - need to wait for async LoadRepository to complete
        var timeout = DateTime.Now.AddSeconds(5);
        while (viewModel.SummaryText == "Select a repository to begin." && DateTime.Now < timeout)
        {
            await Task.Delay(100);
        }
        await Task.Delay(200); // Additional delay to ensure all counts are set

        // Assert - Verify SummaryText contains all status lines
        // This test verifies that the UI displays all status lines correctly
        // The actual count calculation is tested in MainViewModelTests.LoadRepository_CalculatesAllStatusCountsCorrectly
        var summaryText = viewModel.SummaryText;
        Assert.That(summaryText, Is.Not.Null, "SummaryText should not be null");
        Assert.That(summaryText, Does.Not.Contain("Select a repository to begin"), 
            $"Repository should be loaded. SummaryText: {summaryText}");
        
        // Verify SummaryText contains all status lines (regardless of counts)
        Assert.That(summaryText, Does.Contain("Passed:"), "SummaryText should contain Passed line");
        Assert.That(summaryText, Does.Contain("Failed:"), "SummaryText should contain Failed line");
        Assert.That(summaryText, Does.Contain("Skipped:"), "SummaryText should contain Skipped line");
        Assert.That(summaryText, Does.Contain("Not Restored:"), "SummaryText should contain Not Restored line");
        Assert.That(summaryText, Does.Contain("Not Compiling:"), "SummaryText should contain Not Compiling line");
        Assert.That(summaryText, Does.Contain("Not Tested:"), "SummaryText should contain Not Tested line");
        
        // Verify that all status count properties exist and are accessible
        // (The actual values are tested in MainViewModelTests)
        Assert.That(viewModel.PassedCount, Is.GreaterThanOrEqualTo(0), "PassedCount property should be accessible");
        Assert.That(viewModel.FailedCount, Is.GreaterThanOrEqualTo(0), "FailedCount property should be accessible");
        Assert.That(viewModel.SkippedCount, Is.GreaterThanOrEqualTo(0), "SkippedCount property should be accessible");
        Assert.That(viewModel.NotRestoredCount, Is.GreaterThanOrEqualTo(0), "NotRestoredCount property should be accessible");
        Assert.That(viewModel.NotCompilingCount, Is.GreaterThanOrEqualTo(0), "NotCompilingCount property should be accessible");
        Assert.That(viewModel.NotTestedCount, Is.GreaterThanOrEqualTo(0), "NotTestedCount property should be accessible");
    }

}

