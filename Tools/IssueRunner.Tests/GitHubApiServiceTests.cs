using IssueRunner.Models;
using IssueRunner.Services;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace IssueRunner.Tests;

[TestFixture]
public class GitHubApiServiceTests
{
    private ILogger<GitHubApiService> _logger = null!;
    private IEnvironmentService _environmentService = null!;
    private HttpClient _httpClient = null!;
    private TestHttpMessageHandler _messageHandler = null!;

    [SetUp]
    public void SetUp()
    {
        _logger = Substitute.For<ILogger<GitHubApiService>>();
        _environmentService = Substitute.For<IEnvironmentService>();
        _environmentService.RepositoryConfig.Returns(new RepositoryConfig("testowner", "testrepo"));
        _messageHandler = new TestHttpMessageHandler();
        _httpClient = new HttpClient(_messageHandler);
    }

    [TearDown]
    public void TearDown()
    {
        _httpClient?.Dispose();
    }

    [Test]
    public async Task FetchIssueMetadataAsync_ReturnsMetadata_WhenRequestSucceeds()
    {
        // Arrange
        var responseJson = JsonSerializer.Serialize(new
        {
            number = 228,
            title = "Test Issue Title",
            state = "open",
            milestone = (object?)null,
            labels = new[] { new { name = "bug" } },
            html_url = "https://github.com/testowner/testrepo/issues/228"
        });
        _messageHandler.SetResponse(new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        });

        var service = new GitHubApiService(_httpClient, _environmentService, _logger);

