using IssueRunner.Commands;
using IssueRunner.Models;
using IssueRunner.Services;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using System;
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
        // Arrange - Create repository.json for nunit/nunit3-vs-adapter
        var toolsDir = Path.Combine(_tempDir!, "Tools");
        Directory.CreateDirectory(toolsDir);
        var dataDir = Path.Combine(_tempDir!, ".nunit", "IssueRunner");

        var environment = Substitute.For<IEnvironmentService>();
        environment.Root.Returns(_tempDir!);
        environment.RepositoryConfig.Returns(new RepositoryConfig("nunit", "nunit3-vs-adapter"));

        var repoConfig = new { owner = "nunit", name = "nunit3-vs-adapter" };
        var configJson = JsonSerializer.Serialize(repoConfig, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(Path.Combine(toolsDir, "repository.json"), configJson);

        // Create Issue1 folder
        Directory.CreateDirectory(Path.Combine(_tempDir!, "Issue1"));

        // Create command
        var httpClient = new HttpClient();
        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Warning));
        var githubApi = new GitHubApiService(httpClient, environment, loggerFactory.CreateLogger<GitHubApiService>());
        var markerService = new MarkerService(loggerFactory.CreateLogger<MarkerService>());
        var issueDiscovery = new IssueDiscoveryService(environment, loggerFactory.CreateLogger<IssueDiscoveryService>(),markerService);
        var environmentService = new EnvironmentService(loggerFactory.CreateLogger<EnvironmentService>());
        environmentService.AddRoot(_tempDir!);
        var command = new SyncFromGitHubCommand(githubApi, issueDiscovery, environmentService, loggerFactory.CreateLogger<SyncFromGitHubCommand>());

        // Act
        var exitCode = await command.ExecuteAsync(null, CancellationToken.None);

        // Assert
        // If GITHUB_TOKEN is not set, expect failure with 401 Unauthorized
        var hasToken = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("GITHUB_TOKEN"));
        if (!hasToken)
        {
            Assert.That(exitCode, Is.EqualTo(1), "Command should fail without GITHUB_TOKEN");
            // Verify the error message indicates authentication failure
            // The error is logged to console, which is captured in test output
            return; // Test passes - correctly detected missing token
        }
        
        // If token is present, expect success
        Assert.That(exitCode, Is.EqualTo(0), "Command should succeed with valid GITHUB_TOKEN");

        var metadataPath = Path.Combine(dataDir, "issues_metadata.json");
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
        var environment = Substitute.For<IEnvironmentService>();
        environment.Root.Returns(_tempDir!);
        // Don't set RepositoryConfig - it should be null when repository.json doesn't exist
        environment.RepositoryConfig.Returns((RepositoryConfig?)null);

        // Create Issue1 folder
        Directory.CreateDirectory(Path.Combine(_tempDir!, "Issue1"));

        // Create command - use mocked environment service instead of real one to avoid AddRoot throwing
        var httpClient = new HttpClient();
        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        var githubApi = new GitHubApiService(httpClient, environment, loggerFactory.CreateLogger<GitHubApiService>());
        var markerService = new MarkerService(loggerFactory.CreateLogger<MarkerService>());
        var issueDiscovery = new IssueDiscoveryService(environment, loggerFactory.CreateLogger<IssueDiscoveryService>(), markerService);
        // Use the mocked environment service instead of creating a real one
        var command = new SyncFromGitHubCommand(githubApi, issueDiscovery, environment, loggerFactory.CreateLogger<SyncFromGitHubCommand>());

        // Act
        var exitCode = await command.ExecuteAsync(null, CancellationToken.None);

        // Assert
        Assert.That(exitCode, Is.EqualTo(1), "Command should fail without repository config");

        httpClient.Dispose();
    }

    [Test]
    public async Task SyncFromGitHub_CanSuccessfullySyncSingleIssue_WithMockedApi()
    {
        // Arrange
        var toolsDir = Path.Combine(_tempDir!, "Tools");
        Directory.CreateDirectory(toolsDir);
        var dataDir = Path.Combine(_tempDir!, ".nunit", "IssueRunner");

        Directory.CreateDirectory(Path.Combine(_tempDir!, "Issue123"));

        var environment = Substitute.For<IEnvironmentService>();
        environment.Root.Returns(_tempDir!);
        environment.RepositoryConfig.Returns(new RepositoryConfig("test", "test"));
        environment.GetDataDirectory(Arg.Any<string>())
            .Returns(callInfo => dataDir);

        var githubApi = Substitute.For<IGitHubApiService>();
        githubApi.FetchIssueMetadataAsync(123, Arg.Any<CancellationToken>())
            .Returns(new IssueMetadata
            {
                Number = 123,
                Title = "Test Issue",
                State = "open",
                Milestone = "No milestone",
                Labels = ["bug"],
                Url = "https://github.com/test/test/issues/123"
            });

        var issueDiscovery = Substitute.For<IIssueDiscoveryService>();
        issueDiscovery.DiscoverIssueFolders().Returns(new Dictionary<int, string>
        {
            { 123, Path.Combine(_tempDir!, "Issue123") }
        });

        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Warning));
        var command = new SyncFromGitHubCommand(
            githubApi,
            issueDiscovery,
            environment,
            loggerFactory.CreateLogger<SyncFromGitHubCommand>());

        // Act
        var exitCode = await command.ExecuteAsync(null, CancellationToken.None);

        // Assert
        Assert.That(exitCode, Is.EqualTo(0), "Command should succeed");

        var metadataPath = Path.Combine(dataDir, "issues_metadata.json");
        Assert.That(File.Exists(metadataPath), Is.True, "Metadata file should be created");

        var json = await File.ReadAllTextAsync(metadataPath);
        var metadata = JsonSerializer.Deserialize<List<IssueMetadata>>(json);
        Assert.That(metadata, Is.Not.Null);
        Assert.That(metadata!.Count, Is.EqualTo(1));
        Assert.That(metadata[0].Number, Is.EqualTo(123));
        Assert.That(metadata[0].Title, Is.EqualTo("Test Issue"));
    }

    [Test]
    public async Task SyncFromGitHub_WithMissingMetadataFilter_OnlySyncsIssuesWithoutMetadata()
    {
        // Arrange
        var toolsDir = Path.Combine(_tempDir!, "Tools");
        Directory.CreateDirectory(toolsDir);
        var dataDir = Path.Combine(_tempDir!, ".nunit", "IssueRunner");
        Directory.CreateDirectory(dataDir);

        Directory.CreateDirectory(Path.Combine(_tempDir!, "Issue123"));
        Directory.CreateDirectory(Path.Combine(_tempDir!, "Issue456"));

        // Create existing metadata file with issue 456
        var existingMetadata = new List<IssueMetadata>
        {
            new IssueMetadata
            {
                Number = 456,
                Title = "Existing Issue",
                State = "closed",
                Milestone = "No milestone",
                Labels = new List<string>(),
                Url = "https://github.com/test/test/issues/456"
            }
        };
        var existingJson = JsonSerializer.Serialize(existingMetadata, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(Path.Combine(dataDir, "issues_metadata.json"), existingJson);

        var environment = Substitute.For<IEnvironmentService>();
        environment.Root.Returns(_tempDir!);
        environment.RepositoryConfig.Returns(new RepositoryConfig("test", "test"));
        environment.GetDataDirectory(Arg.Any<string>())
            .Returns(callInfo => dataDir);

        var githubApi = Substitute.For<IGitHubApiService>();
        githubApi.FetchIssueMetadataAsync(123, Arg.Any<CancellationToken>())
            .Returns(new IssueMetadata
            {
                Number = 123,
                Title = "New Issue",
                State = "open",
                Milestone = "No milestone",
                Labels = new List<string>(),
                Url = "https://github.com/test/test/issues/123"
            });

        var issueDiscovery = Substitute.For<IIssueDiscoveryService>();
        issueDiscovery.DiscoverIssueFolders().Returns(new Dictionary<int, string>
        {
            { 123, Path.Combine(_tempDir!, "Issue123") },
            { 456, Path.Combine(_tempDir!, "Issue456") }
        });

        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Warning));
        var command = new SyncFromGitHubCommand(
            githubApi,
            issueDiscovery,
            environment,
            loggerFactory.CreateLogger<SyncFromGitHubCommand>());

        // Act
        var exitCode = await command.ExecuteAsync(null, CancellationToken.None, null, "MissingMetadata", false);

        // Assert
        Assert.That(exitCode, Is.EqualTo(0), "Command should succeed");

        // Verify that only issue 123 was fetched (456 already has metadata)
        await githubApi.Received(1).FetchIssueMetadataAsync(123, Arg.Any<CancellationToken>());
        await githubApi.DidNotReceive().FetchIssueMetadataAsync(456, Arg.Any<CancellationToken>());

        // Verify final metadata file contains both issues
        // In MissingMetadata mode, existing metadata should be preserved
        var metadataPath = Path.Combine(dataDir, "issues_metadata.json");
        var json = await File.ReadAllTextAsync(metadataPath);
        var metadata = JsonSerializer.Deserialize<List<IssueMetadata>>(json);
        Assert.That(metadata, Is.Not.Null);
        // The command should preserve existing metadata and add new metadata
        Assert.That(metadata!.Count, Is.EqualTo(2), "Both issues should be in metadata");
        Assert.That(metadata.Any(m => m.Number == 123), Is.True, "Newly synced issue 123 should be in metadata");
        var issue456 = metadata.FirstOrDefault(m => m.Number == 456);
        Assert.That(issue456, Is.Not.Null, "Existing issue 456 should be preserved");
        Assert.That(issue456!.Title, Is.EqualTo("Existing Issue"));
    }

    [Test]
    public async Task SyncFromGitHub_WithUpdateExisting_UpdatesExistingMetadata()
    {
        // Arrange
        var toolsDir = Path.Combine(_tempDir!, "Tools");
        Directory.CreateDirectory(toolsDir);
        var dataDir = Path.Combine(_tempDir!, ".nunit", "IssueRunner");
        Directory.CreateDirectory(dataDir);

        Directory.CreateDirectory(Path.Combine(_tempDir!, "Issue123"));

        // Create existing metadata file with issue 123 in "open" state
        var existingMetadata = new List<IssueMetadata>
        {
            new IssueMetadata
            {
                Number = 123,
                Title = "Old Title",
                State = "open",
                Milestone = "No milestone",
                Labels = new List<string> { "old-label" },
                Url = "https://github.com/test/test/issues/123"
            }
        };
        var existingJson = JsonSerializer.Serialize(existingMetadata, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(Path.Combine(dataDir, "issues_metadata.json"), existingJson);

        var environment = Substitute.For<IEnvironmentService>();
        environment.Root.Returns(_tempDir!);
        environment.RepositoryConfig.Returns(new RepositoryConfig("test", "test"));
        environment.GetDataDirectory(Arg.Any<string>())
            .Returns(callInfo => dataDir);

        // Mock API to return updated metadata (issue is now "closed")
        var githubApi = Substitute.For<IGitHubApiService>();
        githubApi.FetchIssueMetadataAsync(123, Arg.Any<CancellationToken>())
            .Returns(new IssueMetadata
            {
                Number = 123,
                Title = "Updated Title",
                State = "closed",
                Milestone = "No milestone",
                Labels = new List<string> { "new-label" },
                Url = "https://github.com/test/test/issues/123"
            });

        var issueDiscovery = Substitute.For<IIssueDiscoveryService>();
        issueDiscovery.DiscoverIssueFolders().Returns(new Dictionary<int, string>
        {
            { 123, Path.Combine(_tempDir!, "Issue123") }
        });

        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Warning));
        var command = new SyncFromGitHubCommand(
            githubApi,
            issueDiscovery,
            environment,
            loggerFactory.CreateLogger<SyncFromGitHubCommand>());

        // Act
        var exitCode = await command.ExecuteAsync(null, CancellationToken.None, null, "All", true);

        // Assert
        Assert.That(exitCode, Is.EqualTo(0), "Command should succeed");

        // Verify that issue 123 was fetched (because UpdateExisting is true)
        await githubApi.Received(1).FetchIssueMetadataAsync(123, Arg.Any<CancellationToken>());

        // Verify final metadata file contains updated metadata
        var metadataPath = Path.Combine(dataDir, "issues_metadata.json");
        var json = await File.ReadAllTextAsync(metadataPath);
        var metadata = JsonSerializer.Deserialize<List<IssueMetadata>>(json);
        Assert.That(metadata, Is.Not.Null);
        Assert.That(metadata!.Count, Is.EqualTo(1));
        Assert.That(metadata[0].Number, Is.EqualTo(123));
        Assert.That(metadata[0].Title, Is.EqualTo("Updated Title"));
        Assert.That(metadata[0].State, Is.EqualTo("closed"));
        Assert.That(metadata[0].Labels, Contains.Item("new-label"));
    }
}
