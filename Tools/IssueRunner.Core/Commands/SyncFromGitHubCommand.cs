using IssueRunner.Models;
using IssueRunner.Services;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace IssueRunner.Commands;

/// <summary>
/// Progress callback for sync operations.
/// </summary>
/// <param name="issueNumber">The issue number being processed.</param>
/// <param name="found">Whether the issue was found on GitHub.</param>
/// <param name="synced">Whether the issue was successfully synced.</param>
/// <param name="totalProcessed">Total number of issues processed so far.</param>
/// <param name="totalIssues">Total number of issues to process.</param>
public delegate void SyncProgressCallback(int issueNumber, bool found, bool synced, int totalProcessed, int totalIssues);

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
    /// <param name="progressCallback">Optional progress callback.</param>
    /// <param name="syncMode">Sync mode: "All" or "MissingMetadata".</param>
    /// <param name="updateExisting">Whether to update existing metadata.</param>
    /// <returns>Exit code (0 for success).</returns>
    public async Task<int> ExecuteAsync(
        string? outputPath,
        CancellationToken cancellationToken,
        SyncProgressCallback? progressCallback = null,
        string syncMode = "All",
        bool updateExisting = false)
    {
        try
        {
            string repositoryRoot = _environmentService.Root;
            var repositoryConfig = _environmentService.RepositoryConfig;
            
            // Load existing metadata to check which issues already have metadata
            var dataDir = _environmentService.GetDataDirectory(repositoryRoot);
            var existingMetadataPath = Path.Combine(dataDir, "issues_metadata.json");
            var existingMetadata = new Dictionary<int, IssueMetadata>();
            if (File.Exists(existingMetadataPath))
            {
                try
                {
                    var existingJson = await File.ReadAllTextAsync(existingMetadataPath, cancellationToken);
                    var existingList = JsonSerializer.Deserialize<List<IssueMetadata>>(existingJson) ?? [];
                    
                    // Handle duplicate issue numbers gracefully
                    var duplicates = existingList.GroupBy(m => m.Number).Where(g => g.Count() > 1).ToList();
                    if (duplicates.Any())
                    {
                        foreach (var dup in duplicates)
                        {
                            _logger.LogWarning("Duplicate metadata entries found for issue {IssueNumber}. Using the last occurrence.", dup.Key);
                        }
                    }
                    
                    // Use GroupBy().ToDictionary() to take the last occurrence of each duplicate
                    existingMetadata = existingList
                        .GroupBy(m => m.Number)
                        .ToDictionary(g => g.Key, g => g.Last());
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to load existing metadata file");
                }
            }
            
            var issueFolders = _issueDiscovery.DiscoverIssueFolders();
            var allIssueNumbers = issueFolders.Keys.OrderBy(n => n).ToList();
            
            // Filter issues based on sync mode
            var issueNumbers = new List<int>();
            if (syncMode == "MissingMetadata")
            {
                // Only sync issues that don't have metadata
                issueNumbers = allIssueNumbers.Where(n => !existingMetadata.ContainsKey(n)).ToList();
            }
            else
            {
                // Sync all issues
                issueNumbers = allIssueNumbers;
            }

            var successCount = 0;
            var failCount = 0;
            var foundCount = 0;
            var notFoundCount = 0;
            // Start with existing metadata to preserve issues not being synced
            var metadata = new List<IssueMetadata>(existingMetadata.Values);
            var totalIssues = issueNumbers.Count;
            var processedCount = 0;

            foreach (var issueNumber in issueNumbers)
            {
                cancellationToken.ThrowIfCancellationRequested();
                
                // Check if we should skip this issue (has metadata and updateExisting is false)
                if (!updateExisting && existingMetadata.ContainsKey(issueNumber))
                {
                    // Skip existing metadata (it's already in the list from initialization)
                    processedCount++;
                    progressCallback?.Invoke(issueNumber, true, true, processedCount, totalIssues);
                    continue;
                }
                
                // Remove existing metadata for this issue if we're updating it
                if (existingMetadata.ContainsKey(issueNumber))
                {
                    metadata.RemoveAll(m => m.Number == issueNumber);
                }
                
                try
                {
                    var result = await _githubApi.FetchIssueMetadataAsync(
                        issueNumber,
                        cancellationToken);
                    
                    if (result != null)
                    {
                        metadata.Add(result);
                        Console.WriteLine($"[{issueNumber}]: Synced - {result.Title}");
                        successCount++;
                        foundCount++;
                        processedCount++;
                        progressCallback?.Invoke(issueNumber, true, true, processedCount, totalIssues);
                    }
                    else
                    {
                        // Issue not found on GitHub - get the actual error details
                        var errorMsg = _githubApi.GetLastError();
                        var errorDetails = string.IsNullOrEmpty(errorMsg) 
                            ? "Check logs for details (authentication, rate limit, or issue not found)" 
                            : errorMsg;
                        
                        if (existingMetadata.TryGetValue(issueNumber, out var existing))
                        {
                            metadata.Add(existing);
                            Console.WriteLine($"[{issueNumber}]: Not found on GitHub ({errorDetails}), keeping existing metadata");
                        }
                        else
                        {
                            Console.WriteLine($"[{issueNumber}]: Failed - {errorDetails}");
                            notFoundCount++;
                        }
                        failCount++;
                        processedCount++;
                        progressCallback?.Invoke(issueNumber, false, false, processedCount, totalIssues);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error syncing issue {IssueNumber}", issueNumber);
                    Console.WriteLine($"[{issueNumber}]: Exception - {ex.Message}");
                    if (ex.InnerException != null)
                    {
                        Console.WriteLine($"[{issueNumber}]: Inner exception - {ex.InnerException.Message}");
                    }
                    // Keep existing metadata if available
                    if (existingMetadata.TryGetValue(issueNumber, out var existing))
                    {
                        metadata.Add(existing);
                    }
                    failCount++;
                    notFoundCount++;
                    processedCount++;
                    progressCallback?.Invoke(issueNumber, false, false, processedCount, totalIssues);
                }
                
                await Task.Delay(100, cancellationToken);
            }

            Console.WriteLine();
            if (syncMode == "MissingMetadata")
            {
                Console.WriteLine($"Missing Metadata sync complete: {successCount} succeeded, {failCount} failed, {foundCount} found, {notFoundCount} not found");
                Console.WriteLine($"Total issues in repository: {allIssueNumbers.Count}, Issues with metadata before sync: {existingMetadata.Count}, Issues synced: {successCount}");
            }
            else
            {
                Console.WriteLine($"Sync complete: {successCount} succeeded, {failCount} failed, {foundCount} found, {notFoundCount} not found");
            }

            if (metadata.Count > 0)
            {
                outputPath ??= Path.Combine(dataDir, "issues_metadata.json");

                await WriteMetadataFileAsync(outputPath, metadata, cancellationToken);
                Console.WriteLine($"Wrote metadata to {Path.GetFileName(outputPath)}");
            }

            return failCount > 0 ? 1 : 0;
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Sync cancelled by user");
            return 1;
        }
        catch (FileNotFoundException)
        {
            return 1;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during sync operation");
            Console.WriteLine($"Error: {ex.Message}");
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

