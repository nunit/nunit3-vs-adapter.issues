using Avalonia.Headless.NUnit;
using IssueRunner.Gui.ViewModels;
using IssueRunner.Gui.Views;
using IssueRunner.Models;
using IssueRunner.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace IssueRunner.Gui.Tests;

[TestFixture]
public class MainViewModelTests : HeadlessTestBase
{
    private IServiceProvider _services = null!;
    private IEnvironmentService _environmentService = null!;
    private string _currentRoot = null!;
    private IMarkerService _markerService = null!;
    
    [SetUp]
    public void SetUp()
    {
        // Create test directory structure
        var testRepoPath = Path.Combine(Path.GetTempPath(), "IssueRunnerTest");
        if (Directory.Exists(testRepoPath))
        {
            Directory.Delete(testRepoPath, true);
        }
        Directory.CreateDirectory(testRepoPath);
        Directory.CreateDirectory(Path.Combine(testRepoPath, "Tools"));
        
        // Create repository.json
        File.WriteAllText(Path.Combine(testRepoPath, "Tools", "repository.json"), 
            "{\"owner\":\"test\",\"name\":\"test\"}");
        
        // Create empty test result files so LoadDataAsync doesn't fail
        File.WriteAllText(Path.Combine(testRepoPath, "test-passes.json"), "{\"testResults\":[]}");
        File.WriteAllText(Path.Combine(testRepoPath, "test-fails.json"), "{\"testResults\":[]}");
        
        // Initialize current root as a field so closure works correctly
        _currentRoot = testRepoPath;
        _environmentService = Substitute.For<IEnvironmentService>();
        
        // Set up Root property to return the current root value
        _environmentService.Root.Returns(_ => _currentRoot);
        
        // Set up AddRoot to update the root field
        _environmentService.When(x => x.AddRoot(Arg.Any<string>())).Do(callInfo =>
        {
            _currentRoot = callInfo.Arg<string>();
        });
        
        // Set up RepositoryConfig
        _environmentService.RepositoryConfig.Returns(new IssueRunner.Models.RepositoryConfig("test", "test"));
        
        // Set up GetDataDirectory to return the correct path
        _environmentService.GetDataDirectory(Arg.Any<string>()).Returns(callInfo =>
        {
            var repoRoot = callInfo.Arg<string>();
            return Path.Combine(repoRoot, ".nunit", "IssueRunner");
        });

        _markerService = Substitute.For<IMarkerService>();

        var serviceCollection = new ServiceCollection();
        serviceCollection.AddLogging(builder => builder.AddConsole());
        serviceCollection.AddSingleton(_environmentService);
        // Create a mock that can be updated in tests
        var discoverIssueFolders = Substitute.For<IIssueDiscoveryService>();
        // Default: return empty dictionary (tests will override this)
        discoverIssueFolders.DiscoverIssueFolders().Returns(new Dictionary<int, string>());
        _markerService.ShouldSkipIssue(Arg.Any<string>()).Returns(false);
        serviceCollection.AddSingleton<IMarkerService>(_markerService);
        serviceCollection.AddSingleton(discoverIssueFolders);
        serviceCollection.AddSingleton<IProjectAnalyzerService>(Substitute.For<IProjectAnalyzerService>());
        serviceCollection.AddSingleton<IFrameworkUpgradeService>(Substitute.For<IFrameworkUpgradeService>());
        serviceCollection.AddSingleton<IProcessExecutor>(Substitute.For<IProcessExecutor>());
        serviceCollection.AddSingleton<IPackageUpdateService>(Substitute.For<IPackageUpdateService>());
        serviceCollection.AddSingleton<INuGetPackageVersionService>(Substitute.For<INuGetPackageVersionService>());
        serviceCollection.AddSingleton<ITestExecutionService>(Substitute.For<ITestExecutionService>());
        serviceCollection.AddSingleton<IGitHubApiService>(Substitute.For<IGitHubApiService>());
        serviceCollection.AddSingleton<ReportGeneratorService>(sp =>
        {
            var logger = sp.GetRequiredService<ILogger<ReportGeneratorService>>();
            var envService = sp.GetRequiredService<IEnvironmentService>();
            return new ReportGeneratorService(logger, envService);
        });
        serviceCollection.AddSingleton<IssueListViewModel>();
        serviceCollection.AddSingleton<MainViewModel>();
        var diffService = Substitute.For<ITestResultDiffService>();
        diffService.CompareResultsAsync(Arg.Any<string>()).Returns(Task.FromResult(new List<TestResultDiff>()));
        serviceCollection.AddSingleton(diffService);

        _services = serviceCollection.BuildServiceProvider();
    }

