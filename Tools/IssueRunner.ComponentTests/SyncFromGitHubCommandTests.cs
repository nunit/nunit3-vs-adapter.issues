using IssueRunner.Commands;
using IssueRunner.Services;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using System.Text.Json;

namespace IssueRunner.ComponentTests;

[TestFixture]
public class SyncFromGitHubCommandTests
{
    private string? _tempDir;

    [SetUp]
    public void Setup()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), $"issuerunner-test-{Guid.NewGuid()}");
        Directory.CreateDirectory(_tempDir);
    }

    [TearDown]
    public void TearDown()
    {
        if (_tempDir != null && Directory.Exists(_tempDir))
        {
            Directory.Delete(_tempDir, recursive: true);
        }
    }

    [Test]
    [Category("Integration")]
    public async Task SyncFromGitHub_WithRepositoryConfig_FetchesCorrectRepository()
    {
        // Arrange - Create repository.json for nunit/nunit
        var toolsDir = Path.Combine(_tempDir!, "Tools");
        Directory.CreateDirectory(toolsDir);
        
        var repoConfig = new { owner = "nunit", name = "nunit" };
        var configJson = JsonSerializer.Serialize(repoConfig, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(Path.Combine(toolsDir, "repository.json"), configJson);

        // Create Issue1 folder
        Directory.CreateDirectory(Path.Combine(_tempDir!, "Issue1"));

        // Create command
        var httpClient = new HttpClient();
        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Warning));
        var githubApi = new GitHubApiService(httpClient, loggerFactory.CreateLogger<GitHubApiService>());
        var issueDiscovery = new IssueDiscoveryService(loggerFactory.CreateLogger<IssueDiscoveryService>());
        var command = new SyncFromGitHubCommand(githubApi, issueDiscovery, loggerFactory.CreateLogger<SyncFromGitHubCommand>());

        // Act
        var exitCode = await command.ExecuteAsync(_tempDir!, null, CancellationToken.None);

        // Assert
        Assert.That(exitCode, Is.EqualTo(0), "Command should succeed");
        
        var metadataPath = Path.Combine(toolsDir, "issues_metadata.json");
        Assert.That(File.Exists(metadataPath), Is.True, "Metadata file should be created");

        var json = await File.ReadAllTextAsync(metadataPath);
        Assert.That(json, Is.Not.Null.And.Not.Empty);
        
        Console.WriteLine($"Metadata file created at: {metadataPath}");
        Console.WriteLine($"Content length: {json.Length} bytes");
        
        httpClient.Dispose();
    }

    [Test]
    [Category("Integration")]
    public async Task SyncFromGitHub_WithoutRepositoryConfig_FailsWithError()
    {
        // Arrange - No repository.json file
        var toolsDir = Path.Combine(_tempDir!, "Tools");
        Directory.CreateDirectory(toolsDir);

        // Create Issue1 folder
        Directory.CreateDirectory(Path.Combine(_tempDir!, "Issue1"));

        // Create command
        var httpClient = new HttpClient();
        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        var githubApi = new GitHubApiService(httpClient, loggerFactory.CreateLogger<GitHubApiService>());
        var issueDiscovery = new IssueDiscoveryService(loggerFactory.CreateLogger<IssueDiscoveryService>());
        var command = new SyncFromGitHubCommand(githubApi, issueDiscovery, loggerFactory.CreateLogger<SyncFromGitHubCommand>());

        // Act
        var exitCode = await command.ExecuteAsync(_tempDir!, null, CancellationToken.None);

        // Assert
        Assert.That(exitCode, Is.EqualTo(1), "Command should fail without repository config");
        
        httpClient.Dispose();
    }
}
