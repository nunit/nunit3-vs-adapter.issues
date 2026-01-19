using IssueRunner.Commands;
using IssueRunner.Models;
using IssueRunner.Services;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System.IO;
using System.Text.Json;

namespace IssueRunner.Tests;

[TestFixture]
public class GenerateReportCommandTests
{
    private ILogger<GenerateReportCommand> _logger = null!;
    private IEnvironmentService _environmentService = null!;
    private ReportGeneratorService _reportGenerator = null!;
    private string _testRoot = null!;
    private string _dataDir = null!;

    [SetUp]
    public void SetUp()
    {
        _logger = Substitute.For<ILogger<GenerateReportCommand>>();
        _testRoot = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_testRoot);
        _dataDir = Path.Combine(_testRoot, ".nunit", "IssueRunner");
        Directory.CreateDirectory(_dataDir);
        
        _environmentService = Substitute.For<IEnvironmentService>();
        _environmentService.Root.Returns(_testRoot);
        _environmentService.GetDataDirectory(Arg.Any<string>()).Returns(_dataDir);
        
        var reportLogger = Substitute.For<ILogger<ReportGeneratorService>>();
        _reportGenerator = new ReportGeneratorService(reportLogger, _environmentService);
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
        var command = new GenerateReportCommand(_reportGenerator, _environmentService, _logger);

        // Act
        var result = await command.ExecuteAsync(CancellationToken.None);

        // Assert
        Assert.That(result, Is.EqualTo(1));
    }

    [Test]
    public async Task ExecuteAsync_ReturnsError_WhenMetadataFileNotFound()
    {
        // Arrange
        var results = new List<IssueResult>();
        await WriteResultsFile("results.json", results);
        
        var command = new GenerateReportCommand(_reportGenerator, _environmentService, _logger);

        // Act
        var result = await command.ExecuteAsync(CancellationToken.None);

        // Assert
        Assert.That(result, Is.EqualTo(1));
    }

    [Test]
    public async Task ExecuteAsync_GeneratesReport_WhenFilesExist()
    {
        // Arrange
        var results = new List<IssueResult>
        {
            new IssueResult { Number = 1, ProjectPath = "Test.csproj", TargetFrameworks = new List<string>(), Packages = new List<string>(), TestResult = "success" }
        };
        var metadata = new List<IssueMetadata>
        {
            new IssueMetadata { Number = 1, State = "open", Title = "Test Issue", Labels = new List<string>(), Url = "https://github.com/test/test/issues/1" }
        };
        
        await WriteResultsFile("results.json", results);
        await WriteMetadataFile("issues_metadata.json", metadata);
        
        var command = new GenerateReportCommand(_reportGenerator, _environmentService, _logger);

        // Act
        var result = await command.ExecuteAsync(CancellationToken.None);

        // Assert
        Assert.That(result, Is.EqualTo(0));
        var reportPath = Path.Combine(_dataDir, "TestReport.md");
        Assert.That(File.Exists(reportPath), Is.True, "Report should be generated");
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