    [AvaloniaTest]
    public async Task ShowTestStatusCommand_SetsCurrentViewToTestStatusView()
    {
        // Arrange
        var window = CreateTestWindow(_services);
        var viewModel = (MainViewModel)window.DataContext!;
        var testRepoPath = _environmentService.Root;
        viewModel.RepositoryPath = testRepoPath;
        
        // Wait for repository initialization (LoadRepository is async via Task.Run)
        await Task.Delay(500);

        // Verify repository is loaded - check that AddRoot was called
        // After LoadRepository, _environmentService.Root should be set to the repository path
        Assert.That(_environmentService.Root, Is.EqualTo(testRepoPath), 
            "Environment service root should be set to test repository path");
        Assert.That(Directory.Exists(_environmentService.Root), Is.True, 
            "Test repository directory should exist");
        
        // Check log output to see if there were any errors
        var logBefore = viewModel.LogOutput;

        // Act - Execute command and wait for completion
        var completed = false;
        var exception = (Exception?)null;
        viewModel.ShowTestStatusCommand.Execute().Subscribe(
            _ => { },
            ex => { exception = ex; completed = true; },
            () => { completed = true; });
        
        // Wait for command to complete (with timeout)
        var timeout = DateTime.Now.AddSeconds(10);
        while (!completed && DateTime.Now < timeout)
        {
            await Task.Delay(100);
        }
        
        // Wait a bit more for UI thread operations
        // Note: Dispatcher.UIThread.InvokeAsync might not work in unit tests without Avalonia
        // So we might need to check the log to see if validation failed
        await Task.Delay(500);
        
        // Check log output for errors
        var logAfter = viewModel.LogOutput;
        var hasValidationError = logAfter.Contains("No repository loaded", StringComparison.OrdinalIgnoreCase);

        // Assert
        if (hasValidationError)
        {
            // Validation failed - this means AddRoot didn't work correctly or Root isn't set
            Assert.Fail($"Repository validation failed. Log: {logAfter}. " +
                       $"EnvironmentService.Root: {_environmentService.Root}, " +
                       $"Directory exists: {Directory.Exists(_environmentService.Root)}");
        }
        
        if (exception != null)
        {
            // Log the exception for debugging
            Console.WriteLine($"Exception occurred: {exception.GetType().Name}: {exception.Message}");
            if (exception.InnerException != null)
            {
                Console.WriteLine($"Inner exception: {exception.InnerException.Message}");
            }
            Console.WriteLine($"Stack trace: {exception.StackTrace}");
            Console.WriteLine($"Log output: {logAfter}");
        }
        
        // The view should be TestStatusView if validation passed and dispatcher executed
        // If dispatcher didn't execute (no Avalonia in unit test), the view might not change
        // But we can at least verify the command executed without validation errors
        Assert.That(viewModel.CurrentView, Is.InstanceOf<TestStatusView>(),
            $"CurrentView is {viewModel.CurrentView?.GetType().Name}, expected TestStatusView. " +
            $"Log: {logAfter}. Exception: {exception?.Message}");
        Assert.That(viewModel.CurrentViewType, Is.EqualTo("TestStatus"),
            $"CurrentViewType is {viewModel.CurrentViewType}, expected TestStatus");
    }

    [AvaloniaTest]
    public void ShowIssueListCommand_ReturnsCurrentViewToIssueListView()
    {
        // Arrange
        var window = CreateTestWindow(_services);
        var viewModel = (MainViewModel)window.DataContext!;
        viewModel.RepositoryPath = _environmentService.Root;
        
        // Set initial view type to TestStatus to test switching
        var initialViewType = viewModel.CurrentViewType;
        Assert.That(initialViewType, Is.EqualTo("IssueList")); // Should start as IssueList

        // Act - Execute command
        viewModel.ShowIssueListCommand.Execute().Subscribe();
        
        // Allow time for UI thread operations
        Thread.Sleep(100);

        // Assert
        Assert.That(viewModel.CurrentView, Is.InstanceOf<IssueListView>());
        Assert.That(viewModel.CurrentViewType, Is.EqualTo("IssueList"));
    }

