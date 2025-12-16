using IssueRunner.Models;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace IssueRunner.Commands;

/// <summary>
/// Command to check for regression failures.
/// </summary>
public sealed class CheckRegressionsCommand
{
    private readonly ILogger<CheckRegressionsCommand> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="CheckRegressionsCommand"/> class.
    /// </summary>
    public CheckRegressionsCommand(ILogger<CheckRegressionsCommand> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Executes the command.
    /// </summary>
    public async Task<int> ExecuteAsync(
        string repositoryRoot,
        CancellationToken cancellationToken)
    {
        Console.WriteLine("Checking for regression failures...");

        var resultsPath = Path.Combine(repositoryRoot, "results.json");
        var metadataPath = Path.Combine(repositoryRoot, "Tools", "issues_metadata.json");

        if (!File.Exists(resultsPath))
        {
            Console.WriteLine($"Results file not found: {resultsPath}");
            return 1;
        }

        if (!File.Exists(metadataPath))
        {
            Console.WriteLine($"Metadata file not found: {metadataPath}");
            return 1;
        }

        var results = await LoadResultsAsync(resultsPath, cancellationToken);
        var metadata = await LoadMetadataAsync(metadataPath, cancellationToken);

        var metadataDict = metadata.ToDictionary(m => m.Number);

        var regressions = results
            .Where(r =>
                metadataDict.TryGetValue(r.Number, out var m) &&
                m.State == "closed" &&
                r.TestResult == "fail")
            .ToList();

        if (regressions.Count == 0)
        {
            Console.WriteLine("✅ No regression failures detected");
            return 0;
        }

        Console.WriteLine($"❌ {regressions.Count} regression failure(s) detected:");

        foreach (var regression in regressions)
        {
            if (metadataDict.TryGetValue(regression.Number, out var meta))
            {
                Console.WriteLine($"  - Issue #{regression.Number}: {meta.Title}");
            }
        }

        return 1;
    }

    private async Task<List<IssueResult>> LoadResultsAsync(
        string path,
        CancellationToken cancellationToken)
    {
        var json = await File.ReadAllTextAsync(path, cancellationToken);
        return JsonSerializer.Deserialize<List<IssueResult>>(json) ?? [];
    }

    private async Task<List<IssueMetadata>> LoadMetadataAsync(
        string path,
        CancellationToken cancellationToken)
    {
        var json = await File.ReadAllTextAsync(path, cancellationToken);
        return JsonSerializer.Deserialize<List<IssueMetadata>>(json) ?? [];
    }
}
