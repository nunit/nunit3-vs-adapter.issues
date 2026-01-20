using Avalonia.Controls;
using Avalonia.Headless.NUnit;
using IssueRunner.Gui.ViewModels;
using IssueRunner.Gui.Views;
using IssueRunner.Models;
using IssueRunner.Services;
using Microsoft.Extensions.DependencyInjection;

namespace IssueRunner.Gui.Tests;

[TestFixture]
public class IssueListViewHeadlessTests : HeadlessTestBase
{
    [AvaloniaTest]
    public async Task RunTestsButton_AppearsAndIsEnabledWhenFiltersActive()
    {
        // Create services first, then create window with those services
        var services = CreateTestServiceProvider();
        var window = CreateTestWindow(services);
        var mainViewModel = (MainViewModel)window.DataContext!;

        // Initialize repository so callbacks are wired
        var envService = services.GetRequiredService<IEnvironmentService>();
        mainViewModel.RepositoryPath = envService.Root;
        await Task.Delay(300);

        // Get IssueListView and its view model
        var issueListView = mainViewModel.CurrentView as IssueListView;
        Assert.That(issueListView, Is.Not.Null, "CurrentView should be IssueListView");
        var issueListViewModel = (IssueListViewModel)issueListView!.DataContext!;

        // Add a test issue and activate a filter
        issueListViewModel.LoadIssues([
            new IssueListItem { Number = 1, TestTypes = "Scripts", TestResult = "success" }
        ]);
        issueListViewModel.SelectedTestTypes = "Scripts only";

        window.Show();
        await Task.Delay(100);

        var runButton = issueListView.FindControl<Button>("IssueListRunTestsButton");

        Assert.That(runButton, Is.Not.Null);
        Assert.That(runButton!.IsEnabled, Is.True);
    }

    [AvaloniaTest]
    public async Task OptionsButton_AppearsAndOpensRunTestsOptionsDialog()
    {
        var services = CreateTestServiceProvider();
        var window = CreateTestWindow(services);
        var mainViewModel = (MainViewModel)window.DataContext!;

        var envService = services.GetRequiredService<IEnvironmentService>();
        mainViewModel.RepositoryPath = envService.Root;
        await Task.Delay(300);

        var issueListView = mainViewModel.CurrentView as IssueListView;
        Assert.That(issueListView, Is.Not.Null, "CurrentView should be IssueListView");

        window.Show();
        await Task.Delay(100);

        var optionsButton = issueListView!.FindControl<Button>("IssueListOptionsButton");
        Assert.That(optionsButton, Is.Not.Null);

        // Execute the command; we just verify that it is wired and does not throw in headless mode
        Assert.That(optionsButton!.Command, Is.Not.Null);
        optionsButton.Command!.Execute(optionsButton.CommandParameter);
        await Task.Delay(100);
    }

    [AvaloniaTest]
    public async Task TestTypesFilter_ComboBoxFiltersIssuesWhenSelectionChanges()
    {
        var services = CreateTestServiceProvider();
        var window = CreateTestWindow(services);
        var mainViewModel = (MainViewModel)window.DataContext!;

        var envService = services.GetRequiredService<IEnvironmentService>();
        mainViewModel.RepositoryPath = envService.Root;
        await Task.Delay(300);

        var issueListView = mainViewModel.CurrentView as IssueListView;
        Assert.That(issueListView, Is.Not.Null, "CurrentView should be IssueListView");
        var vm = (IssueListViewModel)issueListView!.DataContext!;

        vm.LoadIssues(new[]
        {
            new IssueListItem { Number = 1, TestTypes = "Scripts" },
            new IssueListItem { Number = 2, TestTypes = "DotNet test" }
        });

        // Initial: All
        Assert.That(vm.Issues.Count, Is.EqualTo(2));

        // Filter to Scripts only (maps to \"Scripts\" in the column)
        vm.SelectedTestTypes = "Scripts only";
        await Task.Delay(50);
        Assert.That(vm.Issues.Count, Is.EqualTo(1));
        Assert.That(vm.Issues.First().TestTypes, Is.EqualTo("Scripts"));

        // Filter to dotnet test only (maps to \"DotNet test\" in the column)
        vm.SelectedTestTypes = "dotnet test only";
        await Task.Delay(50);
        Assert.That(vm.Issues.Count, Is.EqualTo(1));
        Assert.That(vm.Issues.First().TestTypes, Is.EqualTo("DotNet test"));
    }

    [AvaloniaTest]
    public async Task TestTypesColumn_DisplaysCorrectValues()
    {
        var services = CreateTestServiceProvider();
        var window = CreateTestWindow(services);
        var mainViewModel = (MainViewModel)window.DataContext!;

        var envService = services.GetRequiredService<IEnvironmentService>();
        mainViewModel.RepositoryPath = envService.Root;
        await Task.Delay(300);

        var issueListView = mainViewModel.CurrentView as IssueListView;
        Assert.That(issueListView, Is.Not.Null, "CurrentView should be IssueListView");
        var vm = (IssueListViewModel)issueListView!.DataContext!;

        vm.LoadIssues(new[]
        {
            new IssueListItem { Number = 1, TestTypes = "Scripts" },
            new IssueListItem { Number = 2, TestTypes = "DotNet test" }
        });

        window.Show();
        await Task.Delay(100);

        // Verify that the view model values that feed the column are correct
        Assert.That(vm.Issues.Any(i => i.TestTypes == "Scripts"), Is.True);
        Assert.That(vm.Issues.Any(i => i.TestTypes == "DotNet test"), Is.True);
    }