    [AvaloniaTest]
    public async Task ViewSwitchingState_IsMaintainedCorrectly()
    {
        // Arrange
        var window = CreateTestWindow(_services);
        var viewModel = (MainViewModel)window.DataContext!;
        viewModel.RepositoryPath = _environmentService.Root;
        
        // Wait for repository initialization
        await Task.Delay(300);

        // Act & Assert - Initial state
        Assert.That(viewModel.CurrentViewType, Is.EqualTo("IssueList"));
        Assert.That(viewModel.IssueListButtonBackground, Is.EqualTo("#0078D4"));
        Assert.That(viewModel.TestStatusButtonBackground, Is.EqualTo("#666"));

        // Switch to Test Status - wait for async command to complete
        var testStatusCompleted = false;
        viewModel.ShowTestStatusCommand.Execute().Subscribe(
            _ => { },
            ex => { testStatusCompleted = true; },
            () => { testStatusCompleted = true; });
        
        // Wait for command to complete
        var timeout = DateTime.Now.AddSeconds(5);
        while (!testStatusCompleted && DateTime.Now < timeout)
        {
            await Task.Delay(50);
        }
        await Task.Delay(100); // Allow time for view update
        
        Assert.That(viewModel.CurrentViewType, Is.EqualTo("TestStatus"));
        Assert.That(viewModel.IssueListButtonBackground, Is.EqualTo("#666"));
        Assert.That(viewModel.TestStatusButtonBackground, Is.EqualTo("#0078D4"));

        // Switch back to Issue List
        viewModel.ShowIssueListCommand.Execute().Subscribe();
        await Task.Delay(100); // Allow time for view update
        
        Assert.That(viewModel.CurrentViewType, Is.EqualTo("IssueList"));
        Assert.That(viewModel.IssueListButtonBackground, Is.EqualTo("#0078D4"));
        Assert.That(viewModel.TestStatusButtonBackground, Is.EqualTo("#666"));
    }

    [AvaloniaTest]
    public async Task LoadIssuesIntoViewAsync_DeterminesStateCorrectly_ForSkippedIssue()
    {
        // Arrange
        var testRepoPath = _environmentService.Root;

        // Create issue folder with marker file
        var issueFolder = Path.Combine(testRepoPath, "Issue1");
        Directory.CreateDirectory(issueFolder);
        await File.WriteAllTextAsync(Path.Combine(issueFolder, "ignore"), ""); // Create ignore marker

        // Create data directory and metadata in the same location used by MainViewModel
        var dataDir = Path.Combine(testRepoPath, ".nunit", "IssueRunner");
        Directory.CreateDirectory(dataDir);

        var metadataPath = Path.Combine(dataDir, "issues_metadata.json");
        var metadata = new List<IssueMetadata>
        {
            new IssueMetadata { Number = 1, State = "open", Title = "Test Issue", Labels = new List<string>(), Url = "https://github.com/test/test/issues/1" }
        };
        var metadataJson = JsonSerializer.Serialize(metadata);
        await File.WriteAllTextAsync(metadataPath, metadataJson);
        
        // Mock issue discovery to return the folder we created - MUST be before MainViewModel is created
        // and before RepositoryPath is set (which triggers LoadRepository)
        var issueDiscovery = _services.GetRequiredService<IIssueDiscoveryService>();
        issueDiscovery.DiscoverIssueFolders().Returns(new Dictionary<int, string> { { 1, issueFolder } });
        _markerService.ShouldSkipIssue(Arg.Any<string>()).Returns(callInfo => 
        {
            var path = callInfo.Arg<string>();
            return path == issueFolder;
        });
        _markerService.GetMarkerReason(Arg.Any<string>()).Returns("Ignored");
        
        var window = CreateTestWindow(_services);
        var viewModel = (MainViewModel)window.DataContext!;
        // RepositoryPath setter triggers LoadRepository which calls DiscoverIssueFolders()
        // So mock must be set up before this line
        viewModel.RepositoryPath = testRepoPath;
        await Task.Delay(800); // Wait for repository to load and LoadIssuesIntoViewAsync to complete
        
        // Act - Get the IssueListViewModel and check issues
        var issueListView = viewModel.CurrentView as IssueListView;
        Assert.That(issueListView, Is.Not.Null);
        var issueListViewModel = (IssueListViewModel)issueListView!.DataContext!;
        
        // Wait for LoadIssuesIntoViewAsync to complete (it runs asynchronously via Task.Run)
        var timeout = DateTime.Now.AddSeconds(5);
        while (issueListViewModel.AllIssues.Count == 0 && DateTime.Now < timeout)
        {
            await Task.Delay(100);
        }
        await Task.Delay(200); // Additional delay to ensure UI updates complete
        
        // Assert
        var issues = issueListViewModel.AllIssues.ToList();
        // Debug: Check if mock was called and what folders were passed to LoadIssuesIntoViewAsync
        if (issues.Count == 0)
        {
            // Verify the mock was actually called
            issueDiscovery.Received().DiscoverIssueFolders();
            var receivedCalls = issueDiscovery.ReceivedCalls().ToList();
            // Check if metadata file exists
            var metadataFilePath = Path.Combine(testRepoPath, "Tools", "issues_metadata.json");
            var metadataExists = File.Exists(metadataFilePath);
            Assert.Fail($"No issues loaded. Mock was called {receivedCalls.Count} times. " +
                       $"CurrentView is {viewModel.CurrentView?.GetType().Name}, " +
                       $"RepositoryPath is {viewModel.RepositoryPath}, " +
                       $"Metadata file exists: {metadataExists} at {metadataFilePath}, " +
                       $"LogOutput contains: {viewModel.LogOutput}");
        }
        Assert.That(issues.Count, Is.GreaterThan(0), $"Expected issues to be loaded. Log: {viewModel.LogOutput}");
        var issue1 = issues.FirstOrDefault(i => i.Number == 1);
        Assert.That(issue1, Is.Not.Null);
        Assert.That(issue1!.StateValue, Is.EqualTo(IssueState.Skipped));
        Assert.That(issue1.NotTestedReason, Does.Contain("Ignored"));
    }

