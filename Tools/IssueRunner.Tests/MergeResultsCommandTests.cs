using IssueRunner.Commands;
using IssueRunner.Models;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System.IO;
using System.Text.Json;

namespace IssueRunner.Tests;

[TestFixture]
public class MergeResultsCommandTests
{
    private ILogger<MergeResultsCommand> _logger = null!;
    private string _testRoot = null!;

    [SetUp]
    public void SetUp()
    {
        _logger = Substitute.For<ILogger<MergeResultsCommand>>();
        _testRoot = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_testRoot);
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
    public async Task ExecuteAsync_MergesResults_FromLinuxAndWindows()
    {
        // Arrange
        var linuxPath = Path.Combine(_testRoot, "linux");
        var windowsPath = Path.Combine(_testRoot, "windows");
        var outputPath = Path.Combine(_testRoot, "output");
        Directory.CreateDirectory(linuxPath);
        Directory.CreateDirectory(windowsPath);
        Directory.CreateDirectory(outputPath);
        
        var linuxResults = new List<IssueResult>
        {
            new IssueResult { Number = 1, ProjectPath = "Test.csproj", TargetFrameworks = new List<string>(), Packages = new List<string>(), TestResult = "success" }
        };
        var windowsResults = new List<IssueResult>
        {
            new IssueResult { Number = 2, ProjectPath = "Test2.csproj", TargetFrameworks = new List<string>(), Packages = new List<string>(), TestResult = "success" }
        };
        
        await WriteResultsFile(Path.Combine(linuxPath, "results.json"), linuxResults);
        await WriteResultsFile(Path.Combine(windowsPath, "results.json"), windowsResults);
        
        var command = new MergeResultsCommand(_logger);

        // Act
        var result = await command.ExecuteAsync(linuxPath, windowsPath, outputPath, CancellationToken.None);

        // Assert
        Assert.That(result, Is.EqualTo(0));
        var mergedPath = Path.Combine(outputPath, "results.json");
        Assert.That(File.Exists(mergedPath), Is.True);
        
        var mergedJson = await File.ReadAllTextAsync(mergedPath);
        var merged = JsonSerializer.Deserialize<List<IssueResult>>(mergedJson);
        Assert.That(merged, Is.Not.Null);
        Assert.That(merged!.Count, Is.EqualTo(2), "Should merge both results");
    }

    [Test]
    public async Task ExecuteAsync_DeduplicatesResults_WithSameKey()
    {
        // Arrange
        var linuxPath = Path.Combine(_testRoot, "linux");
        var windowsPath = Path.Combine(_testRoot, "windows");
        var outputPath = Path.Combine(_testRoot, "output");
        Directory.CreateDirectory(linuxPath);
        Directory.CreateDirectory(windowsPath);
        Directory.CreateDirectory(outputPath);
        
        var linuxResults = new List<IssueResult>
        {
            new IssueResult { Number = 1, ProjectPath = "Test.csproj", TargetFrameworks = new List<string>(), Packages = new List<string>(), TestResult = "success" }
        };
        var windowsResults = new List<IssueResult>
        {
            new IssueResult { Number = 1, ProjectPath = "Test.csproj", TargetFrameworks = new List<string>(), Packages = new List<string>(), TestResult = "fail" }
        };
        
        await WriteResultsFile(Path.Combine(linuxPath, "results.json"), linuxResults);
        await WriteResultsFile(Path.Combine(windowsPath, "results.json"), windowsResults);
        
        var command = new MergeResultsCommand(_logger);

        // Act
        var result = await command.ExecuteAsync(linuxPath, windowsPath, outputPath, CancellationToken.None);

        // Assert
        Assert.That(result, Is.EqualTo(0));
        var mergedPath = Path.Combine(outputPath, "results.json");
        var mergedJson = await File.ReadAllTextAsync(mergedPath);
        var merged = JsonSerializer.Deserialize<List<IssueResult>>(mergedJson);
        Assert.That(merged!.Count, Is.EqualTo(1), "Should deduplicate by key");
    }

    [Test]
    public async Task ExecuteAsync_HandlesMissingFiles_Gracefully()
    {
        // Arrange
        var linuxPath = Path.Combine(_testRoot, "linux");
        var windowsPath = Path.Combine(_testRoot, "windows");
        var outputPath = Path.Combine(_testRoot, "output");
        Directory.CreateDirectory(linuxPath);
        Directory.CreateDirectory(windowsPath);
        Directory.CreateDirectory(outputPath);
        
        // Only create Linux results
        var linuxResults = new List<IssueResult>
        {
            new IssueResult { Number = 1, ProjectPath = "Test.csproj", TargetFrameworks = new List<string>(), Packages = new List<string>(), TestResult = "success" }
        };
        await WriteResultsFile(Path.Combine(linuxPath, "results.json"), linuxResults);
        
        var command = new MergeResultsCommand(_logger);

        // Act
        var result = await command.ExecuteAsync(linuxPath, windowsPath, outputPath, CancellationToken.None);

        // Assert
        Assert.That(result, Is.EqualTo(0), "Should handle missing files gracefully");
    }

    private async Task WriteResultsFile(string path, List<IssueResult> results)
    {
        var json = JsonSerializer.Serialize(results);
        await File.WriteAllTextAsync(path, json);
    }
}
