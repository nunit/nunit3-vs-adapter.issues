using IssueRunner.Commands;
using IssueRunner.Models;
using IssueRunner.Services;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System.IO;
using System.Text.Json;

namespace IssueRunner.Tests;

[TestFixture]
public class CheckRegressionsCommandTests
{
    private ILogger<CheckRegressionsCommand> _logger = null!;
    private IEnvironmentService _environmentService = null!;
    private string _testRoot = null!;
    private string _dataDir = null!;

    [SetUp]
    public void SetUp()
    {
        _logger = Substitute.For<ILogger<CheckRegressionsCommand>>();
        _testRoot = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_testRoot);
        _dataDir = Path.Combine(_testRoot, ".nunit", "IssueRunner");
        Directory.CreateDirectory(_dataDir);
        
        _environmentService = Substitute.For<IEnvironmentService>();
        _environmentService.GetDataDirectory(Arg.Any<string>()).Returns(_dataDir);
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
    public async Task ExecuteAsync_ReturnsError_WhenResultsFileNotFound()
    {
        // Arrange
        var command = new CheckRegressionsCommand(_logger, _environmentService);

        // Act
        var result = await command.ExecuteAsync(_testRoot, CancellationToken.None);

        // Assert
        Assert.That(result, Is.EqualTo(1));
    }

    [Test]
    public async Task ExecuteAsync_ReturnsError_WhenMetadataFileNotFound()
    {
        // Arrange
        var results = new List<IssueResult>();
        await WriteResultsFile("results.json", results);
        
        var command = new CheckRegressionsCommand(_logger, _environmentService);

        // Act
        var result = await command.ExecuteAsync(_testRoot, CancellationToken.None);

        // Assert
        Assert.That(result, Is.EqualTo(1));
    }

    [Test]
    public async Task ExecuteAsync_ReturnsSuccess_WhenNoRegressions()
    {
        // Arrange
        var results = new List<IssueResult>
        {
            new IssueResult { Number = 1, ProjectPath = "Test.csproj", TargetFrameworks = new List<string>(), Packages = new List<string>(), TestResult = "success" }
        };
        var metadata = new List<IssueMetadata>
        {
            new IssueMetadata { Number = 1, State = "closed", Title = "Test Issue", Labels = new List<string>(), Url = "https://github.com/test/test/issues/1" }
        };
        
        await WriteResultsFile("results.json", results);
        await WriteMetadataFile("issues_metadata.json", metadata);
        
        var command = new CheckRegressionsCommand(_logger, _environmentService);

        // Act
        var result = await command.ExecuteAsync(_testRoot, CancellationToken.None);

        // Assert
        Assert.That(result, Is.EqualTo(0));
    }

    [Test]
    public async Task ExecuteAsync_ReturnsError_WhenRegressionsDetected()
    {
        // Arrange
        var results = new List<IssueResult>
        {
            new IssueResult { Number = 1, ProjectPath = "Test.csproj", TargetFrameworks = new List<string>(), Packages = new List<string>(), TestResult = "fail" }
        };
        var metadata = new List<IssueMetadata>
        {
            new IssueMetadata { Number = 1, State = "closed", Title = "Closed Issue", Labels = new List<string>(), Url = "https://github.com/test/test/issues/1" }
        };
        
        await WriteResultsFile("results.json", results);
        await WriteMetadataFile("issues_metadata.json", metadata);
        
        var command = new CheckRegressionsCommand(_logger, _environmentService);

        // Act
        var result = await command.ExecuteAsync(_testRoot, CancellationToken.None);

        // Assert
        Assert.That(result, Is.EqualTo(1));
    }

    [Test]
    public async Task ExecuteAsync_IgnoresOpenIssues()
    {
        // Arrange
        var results = new List<IssueResult>
        {
            new IssueResult { Number = 1, ProjectPath = "Test.csproj", TargetFrameworks = new List<string>(), Packages = new List<string>(), TestResult = "fail" }
        };
        var metadata = new List<IssueMetadata>
        {
            new IssueMetadata { Number = 1, State = "open", Title = "Open Issue", Labels = new List<string>(), Url = "https://github.com/test/test/issues/1" }
        };
        
        await WriteResultsFile("results.json", results);
        await WriteMetadataFile("issues_metadata.json", metadata);
        
        var command = new CheckRegressionsCommand(_logger, _environmentService);

        // Act
        var result = await command.ExecuteAsync(_testRoot, CancellationToken.None);

        // Assert
        Assert.That(result, Is.EqualTo(0), "Open issues should not be considered regressions");
    }

    [Test]
    public async Task ExecuteAsync_HandlesDuplicateMetadata()
    {
        // Arrange
        var results = new List<IssueResult>
        {
            new IssueResult { Number = 1, ProjectPath = "Test.csproj", TargetFrameworks = new List<string>(), Packages = new List<string>(), TestResult = "fail" }
        };
        var metadata = new List<IssueMetadata>
        {
            new IssueMetadata { Number = 1, State = "open", Title = "First", Labels = new List<string>(), Url = "https://github.com/test/test/issues/1" },
            new IssueMetadata { Number = 1, State = "closed", Title = "Second", Labels = new List<string>(), Url = "https://github.com/test/test/issues/1" }
        };
        
        await WriteResultsFile("results.json", results);
        await WriteMetadataFile("issues_metadata.json", metadata);
        
        var command = new CheckRegressionsCommand(_logger, _environmentService);

        // Act
        var result = await command.ExecuteAsync(_testRoot, CancellationToken.None);

        // Assert
        Assert.That(result, Is.EqualTo(1), "Should use last occurrence of duplicate metadata");
    }

    private async Task WriteResultsFile(string fileName, List<IssueResult> results)
    {
        var filePath = Path.Combine(_dataDir, fileName);
        var json = JsonSerializer.Serialize(results);
        await File.WriteAllTextAsync(filePath, json);
    }

    private async Task WriteMetadataFile(string fileName, List<IssueMetadata> metadata)
    {
        var filePath = Path.Combine(_dataDir, fileName);
        var json = JsonSerializer.Serialize(metadata);
        await File.WriteAllTextAsync(filePath, json);
    }
}