    [AvaloniaTest]
    public async Task LoadIssuesIntoViewAsync_DeterminesStateCorrectly_ForFailedCompileIssue()
    {
        // Arrange
        var testRepoPath = _environmentService.Root;
        
        // Create issue folder
        var issueFolder = Path.Combine(testRepoPath, "Issue1");
        Directory.CreateDirectory(issueFolder);
        
        // Create data directory and results.json with build failure
        var dataDir = Path.Combine(testRepoPath, ".nunit", "IssueRunner");
        Directory.CreateDirectory(dataDir);
        var resultsPath = Path.Combine(dataDir, "results.json");
        var issueResult = new IssueResult
        {
            Number = 1,
            ProjectPath = "Issue1.csproj",
            TargetFrameworks = new List<string> { "net10.0" },
            Packages = new List<string>(),
            BuildResult = "fail",
            RestoreResult = "success",
            TestResult = null,
            LastRun = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ")
        };
        var resultsJson = JsonSerializer.Serialize(new List<IssueResult> { issueResult });
        await File.WriteAllTextAsync(resultsPath, resultsJson);
        
        // Verify file was created
        Assert.That(File.Exists(resultsPath), Is.True, $"results.json should exist at {resultsPath}");
        
        // Create metadata
        var metadataPath = Path.Combine(dataDir, "issues_metadata.json");
        var metadata = new List<IssueMetadata>
        {
            new IssueMetadata { Number = 1, State = "open", Title = "Test Issue", Labels = new List<string>(), Url = "https://github.com/test/test/issues/1" }
        };
        var metadataJson = JsonSerializer.Serialize(metadata);
        await File.WriteAllTextAsync(metadataPath, metadataJson);
        
        // Mock issue discovery to return the folder we created - MUST be before MainViewModel is created
        // and before RepositoryPath is set (which triggers LoadRepository)
        var issueDiscovery = _services.GetRequiredService<IIssueDiscoveryService>();
        issueDiscovery.DiscoverIssueFolders().Returns(new Dictionary<int, string> { { 1, issueFolder } });
        _markerService.ShouldSkipIssue(Arg.Any<string>()).Returns(false);
        
        var window = CreateTestWindow(_services);
        var viewModel = (MainViewModel)window.DataContext!;
        // RepositoryPath setter triggers LoadRepository which calls DiscoverIssueFolders()
        // So mock must be set up before this line
        viewModel.RepositoryPath = testRepoPath;
        
        // Wait for repository initialization
        await Task.Delay(500);
        
        // Act
        var issueListView = viewModel.CurrentView as IssueListView;
        Assert.That(issueListView, Is.Not.Null);
        var issueListViewModel = (IssueListViewModel)issueListView!.DataContext!;
        
        // Wait for LoadIssuesIntoViewAsync to complete (it runs asynchronously via Task.Run)
        var timeout = DateTime.Now.AddSeconds(5);
        while (issueListViewModel.AllIssues.Count == 0 && DateTime.Now < timeout)
        {
            await Task.Delay(100);
        }
        await Task.Delay(200); // Additional delay to ensure UI updates complete
        
        // Assert
        var issues = issueListViewModel.AllIssues.ToList();
        Assert.That(issues.Count, Is.GreaterThan(0), $"Expected at least 1 issue but found {issues.Count}. Log: {viewModel.LogOutput}");
        var issue1 = issues.FirstOrDefault(i => i.Number == 1);
        Assert.That(issue1, Is.Not.Null);
        Assert.That(issue1!.StateValue, Is.EqualTo(IssueState.FailedCompile), 
            $"Expected FailedCompile but got {issue1.StateValue}. DetailedState: {issue1.DetailedState}, NotTestedReason: {issue1.NotTestedReason}, Log: {viewModel.LogOutput}");
    }

