using IssueRunner.Models;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace IssueRunner.Services;

/// <summary>
/// Implementation of GitHub API service.
/// </summary>
public sealed class GitHubApiService : IGitHubApiService
{
    private const string ApiBaseUrl = "https://api.github.com";
    
    private readonly HttpClient _httpClient;
    private readonly IEnvironmentService _environmentService;
    private readonly ILogger<GitHubApiService> _logger;
    private RepositoryConfig RepositoryConfig { get; set; }
    private string? _lastError;

    /// <summary>
    /// Initializes a new instance of the <see cref="GitHubApiService"/> class.
    /// </summary>
    public GitHubApiService(
        HttpClient httpClient,
        IEnvironmentService environmentService,
        ILogger<GitHubApiService> logger)
    {
        _httpClient = httpClient;
        _environmentService = environmentService;
        _logger = logger;
        RepositoryConfig = environmentService.RepositoryConfig;
        ConfigureHttpClient();
    }

    /// <inheritdoc />
    public async Task<IssueMetadata?> FetchIssueMetadataAsync(
        int issueNumber,
        CancellationToken cancellationToken = default)
    {
        var url = $"{ApiBaseUrl}/repos/{RepositoryConfig.Owner}/{RepositoryConfig.Name}/issues/{issueNumber}";
        
        try
        {
            var response = await _httpClient.GetAsync(url, cancellationToken);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                _lastError = $"HTTP {(int)response.StatusCode} {response.StatusCode}: {errorContent}";
                _logger.LogWarning(
                    "Failed to fetch issue {IssueNumber}: {StatusCode} - {ErrorContent}",
                    issueNumber,
                    response.StatusCode,
                    errorContent);
                return null;
            }

            var githubIssue = await response.Content
                .ReadFromJsonAsync<GitHubIssueResponse>(cancellationToken);
            
            if (githubIssue == null)
            {
                return null;
            }

            _lastError = null; // Clear error on success
            return MapToIssueMetadata(githubIssue);
        }
        catch (Exception ex)
        {
            _lastError = $"Exception: {ex.Message}";
            _logger.LogWarning(
                ex,
                "Error fetching issue {IssueNumber}: {Message}",
                issueNumber,
                ex.Message);
            return null;
        }
    }

    /// <inheritdoc />
    public string? GetLastError() => _lastError;

    /// <inheritdoc />
    public async Task<List<IssueMetadata>> FetchMultipleIssuesAsync(
        IEnumerable<int> issueNumbers,
        CancellationToken cancellationToken = default)
    {
        var results = new List<IssueMetadata>();
        
        foreach (var issueNumber in issueNumbers)
        {
            var metadata = await FetchIssueMetadataAsync(
                issueNumber,
                cancellationToken);
            
            if (metadata != null)
            {
                results.Add(metadata);
            }
            
            await Task.Delay(100, cancellationToken);
        }
        
        return results;
    }

    private void ConfigureHttpClient()
    {
        _httpClient.DefaultRequestHeaders.UserAgent.Add(
            new ProductInfoHeaderValue("IssueRunner", "1.0"));
        
        var token = Environment.GetEnvironmentVariable("GITHUB_TOKEN");
        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }
    }

    private static IssueMetadata MapToIssueMetadata(
        GitHubIssueResponse response)
    {
        return new IssueMetadata
        {
            Number = response.Number,
            Title = response.Title,
            State = response.State,
            Milestone = response.Milestone?.Title ?? "No milestone",
            Labels = response.Labels.Select(l => l.Name).ToList(),
            Url = response.HtmlUrl
        };
    }

    private sealed class GitHubIssueResponse
    {
        [JsonPropertyName("number")]
        public required int Number { get; init; }

        [JsonPropertyName("title")]
        public required string Title { get; init; }

        [JsonPropertyName("state")]
        public required string State { get; init; }

        [JsonPropertyName("milestone")]
        public MilestoneResponse? Milestone { get; init; }

        [JsonPropertyName("labels")]
        public required List<LabelResponse> Labels { get; init; }

        [JsonPropertyName("html_url")]
        public required string HtmlUrl { get; init; }
    }

    private sealed class MilestoneResponse
    {
        [JsonPropertyName("title")]
        public required string Title { get; init; }
    }

    private sealed class LabelResponse
    {
        [JsonPropertyName("name")]
        public required string Name { get; init; }
    }
}

