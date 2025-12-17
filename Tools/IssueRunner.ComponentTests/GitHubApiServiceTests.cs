using IssueRunner.Models;
using IssueRunner.Services;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

namespace IssueRunner.ComponentTests;

[TestFixture]
public class GitHubApiServiceTests
{
    private HttpClient? _httpClient;
    private ILogger<GitHubApiService>? _logger;

    [SetUp]
    public void Setup()
    {
        _httpClient = new HttpClient();
        _logger = LoggerFactory.Create(builder => builder.AddConsole())
            .CreateLogger<GitHubApiService>();
    }

    [TearDown]
    public void TearDown()
    {
        _httpClient?.Dispose();
    }

    [Test]
    [Category("Integration")]
    public async Task FetchIssueMetadata_WithValidIssueNumber_ReturnsMetadata()
    {
        // Arrange
        var environment = Substitute.For<IEnvironmentService>();
        environment.RepositoryConfig.Returns(new RepositoryConfig("nunit", "nunit3-vs-adapter"));
        var service = new GitHubApiService(_httpClient!, environment, _logger!);
        
        // Act - Use issue #1 which should always exist
        var metadata = await service.FetchIssueMetadataAsync(1, CancellationToken.None);

        // Assert
        Assert.That(metadata, Is.Not.Null);
        Assert.That(metadata!.Number, Is.EqualTo(1));
        Assert.That(metadata.Title, Is.Not.Null.And.Not.Empty);
        Assert.That(metadata.State, Is.Not.Null.And.Not.Empty);
        
        Console.WriteLine($"Fetched issue #{metadata.Number}: {metadata.Title}");
        Console.WriteLine($"State: {metadata.State}");
        Console.WriteLine($"Labels: {string.Join(", ", metadata.Labels)}");
    }

    [Test]
    [Category("Integration")]
    public async Task FetchIssueMetadata_WithDifferentRepository_ReturnsMetadata()
    {
        // Arrange
        var environment = Substitute.For<IEnvironmentService>();
        environment.RepositoryConfig.Returns(new RepositoryConfig("nunit", "nunit3-vs-adapter"));
        var service = new GitHubApiService(_httpClient!, environment, _logger!);
        
        // Act - Use issue #1 from nunit/nunit
        var metadata = await service.FetchIssueMetadataAsync(1, CancellationToken.None);

        // Assert
        Assert.That(metadata, Is.Not.Null);
        Assert.That(metadata!.Number, Is.EqualTo(1));
        Assert.That(metadata.Title, Is.Not.Null.And.Not.Empty);
        
        Console.WriteLine($"Fetched issue #{metadata.Number} from nunit/nunit: {metadata.Title}");
    }

    [Test]
    [Category("Integration")]
    public async Task FetchIssueMetadata_WithInvalidIssueNumber_ReturnsNull()
    {
        // Arrange
        var environment = Substitute.For<IEnvironmentService>();
        environment.RepositoryConfig.Returns(new RepositoryConfig("nunit", "nunit3-vs-adapter"));
        var service = new GitHubApiService(_httpClient!, environment, _logger!);
        
        // Act - Use a very high issue number that shouldn't exist
        var metadata = await service.FetchIssueMetadataAsync(999999, CancellationToken.None);

        // Assert
        Assert.That(metadata, Is.Null);
    }

    [Test]
    [Category("Integration")]
    public async Task FetchMultipleIssues_ReturnsMetadataList()
    {
        // Arrange
        var environment = Substitute.For<IEnvironmentService>();
        environment.RepositoryConfig.Returns(new RepositoryConfig("nunit", "nunit3-vs-adapter"));
        var service = new GitHubApiService(_httpClient!, environment, _logger!);
        var issueNumbers = new[] { 1, 2, 3 };
        
        // Act
        var metadataList = await service.FetchMultipleIssuesAsync(issueNumbers, CancellationToken.None);

        // Assert
        Assert.That(metadataList, Is.Not.Null);
        Assert.That(metadataList.Count, Is.GreaterThan(0));
        
        foreach (var metadata in metadataList)
        {
            Console.WriteLine($"Fetched issue #{metadata.Number}: {metadata.Title}");
        }
    }
}
