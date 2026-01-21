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
public class TitleDisplayIntegrationTests : HeadlessTestBase
{
    private IEnvironmentService _environmentService = null!;
    private IIssueDiscoveryService _issueDiscoveryService = null!;
    private string _testRepoPath = null!;
    private string _dataDir = null!;

    private IServiceProvider CreateTestServiceProviderWithRealDiscovery()
    {
        // Create test directory structure
        _testRepoPath = Path.Combine(Path.GetTempPath(), "IssueRunnerIntegrationTest", Guid.NewGuid().ToString("N"));
        if (Directory.Exists(_testRepoPath))
        {
            Directory.Delete(_testRepoPath, true);
        }
        Directory.CreateDirectory(_testRepoPath);
        _dataDir = Path.Combine(_testRepoPath, ".nunit", "IssueRunner");
        Directory.CreateDirectory(_dataDir);
        
        // Create repository.json
        var repoConfig = new RepositoryConfig("test", "test");
        var repoConfigJson = JsonSerializer.Serialize(repoConfig);
        File.WriteAllText(Path.Combine(_dataDir, "repository.json"), repoConfigJson);
        
        // Create empty test result files
        File.WriteAllText(Path.Combine(_testRepoPath, "test-passes.json"), "{\"testResults\":[]}");
        File.WriteAllText(Path.Combine(_testRepoPath, "test-fails.json"), "{\"testResults\":[]}");
        
        // Initialize environment service
        var currentRoot = _testRepoPath;
        _environmentService = Substitute.For<IEnvironmentService>();
        _environmentService.Root.Returns(_ => currentRoot);
        _environmentService.When(x => x.AddRoot(Arg.Any<string>())).Do(callInfo =>
        {
            currentRoot = callInfo.Arg<string>();
        });
        _environmentService.RepositoryConfig.Returns(repoConfig);
        _environmentService.GetDataDirectory(Arg.Any<string>()).Returns(callInfo =>
        {
            var repoRoot = callInfo.Arg<string>();
            return Path.Combine(repoRoot, ".nunit", "IssueRunner");
        });
        
        // Create real issue discovery service
        var logger = Substitute.For<ILogger<IssueDiscoveryService>>();
        var markerService = Substitute.For<IMarkerService>();
        _issueDiscoveryService = new IssueDiscoveryService(_environmentService, logger,markerService);
        
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddLogging(builder => builder.AddConsole());
        serviceCollection.AddSingleton(_environmentService);
        serviceCollection.AddSingleton(_issueDiscoveryService);
        serviceCollection.AddSingleton(Substitute.For<IProjectAnalyzerService>());
        serviceCollection.AddSingleton(Substitute.For<IFrameworkUpgradeService>());
        serviceCollection.AddSingleton(Substitute.For<IProcessExecutor>());
        serviceCollection.AddSingleton(Substitute.For<IPackageUpdateService>());
        serviceCollection.AddSingleton(Substitute.For<INuGetPackageVersionService>());
        serviceCollection.AddSingleton(Substitute.For<ITestExecutionService>());
        serviceCollection.AddSingleton(Substitute.For<IGitHubApiService>());
        serviceCollection.AddSingleton(Substitute.For<IMarkerService>());
        serviceCollection.AddSingleton<ReportGeneratorService>(sp =>
        {
            var log = sp.GetRequiredService<ILogger<ReportGeneratorService>>();
            var envService = sp.GetRequiredService<IEnvironmentService>();
            return new ReportGeneratorService(log, envService);
        });
        serviceCollection.AddSingleton<IssueListViewModel>();
        serviceCollection.AddSingleton<MainViewModel>();
        var diffService = Substitute.For<ITestResultDiffService>();
        diffService.CompareResultsAsync(Arg.Any<string>()).Returns(Task.FromResult(new List<TestResultDiff>()));
        serviceCollection.AddSingleton(diffService);

        return serviceCollection.BuildServiceProvider();
    }

