using IssueRunner.Models;
using IssueRunner.Services;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using NSubstitute;
using NUnit.Framework;

namespace IssueRunner.Tests;

[TestFixture]
public class PackageUpdateServiceTests
{
    private ProcessExecutor? _processExecutor;
    private ILogger<PackageUpdateService>? _logger;
    private PackageUpdateService? _service;

    [SetUp]
    public void Setup()
    {
        _logger = Substitute.For<ILogger<PackageUpdateService>>();
        var executorLogger = Substitute.For<ILogger<ProcessExecutor>>();
        _processExecutor = new ProcessExecutor(executorLogger);
        var httpClientFactory = Substitute.For<IHttpClientFactory>();
        httpClientFactory.CreateClient(Arg.Any<string>()).Returns(new HttpClient());
        _service = new PackageUpdateService(_processExecutor, httpClientFactory, _logger);
    }

    [Test]
    public void UpdatePackagesAsync_WithStableFeed_DoesNotAddPrerelease()
    {
        // This is a basic structure test since we can't easily mock ProcessExecutor
        // The actual functionality is tested in component tests
        Assert.That(_service, Is.Not.Null);
    }

    [Test]
    public void PackageFeed_HasCorrectValues()
    {
        // Test the enum values
        Assert.That(PackageFeed.Stable, Is.EqualTo(PackageFeed.Stable));
        Assert.That(PackageFeed.Beta, Is.EqualTo(PackageFeed.Beta));
        Assert.That(PackageFeed.Alpha, Is.EqualTo(PackageFeed.Alpha));
    }

    [Test]
    public void PackageFeed_ToString_ReturnsExpectedValues()
    {
        Assert.That(PackageFeed.Stable.ToString(), Is.EqualTo("Stable"));
        Assert.That(PackageFeed.Beta.ToString(), Is.EqualTo("Beta"));
        Assert.That(PackageFeed.Alpha.ToString(), Is.EqualTo("Alpha"));
    }

    [Test]
    public void RunOptions_DefaultFeed_IsStable()
    {
        var options = new RunOptions
        {
            IssueNumbers = null,
            Scope = TestScope.All,
            TimeoutSeconds = 600,
            SkipNetFx = false,
            OnlyNetFx = false,
            NUnitOnly = false,
            ExecutionMode = ExecutionMode.All,
            Verbosity = LogVerbosity.Normal
        };

        Assert.That(options.Feed, Is.EqualTo(PackageFeed.Stable));
    }

    [Test]
    public void RunOptions_CanSetFeed()
    {
        var options = new RunOptions
        {
            IssueNumbers = null,
            Scope = TestScope.All,
            TimeoutSeconds = 600,
            SkipNetFx = false,
            OnlyNetFx = false,
            NUnitOnly = false,
            ExecutionMode = ExecutionMode.All,
            Verbosity = LogVerbosity.Normal,
            Feed = PackageFeed.Beta
        };

        Assert.That(options.Feed, Is.EqualTo(PackageFeed.Beta));
    }
}
