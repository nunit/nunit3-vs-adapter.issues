using IssueRunner.Models;
using IssueRunner.Services;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace IssueRunner.Commands;

/// <summary>
/// Command to generate test report.
/// </summary>
public sealed class GenerateReportCommand
{
    private readonly ReportGeneratorService _reportGenerator;
    private readonly IEnvironmentService _environmentService;
    private readonly ILogger<GenerateReportCommand> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="GenerateReportCommand"/> class.
    /// </summary>
    public GenerateReportCommand(
        ReportGeneratorService reportGenerator,
        IEnvironmentService environmentService,
        ILogger<GenerateReportCommand> logger)
    {
        _reportGenerator = reportGenerator;
        _environmentService = environmentService;
        _logger = logger;
    }

    /// <summary>
    /// Executes the command.
    /// </summary>
    public async Task<int> ExecuteAsync( CancellationToken cancellationToken)
    {
        Console.WriteLine("Generating test report...");
        var repositoryRoot =  _environmentService.Root;
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

        var report = _reportGenerator.GenerateReport(results, metadata);

        var reportPath = Path.Combine(repositoryRoot, "TestReport.md");
        await File.WriteAllTextAsync(reportPath, report, cancellationToken);

        Console.WriteLine($"Report generated: {reportPath}");

        return 0;
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