    [AvaloniaTest]
    public async Task LoadIssuesIntoViewAsync_DeterminesStateCorrectly_ForRunnableIssue()
    {
        // Arrange
        var testRepoPath = _environmentService.Root;

        // Create issue folder
        var issueFolder = Path.Combine(testRepoPath, "Issue1");
        Directory.CreateDirectory(issueFolder);

        // Create data directory and metadata in the same location used by MainViewModel
        var dataDir = Path.Combine(testRepoPath, ".nunit", "IssueRunner");
        Directory.CreateDirectory(dataDir);

        var metadataPath = Path.Combine(dataDir, "issues_metadata.json");
        var metadata = new List<IssueMetadata>
        {
            new() { Number = 1, State = "open", Title = "Test Issue", Labels = [], Url = "https://github.com/test/test/issues/1" }
        };
        var metadataJson = JsonSerializer.Serialize(metadata);
        await File.WriteAllTextAsync(metadataPath, metadataJson);

        // Create results.json entry with a successful test run for issue 1
        var resultsPath = Path.Combine(dataDir, "results.json");
        var issueResult = new IssueResult
        {
            Number = 1,
            ProjectPath = "Issue1\\test.csproj",
            TargetFrameworks = new List<string> { "net8.0" },
            Packages = new List<string>(),
            RestoreResult = "success",
            BuildResult = "success",
            TestResult = "success",
            LastRun = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ")
        };
        var resultsJson = JsonSerializer.Serialize(new List<IssueResult> { issueResult });
        await File.WriteAllTextAsync(resultsPath, resultsJson);
        
        // Mock issue discovery to return the folder we created - MUST be before MainViewModel is created
        // and before RepositoryPath is set (which triggers LoadRepository)
        var issueDiscovery = _services.GetRequiredService<IIssueDiscoveryService>();
        issueDiscovery.DiscoverIssueFolders().Returns(new Dictionary<int, string> { { 1, issueFolder } });
        _markerService.ShouldSkipIssue(Arg.Any<string>()).Returns(false);
        
        var window = CreateTestWindow(_services);
        var viewModel = (MainViewModel)window.DataContext!;
        // RepositoryPath setter triggers LoadRepository which calls DiscoverIssueFolders()
        // So mock must be set up before this line
        viewModel.RepositoryPath = testRepoPath;
        await Task.Delay(800); // Wait for repository to load and LoadIssuesIntoViewAsync to complete
        
        // Act
        var issueListView = viewModel.CurrentView as IssueListView;
        Assert.That(issueListView, Is.Not.Null);
        var issueListViewModel = (IssueListViewModel)issueListView!.DataContext!;
        await Task.Delay(500); // Wait for issues to load
        
        // Assert
        var issues = issueListViewModel.AllIssues.ToList();
        Assert.That(issues.Count, Is.GreaterThan(0));
        var issue1 = issues.FirstOrDefault(i => i.Number == 1);
        Assert.That(issue1, Is.Not.Null);
        Assert.That(issue1!.StateValue, Is.EqualTo(IssueState.Runnable));
    }

