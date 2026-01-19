using IssueRunner.Services;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System.IO;

namespace IssueRunner.Tests;

[TestFixture]
public class IssueDiscoveryServiceTests
{
    private IEnvironmentService _environmentService = null!;
    private ILogger<IssueDiscoveryService> _logger = null!;
    private string _testRoot = null!;
    private IIssueDiscoveryService service = null!;

    [SetUp]
    public void SetUp()
    {
        _testRoot = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_testRoot);

        _environmentService = Substitute.For<IEnvironmentService>();
        _environmentService.Root.Returns(_testRoot);

        _logger = Substitute.For<ILogger<IssueDiscoveryService>>();
        var markerLogger = Substitute.For<ILogger<MarkerService>>();
        var markerService = new MarkerService(markerLogger);
        service = new IssueDiscoveryService(_environmentService, _logger, markerService);

    }

    [TearDown]
    public void TearDown()
    {
        if (Directory.Exists(_testRoot))
        {
            Directory.Delete(_testRoot, true);
        }
    }

    [Test]
    public void DiscoverIssueFolders_ReturnsEmptyDictionary_WhenNoIssueFoldersExist()
    {
        // Act
        var result = service.DiscoverIssueFolders();

        // Assert
        Assert.That(result, Is.Empty);
    }

    [Test]
    public void DiscoverIssueFolders_ReturnsEmptyDictionary_WhenRootPathDoesNotExist()
    {
        // Arrange
        var nonExistentPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        _environmentService.Root.Returns(nonExistentPath);


        // Act
        var result = service.DiscoverIssueFolders();

        // Assert
        Assert.That(result, Is.Empty);
    }

    [Test]
    public void DiscoverIssueFolders_ExtractsIssueNumber_FromFolderName()
    {
        // Arrange
        var issue228Path = Path.Combine(_testRoot, "Issue228");
        Directory.CreateDirectory(issue228Path);


        // Act
        var result = service.DiscoverIssueFolders();

        // Assert
        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result.ContainsKey(228), Is.True);
        Assert.That(result[228], Is.EqualTo(issue228Path));
    }

    [Test]
    public void DiscoverIssueFolders_ExtractsIssueNumber_FromFolderNameWithLeadingZeros()
    {
        // Arrange
        var issue1Path = Path.Combine(_testRoot, "Issue0001");
        Directory.CreateDirectory(issue1Path);


        // Act
        var result = service.DiscoverIssueFolders();

        // Assert
        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result.ContainsKey(1), Is.True);
        Assert.That(result[1], Is.EqualTo(issue1Path));
    }

    [Test]
    public void DiscoverIssueFolders_ExtractsIssueNumber_FromFolderNameWithSuffix()
    {
        // Arrange
        var issuePath = Path.Combine(_testRoot, "Issue123Test");
        Directory.CreateDirectory(issuePath);


        // Act
        var result = service.DiscoverIssueFolders();

        // Assert
        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result.ContainsKey(123), Is.True);
    }

    [Test]
    public void DiscoverIssueFolders_ReturnsMultipleIssues_WhenMultipleFoldersExist()
    {
        // Arrange
        Directory.CreateDirectory(Path.Combine(_testRoot, "Issue1"));
        Directory.CreateDirectory(Path.Combine(_testRoot, "Issue228"));
        Directory.CreateDirectory(Path.Combine(_testRoot, "Issue999"));


        // Act
        var result = service.DiscoverIssueFolders();

        // Assert
        Assert.That(result, Has.Count.EqualTo(3));
        Assert.That(result.ContainsKey(1), Is.True);
        Assert.That(result.ContainsKey(228), Is.True);
        Assert.That(result.ContainsKey(999), Is.True);
    }

    [Test]
    public void DiscoverIssueFolders_IgnoresFolders_ThatDoNotStartWithIssue()
    {
        // Arrange
        Directory.CreateDirectory(Path.Combine(_testRoot, "Issue228"));
        Directory.CreateDirectory(Path.Combine(_testRoot, "OtherFolder"));
        Directory.CreateDirectory(Path.Combine(_testRoot, "Test123"));


        // Act
        var result = service.DiscoverIssueFolders();

        // Assert
        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result.ContainsKey(228), Is.True);
    }

    [Test]
    public void DiscoverIssueFolders_IgnoresFolders_WithoutNumbers()
    {
        // Arrange
        Directory.CreateDirectory(Path.Combine(_testRoot, "Issue"));
        Directory.CreateDirectory(Path.Combine(_testRoot, "IssueABC"));


        // Act
        var result = service.DiscoverIssueFolders();

        // Assert
        Assert.That(result, Is.Empty);
    }

    [Test]
    public void DiscoverIssueFolders_ReturnsCorrectIssueNumberType_AsInt()
    {
        // Arrange
        var issue228Path = Path.Combine(_testRoot, "Issue228");
        Directory.CreateDirectory(issue228Path);


        // Act
        var result = service.DiscoverIssueFolders();

        // Assert
        Assert.That(result.Keys, Is.All.InstanceOf<int>());
        Assert.That(result[228], Is.EqualTo(issue228Path));
    }

    [Test]
    public void DiscoverIssueFolders_HandlesLargeIssueNumbers()
    {
        // Arrange
        var issue9999Path = Path.Combine(_testRoot, "Issue9999");
        Directory.CreateDirectory(issue9999Path);


        // Act
        var result = service.DiscoverIssueFolders();

        // Assert
        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result.ContainsKey(9999), Is.True);
        Assert.That(result[9999], Is.EqualTo(issue9999Path));
    }
}
