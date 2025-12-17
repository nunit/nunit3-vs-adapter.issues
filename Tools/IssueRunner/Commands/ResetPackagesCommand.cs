using IssueRunner.Models;
using IssueRunner.Services;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Xml.Linq;

namespace IssueRunner.Commands;

/// <summary>
/// Full issue metadata including packages (from issue folder).
/// </summary>
internal sealed class IssueMetadataFull
{
    [JsonPropertyName("number")]
    public int Number { get; init; }

    [JsonPropertyName("title")]
    public string? Title { get; init; }

    [JsonPropertyName("packages")]
    public List<PackageInfo>? Packages { get; init; }
}

/// <summary>
/// Command to reset package versions to metadata values.
/// </summary>
public sealed class ResetPackagesCommand
{
    private readonly IIssueDiscoveryService _issueDiscovery;
    private readonly IProjectAnalyzerService _projectAnalyzer;
    private readonly ILogger<ResetPackagesCommand> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ResetPackagesCommand"/> class.
    /// </summary>
    public ResetPackagesCommand(
        IIssueDiscoveryService issueDiscovery,
        IProjectAnalyzerService projectAnalyzer,
        ILogger<ResetPackagesCommand> logger)
    {
        _issueDiscovery = issueDiscovery;
        _projectAnalyzer = projectAnalyzer;
        _logger = logger;
    }

    /// <summary>
    /// Executes the command to reset packages.
    /// </summary>
    public async Task<int> ExecuteAsync(
        string repositoryRoot,
        List<int>? issueNumbers,
        CancellationToken cancellationToken)
    {
        try
        {
            var issueFolders = _issueDiscovery.DiscoverIssueFolders(repositoryRoot);

            var issuesToReset = issueNumbers == null
                ? issueFolders
                : issueFolders.Where(kvp => issueNumbers.Contains(kvp.Key)).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            Console.WriteLine($"Resetting packages for {issuesToReset.Count} issues...");

            foreach (var (issueNumber, folderPath) in issuesToReset)
            {
                if (_issueDiscovery.ShouldSkipIssue(folderPath))
                {
                    Console.WriteLine($"[{issueNumber}] Skipped due to marker file");
                    continue;
                }

                var metadata = await LoadIssueMetadataAsync(folderPath, issueNumber, cancellationToken);
                if (metadata == null)
                {
                    Console.WriteLine($"[{issueNumber}] Skipped - no metadata found");
                    continue;
                }

                await ResetIssuePackagesAsync(issueNumber, folderPath, metadata, cancellationToken);
            }

            Console.WriteLine("Reset completed.");
            return 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resetting packages");
            Console.WriteLine($"Error: {ex.Message}");
            return 1;
        }
    }

    private async Task<IssueMetadataFull?> LoadIssueMetadataAsync(
        string folderPath,
        int issueNumber,
        CancellationToken cancellationToken)
    {
        var metadataPath = Path.Combine(folderPath, "issue_metadata.json");

        if (!File.Exists(metadataPath))
        {
            return null;
        }

        try
        {
            var json = await File.ReadAllTextAsync(metadataPath, cancellationToken);
            var list = JsonSerializer.Deserialize<List<IssueMetadataFull>>(json);
            return list?.FirstOrDefault(m => m.Number == issueNumber);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[{Issue}] Error loading metadata", issueNumber);
            return null;
        }
    }

    private async Task ResetIssuePackagesAsync(
        int issueNumber,
        string folderPath,
        IssueMetadataFull metadata,
        CancellationToken cancellationToken)
    {
        var projectFiles = _projectAnalyzer.FindProjectFiles(folderPath);

        if (projectFiles.Count == 0)
        {
            Console.WriteLine($"[{issueNumber}] No project files found");
            return;
        }

        var projectFile = projectFiles.First();

        try
        {
            var doc = XDocument.Load(projectFile);
            var root = doc.Root;

            if (root == null)
            {
                Console.WriteLine($"[{issueNumber}] Invalid project file");
                return;
            }

            var metadataPackages = metadata.Packages.ToDictionary(p => p.Name, p => p.Version);
            var updated = false;

            foreach (var packageRef in root.Descendants("PackageReference"))
            {
                var name = packageRef.Attribute("Include")?.Value;

                if (name != null && metadataPackages.TryGetValue(name, out var version))
                {
                    var versionAttr = packageRef.Attribute("Version");
                    if (versionAttr != null && versionAttr.Value != version)
                    {
                        versionAttr.Value = version;
                        updated = true;
                        _logger.LogDebug(
                            "[{Issue}] Reset {Package} to {Version}",
                            issueNumber,
                            name,
                            version);
                    }
                }
            }

            if (updated)
            {
                doc.Save(projectFile);
                Console.WriteLine($"[{issueNumber}] Reset packages to metadata versions");
            }
            else
            {
                Console.WriteLine($"[{issueNumber}] No changes needed");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[{Issue}] Error resetting packages", issueNumber);
            Console.WriteLine($"[{issueNumber}] Error: {ex.Message}");
        }

        await Task.CompletedTask;
    }
}
