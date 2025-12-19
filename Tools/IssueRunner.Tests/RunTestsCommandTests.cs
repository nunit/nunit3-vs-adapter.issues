using IssueRunner.Commands;
using IssueRunner.Models;
using IssueRunner.Services;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;
using System.Text.Json;

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
            _environmentService);
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
            new IssueResult
            {
                Number = 1,
                ProjectPath = "test.csproj",
                TargetFrameworks = new List<string> { "net10.0" },
                Packages = new List<string>(),
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
            new IssueResult
            {
                Number = 1,
                ProjectPath = "test.csproj",
                TargetFrameworks = new List<string> { "net10.0" },
                Packages = new List<string>(),
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
            new IssueResult
            {
                Number = 1,
                ProjectPath = "test.csproj",
                TargetFrameworks = new List<string> { "net10.0" },
                Packages = new List<string>(),
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
}