    [TearDown]
    public void TearDown()
    {
        if (Directory.Exists(_testRepoPath))
        {
            Directory.Delete(_testRepoPath, true);
        }
    }

    [AvaloniaTest]
    public async Task LoadIssuesIntoViewAsync_ExtractsTitleFromMetadata_WhenFolderAndMetadataMatch()
    {
        // Arrange
        var services = CreateTestServiceProviderWithRealDiscovery();
        var issue228Path = Path.Combine(_testRepoPath, "Issue228");
        Directory.CreateDirectory(issue228Path);
        
        var metadata = new List<IssueMetadata>
        {
            new()
            {
                Number = 228,
                Title = "Tests inherited from Generic test fixture",
                State = "closed",
                Milestone = "No milestone",
                Labels = ["is:bug", "pri:normal"],
                Url = "https://github.com/test/test/issues/228"
            }
        };
        
        var metadataJson = JsonSerializer.Serialize(metadata);
        var metadataPath = Path.Combine(_dataDir, "issues_metadata.json");
        await File.WriteAllTextAsync(metadataPath, metadataJson);
        
        // Create empty results.json
        await File.WriteAllTextAsync(Path.Combine(_dataDir, "results.json"), "[]");
        
        var window = CreateTestWindow(services);
        var mainViewModel = (MainViewModel)window.DataContext!;
        mainViewModel.RepositoryPath = _testRepoPath;
        
        // Wait for repository initialization
        await Task.Delay(500);
        
        var issueListView = mainViewModel.CurrentView as IssueListView;
        Assert.That(issueListView, Is.Not.Null);
        var issueListViewModel = (IssueListViewModel)issueListView!.DataContext!;
        
        // Act - wait for async operations
        await Task.Delay(1000); // Wait for LoadIssuesIntoViewAsync to complete
        
        // Assert
        Assert.That(issueListViewModel.AllIssues, Is.Not.Empty, "Issues should be loaded");
        var issue228 = issueListViewModel.AllIssues.FirstOrDefault(i => i.Number == 228);
        Assert.That(issue228, Is.Not.Null, "Issue 228 should be found");
        Assert.That(issue228!.Title, Is.EqualTo("Tests inherited from Generic test fixture"), 
            "Title should be extracted from metadata");
    }

    [AvaloniaTest]
    public async Task LoadIssuesIntoViewAsync_UsesFallbackTitle_WhenMetadataNotFound()
    {
        // Arrange
        var services = CreateTestServiceProviderWithRealDiscovery();
        // Create folder BEFORE setting repository path to ensure it exists when discovery runs
        var issue999Path = Path.Combine(_testRepoPath, "Issue999");
        Directory.CreateDirectory(issue999Path);
        
        // Verify folder exists
        Assert.That(Directory.Exists(issue999Path), Is.True, "Issue999 folder should exist before repository is loaded");
        
        // Create metadata file without issue 999
        var metadata = new List<IssueMetadata>
        {
            new()
            {
                Number = 228,
                Title = "Some other issue",
                State = "closed",
                Milestone = "No milestone",
                Labels = [],
                Url = "https://github.com/test/test/issues/228"
            }
        };
        
        var metadataJson = JsonSerializer.Serialize(metadata);
        var metadataPath = Path.Combine(_dataDir, "issues_metadata.json");
        await File.WriteAllTextAsync(metadataPath, metadataJson);
        
        // Create empty results.json
        await File.WriteAllTextAsync(Path.Combine(_dataDir, "results.json"), "[]");
        
        var window = CreateTestWindow(services);
        var mainViewModel = (MainViewModel)window.DataContext!;
        mainViewModel.RepositoryPath = _testRepoPath;
        
        // Wait for repository initialization
        await Task.Delay(500);
        
        var issueListView = mainViewModel.CurrentView as IssueListView;
        Assert.That(issueListView, Is.Not.Null);
        var issueListViewModel = (IssueListViewModel)issueListView!.DataContext!;
        
        // Act - wait for async operations to complete
        // LoadIssuesIntoViewAsync is called via Task.Run, so we need to wait for it
        await Task.Delay(1000); // Wait for LoadIssuesIntoViewAsync to complete
        
        // Assert
        var issue999 = issueListViewModel.AllIssues.FirstOrDefault(i => i.Number == 999);
        Assert.That(issue999, Is.Not.Null, $"Issue 999 should be found. Found {issueListViewModel.AllIssues.Count} issues: {string.Join(", ", issueListViewModel.AllIssues.Select(i => i.Number))}");
        Assert.That(issue999!.Title, Is.EqualTo("Issue 999"), 
            "Fallback title should be used when metadata is not found");
    }