    [AvaloniaTest]
    public async Task LoadIssuesIntoViewAsync_LoadsMetadata_FromIssuesMetadataJson()
    {
        // Arrange
        var testRepoPath = _environmentService.Root;
        var dataDir = Path.Combine(testRepoPath, ".nunit", "IssueRunner");
        Directory.CreateDirectory(dataDir);
        
        var metadataPath = Path.Combine(dataDir, "issues_metadata.json");
        var metadata = new List<IssueMetadata>
        {
            new IssueMetadata { Number = 1, State = "open", Title = "Test Issue 1", Labels = new List<string>(), Url = "https://github.com/test/test/issues/1" },
            new IssueMetadata { Number = 228, State = "closed", Title = "Test Issue 228", Labels = new List<string>(), Url = "https://github.com/test/test/issues/228" }
        };
        var metadataJson = JsonSerializer.Serialize(metadata);
        await File.WriteAllTextAsync(metadataPath, metadataJson);
        
        var issueFolder1 = Path.Combine(testRepoPath, "Issue1");
        var issueFolder228 = Path.Combine(testRepoPath, "Issue228");
        Directory.CreateDirectory(issueFolder1);
        Directory.CreateDirectory(issueFolder228);
        
        var issueDiscovery = _services.GetRequiredService<IIssueDiscoveryService>();
        issueDiscovery.DiscoverIssueFolders().Returns(new Dictionary<int, string> 
        { 
            { 1, issueFolder1 },
            { 228, issueFolder228 }
        });
        _markerService.ShouldSkipIssue(Arg.Any<string>()).Returns(false);
        
        var window = CreateTestWindow(_services);
        var viewModel = (MainViewModel)window.DataContext!;
        viewModel.RepositoryPath = testRepoPath;
        await Task.Delay(800);
        
        // Act
        var issueListView = viewModel.CurrentView as IssueListView;
        var issueListViewModel = (IssueListViewModel)issueListView!.DataContext!;
        await Task.Delay(500);
        
        // Assert
        var issues = issueListViewModel.AllIssues.ToList();
        Assert.That(issues.Count, Is.EqualTo(2));
        Assert.That(issues.First(i => i.Number == 1).Title, Is.EqualTo("Test Issue 1"));
        Assert.That(issues.First(i => i.Number == 228).Title, Is.EqualTo("Test Issue 228"));
    }

    [AvaloniaTest]
    public async Task LoadIssuesIntoViewAsync_HandlesDuplicateIssueNumbers_TakesLastOccurrence()
    {
        // Arrange
        var testRepoPath = _environmentService.Root;
        var dataDir = Path.Combine(testRepoPath, ".nunit", "IssueRunner");
        Directory.CreateDirectory(dataDir);
        
        var metadataPath = Path.Combine(dataDir, "issues_metadata.json");
        var metadata = new List<IssueMetadata>
        {
            new IssueMetadata { Number = 1, State = "open", Title = "First Title", Labels = new List<string>(), Url = "https://github.com/test/test/issues/1" },
            new IssueMetadata { Number = 1, State = "closed", Title = "Last Title", Labels = new List<string>(), Url = "https://github.com/test/test/issues/1" }
        };
        var metadataJson = JsonSerializer.Serialize(metadata);
        await File.WriteAllTextAsync(metadataPath, metadataJson);
        
        var issueFolder1 = Path.Combine(testRepoPath, "Issue1");
        Directory.CreateDirectory(issueFolder1);
        
        var issueDiscovery = _services.GetRequiredService<IIssueDiscoveryService>();
        issueDiscovery.DiscoverIssueFolders().Returns(new Dictionary<int, string> { { 1, issueFolder1 } });
        _markerService.ShouldSkipIssue(Arg.Any<string>()).Returns(false);
        
        var window = CreateTestWindow(_services);
        var viewModel = (MainViewModel)window.DataContext!;
        viewModel.RepositoryPath = testRepoPath;
        await Task.Delay(800);
        
        // Act
        var issueListView = viewModel.CurrentView as IssueListView;
        var issueListViewModel = (IssueListViewModel)issueListView!.DataContext!;
        await Task.Delay(500);
        
        // Assert
        var issues = issueListViewModel.AllIssues.ToList();
        Assert.That(issues.Count, Is.EqualTo(1));
        Assert.That(issues.First().Title, Is.EqualTo("Last Title"));
        Assert.That(issues.First().State, Is.EqualTo("closed"));
    }

