using IssueRunner.Models;
using IssueRunner.Services;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace IssueRunner.Commands;

/// <summary>
/// Command to sync metadata from GitHub to central file.
/// </summary>
public sealed class SyncFromGitHubCommand
{
    private readonly IGitHubApiService _githubApi;
    private readonly IIssueDiscoveryService _issueDiscovery;
    private readonly IEnvironmentService _environmentService;
    private readonly ILogger<SyncFromGitHubCommand> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="SyncFromGitHubCommand"/> class.
    /// </summary>
    public SyncFromGitHubCommand(
        IGitHubApiService githubApi,
        IIssueDiscoveryService issueDiscovery,
        IEnvironmentService environmentService,
        ILogger<SyncFromGitHubCommand> logger)
    {
        _githubApi = githubApi;
        _issueDiscovery = issueDiscovery;
        _environmentService = environmentService;
        _logger = logger;
    }

    /// <summary>
    /// Executes the command.
    /// </summary>
    /// <param name="outputPath">Path to write the central metadata file.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Exit code (0 for success).</returns>
    public async Task<int> ExecuteAsync(string? outputPath, CancellationToken cancellationToken)
    {
        try
        {
            string repositoryRoot = _environmentService.Root;
            var repositoryConfig = _environmentService.RepositoryConfig;
            
            var issueFolders = _issueDiscovery.DiscoverIssueFolders();
            var issueNumbers = issueFolders.Keys.OrderBy(n => n).ToList();

            var successCount = 0;
            var failCount = 0;
            var metadata = new List<IssueMetadata>();

            foreach (var issueNumber in issueNumbers)
            {
                var result = await _githubApi.FetchIssueMetadataAsync(
                    issueNumber,
                    cancellationToken);
                
                if (result != null)
                {
                    metadata.Add(result);
                    Console.WriteLine($"[{issueNumber}]: Synced - {result.Title}");
                    successCount++;
                }
                else
                {
                    Console.WriteLine($"[{issueNumber}]: Failed");
                    Console.WriteLine($"  Issue not found in repository {repositoryConfig}");
                    failCount++;
                }
                
                await Task.Delay(100, cancellationToken);
            }

            Console.WriteLine();
            Console.WriteLine($"Sync complete: {successCount} succeeded, {failCount} failed");

            if (metadata.Count > 0)
            {
                outputPath ??= Path.Combine(
                    repositoryRoot,
                    "Tools",
                    "issues_metadata.json");

                await WriteMetadataFileAsync(outputPath, metadata, cancellationToken);
                Console.WriteLine($"Wrote metadata to {Path.GetFileName(outputPath)}");
            }

            return failCount > 0 ? 1 : 0;
        }
        catch (FileNotFoundException)
        {
            return 1;
        }
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