        // Act
        var result = await service.FetchIssueMetadataAsync(228);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Number, Is.EqualTo(228));
        Assert.That(result.Title, Is.EqualTo("Test Issue Title"));
        Assert.That(result.State, Is.EqualTo("open"));
        Assert.That(result.Labels, Contains.Item("bug"));
        Assert.That(result.Url, Is.EqualTo("https://github.com/testowner/testrepo/issues/228"));
        Assert.That(service.GetLastError(), Is.Null, "No error should be set on success");
    }

    [Test]
    public async Task FetchIssueMetadataAsync_ReturnsNull_WhenIssueNotFound()
    {
        // Arrange
        _messageHandler.SetResponse(new HttpResponseMessage(HttpStatusCode.NotFound)
        {
            Content = new StringContent("Not Found", Encoding.UTF8, "text/plain")
        });

        var service = new GitHubApiService(_httpClient, _environmentService, _logger);

        // Act
        var result = await service.FetchIssueMetadataAsync(999);

        // Assert
        Assert.That(result, Is.Null);
        Assert.That(service.GetLastError(), Is.Not.Null, "Error should be set on failure");
        Assert.That(service.GetLastError(), Does.Contain("404"));
    }

    [Test]
    public async Task FetchIssueMetadataAsync_ReturnsNull_WhenRequestFails()
    {
        // Arrange
        _messageHandler.SetResponse(new HttpResponseMessage(HttpStatusCode.InternalServerError)
        {
            Content = new StringContent("Server Error", Encoding.UTF8, "text/plain")
        });

        var service = new GitHubApiService(_httpClient, _environmentService, _logger);

        // Act
        var result = await service.FetchIssueMetadataAsync(228);

        // Assert
        Assert.That(result, Is.Null);
        Assert.That(service.GetLastError(), Is.Not.Null);
        Assert.That(service.GetLastError(), Does.Contain("500"));
    }

    [Test]
    public async Task FetchIssueMetadataAsync_HandlesMilestone()
    {
        // Arrange
        var responseJson = JsonSerializer.Serialize(new
        {
            number = 1,
            title = "Issue with milestone",
            state = "closed",
            milestone = new { title = "v1.0" },
            labels = Array.Empty<object>(),
            html_url = "https://github.com/testowner/testrepo/issues/1"
        });
        _messageHandler.SetResponse(new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        });

        var service = new GitHubApiService(_httpClient, _environmentService, _logger);

        // Act
        var result = await service.FetchIssueMetadataAsync(1);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Milestone, Is.EqualTo("v1.0"));
    }

    [Test]
    public async Task FetchIssueMetadataAsync_SetsMilestoneToNoMilestone_WhenNull()
    {
        // Arrange
        var responseJson = JsonSerializer.Serialize(new
        {
            number = 1,
            title = "Issue without milestone",
            state = "open",
            milestone = (object?)null,
            labels = Array.Empty<object>(),
            html_url = "https://github.com/testowner/testrepo/issues/1"
        });
        _messageHandler.SetResponse(new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        });

        var service = new GitHubApiService(_httpClient, _environmentService, _logger);

        // Act
        var result = await service.FetchIssueMetadataAsync(1);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Milestone, Is.EqualTo("No milestone"));
    }

    [Test]
    public async Task FetchIssueMetadataAsync_HandlesMultipleLabels()
    {
        // Arrange
        var responseJson = JsonSerializer.Serialize(new
        {
            number = 1,
            title = "Issue with labels",
            state = "open",
            milestone = (object?)null,
            labels = new[]
            {
                new { name = "bug" },
                new { name = "priority:high" },
                new { name = "area:testing" }
            },
            html_url = "https://github.com/testowner/testrepo/issues/1"
        });
        _messageHandler.SetResponse(new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        });

        var service = new GitHubApiService(_httpClient, _environmentService, _logger);

        // Act
        var result = await service.FetchIssueMetadataAsync(1);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Labels, Has.Count.EqualTo(3));
        Assert.That(result.Labels, Contains.Item("bug"));
        Assert.That(result.Labels, Contains.Item("priority:high"));
        Assert.That(result.Labels, Contains.Item("area:testing"));
    }

    [Test]
    public async Task FetchIssueMetadataAsync_HandlesHttpException()
    {
        // Arrange
        _messageHandler.SetException(new HttpRequestException("Network error"));

        var service = new GitHubApiService(_httpClient, _environmentService, _logger);

        // Act
        var result = await service.FetchIssueMetadataAsync(228);

        // Assert
        Assert.That(result, Is.Null);
        Assert.That(service.GetLastError(), Is.Not.Null);
        Assert.That(service.GetLastError(), Does.Contain("Exception"));
    }

    [Test]
    public async Task FetchMultipleIssuesAsync_FetchesAllIssues()
    {
        // Arrange
        var issueNumbers = new[] { 1, 2, 3 };
        var responses = new Dictionary<int, string>
        {
            { 1, JsonSerializer.Serialize(new { number = 1, title = "Issue 1", state = "open", milestone = (object?)null, labels = Array.Empty<object>(), html_url = "https://github.com/testowner/testrepo/issues/1" }) },
            { 2, JsonSerializer.Serialize(new { number = 2, title = "Issue 2", state = "closed", milestone = (object?)null, labels = Array.Empty<object>(), html_url = "https://github.com/testowner/testrepo/issues/2" }) },
            { 3, JsonSerializer.Serialize(new { number = 3, title = "Issue 3", state = "open", milestone = (object?)null, labels = Array.Empty<object>(), html_url = "https://github.com/testowner/testrepo/issues/3" }) }
        };

        _messageHandler.SetResponses(issueNumbers.Select(num => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responses[num], Encoding.UTF8, "application/json")
        }));

        var service = new GitHubApiService(_httpClient, _environmentService, _logger);

        // Act
        var results = await service.FetchMultipleIssuesAsync(issueNumbers);

        // Assert
        Assert.That(results, Has.Count.EqualTo(3));
        Assert.That(results.Select(r => r.Number), Is.EquivalentTo(new[] { 1, 2, 3 }));
    }

    [Test]
    public async Task FetchMultipleIssuesAsync_SkipsFailedIssues()
    {
        // Arrange
        var issueNumbers = new[] { 1, 2, 3 };
        _messageHandler.SetResponses(new[]
        {
            new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonSerializer.Serialize(new { number = 1, title = "Issue 1", state = "open", milestone = (object?)null, labels = Array.Empty<object>(), html_url = "https://github.com/testowner/testrepo/issues/1" }), Encoding.UTF8, "application/json")
            },
            new HttpResponseMessage(HttpStatusCode.NotFound),
            new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonSerializer.Serialize(new { number = 3, title = "Issue 3", state = "open", milestone = (object?)null, labels = Array.Empty<object>(), html_url = "https://github.com/testowner/testrepo/issues/3" }), Encoding.UTF8, "application/json")
            }
        });

        var service = new GitHubApiService(_httpClient, _environmentService, _logger);

        // Act
        var results = await service.FetchMultipleIssuesAsync(issueNumbers);

        // Assert
        Assert.That(results, Has.Count.EqualTo(2));
        Assert.That(results.Select(r => r.Number), Is.EquivalentTo(new[] { 1, 3 }));
    }

    [Test]
    public async Task FetchIssueMetadataAsync_UsesCorrectApiUrl()
    {
        // Arrange
        _environmentService.RepositoryConfig.Returns(new RepositoryConfig("nunit", "nunit3-vs-adapter"));
        var responseJson = JsonSerializer.Serialize(new
        {
            number = 228,
            title = "Test",
            state = "open",
            milestone = (object?)null,
            labels = Array.Empty<object>(),
            html_url = "https://github.com/nunit/nunit3-vs-adapter/issues/228"
        });
        _messageHandler.SetResponse(new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        });

        var service = new GitHubApiService(_httpClient, _environmentService, _logger);

        // Act
        await service.FetchIssueMetadataAsync(228);

        // Assert
        var request = _messageHandler.LastRequest;
        Assert.That(request, Is.Not.Null);
        Assert.That(request!.RequestUri!.ToString(), Does.Contain("nunit/nunit3-vs-adapter"));
        Assert.That(request.RequestUri.ToString(), Does.Contain("issues/228"));
    }

    private class TestHttpMessageHandler : HttpMessageHandler
    {
        private readonly Queue<HttpResponseMessage> _responses = new();
        private Exception? _exception;
        public HttpRequestMessage? LastRequest { get; private set; }

        public void SetResponse(HttpResponseMessage response)
        {
            _responses.Clear();
            _responses.Enqueue(response);
        }

        public void SetResponses(IEnumerable<HttpResponseMessage> responses)
        {
            _responses.Clear();
            foreach (var response in responses)
            {
                _responses.Enqueue(response);
            }
        }

        public void SetException(Exception exception)
        {
            _exception = exception;
        }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            LastRequest = request;

            if (_exception != null)
            {
                throw _exception;
            }

            if (_responses.Count == 0)
            {
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound));
            }

            return Task.FromResult(_responses.Dequeue());
        }
    }
}
