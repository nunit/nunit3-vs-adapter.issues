using IssueRunner.Models;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace IssueRunner.Commands;

/// <summary>
/// Command to merge results from multiple OS runs.
/// </summary>
public sealed class MergeResultsCommand
{
    private readonly ILogger<MergeResultsCommand> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="MergeResultsCommand"/> class.
    /// </summary>
    public MergeResultsCommand(ILogger<MergeResultsCommand> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Executes the command.
    /// </summary>
    public async Task<int> ExecuteAsync(
        string linuxArtifactsPath,
        string windowsArtifactsPath,
        string outputPath,
        CancellationToken cancellationToken)
    {
        Console.WriteLine("Merging results from Linux and Windows runs...");

        var linuxResults = await LoadResultsAsync(
            Path.Combine(linuxArtifactsPath, "results.json"),
            cancellationToken);

        var windowsResults = await LoadResultsAsync(
            Path.Combine(windowsArtifactsPath, "results.json"),
            cancellationToken);

        var mergedResults = MergeResults(linuxResults, windowsResults);

        await SaveResultsAsync(
            Path.Combine(outputPath, "results.json"),
            mergedResults,
            cancellationToken);

        await MergeTextFilesAsync(
            "TestResults-consolelog.md",
            linuxArtifactsPath,
            windowsArtifactsPath,
            outputPath,
            cancellationToken);

        await MergeTextFilesAsync(
            "testupdate.json",
            linuxArtifactsPath,
            windowsArtifactsPath,
            outputPath,
            cancellationToken);

        Console.WriteLine($"Merged {mergedResults.Count} results");

        return 0;
    }

    private List<IssueResult> MergeResults(
        List<IssueResult> linuxResults,
        List<IssueResult> windowsResults)
    {
        var merged = new Dictionary<string, IssueResult>();

        foreach (var result in linuxResults.Concat(windowsResults))
        {
            var key = $"{result.Number}|{result.ProjectPath}";
            
            if (!merged.ContainsKey(key))
            {
                merged[key] = result;
            }
        }

        return [.. merged.Values.OrderBy(r => r.Number)];
    }

    private async Task<List<IssueResult>> LoadResultsAsync(
        string path,
        CancellationToken cancellationToken)
    {
        if (!File.Exists(path))
        {
            _logger.LogWarning("Results file not found: {Path}", path);
            return [];
        }

        var json = await File.ReadAllTextAsync(path, cancellationToken);
        return JsonSerializer.Deserialize<List<IssueResult>>(json) ?? [];
    }

    private async Task SaveResultsAsync(
        string path,
        List<IssueResult> results,
        CancellationToken cancellationToken)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        var json = JsonSerializer.Serialize(results, options);
        await File.WriteAllTextAsync(path, json, cancellationToken);
    }

    private async Task MergeTextFilesAsync(
        string fileName,
        string linuxPath,
        string windowsPath,
        string outputPath,
        CancellationToken cancellationToken)
    {
        var content = new List<string>();

        var linuxFile = Path.Combine(linuxPath, fileName);
        if (File.Exists(linuxFile))
        {
            content.Add("# Linux Results");
            content.Add("");
            content.Add(await File.ReadAllTextAsync(linuxFile, cancellationToken));
            content.Add("");
        }

        var windowsFile = Path.Combine(windowsPath, fileName);
        if (File.Exists(windowsFile))
        {
            content.Add("# Windows Results");
            content.Add("");
            content.Add(await File.ReadAllTextAsync(windowsFile, cancellationToken));
        }

        var outputFile = Path.Combine(outputPath, fileName);
        await File.WriteAllTextAsync(
            outputFile,
            string.Join(Environment.NewLine, content),
            cancellationToken);
    }
}

