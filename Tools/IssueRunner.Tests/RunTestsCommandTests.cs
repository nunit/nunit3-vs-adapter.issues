using IssueRunner.Commands;
using IssueRunner.Models;
using IssueRunner.Services;
using Microsoft.Extensions.Logging;

namespace IssueRunner.Tests;

[TestFixture]
public class RunTestsCommandTests
{
    private IIssueDiscoveryService? _issueDiscovery;
    private IProjectAnalyzerService? _projectAnalyzer;
    private IFrameworkUpgradeService? _frameworkUpgrade;
    private IPackageUpdateService? _packageUpdate;
    private ITestExecutionService? _testExecution;
    private ILogger<RunTestsCommand>? _logger;
    private ILoggerFactory? _loggerFactory;
    private IProcessExecutor? _processExecutor;
    private RunTestsCommand? _command;
    private IEnvironmentService? _environmentService;
    private INuGetPackageVersionService? _nugetVersions;
    private IMarkerService _markerService = null!;

    [SetUp]
    public void Setup()
    {
        _issueDiscovery = Substitute.For<IIssueDiscoveryService>();
        _projectAnalyzer = Substitute.For<IProjectAnalyzerService>();
        _frameworkUpgrade = Substitute.For<IFrameworkUpgradeService>();
        _packageUpdate = Substitute.For<IPackageUpdateService>();
        _testExecution = Substitute.For<ITestExecutionService>();
        _logger = Substitute.For<ILogger<RunTestsCommand>>();
        _loggerFactory = Substitute.For<ILoggerFactory>();
        _processExecutor = Substitute.For<IProcessExecutor>();
        _environmentService = Substitute.For<IEnvironmentService>();
        _nugetVersions = Substitute.For<INuGetPackageVersionService>();
        _markerService = Substitute.For<IMarkerService>();

        _command = new RunTestsCommand(
            _issueDiscovery,
            _projectAnalyzer,
            _frameworkUpgrade,
            _packageUpdate,
            _testExecution,
            _logger,
            _loggerFactory,
            _processExecutor,
            _nugetVersions,
            _environmentService,
            _markerService);
    }

    [Test]
    public void Constructor_InitializesSuccessfully()
    {
        Assert.That(_command, Is.Not.Null);
    }

    [Test]
    public void GetPreviousFeed_WithNoResults_ReturnsStable()
    {
        // Test the feed extraction logic
        var emptyResults = new List<IssueResult>();
        var feedString = GetFeedFromResults(emptyResults);

        Assert.That(feedString, Is.EqualTo("Stable"));
    }

    [Test]
    public void GetPreviousFeed_WithResults_ReturnsFeed()
    {
        var results = new List<IssueResult>
        {
            new()
            {
                Number = 1,
                ProjectPath = "test.csproj",
                TargetFrameworks = ["net10.0"],
                Packages = [],
                Feed = "Beta"
            }
        };

        var feedString = GetFeedFromResults(results);

        Assert.That(feedString, Is.EqualTo("Beta"));
    }

    [Test]
    public void CheckFeedChanged_WithSameFeed_ReturnsFalse()
    {
        var results = new List<IssueResult>
        {
            new()
            {
                Number = 1,
                ProjectPath = "test.csproj",
                TargetFrameworks = ["net10.0"],
                Packages = [],
                Feed = "Stable"
            }
        };

        var changed = IsChanged(results, PackageFeed.Stable);

        Assert.That(changed, Is.False);
    }

    [Test]
    public void CheckFeedChanged_WithDifferentFeed_ReturnsTrue()
    {
        var results = new List<IssueResult>
        {
            new()
            {
                Number = 1,
                ProjectPath = "test.csproj",
                TargetFrameworks = ["net10.0"],
                Packages = [],
                Feed = "Stable"
            }
        };

        var changed = IsChanged(results, PackageFeed.Beta);

        Assert.That(changed, Is.True);
    }

    [Test]
    public void CheckFeedChanged_WithNoResults_ReturnsFalse()
    {
        var emptyResults = new List<IssueResult>();

        var changed = IsChanged(emptyResults, PackageFeed.Stable);

        Assert.That(changed, Is.False);
    }

    // Helper methods to simulate the logic from RunTestsCommand
    private static string GetFeedFromResults(List<IssueResult> results)
    {
        return results.FirstOrDefault()?.Feed ?? "Stable";
    }