    [AvaloniaTest]
    public async Task LoadIssuesIntoViewAsync_ReturnsEmptyDictionary_WhenMetadataFileNotFound()
    {
        // Arrange
        var testRepoPath = _environmentService.Root;
        var dataDir = Path.Combine(testRepoPath, ".nunit", "IssueRunner");
        Directory.CreateDirectory(dataDir);
        
        // Don't create metadata file
        
        var issueFolder1 = Path.Combine(testRepoPath, "Issue1");
        Directory.CreateDirectory(issueFolder1);
        
        var issueDiscovery = _services.GetRequiredService<IIssueDiscoveryService>();
        issueDiscovery.DiscoverIssueFolders().Returns(new Dictionary<int, string> { { 1, issueFolder1 } });
        _markerService.ShouldSkipIssue(Arg.Any<string>()).Returns(false);
        
        var window = CreateTestWindow(_services);
        var viewModel = (MainViewModel)window.DataContext!;
        viewModel.RepositoryPath = testRepoPath;
        await Task.Delay(800);
        
        // Act
        var issueListView = viewModel.CurrentView as IssueListView;
        var issueListViewModel = (IssueListViewModel)issueListView!.DataContext!;
        await Task.Delay(500);
        
        // Assert
        var issues = issueListViewModel.AllIssues.ToList();
        Assert.That(issues.Count, Is.EqualTo(1));
        Assert.That(issues.First().Title, Is.EqualTo("Issue 1")); // Fallback title
    }

    [AvaloniaTest]
    public async Task LoadIssuesIntoViewAsync_SetsTitleFromMetadata_WhenMetadataExists()
    {
        // Arrange
        var testRepoPath = _environmentService.Root;
        var dataDir = Path.Combine(testRepoPath, ".nunit", "IssueRunner");
        Directory.CreateDirectory(dataDir);
        
        var metadataPath = Path.Combine(dataDir, "issues_metadata.json");
        var metadata = new List<IssueMetadata>
        {
            new() { Number = 228, State = "closed", Title = "Tests inherited from Generic test fixture", Labels = [], Url = "https://github.com/test/test/issues/228" }
        };
        var metadataJson = JsonSerializer.Serialize(metadata);
        await File.WriteAllTextAsync(metadataPath, metadataJson);
        
        var issueFolder228 = Path.Combine(testRepoPath, "Issue228");
        Directory.CreateDirectory(issueFolder228);
        
        var issueDiscovery = _services.GetRequiredService<IIssueDiscoveryService>();
        issueDiscovery.DiscoverIssueFolders().Returns(new Dictionary<int, string> { { 228, issueFolder228 } });
        _markerService.ShouldSkipIssue(Arg.Any<string>()).Returns(false);
        
        var window = CreateTestWindow(_services);
        var viewModel = (MainViewModel)window.DataContext!;
        viewModel.RepositoryPath = testRepoPath;
        
        // Wait for repository initialization
        await Task.Delay(500);
        
        // Act
        var issueListView = viewModel.CurrentView as IssueListView;
        Assert.That(issueListView, Is.Not.Null);
        var issueListViewModel = (IssueListViewModel)issueListView!.DataContext!;
        
        // Wait for LoadIssuesIntoViewAsync to complete (it runs asynchronously via Task.Run)
        var timeout = DateTime.Now.AddSeconds(5);
        while (issueListViewModel.AllIssues.Count == 0 && DateTime.Now < timeout)
        {
            await Task.Delay(100);
        }
        await Task.Delay(200); // Additional delay to ensure UI updates complete
        
        // Assert
        var issues = issueListViewModel.AllIssues.ToList();
        Assert.That(issues.Count, Is.EqualTo(1), $"Expected 1 issue but found {issues.Count}. Log: {viewModel.LogOutput}");
        var issue = issues.First();
        Assert.That(issue.Number, Is.EqualTo(228));
        
        // Debug: Check if metadata was loaded
        var debugMetadataPath = Path.Combine(dataDir, "issues_metadata.json");
        var metadataExists = File.Exists(debugMetadataPath);
        var metadataContent = metadataExists ? await File.ReadAllTextAsync(debugMetadataPath) : "File does not exist";
        
        Assert.That(issue.Title, Is.EqualTo("Tests inherited from Generic test fixture"), 
            $"Title mismatch. Metadata file exists: {metadataExists}, Path: {debugMetadataPath}, " +
            $"Content preview: {metadataContent[..Math.Min(200, metadataContent.Length)]}, " +
            $"Log: {viewModel.LogOutput}");
    }

