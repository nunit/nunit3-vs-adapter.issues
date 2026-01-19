using IssueRunner.Models;
using IssueRunner.Services;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Xml.Linq;

namespace IssueRunner.Commands;

/// <summary>
/// Full issue metadata including packages and frameworks (from issue folder).
/// </summary>
internal sealed class IssueMetadataFull
{
    [JsonPropertyName("number")]
    public int Number { get; init; }

    [JsonPropertyName("title")]
    public string? Title { get; init; }

    [JsonPropertyName("target_frameworks")]
    public List<string>? TargetFrameworks { get; init; }

    [JsonPropertyName("packages")]
    public List<PackageInfo>? Packages { get; init; }
}

/// <summary>
/// Command to reset package versions to metadata values.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="ResetPackagesCommand"/> class.
/// </remarks>
public sealed class ResetPackagesCommand(
    IIssueDiscoveryService issueDiscovery,
    IProjectAnalyzerService projectAnalyzer,
    ILogger<ResetPackagesCommand> logger,
    IMarkerService markerService)
{

    /// <summary>
    /// Executes the command to reset packages.
    /// </summary>
    public async Task<int> ExecuteAsync(
        string repositoryRoot,
        List<int>? issueNumbers,
        LogVerbosity verbosity,
        CancellationToken cancellationToken)
    {
        try
        {
            var issueFolders = issueDiscovery.DiscoverIssueFolders();

            var issuesToReset = issueNumbers == null
                ? issueFolders
                : issueFolders.Where(kvp => issueNumbers.Contains(kvp.Key)).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            Console.WriteLine($"Resetting packages for {issuesToReset.Count} issues...");

            foreach (var (issueNumber, folderPath) in issuesToReset)
            {
                if (markerService.ShouldSkipIssue(folderPath))
                {
                    if (verbosity == LogVerbosity.Verbose)
                    {
                        Console.WriteLine($"[{issueNumber}] Skipped due to marker file");
                    }
                    continue;
                }

                var metadata = await LoadIssueMetadataAsync(folderPath, issueNumber, cancellationToken);
                if (metadata == null)
                {
                    if (verbosity == LogVerbosity.Verbose)
                    {
                        Console.WriteLine($"[{issueNumber}] Skipped - no metadata found");
                    }
                    continue;
                }

                await ResetIssuePackagesAsync(issueNumber, folderPath, metadata, verbosity, cancellationToken);
            }

            Console.WriteLine("Reset completed.");
            return 0;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error resetting packages");
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
            logger.LogError(ex, "[{Issue}] Error loading metadata", issueNumber);
            return null;
        }
    }

    private async Task ResetIssuePackagesAsync(
        int issueNumber,
        string folderPath,
        IssueMetadataFull metadata,
        LogVerbosity verbosity,
        CancellationToken cancellationToken)
    {
        var projectFiles = projectAnalyzer.FindProjectFiles(folderPath);

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

            var updated = false;

            // Reset frameworks if metadata has them
            if (metadata.TargetFrameworks != null && metadata.TargetFrameworks.Count > 0)
            {
                var metadataFrameworks = string.Join(";", metadata.TargetFrameworks);
                
                // Check for TargetFrameworks (plural) first
                var targetFrameworksElement = root.Descendants("TargetFrameworks").FirstOrDefault();
                var targetFrameworkElement = root.Descendants("TargetFramework").FirstOrDefault();
                
                if (metadata.TargetFrameworks.Count > 1)
                {
                    // Should use TargetFrameworks (plural)
                    if (targetFrameworkElement != null)
                    {
                        // Convert from singular to plural
                        targetFrameworkElement.Name = "TargetFrameworks";
                        targetFrameworkElement.Value = metadataFrameworks;
                        updated = true;
                        logger.LogDebug("[{Issue}] Converted TargetFramework to TargetFrameworks: {Frameworks}", issueNumber, metadataFrameworks);
                    }
                    else if (targetFrameworksElement != null && targetFrameworksElement.Value != metadataFrameworks)
                    {
                        targetFrameworksElement.Value = metadataFrameworks;
                        updated = true;
                        logger.LogDebug("[{Issue}] Reset TargetFrameworks to {Frameworks}", issueNumber, metadataFrameworks);
                    }
                }
                else
                {
                    // Should use TargetFramework (singular)
                    var singleFramework = metadata.TargetFrameworks[0];
                    
                    if (targetFrameworksElement != null)
                    {
                        // Convert from plural to singular
                        targetFrameworksElement.Name = "TargetFramework";
                        targetFrameworksElement.Value = singleFramework;
                        updated = true;
                        logger.LogDebug("[{Issue}] Converted TargetFrameworks to TargetFramework: {Framework}", issueNumber, singleFramework);
                    }
                    else if (targetFrameworkElement != null && targetFrameworkElement.Value != singleFramework)
                    {
                        targetFrameworkElement.Value = singleFramework;
                        updated = true;
                        logger.LogDebug("[{Issue}] Reset TargetFramework to {Framework}", issueNumber, singleFramework);
                    }
                }
            }

            // Reset packages
            var metadataPackages = metadata.Packages?.ToDictionary(p => p.Name, p => p.Version);
            if (metadataPackages != null)
            {
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
                            logger.LogDebug(
                                "[{Issue}] Reset {Package} to {Version}",
                                issueNumber,
                                name,
                                version);
                        }
                    }
                }
            }

            if (updated)
            {
                doc.Save(projectFile);
                Console.WriteLine($"[{issueNumber}] Reset to metadata versions");
            }
            else
            {
                if (verbosity == LogVerbosity.Verbose)
                {
                    Console.WriteLine($"[{issueNumber}] No changes needed");
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[{Issue}] Error resetting project", issueNumber);
            Console.WriteLine($"[{issueNumber}] Error: {ex.Message}");
        }

        await Task.CompletedTask;
    }
}