    private static bool IsChanged(List<IssueResult> previousResults, PackageFeed currentFeed)
    {
        var previousFeed = GetFeedFromResults(previousResults);
        return previousFeed != currentFeed.ToString();
    }

    [Test]
    public async Task FilterIssuesAsync_FiltersByTestTypes_WhenTestTypesIsCustom()
    {
        // Arrange
        var folders = new Dictionary<int, string>
        {
            { 1, "Issue1" },
            { 2, "Issue2" },
            { 3, "Issue3" }
        };
        _issueDiscovery!.DiscoverIssueFolders().Returns(folders);
        _testExecution!.HasCustomRunners("Issue1").Returns(true);
        _testExecution!.HasCustomRunners("Issue2").Returns(false);
        _testExecution!.HasCustomRunners("Issue3").Returns(true);

        var options = new RunOptions
        {
            TestTypes = TestTypes.Custom
        };

        // Act
        var filtered = await FilterIssuesAsyncWithMetadata(options, folders, []);

        // Assert
        Assert.That(filtered.Count, Is.EqualTo(2));
        Assert.That(filtered.ContainsKey(1), Is.True);
        Assert.That(filtered.ContainsKey(3), Is.True);
        Assert.That(filtered.ContainsKey(2), Is.False);
    }

    [Test]
    public async Task FilterIssuesAsync_FiltersByTestTypes_WhenTestTypesIsDirect()
    {
        // Arrange
        var folders = new Dictionary<int, string>
        {
            { 1, "Issue1" },
            { 2, "Issue2" },
            { 3, "Issue3" }
        };
        _issueDiscovery!.DiscoverIssueFolders().Returns(folders);
        _testExecution!.HasCustomRunners("Issue1").Returns(true);
        _testExecution!.HasCustomRunners("Issue2").Returns(false);
        _testExecution!.HasCustomRunners("Issue3").Returns(true);

        var options = new RunOptions
        {
            TestTypes = TestTypes.Direct
        };

        // Act
        var filtered = await FilterIssuesAsyncWithMetadata(options, folders, []);

        // Assert
        Assert.That(filtered.Count, Is.EqualTo(1));
        Assert.That(filtered.ContainsKey(2), Is.True);
        Assert.That(filtered.ContainsKey(1), Is.False);
        Assert.That(filtered.ContainsKey(3), Is.False);
    }

    [Test]
    public async Task FilterIssuesAsync_ReturnsAllIssues_WhenTestTypesIsAll()
    {
        // Arrange
        var folders = new Dictionary<int, string>
        {
            { 1, "Issue1" },
            { 2, "Issue2" },
            { 3, "Issue3" }
        };
        _issueDiscovery!.DiscoverIssueFolders().Returns(folders);
        _testExecution!.HasCustomRunners(Arg.Any<string>()).Returns(false);

        var options = new RunOptions
        {
            TestTypes = TestTypes.All
        };

        // Act
        var filtered = await FilterIssuesAsyncWithMetadata(options, folders, []);

        // Assert
        Assert.That(filtered.Count, Is.EqualTo(3));
        Assert.That(filtered.ContainsKey(1), Is.True);
        Assert.That(filtered.ContainsKey(2), Is.True);
        Assert.That(filtered.ContainsKey(3), Is.True);
    }

    [Test]
    public async Task FilterIssuesAsync_FiltersByScope_WhenScopeIsRegression()
    {
        // Arrange
        var folders = new Dictionary<int, string>
        {
            { 1, "Issue1" },
            { 2, "Issue2" },
            { 3, "Issue3" }
        };
        _issueDiscovery!.DiscoverIssueFolders().Returns(folders);
        _environmentService!.Root.Returns("C:\\test");
        
        // Create metadata with different states
        var metadataPath = Path.Combine("C:\\test", "Tools", "issues_metadata.json");
        var metadataDir = Path.GetDirectoryName(metadataPath);
        if (metadataDir != null && !Directory.Exists(metadataDir))
        {
            Directory.CreateDirectory(metadataDir);
        }
        
        var metadata = new List<IssueMetadata>
        {
            new() { Number = 1, State = "closed", Title = "Issue 1", Labels = [], Url = "https://github.com/test/test/issues/1" },
            new() { Number = 2, State = "open", Title = "Issue 2", Labels = [], Url = "https://github.com/test/test/issues/2" },
            new() { Number = 3, State = "closed", Title = "Issue 3", Labels = [], Url = "https://github.com/test/test/issues/3" }
        };
        
        var metadataJson = JsonSerializer.Serialize(metadata);
        await File.WriteAllTextAsync(metadataPath, metadataJson);

        var options = new RunOptions
        {
            Scope = TestScope.Regression
        };

        // Act - Need to call the actual FilterIssuesAsync with proper parameters
        var filtered = await FilterIssuesAsyncWithMetadata(options, folders, metadata);

        // Assert
        Assert.That(filtered.Count, Is.EqualTo(2));
        Assert.That(filtered.ContainsKey(1), Is.True);
        Assert.That(filtered.ContainsKey(3), Is.True);
        Assert.That(filtered.ContainsKey(2), Is.False);
        
        // Cleanup
        if (File.Exists(metadataPath))
        {
            File.Delete(metadataPath);
        }
    }

