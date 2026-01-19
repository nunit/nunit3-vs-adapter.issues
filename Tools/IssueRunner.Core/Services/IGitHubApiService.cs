using IssueRunner.Models;

namespace IssueRunner.Services;

/// <summary>
/// Service for fetching issue metadata from GitHub API.
/// </summary>
public interface IGitHubApiService
{
    /// <summary>
    /// Fetches issue metadata from GitHub for the specified issue number.
    /// </summary>
    /// <param name="issueNumber">The issue number to fetch.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Issue metadata or null if not found.</returns>
    Task<IssueMetadata?> FetchIssueMetadataAsync(
        int issueNumber,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Fetches metadata for multiple issues.
    /// </summary>
    /// <param name="issueNumbers">Issue numbers to fetch.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of successfully fetched issue metadata.</returns>
    Task<List<IssueMetadata>> FetchMultipleIssuesAsync(
        IEnumerable<int> issueNumbers,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the last error message from the most recent API call, if any.
    /// </summary>
    /// <returns>The last error message or null if no error occurred.</returns>
    string? GetLastError();
}