    [AvaloniaTest]
    public async Task LoadIssuesIntoViewAsync_ResolvesMetadataPath_Correctly()
    {
        // Arrange
        var services = CreateTestServiceProviderWithRealDiscovery();
        var issue1Path = Path.Combine(_testRepoPath, "Issue1");
        Directory.CreateDirectory(issue1Path);
        
        var metadata = new List<IssueMetadata>
        {
            new()
            {
                Number = 1,
                Title = "Test Issue 1",
                State = "open",
                Milestone = null,
                Labels = [],
                Url = "https://github.com/test/test/issues/1"
            }
        };
        
        var metadataJson = JsonSerializer.Serialize(metadata);
        var expectedMetadataPath = Path.Combine(_dataDir, "issues_metadata.json");
        await File.WriteAllTextAsync(expectedMetadataPath, metadataJson);
        
        // Verify the path is correct
        Assert.That(File.Exists(expectedMetadataPath), Is.True, "Metadata file should exist at expected path");
        
        // Create empty results.json
        await File.WriteAllTextAsync(Path.Combine(_dataDir, "results.json"), "[]");
        
        var window = CreateTestWindow(services);
        var mainViewModel = (MainViewModel)window.DataContext!;
        mainViewModel.RepositoryPath = _testRepoPath;
        
        // Wait for repository initialization
        await Task.Delay(500);
        
        var issueListView = mainViewModel.CurrentView as IssueListView;
        Assert.That(issueListView, Is.Not.Null);
        var issueListViewModel = (IssueListViewModel)issueListView!.DataContext!;
        
        // Act
        await Task.Delay(1000);
        
        // Assert - if title is loaded, path resolution worked
        var issue1 = issueListViewModel.AllIssues.FirstOrDefault(i => i.Number == 1);
        Assert.That(issue1, Is.Not.Null, "Issue 1 should be found");
        Assert.That(issue1!.Title, Is.EqualTo("Test Issue 1"), 
            "Title should be loaded from metadata file, proving path resolution worked");
    }

    [AvaloniaTest]
    public async Task LoadIssuesIntoViewAsync_DeserializesMetadataJson_Correctly()
    {
        // Arrange
        var services = CreateTestServiceProviderWithRealDiscovery();
        var issue228Path = Path.Combine(_testRepoPath, "Issue228");
        Directory.CreateDirectory(issue228Path);
        
        // Create metadata JSON matching real structure
        var metadataJson = @"[
  {
    ""number"": 228,
    ""title"": ""Tests inherited from Generic test fixture"",
    ""state"": ""closed"",
    ""milestone"": ""No milestone"",
    ""labels"": [
      ""is:bug"",
      ""pri:normal"",
      ""closed:fixedinnewversion""
    ],
    ""url"": ""https://github.com/nunit/nunit3-vs-adapter/issues/228""
  }
]";
        
        var metadataPath = Path.Combine(_dataDir, "issues_metadata.json");
        await File.WriteAllTextAsync(metadataPath, metadataJson);
        
