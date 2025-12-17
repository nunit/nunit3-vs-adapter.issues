using IssueRunner.Models;
using IssueRunner.Services;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace IssueRunner.Commands;

/// <summary>
/// Repository configuration model.
/// </summary>
internal class RepositoryConfig
{
    [JsonPropertyName("owner")]
    public string? Owner { get; set; }
    
    [JsonPropertyName("name")]
    public string? Name { get; set; }
}

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
        try
        {
            // Load repository configuration
            var (owner, name) = LoadRepositoryConfig(repositoryRoot);
            Console.WriteLine($"Syncing from {owner}/{name}...");
            Console.WriteLine();
            
            // Configure the GitHub API service with the repository
            _githubApi.SetRepository(owner, name);

            var issueFolders = _issueDiscovery.DiscoverIssueFolders(repositoryRoot);
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
                    Console.WriteLine($"  Issue not found in repository {owner}/{name}");
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

    private (string owner, string name) LoadRepositoryConfig(string repositoryRoot)
    {
        // Try Tools/repository.json first, then root/repository.json
        var configPath = Path.Combine(repositoryRoot, "Tools", "repository.json");
        if (!File.Exists(configPath))
        {
            configPath = Path.Combine(repositoryRoot, "repository.json");
        }
        
        if (!File.Exists(configPath))
        {
            Console.WriteLine("ERROR: Repository configuration file not found");
            Console.WriteLine();
            Console.WriteLine("Create repository.json at one of these locations:");
            Console.WriteLine($"  - {Path.Combine(repositoryRoot, "Tools", "repository.json")}");
            Console.WriteLine($"  - {Path.Combine(repositoryRoot, "repository.json")}");
            Console.WriteLine();
            Console.WriteLine("Content should be:");
            Console.WriteLine("{");
            Console.WriteLine("  \"owner\": \"nunit\",");
            Console.WriteLine("  \"name\": \"nunit\"");
            Console.WriteLine("}");
            throw new FileNotFoundException("Repository configuration file (repository.json) is required");
        }

        try
        {
            var json = File.ReadAllText(configPath);
            var config = JsonSerializer.Deserialize<RepositoryConfig>(json);
            
            if (config?.Owner == null || config?.Name == null)
            {
                throw new InvalidOperationException("Invalid repository.json: owner and name are required");
            }

            return (config.Owner, config.Name);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to parse repository config from {Path}", configPath);
            Console.WriteLine($"ERROR: Invalid JSON in {configPath}");
            Console.WriteLine($"Details: {ex.Message}");
            throw;
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