    [AvaloniaTest]
    public async Task LoadIssuesIntoViewAsync_SetsFallbackTitle_WhenMetadataIsNull()
    {
        // Arrange
        var window = CreateTestWindow(_services);
        var testRepoPath = _environmentService.Root;
        var dataDir = Path.Combine(testRepoPath, ".nunit", "IssueRunner");
        Directory.CreateDirectory(dataDir);
        
        // Create metadata file but without issue 999
        var metadataPath = Path.Combine(dataDir, "issues_metadata.json");
        var metadata = new List<IssueMetadata>
        {
            new IssueMetadata { Number = 1, State = "open", Title = "Test Issue", Labels = new List<string>(), Url = "https://github.com/test/test/issues/1" }
        };
        var metadataJson = JsonSerializer.Serialize(metadata);
        await File.WriteAllTextAsync(metadataPath, metadataJson);
        
        var issueFolder999 = Path.Combine(testRepoPath, "Issue999");
        Directory.CreateDirectory(issueFolder999);
        
        var issueDiscovery = _services.GetRequiredService<IIssueDiscoveryService>();
        issueDiscovery.DiscoverIssueFolders().Returns(new Dictionary<int, string> { { 999, issueFolder999 } });
        _markerService.ShouldSkipIssue(Arg.Any<string>()).Returns(false);
        
        var viewModel = (MainViewModel)window.DataContext!;
        viewModel.RepositoryPath = testRepoPath;
        await Task.Delay(800); // Wait for repository to load and LoadIssuesIntoViewAsync to start
        
        // Act
        var issueListView = viewModel.CurrentView as IssueListView;
        var issueListViewModel = (IssueListViewModel)issueListView!.DataContext!;
        
        // Wait for LoadIssuesIntoViewAsync to complete (it runs asynchronously via Task.Run)
        var timeout = DateTime.Now.AddSeconds(5);
        while (issueListViewModel.AllIssues.Count == 0 && DateTime.Now < timeout)
        {
            await Task.Delay(100);
        }
        await Task.Delay(200); // Additional delay to ensure UI updates complete
        
        // Assert
        var issues = issueListViewModel.AllIssues.ToList();
        Assert.That(issues.Count, Is.EqualTo(1), $"Expected 1 issue but found {issues.Count}. Log: {viewModel.LogOutput}");
        Assert.That(issues.First().Number, Is.EqualTo(999));
        Assert.That(issues.First().Title, Is.EqualTo("Issue 999")); // Fallback title
    }

    [AvaloniaTest]
    public async Task LoadIssuesIntoViewAsync_CreatesIssueListItem_WithTitlePropertySet()
    {
        // Arrange
        var testRepoPath = _environmentService.Root;
        var dataDir = Path.Combine(testRepoPath, ".nunit", "IssueRunner");
        Directory.CreateDirectory(dataDir);
        
        var metadataPath = Path.Combine(dataDir, "issues_metadata.json");
        var metadata = new List<IssueMetadata>
        {
            new() { Number = 1, State = "open", Title = "Test Title", Labels = [], Url = "https://github.com/test/test/issues/1" }
        };
        var metadataJson = JsonSerializer.Serialize(metadata);
        await File.WriteAllTextAsync(metadataPath, metadataJson);
        
        var issueFolder1 = Path.Combine(testRepoPath, "Issue1");
        Directory.CreateDirectory(issueFolder1);
        
        var issueDiscovery = _services.GetRequiredService<IIssueDiscoveryService>();
        issueDiscovery.DiscoverIssueFolders().Returns(new Dictionary<int, string> { { 1, issueFolder1 } });
        _markerService.ShouldSkipIssue(Arg.Any<string>()).Returns(false);
        
        var window = CreateTestWindow(_services);
        var viewModel = (MainViewModel)window.DataContext!;
        viewModel.RepositoryPath = testRepoPath;
        await Task.Delay(800);
        
        // Act
        var issueListView = viewModel.CurrentView as IssueListView;
        var issueListViewModel = (IssueListViewModel)issueListView!.DataContext!;
        await Task.Delay(500);
        
        // Assert
        var issues = issueListViewModel.AllIssues.ToList();
        Assert.That(issues.Count, Is.EqualTo(1));
        var issue = issues.First();
        Assert.That(issue.Title, Is.Not.Null);
        Assert.That(issue.Title, Is.Not.Empty);
        Assert.That(issue.Title, Is.EqualTo("Test Title"));
    }
}

