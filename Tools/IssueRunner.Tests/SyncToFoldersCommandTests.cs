using IssueRunner.Commands;
using IssueRunner.Models;
using IssueRunner.Services;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System.IO;
using System.Text.Json;

namespace IssueRunner.Tests;

[TestFixture]
public class SyncToFoldersCommandTests
{
    private ILogger<SyncToFoldersCommand> _logger = null!;
    private IEnvironmentService _environmentService = null!;
    private IIssueDiscoveryService _issueDiscovery = null!;
    private IProjectAnalyzerService _projectAnalyzer = null!;
    private string _testRoot = null!;
    private string _dataDir = null!;

    [SetUp]
    public void SetUp()
    {
        _logger = Substitute.For<ILogger<SyncToFoldersCommand>>();
        _testRoot = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_testRoot);
        _dataDir = Path.Combine(_testRoot, ".nunit", "IssueRunner");
        Directory.CreateDirectory(_dataDir);
        
        _environmentService = Substitute.For<IEnvironmentService>();
        _environmentService.GetDataDirectory(Arg.Any<string>()).Returns(_dataDir);
        
        _issueDiscovery = Substitute.For<IIssueDiscoveryService>();
        _projectAnalyzer = Substitute.For<IProjectAnalyzerService>();
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
    public async Task ExecuteAsync_ReturnsError_WhenMetadataFileNotFound()
    {
        // Arrange
        var command = new SyncToFoldersCommand(_issueDiscovery, _projectAnalyzer, _logger, _environmentService);

        // Act
        var result = await command.ExecuteAsync(_testRoot, null, CancellationToken.None);

        // Assert
        Assert.That(result, Is.EqualTo(1));
    }

    [Test]
    public async Task ExecuteAsync_SyncsMetadata_ToIssueFolders()
    {
        // Arrange
        var issue1Path = Path.Combine(_testRoot, "Issue1");
        Directory.CreateDirectory(issue1Path);
        
        var metadata = new List<IssueMetadata>
        {
            new IssueMetadata { Number = 1, State = "open", Title = "Test Issue", Labels = new List<string>(), Url = "https://github.com/test/test/issues/1" }
        };
        
        var metadataPath = Path.Combine(_dataDir, "issues_metadata.json");
        var json = JsonSerializer.Serialize(metadata);
        await File.WriteAllTextAsync(metadataPath, json);
        
        _issueDiscovery.DiscoverIssueFolders().Returns(new Dictionary<int, string> { { 1, issue1Path } });
        _projectAnalyzer.FindProjectFiles(Arg.Any<string>()).Returns(new List<string> { Path.Combine(issue1Path, "Test.csproj") });
        
        var command = new SyncToFoldersCommand(_issueDiscovery, _projectAnalyzer, _logger, _environmentService);

        // Act
        var result = await command.ExecuteAsync(_testRoot, null, CancellationToken.None);

        // Assert
        Assert.That(result, Is.EqualTo(0));
    }

    [Test]
    public async Task ExecuteAsync_SkipsIssues_WithoutMetadata()
    {
        // Arrange
        var issue1Path = Path.Combine(_testRoot, "Issue1");
        Directory.CreateDirectory(issue1Path);
        
        var metadata = new List<IssueMetadata>(); // Empty metadata
        
        var metadataPath = Path.Combine(_dataDir, "issues_metadata.json");
        var json = JsonSerializer.Serialize(metadata);
        await File.WriteAllTextAsync(metadataPath, json);
        
        _issueDiscovery.DiscoverIssueFolders().Returns(new Dictionary<int, string> { { 1, issue1Path } });
        
        var command = new SyncToFoldersCommand(_issueDiscovery, _projectAnalyzer, _logger, _environmentService);

        // Act
        var result = await command.ExecuteAsync(_testRoot, null, CancellationToken.None);

        // Assert
        Assert.That(result, Is.EqualTo(0), "Should skip issues without metadata");
    }
}
