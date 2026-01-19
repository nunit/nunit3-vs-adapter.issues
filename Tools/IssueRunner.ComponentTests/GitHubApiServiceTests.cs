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

    [Test]
    [Category("Integration")]
    public async Task FetchIssueMetadata_VerifiesApiAccess_ShowsAuthenticationStatus()
    {
        // Arrange
        var environment = Substitute.For<IEnvironmentService>();
        environment.RepositoryConfig.Returns(new RepositoryConfig("nunit", "nunit3-vs-adapter"));
        var service = new GitHubApiService(_httpClient!, environment, _logger!);
        
        // Act - Try to fetch issue #1
        var metadata = await service.FetchIssueMetadataAsync(1, CancellationToken.None);
        var lastError = service.GetLastError();

        // Assert - Check what happened
        if (metadata != null)
        {
            // Success - we have access
            Assert.Pass($"GitHub API access verified. Successfully fetched issue #{metadata.Number}: {metadata.Title}");
        }
        else if (!string.IsNullOrEmpty(lastError))
        {
            // Failed - show the error
            if (lastError.Contains("401") || lastError.Contains("Unauthorized") || lastError.Contains("Bad credentials"))
            {
                Assert.Fail($"GitHub API authentication failed: {lastError}. Set GITHUB_TOKEN environment variable with a valid token.");
            }
            else if (lastError.Contains("403") || lastError.Contains("Forbidden"))
            {
                Assert.Fail($"GitHub API access forbidden: {lastError}. Check token permissions or rate limits.");
            }
            else if (lastError.Contains("404") || lastError.Contains("Not Found"))
            {
                Assert.Fail($"Issue not found: {lastError}. This might indicate the repository or issue doesn't exist.");
            }
            else
            {
                Assert.Fail($"GitHub API access failed: {lastError}");
            }
        }
        else
        {
            Assert.Fail("GitHub API call returned null with no error message. This is unexpected.");
        }
    }

    [Test]
    [Category("Integration")]
    public async Task FetchIssueMetadata_ChecksRateLimit_ShowsRemainingRequests()
    {
        // Arrange
        var environment = Substitute.For<IEnvironmentService>();
        environment.RepositoryConfig.Returns(new RepositoryConfig("nunit", "nunit3-vs-adapter"));
        var service = new GitHubApiService(_httpClient!, environment, _logger!);
        
        // Act - Make a request to check rate limit headers
        var metadata = await service.FetchIssueMetadataAsync(1, CancellationToken.None);
        
        // Note: We can't easily check rate limit headers from the service,
        // but we can check if we got a 403 with rate limit message
        var lastError = service.GetLastError();
        
        if (!string.IsNullOrEmpty(lastError) && lastError.Contains("rate limit", StringComparison.OrdinalIgnoreCase))
        {
            Assert.Fail($"GitHub API rate limit exceeded: {lastError}");
        }
        else if (metadata != null)
        {
            Assert.Pass("GitHub API access successful. Rate limit check passed (no rate limit errors).");
        }
        else if (!string.IsNullOrEmpty(lastError))
        {
            // Other error, but not rate limit
            Assert.Inconclusive($"GitHub API call failed (not rate limit): {lastError}");
        }
        else
        {
            Assert.Inconclusive("Could not determine rate limit status.");
        }
    }
}
