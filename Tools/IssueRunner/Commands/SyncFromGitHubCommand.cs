using IssueRunner.Models;
using IssueRunner.Services;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace IssueRunner.Commands;

/// <summary>
/// Command to sync metadata from GitHub to central file.
/// </summary>
public sealed class SyncFromGitHubCommand
{
    private readonly IGitHubApiService _githubApi;
    private readonly IIssueDiscoveryService _issueDiscovery;
    private readonly ILogger<SyncFromGitHubCommand> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="SyncFromGitHubCommand"/> class.
    /// </summary>
    public SyncFromGitHubCommand(
        IGitHubApiService githubApi,
        IIssueDiscoveryService issueDiscovery,
        ILogger<SyncFromGitHubCommand> logger)
    {
        _githubApi = githubApi;
        _issueDiscovery = issueDiscovery;
        _logger = logger;
    }

    /// <summary>
    /// Executes the command.
    /// </summary>
    /// <param name="repositoryRoot">Root path of the repository.</param>
    /// <param name="outputPath">Path to write the central metadata file.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Exit code (0 for success).</returns>
    public async Task<int> ExecuteAsync(
        string repositoryRoot,
        string? outputPath,
        CancellationToken cancellationToken)
    {
        Console.WriteLine("Syncing issue metadata from GitHub...");

        var issueFolders = _issueDiscovery.DiscoverIssueFolders(repositoryRoot);
        var issueNumbers = issueFolders.Keys.OrderBy(n => n);

        Console.WriteLine($"Found {issueFolders.Count} issue folders, fetching metadata...");

        var metadata = await _githubApi.FetchMultipleIssuesAsync(
            issueNumbers,
            cancellationToken);

        outputPath ??= Path.Combine(
            repositoryRoot,
            "Tools",
            "issues_metadata.json");

        await WriteMetadataFileAsync(outputPath, metadata, cancellationToken);

        Console.WriteLine($"Successfully synced {metadata.Count} issues to {outputPath}");

        return 0;
    }

    private async Task WriteMetadataFileAsync(
        string outputPath,
        List<IssueMetadata> metadata,
        CancellationToken cancellationToken)
    {
        var directory = Path.GetDirectoryName(outputPath);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        var json = JsonSerializer.Serialize(metadata, options);
        await File.WriteAllTextAsync(outputPath, json, cancellationToken);
    }
}