        // Verify deserialization works
        var deserialized = JsonSerializer.Deserialize<List<IssueMetadata>>(metadataJson);
        Assert.That(deserialized, Is.Not.Null, "Metadata should deserialize");
        Assert.That(deserialized![0].Number, Is.EqualTo(228), "Issue number should be correct");
        Assert.That(deserialized[0].Title, Is.EqualTo("Tests inherited from Generic test fixture"), "Title should be correct");
        
        // Create empty results.json
        await File.WriteAllTextAsync(Path.Combine(_dataDir, "results.json"), "[]");
        
        var window = CreateTestWindow(services);
        var mainViewModel = (MainViewModel)window.DataContext!;
        mainViewModel.RepositoryPath = _testRepoPath;
        
        // Wait for repository initialization
        await Task.Delay(500);
        
        var issueListView = mainViewModel.CurrentView as IssueListView;
        Assert.That(issueListView, Is.Not.Null);
        var issueListViewModel = (IssueListViewModel)issueListView!.DataContext!;
        
        // Act
        await Task.Delay(1000);
        
        // Assert
        var issue228 = issueListViewModel.AllIssues.FirstOrDefault(i => i.Number == 228);
        Assert.That(issue228, Is.Not.Null, "Issue 228 should be found");
        Assert.That(issue228!.Title, Is.EqualTo("Tests inherited from Generic test fixture"), 
            "Title should be extracted from deserialized metadata");
    }

    [AvaloniaTest]
    public async Task LoadIssuesIntoViewAsync_CreatesIssueListItem_WithAllPropertiesIncludingTitle()
    {
        // Arrange
        var services = CreateTestServiceProviderWithRealDiscovery();
        var issue228Path = Path.Combine(_testRepoPath, "Issue228");
        Directory.CreateDirectory(issue228Path);
        
        var metadata = new List<IssueMetadata>
        {
            new()
            {
                Number = 228,
                Title = "Tests inherited from Generic test fixture",
                State = "closed",
                Milestone = "No milestone",
                Labels = ["is:bug"],
                Url = "https://github.com/test/test/issues/228"
            }
        };
        
        var metadataJson = JsonSerializer.Serialize(metadata);
        var metadataPath = Path.Combine(_dataDir, "issues_metadata.json");
        await File.WriteAllTextAsync(metadataPath, metadataJson);
        
        // Create empty results.json
        await File.WriteAllTextAsync(Path.Combine(_dataDir, "results.json"), "[]");
        
        var window = CreateTestWindow(services);
        var mainViewModel = (MainViewModel)window.DataContext!;
        mainViewModel.RepositoryPath = _testRepoPath;
        
        // Wait for repository initialization
        await Task.Delay(500);
        
        var issueListView = mainViewModel.CurrentView as IssueListView;
        Assert.That(issueListView, Is.Not.Null);
        var issueListViewModel = (IssueListViewModel)issueListView!.DataContext!;
        
        // Act
        await Task.Delay(1000);
        
        // Assert
        var issue228 = issueListViewModel.AllIssues.FirstOrDefault(i => i.Number == 228);
        Assert.That(issue228, Is.Not.Null, "Issue 228 should be found");
        Assert.That(issue228!.Number, Is.EqualTo(228), "Number should be set");
        Assert.That(issue228.Title, Is.EqualTo("Tests inherited from Generic test fixture"), "Title should be set");
        Assert.That(issue228.State, Is.EqualTo("closed"), "State should be set");
        Assert.That(issue228.GitHubUrl, Is.Not.Empty, "GitHubUrl should be set");
    }

    [AvaloniaTest]
    public async Task FolderDiscovery_AndMetadataLookup_Integration_WithRealData()
    {
        // Arrange - create multiple folders with different issue numbers
        var services = CreateTestServiceProviderWithRealDiscovery();
        // Create folders BEFORE setting repository path to ensure they exist when discovery runs
        var issue1Path = Path.Combine(_testRepoPath, "Issue1");
        var issue228Path = Path.Combine(_testRepoPath, "Issue228");
        var issue999Path = Path.Combine(_testRepoPath, "Issue999");
        Directory.CreateDirectory(issue1Path);
        Directory.CreateDirectory(issue228Path);
        Directory.CreateDirectory(issue999Path);
        
        // Verify folders exist
        Assert.That(Directory.Exists(issue1Path), Is.True, "Issue1 folder should exist");
        Assert.That(Directory.Exists(issue228Path), Is.True, "Issue228 folder should exist");
        Assert.That(Directory.Exists(issue999Path), Is.True, "Issue999 folder should exist");
        
        // Create metadata for some but not all issues
        var metadata = new List<IssueMetadata>
        {
            new()
            {
                Number = 1,
                Title = "First Issue",
                State = "open",
                Milestone = null,
                Labels = [],
                Url = "https://github.com/test/test/issues/1"
            },
            new()
            {
                Number = 228,
                Title = "Tests inherited from Generic test fixture",
                State = "closed",
                Milestone = "No milestone",
                Labels = [],
                Url = "https://github.com/test/test/issues/228"
            }
            // Issue 999 is missing from metadata - should use fallback title
        };
        
        var metadataJson = JsonSerializer.Serialize(metadata);
        var metadataPath = Path.Combine(_dataDir, "issues_metadata.json");
        await File.WriteAllTextAsync(metadataPath, metadataJson);
        
        // Create empty results.json
        await File.WriteAllTextAsync(Path.Combine(_dataDir, "results.json"), "[]");
        
        var window = CreateTestWindow(services);
        var mainViewModel = (MainViewModel)window.DataContext!;
        mainViewModel.RepositoryPath = _testRepoPath;
        
        // Wait for repository initialization
        await Task.Delay(500);
        
        var issueListView = mainViewModel.CurrentView as IssueListView;
        Assert.That(issueListView, Is.Not.Null);
        var issueListViewModel = (IssueListViewModel)issueListView!.DataContext!;
        
        // Act - wait for async operations to complete
        var maxWait = 5000; // 5 seconds max
        var waited = 0;
        while (issueListViewModel.AllIssues.Count < 3 && waited < maxWait)
        {
            await Task.Delay(100);
            waited += 100;
        }
        
        // Additional wait to ensure all issues are loaded
        await Task.Delay(500);
        
        // Assert - verify all three issues are discovered and titles are correct
        var foundIssues = string.Join(", ", issueListViewModel.AllIssues.Select(i => i.Number));
        Assert.That(issueListViewModel.AllIssues.Count, Is.GreaterThanOrEqualTo(3), 
            $"At least 3 issues should be discovered. Found: {issueListViewModel.AllIssues.Count} ({foundIssues})");
        
        var issue1 = issueListViewModel.AllIssues.FirstOrDefault(i => i.Number == 1);
        Assert.That(issue1, Is.Not.Null, $"Issue 1 should be found. Found issues: {foundIssues}");
        Assert.That(issue1!.Title, Is.EqualTo("First Issue"), "Issue 1 title should come from metadata");
        
        var issue228 = issueListViewModel.AllIssues.FirstOrDefault(i => i.Number == 228);
        Assert.That(issue228, Is.Not.Null, $"Issue 228 should be found. Found issues: {foundIssues}");
        Assert.That(issue228!.Title, Is.EqualTo("Tests inherited from Generic test fixture"), 
            "Issue 228 title should come from metadata");
        
        var issue999 = issueListViewModel.AllIssues.FirstOrDefault(i => i.Number == 999);
        Assert.That(issue999, Is.Not.Null, $"Issue 999 should be found. Found issues: {foundIssues}");
        Assert.That(issue999!.Title, Is.EqualTo("Issue 999"), 
            "Issue 999 should use fallback title when metadata is missing");
    }
}
