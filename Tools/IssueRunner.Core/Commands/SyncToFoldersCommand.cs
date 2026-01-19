using IssueRunner.Models;
using IssueRunner.Services;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace IssueRunner.Commands;

/// <summary>
/// Command to sync metadata from central file to issue folders.
/// </summary>
public sealed class SyncToFoldersCommand
{
    private readonly IIssueDiscoveryService _issueDiscovery;
    private readonly IProjectAnalyzerService _projectAnalyzer;
    private readonly ILogger<SyncToFoldersCommand> _logger;
    private readonly IEnvironmentService _environmentService;

    /// <summary>
    /// Initializes a new instance of the <see cref="SyncToFoldersCommand"/> class.
    /// </summary>
    public SyncToFoldersCommand(
        IIssueDiscoveryService issueDiscovery,
        IProjectAnalyzerService projectAnalyzer,
        ILogger<SyncToFoldersCommand> logger,
        IEnvironmentService environmentService)
    {
        _issueDiscovery = issueDiscovery;
        _projectAnalyzer = projectAnalyzer;
        _logger = logger;
        _environmentService = environmentService;
    }

    /// <summary>
    /// Executes the command.
    /// </summary>
    /// <param name="repositoryRoot">Root path of the repository.</param>
    /// <param name="centralMetadataPath">Path to the central metadata file.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Exit code (0 for success).</returns>
    public async Task<int> ExecuteAsync(
        string repositoryRoot,
        string? centralMetadataPath,
        CancellationToken cancellationToken)
    {
        Console.WriteLine("Syncing metadata to issue folders...");
        Console.WriteLine();

        centralMetadataPath ??= Path.Combine(
            _environmentService.GetDataDirectory(repositoryRoot),
            "issues_metadata.json");

        if (!File.Exists(centralMetadataPath))
        {
            Console.WriteLine($"ERROR: Central metadata file not found: {centralMetadataPath}");
            return 1;
        }

        var centralMetadata = await LoadCentralMetadataAsync(
            centralMetadataPath,
            cancellationToken);

        var issueFolders = _issueDiscovery.DiscoverIssueFolders();

        var successCount = 0;
        var skippedCount = 0;

        foreach (var (issueNumber, folderPath) in issueFolders.OrderBy(kvp => kvp.Key))
        {
            if (!centralMetadata.TryGetValue(issueNumber, out var metadata))
            {
                Console.WriteLine($"[{issueNumber}]: Skipped");
                Console.WriteLine($"  No metadata found in central file");
                skippedCount++;
                continue;
            }

            var projectCount = await ProcessIssueFolderAsync(
                folderPath,
                metadata,
                cancellationToken);

            Console.WriteLine($"[{issueNumber}]: Updated - {metadata.Title}");
            if (projectCount > 1)
            {
                Console.WriteLine($"  {projectCount} projects processed");
            }
            successCount++;
        }

        Console.WriteLine();
        Console.WriteLine($"Sync complete: {successCount} updated, {skippedCount} skipped");

        return 0;
    }

    private async Task<Dictionary<int, IssueMetadata>> LoadCentralMetadataAsync(
        string path,
        CancellationToken cancellationToken)
    {
        if (!File.Exists(path))
        {
            Console.WriteLine($"ERROR: Central metadata file not found: {path}");
            return new Dictionary<int, IssueMetadata>();
        }

        var json = await File.ReadAllTextAsync(path, cancellationToken);
        if (string.IsNullOrWhiteSpace(json))
            return new Dictionary<int, IssueMetadata>();
        var items = JsonSerializer.Deserialize<List<IssueMetadata>>(json)
                        ?? new List<IssueMetadata>();

        var map = new Dictionary<int, IssueMetadata>(items.Count);

        foreach (var item in items)
            map.TryAdd(item.Number, item);
        return map;
    }

    private async Task<int> ProcessIssueFolderAsync(
        string folderPath,
        IssueMetadata metadata,
        CancellationToken cancellationToken)
    {
        var projectFiles = _projectAnalyzer.FindProjectFiles(folderPath);
        var projectMetadataList = new List<IssueProjectMetadata>();

        foreach (var projectFile in projectFiles)
        {
            var (frameworks, packages) =
                _projectAnalyzer.ParseProjectFile(projectFile);

            var relativePath = Path.GetRelativePath(folderPath, projectFile);

            projectMetadataList.Add(new IssueProjectMetadata
            {
                Number = metadata.Number,
                Title = metadata.Title,
                State = metadata.State,
                Milestone = metadata.Milestone,
                Labels = metadata.Labels,
                Url = metadata.Url,
                ProjectPath = relativePath,
                TargetFrameworks = frameworks,
                Packages = packages
            });
        }

        var outputPath = Path.Combine(folderPath, "issue_metadata.json");
        await WriteIssueMetadataAsync(
            outputPath,
            projectMetadataList,
            cancellationToken);

        return projectMetadataList.Count;
    }

    private static async Task WriteIssueMetadataAsync(
        string outputPath,
        List<IssueProjectMetadata> metadata,
        CancellationToken cancellationToken)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        var json = JsonSerializer.Serialize(metadata, options);
        await File.WriteAllTextAsync(outputPath, json, cancellationToken);
    }
}