    [Test]
    public async Task FilterIssuesAsync_FiltersByScope_WhenScopeIsOpen()
    {
        // Arrange
        var folders = new Dictionary<int, string>
        {
            { 1, "Issue1" },
            { 2, "Issue2" },
            { 3, "Issue3" }
        };
        _issueDiscovery!.DiscoverIssueFolders().Returns(folders);
        _environmentService!.Root.Returns("C:\\test");
        
        var metadataPath = Path.Combine("C:\\test", "Tools", "issues_metadata.json");
        var metadataDir = Path.GetDirectoryName(metadataPath);
        if (metadataDir != null && !Directory.Exists(metadataDir))
        {
            Directory.CreateDirectory(metadataDir);
        }
        
        var metadata = new List<IssueMetadata>
        {
            new() { Number = 1, State = "closed", Title = "Issue 1", Labels = [], Url = "https://github.com/test/test/issues/1" },
            new() { Number = 2, State = "open", Title = "Issue 2", Labels = [], Url = "https://github.com/test/test/issues/2" },
            new() { Number = 3, State = "open", Title = "Issue 3", Labels = [], Url = "https://github.com/test/test/issues/3" }
        };
        
        var metadataJson = JsonSerializer.Serialize(metadata);
        await File.WriteAllTextAsync(metadataPath, metadataJson);

        var options = new RunOptions
        {
            Scope = TestScope.Open
        };

        // Act
        var filtered = await FilterIssuesAsyncWithMetadata(options, folders, metadata);

        // Assert
        Assert.That(filtered.Count, Is.EqualTo(2));
        Assert.That(filtered.ContainsKey(2), Is.True);
        Assert.That(filtered.ContainsKey(3), Is.True);
        Assert.That(filtered.ContainsKey(1), Is.False);
        
        // Cleanup
        if (File.Exists(metadataPath))
        {
            File.Delete(metadataPath);
        }
    }

    [Test]
    public async Task FilterIssuesAsync_ReturnsAllIssues_WhenScopeIsAll()
    {
        // Arrange
        var folders = new Dictionary<int, string>
        {
            { 1, "Issue1" },
            { 2, "Issue2" },
            { 3, "Issue3" }
        };
        _issueDiscovery!.DiscoverIssueFolders().Returns(folders);
        _environmentService!.Root.Returns("C:\\test");
        
        var metadataPath = Path.Combine("C:\\test", "Tools", "issues_metadata.json");
        var metadataDir = Path.GetDirectoryName(metadataPath);
        if (metadataDir != null && !Directory.Exists(metadataDir))
        {
            Directory.CreateDirectory(metadataDir);
        }
        
        var metadata = new List<IssueMetadata>
        {
            new() { Number = 1, State = "closed", Title = "Issue 1", Labels = [], Url = "https://github.com/test/test/issues/1" },
            new() { Number = 2, State = "open", Title = "Issue 2", Labels = [], Url = "https://github.com/test/test/issues/2" },
            new() { Number = 3, State = "closed", Title = "Issue 3", Labels = [], Url = "https://github.com/test/test/issues/3" }
        };
        
        var metadataJson = JsonSerializer.Serialize(metadata);
        await File.WriteAllTextAsync(metadataPath, metadataJson);

        var options = new RunOptions
        {
            Scope = TestScope.All
        };

        // Act
        var filtered = await FilterIssuesAsyncWithMetadata(options, folders, metadata);

        // Assert
        Assert.That(filtered.Count, Is.EqualTo(3));
        Assert.That(filtered.ContainsKey(1), Is.True);
        Assert.That(filtered.ContainsKey(2), Is.True);
        Assert.That(filtered.ContainsKey(3), Is.True);
        
        // Cleanup
        if (File.Exists(metadataPath))
        {
            File.Delete(metadataPath);
        }
    }

