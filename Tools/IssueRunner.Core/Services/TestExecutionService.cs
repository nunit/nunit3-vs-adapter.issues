using Microsoft.Extensions.Logging;

namespace IssueRunner.Services;

/// <summary>
/// Implementation of test execution service.
/// </summary>
public sealed class TestExecutionService : ITestExecutionService
{
    private readonly IProcessExecutor _processExecutor;
    private readonly IProjectAnalyzerService _projectAnalyzer;
    private readonly ILogger<TestExecutionService> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="TestExecutionService"/> class.
    /// </summary>
    public TestExecutionService(
        IProcessExecutor processExecutor,
        IProjectAnalyzerService projectAnalyzer,
        ILogger<TestExecutionService> logger)
    {
        _processExecutor = processExecutor;
        _projectAnalyzer = projectAnalyzer;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<TestExecutionResult> ExecuteTestsAsync(
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

    private async Task<TestExecutionResult> ExecuteCustomScriptsAsync(
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

        // For custom scripts, restore and build are NotRun, test result is the script execution result
        return new TestExecutionResult
        {
            RestoreResult = new StepResult { Status = StepStatus.NotRun, Output = "", Error = "" },
            BuildResult = new StepResult { Status = StepStatus.NotRun, Output = "", Error = "" },
            TestResult = new StepResult
            {
                Status = allSuccess ? StepStatus.Success : StepStatus.Failed,
                Output = string.Join(Environment.NewLine, allOutput),
                Error = string.Join(Environment.NewLine, allErrors)
            },
            RunSettings = null,
            Scripts = scripts.Select(Path.GetFileName).ToList()!
        };
    }

    private async Task<TestExecutionResult> ExecuteDotnetTestAsync(
        string projectPath,
        string issueFolderPath,
        int timeoutSeconds,
        CancellationToken cancellationToken)
    {
        var runSettings = FindRunSettings(issueFolderPath);
        
        // Step 1: Restore
        var restoreResult = await ExecuteRestoreAsync(projectPath, timeoutSeconds, cancellationToken);
        var restoreStepResult = new StepResult
        {
            Status = restoreResult.Success ? StepStatus.Success : StepStatus.Failed,
            Output = restoreResult.Output,
            Error = restoreResult.Error
        };
        
        // If restore failed, return early with NotRun for build and test
        if (!restoreResult.Success)
        {
            return new TestExecutionResult
            {
                RestoreResult = restoreStepResult,
                BuildResult = new StepResult { Status = StepStatus.NotRun, Output = "", Error = "" },
                TestResult = new StepResult { Status = StepStatus.NotRun, Output = "", Error = "" },
                RunSettings = runSettings,
                Scripts = null
            };
        }
        
        // Step 2: Build
        var buildResult = await ExecuteBuildAsync(projectPath, timeoutSeconds, cancellationToken);
        var buildStepResult = new StepResult
        {
            Status = buildResult.Success ? StepStatus.Success : StepStatus.Failed,
            Output = buildResult.Output,
            Error = buildResult.Error
        };
        
        // If build failed, return with NotRun for test
        if (!buildResult.Success)
        {
            return new TestExecutionResult
            {
                RestoreResult = restoreStepResult,
                BuildResult = buildStepResult,
                TestResult = new StepResult { Status = StepStatus.NotRun, Output = "", Error = "" },
                RunSettings = runSettings,
                Scripts = null
            };
        }
        
        // Step 3: Test
        var workingDir = Path.GetDirectoryName(projectPath)!;
        var usesMtp = _projectAnalyzer.UsesTestingPlatform(projectPath);
        var projectDir = Path.GetDirectoryName(projectPath)!;
        var solutionFiles = Directory.GetFiles(projectDir, "*.sln");
        
        string args;
        if (usesMtp)
        {
            // MTP requires explicit flags
            if (solutionFiles.Length == 1)
            {
                var solutionFileName = Path.GetFileName(solutionFiles[0]);
                args = $"test --no-build --no-restore --solution \"{solutionFileName}\"";
            }
            else
            {
                var projectFileName = Path.GetFileName(projectPath);
                args = $"test --no-build --no-restore --project \"{projectFileName}\"";
            }
        }
        else
        {
            // Traditional VSTest - use positional arguments
            if (solutionFiles.Length == 1)
            {
                var solutionFileName = Path.GetFileName(solutionFiles[0]);
                args = $"test --no-build --no-restore \"{solutionFileName}\"";
            }
            else
            {
                var projectFileName = Path.GetFileName(projectPath);
                args = $"test --no-build --no-restore \"{projectFileName}\"";
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

        var testSuccess = exitCode == 0;
        
        _logger.LogDebug(
            "Executed dotnet test for {Project}: exit code {Code}",
            Path.GetFileName(projectPath),
            exitCode);

        var testStepResult = new StepResult
        {
            Status = testSuccess ? StepStatus.Success : StepStatus.Failed,
            Output = output,
            Error = error
        };
        
        return new TestExecutionResult
        {
            RestoreResult = restoreStepResult,
            BuildResult = buildStepResult,
            TestResult = testStepResult,
            RunSettings = runSettings,
            Scripts = null
        };
    }

    private static string? FindRunSettings(string issueFolderPath)
    {
        var runSettings = Path.Combine(issueFolderPath, ".runsettings");
        return File.Exists(runSettings) ? runSettings : null;
    }

    /// <summary>
    /// Executes dotnet restore for a project.
    /// </summary>
    /// <param name="projectPath">Path to the project or solution.</param>
    /// <param name="timeoutSeconds">Command timeout in seconds.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Restore result with success status, output, and error.</returns>
    private async Task<(bool Success, string Output, string Error)> ExecuteRestoreAsync(
        string projectPath,
        int timeoutSeconds,
        CancellationToken cancellationToken)
    {
        var workingDir = Path.GetDirectoryName(projectPath)!;
        
        // Look for a solution file in the same directory
        var projectDir = Path.GetDirectoryName(projectPath)!;
        var solutionFiles = Directory.GetFiles(projectDir, "*.sln");
        
        string args;
        if (solutionFiles.Length == 1)
        {
            var solutionFileName = Path.GetFileName(solutionFiles[0]);
            args = $"restore \"{solutionFileName}\"";
        }
        else
        {
            var projectFileName = Path.GetFileName(projectPath);
            args = $"restore \"{projectFileName}\"";
        }

        var (exitCode, output, error) = await _processExecutor.ExecuteAsync(
            "dotnet",
            args,
            workingDir,
            timeoutSeconds,
            cancellationToken);

        var success = exitCode == 0;
        
        _logger.LogDebug(
            "Executed dotnet restore for {Project}: exit code {Code}",
            Path.GetFileName(projectPath),
            exitCode);

        return (success, output, error);
    }

    /// <summary>
    /// Executes dotnet build for a project.
    /// </summary>
    /// <param name="projectPath">Path to the project or solution.</param>
    /// <param name="timeoutSeconds">Command timeout in seconds.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Build result with success status, output, and error.</returns>
    private async Task<(bool Success, string Output, string Error)> ExecuteBuildAsync(
        string projectPath,
        int timeoutSeconds,
        CancellationToken cancellationToken)
    {
        var workingDir = Path.GetDirectoryName(projectPath)!;
        
        // Look for a solution file in the same directory
        var projectDir = Path.GetDirectoryName(projectPath)!;
        var solutionFiles = Directory.GetFiles(projectDir, "*.sln");
        
        string args;
        if (solutionFiles.Length == 1)
        {
            var solutionFileName = Path.GetFileName(solutionFiles[0]);
            args = $"build --no-restore \"{solutionFileName}\"";
        }
        else
        {
            var projectFileName = Path.GetFileName(projectPath);
            args = $"build --no-restore \"{projectFileName}\"";
        }

        var (exitCode, output, error) = await _processExecutor.ExecuteAsync(
            "dotnet",
            args,
            workingDir,
            timeoutSeconds,
            cancellationToken);

        var success = exitCode == 0;
        
        _logger.LogDebug(
            "Executed dotnet build for {Project}: exit code {Code}",
            Path.GetFileName(projectPath),
            exitCode);

        return (success, output, error);
    }
}