    [AvaloniaTest]
    public async Task ScopeFilterComboBox_ShowsNewValues()
    {
        var services = CreateTestServiceProvider();
        var window = CreateTestWindow(services);
        var mainViewModel = (MainViewModel)window.DataContext!;

        var envService = services.GetRequiredService<IEnvironmentService>();
        mainViewModel.RepositoryPath = envService.Root;
        await Task.Delay(300);

        var issueListView = mainViewModel.CurrentView as IssueListView;
        Assert.That(issueListView, Is.Not.Null, "CurrentView should be IssueListView");
        var vm = (IssueListViewModel)issueListView!.DataContext!;

        window.Show();
        await Task.Delay(100);

        // Verify AvailableScopes contains the new values
        var scopes = vm.AvailableScopes.ToList();
        Assert.That(scopes, Contains.Item(TestScope.All));
        Assert.That(scopes, Contains.Item(TestScope.Regression));
        Assert.That(scopes, Contains.Item(TestScope.Open));
        Assert.That(scopes.Count, Is.EqualTo(3), "Should only have All, Regression, and Open");
    }

    [AvaloniaTest]
    public async Task StateFilterComboBox_ShowsNewValues()
    {
        var services = CreateTestServiceProvider();
        var window = CreateTestWindow(services);
        var mainViewModel = (MainViewModel)window.DataContext!;

        var envService = services.GetRequiredService<IEnvironmentService>();
        mainViewModel.RepositoryPath = envService.Root;
        await Task.Delay(300);

        var issueListView = mainViewModel.CurrentView as IssueListView;
        Assert.That(issueListView, Is.Not.Null, "CurrentView should be IssueListView");
        var vm = (IssueListViewModel)issueListView!.DataContext!;

        window.Show();
        await Task.Delay(100);

        // Verify AvailableStates contains the new values
        var states = vm.AvailableStates.ToList();
        Assert.That(states, Contains.Item("All"));
        Assert.That(states, Contains.Item("New"));
        Assert.That(states, Contains.Item("Synced"));
        Assert.That(states, Contains.Item("Failed restore"));
        Assert.That(states, Contains.Item("Failed compile"));
        Assert.That(states, Contains.Item("Runnable"));
        Assert.That(states, Contains.Item("Skipped"));
    }

    [AvaloniaTest]
    public async Task NotTestedReason_DisplaysMarkerFileType_WhenSkipped()
    {
        var services = CreateTestServiceProvider();
        var window = CreateTestWindow(services);
        var mainViewModel = (MainViewModel)window.DataContext!;

        var envService = services.GetRequiredService<IEnvironmentService>();
        mainViewModel.RepositoryPath = envService.Root;
        await Task.Delay(300);

        var issueListView = mainViewModel.CurrentView as IssueListView;
        Assert.That(issueListView, Is.Not.Null, "CurrentView should be IssueListView");
        var vm = (IssueListViewModel)issueListView!.DataContext!;

        // Create test issues with different marker types
        vm.LoadIssues(new[]
        {
            new IssueListItem { Number = 1, StateValue = IssueState.Skipped, NotTestedReason = "Skipped (Ignored)" },
            new IssueListItem { Number = 2, StateValue = IssueState.Skipped, NotTestedReason = "Skipped (Explicit)" },
            new IssueListItem { Number = 3, StateValue = IssueState.Skipped, NotTestedReason = "Skipped (GUI)" },
            new IssueListItem { Number = 4, StateValue = IssueState.Skipped, NotTestedReason = "Skipped (WIP)" }
        });

        window.Show();
        await Task.Delay(100);

        // Verify marker types are displayed in NotTestedReason
        var issues = vm.Issues.ToList();
        Assert.That(issues.Count, Is.EqualTo(4));
        Assert.That(issues.First(i => i.Number == 1).NotTestedReason, Does.Contain("Ignored"));
        Assert.That(issues.First(i => i.Number == 2).NotTestedReason, Does.Contain("Explicit"));
        Assert.That(issues.First(i => i.Number == 3).NotTestedReason, Does.Contain("GUI"));
        Assert.That(issues.First(i => i.Number == 4).NotTestedReason, Does.Contain("WIP"));
    }

    [AvaloniaTest]
    public async Task SyncFromGitHubButton_AppearsInIssueListView()
    {
        var services = CreateTestServiceProvider();
        var window = CreateTestWindow(services);
        var mainViewModel = (MainViewModel)window.DataContext!;

        var envService = services.GetRequiredService<IEnvironmentService>();
        mainViewModel.RepositoryPath = envService.Root;
        await Task.Delay(300);

        var issueListView = mainViewModel.CurrentView as IssueListView;
        Assert.That(issueListView, Is.Not.Null, "CurrentView should be IssueListView");

        window.Show();
        await Task.Delay(100);

        var syncButton = issueListView!.FindControl<Button>("IssueListSyncFromGitHubButton");

        Assert.That(syncButton, Is.Not.Null, "Sync from GitHub button should exist");
        Assert.That(syncButton!.Command, Is.Not.Null, "Sync from GitHub button should have a command");
    }
}