    private async Task<Dictionary<int, string>> FilterIssuesAsyncWithMetadata(
        RunOptions options, 
        Dictionary<int, string> folders,
        List<IssueMetadata> metadata)
    {
        // Use reflection to access the private FilterIssuesAsync method with all parameters
        var method = typeof(RunTestsCommand).GetMethod("FilterIssuesAsync", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance,
            null,
            [
                typeof(Dictionary<int, string>), 
                typeof(Dictionary<int, IssueMetadata>), 
                typeof(RunOptions), 
                typeof(string), 
                typeof(CancellationToken)
            ],
            null);
        
        if (method == null)
        {
            throw new InvalidOperationException("FilterIssuesAsync method not found");
        }

        var metadataDict = metadata.ToDictionary(m => m.Number);
        var result = method.Invoke(_command, [
            folders, 
            metadataDict, 
            options, 
            _environmentService!.Root, 
            CancellationToken.None
        ]);
        return await (Task<Dictionary<int, string>>)result!;
    }

    [Test]
    public void BuildConclusion_ReportsRestoreFailed_WhenRestoreStepFails()
    {
        // Arrange
        var testSuccess = false;
        var updateSuccess = true;
        var testOutput = "=== Restore ===\nRestore error";
        var testError = "=== Restore Error ===\nRestore failed";
        var failedStep = "restore";

        // Act
        var conclusion = BuildConclusion(testSuccess, updateSuccess, testOutput, testError, failedStep);

        // Assert
        Assert.That(conclusion, Does.Contain("Restore failed"));
    }

    [Test]
    public void BuildConclusion_ReportsBuildFailed_WhenBuildStepFails()
    {
        // Arrange
        var testSuccess = false;
        var updateSuccess = true;
        var testOutput = "=== Build ===\nBuild error";
        var testError = "=== Build Error ===\nBuild failed";
        var failedStep = "build";

        // Act
        var conclusion = BuildConclusion(testSuccess, updateSuccess, testOutput, testError, failedStep);

        // Assert
        Assert.That(conclusion, Does.Contain("Build failed"));
    }

    [Test]
    public void BuildConclusion_ReportsTestFailed_WhenTestStepFails()
    {
        // Arrange
        var testSuccess = false;
        var updateSuccess = true;
        var testOutput = "=== Test ===\nTest error";
        var testError = "=== Test Error ===\nTest failed";
        var failedStep = "test";

        // Act
        var conclusion = BuildConclusion(testSuccess, updateSuccess, testOutput, testError, failedStep);

        // Assert
        Assert.That(conclusion, Does.Contain("Failure:"));
        // Should use DetermineFailureReason for test failures
        Assert.That(conclusion, Does.Not.Contain("Restore failed"));
        Assert.That(conclusion, Does.Not.Contain("Build failed"));
    }

    [Test]
    public void BuildConclusion_ReportsSuccess_WhenAllStepsPass()
    {
        // Arrange
        var testSuccess = true;
        var updateSuccess = true;
        var testOutput = "=== Test ===\nAll tests passed";
        var testError = "";
        var failedStep = (string?)null;

        // Act
        var conclusion = BuildConclusion(testSuccess, updateSuccess, testOutput, testError, failedStep);

        // Assert
        Assert.That(conclusion, Does.Contain("Success"));
        Assert.That(conclusion, Does.Contain("No regression failure"));
    }

    private static string BuildConclusion(bool testSuccess, bool updateSuccess, string testOutput, string testError, string? failedStep)
    {
        // Use reflection to access the private BuildConclusion method
        var method = typeof(RunTestsCommand).GetMethod("BuildConclusion",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static,
            null,
            [
                typeof(bool),
                typeof(bool),
                typeof(string),
                typeof(string),
                typeof(string)
            ],
            null);

        if (method == null)
        {
            throw new InvalidOperationException("BuildConclusion method not found");
        }

        var result = method.Invoke(null, [testSuccess, updateSuccess, testOutput, testError, failedStep]);
        return (string)result!;
    }
}
