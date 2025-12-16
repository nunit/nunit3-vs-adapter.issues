using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace IssueRunner.Services;

/// <summary>
/// Implementation of test execution service.
/// </summary>
public sealed partial class TestExecutionService : ITestExecutionService
{
    private readonly ProcessExecutor _processExecutor;
    private readonly IProjectAnalyzerService _projectAnalyzer;
    private readonly ILogger<TestExecutionService> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="TestExecutionService"/> class.
    /// </summary>
    public TestExecutionService(
        ProcessExecutor processExecutor,
        IProjectAnalyzerService projectAnalyzer,
        ILogger<TestExecutionService> logger)
    {
        _processExecutor = processExecutor;
        _projectAnalyzer = projectAnalyzer;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<(bool Success, string Output, string Error, string? RunSettings, List<string>? Scripts)>
        ExecuteTestsAsync(
            string projectPath,
            string issueFolderPath,
            int timeoutSeconds,
            CancellationToken cancellationToken)
    {
        var customScripts = FindCustomScripts(issueFolderPath);
        
        if (customScripts.Count > 0)
        {
            return await ExecuteCustomScriptsAsync(
                customScripts,
                issueFolderPath,
                timeoutSeconds,
                cancellationToken);
        }

        return await ExecuteDotnetTestAsync(
            projectPath,
            issueFolderPath,
            timeoutSeconds,
            cancellationToken);
    }

    /// <inheritdoc />
    public bool HasCustomRunners(string issueFolderPath)
    {
        return FindCustomScripts(issueFolderPath).Count > 0;
    }

    private List<string> FindCustomScripts(string issueFolderPath)
    {
        var scripts = new List<string>();

        var isWindows = OperatingSystem.IsWindows();
        var extension = isWindows ? ".cmd" : ".sh";

        var files = Directory.GetFiles(issueFolderPath, $"run_*{extension}");
        scripts.AddRange(files);

        return scripts;
    }

    private async Task<(bool Success, string Output, string Error, string? RunSettings, List<string>? Scripts)>
        ExecuteCustomScriptsAsync(
            List<string> scripts,
            string issueFolderPath,
            int timeoutSeconds,
            CancellationToken cancellationToken)
    {
        var allOutput = new List<string>();
        var allErrors = new List<string>();
        var allSuccess = true;

        foreach (var script in scripts)
        {
            var (exitCode, output, error) = await _processExecutor.ExecuteAsync(
                script,
                "",
                issueFolderPath,
                timeoutSeconds,
                cancellationToken);

            allOutput.Add($"=== {Path.GetFileName(script)} ===");
            allOutput.Add(output);
            
            if (!string.IsNullOrEmpty(error))
            {
                allErrors.Add(error);
            }

            if (exitCode != 0)
            {
                allSuccess = false;
            }

            _logger.LogDebug(
                "Executed {Script}: exit code {Code}",
                Path.GetFileName(script),
                exitCode);
        }

        return (
            allSuccess,
            string.Join(Environment.NewLine, allOutput),
            string.Join(Environment.NewLine, allErrors),
            null,
            scripts.Select(Path.GetFileName).ToList()!);
    }

    private async Task<(bool Success, string Output, string Error, string? RunSettings, List<string>? Scripts)>
        ExecuteDotnetTestAsync(
            string projectPath,
            string issueFolderPath,
            int timeoutSeconds,
            CancellationToken cancellationToken)
    {
        var runSettings = FindRunSettings(issueFolderPath);
        var workingDir = Path.GetDirectoryName(projectPath)!;

        // Check if this is an MTP project - they require --solution/--project flags
        var usesMtp = _projectAnalyzer.UsesTestingPlatform(projectPath);
        
        // Look for a solution file in the same directory
        var projectDir = Path.GetDirectoryName(projectPath)!;
        var solutionFiles = Directory.GetFiles(projectDir, "*.sln");
        
        string args;
        if (usesMtp)
        {
            // MTP requires explicit flags
            if (solutionFiles.Length == 1)
            {
                var solutionFileName = Path.GetFileName(solutionFiles[0]);
                args = $"test --solution \"{solutionFileName}\"";
            }
            else
            {
                var projectFileName = Path.GetFileName(projectPath);
                args = $"test --project \"{projectFileName}\"";
            }
        }
        else
        {
            // Traditional VSTest - use positional arguments
            if (solutionFiles.Length == 1)
            {
                var solutionFileName = Path.GetFileName(solutionFiles[0]);
                args = $"test \"{solutionFileName}\"";
            }
            else
            {
                var projectFileName = Path.GetFileName(projectPath);
                args = $"test \"{projectFileName}\"";
            }
        }
        
        if (!string.IsNullOrEmpty(runSettings))
        {
            args += $" --settings \"{runSettings}\"";
        }

        var (exitCode, output, error) = await _processExecutor.ExecuteAsync(
            "dotnet",
            args,
            workingDir,
            timeoutSeconds,
            cancellationToken);

        var success = exitCode == 0;
        
        _logger.LogDebug(
            "Executed dotnet test for {Project}: exit code {Code}",
            Path.GetFileName(projectPath),
            exitCode);

        return (success, output, error, runSettings, null);
    }

    private static string? FindRunSettings(string issueFolderPath)
    {
        var runSettings = Path.Combine(issueFolderPath, ".runsettings");
        return File.Exists(runSettings) ? runSettings : null;
    }
}
