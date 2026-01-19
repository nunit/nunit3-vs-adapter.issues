namespace IssueRunner.Services;

/// <summary>
/// Represents the result of a test execution step (restore, build, or test).
/// </summary>
public enum StepStatus
{
    /// <summary>Step has not been run yet.</summary>
    NotRun,
    
    /// <summary>Step completed successfully.</summary>
    Success,
    
    /// <summary>Step failed.</summary>
    Failed
}

/// <summary>
/// Represents the result of a test execution step with output and error information.
/// </summary>
public sealed class StepResult
{
    /// <summary>
    /// Gets or sets the step status.
    /// </summary>
    public StepStatus Status { get; init; }
    
    /// <summary>
    /// Gets or sets the step output.
    /// </summary>
    public string Output { get; init; } = "";
    
    /// <summary>
    /// Gets or sets the step error output.
    /// </summary>
    public string Error { get; init; } = "";
}

/// <summary>
/// Service for executing tests.
/// </summary>
public interface ITestExecutionService
{
    /// <summary>
    /// Executes tests for a project, running restore, build, and test as separate steps.
    /// </summary>
    /// <param name="projectPath">Path to the project or solution.</param>
    /// <param name="issueFolderPath">Path to the issue folder.</param>
    /// <param name="timeoutSeconds">Command timeout in seconds.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Test execution result with separate restore, build, and test results.</returns>
    Task<TestExecutionResult> ExecuteTestsAsync(
        string projectPath,
        string issueFolderPath,
        int timeoutSeconds,
        CancellationToken cancellationToken);

    /// <summary>
    /// Determines if an issue folder has custom runner scripts.
    /// </summary>
    /// <param name="issueFolderPath">Path to the issue folder.</param>
    /// <returns>True if custom scripts are present.</returns>
    bool HasCustomRunners(string issueFolderPath);
}

/// <summary>
/// Represents the complete result of test execution including restore, build, and test steps.
/// </summary>
public sealed class TestExecutionResult
{
    /// <summary>
    /// Gets or sets the restore step result.
    /// </summary>
    public required StepResult RestoreResult { get; init; }
    
    /// <summary>
    /// Gets or sets the build step result.
    /// </summary>
    public required StepResult BuildResult { get; init; }
    
    /// <summary>
    /// Gets or sets the test step result.
    /// </summary>
    public required StepResult TestResult { get; init; }
    
    /// <summary>
    /// Gets or sets the runsettings file path if used.
    /// </summary>
    public string? RunSettings { get; init; }
    
    /// <summary>
    /// Gets or sets the custom runner scripts used (if any).
    /// </summary>
    public List<string>? Scripts { get; init; }
    
    /// <summary>
    /// Gets a value indicating whether all executed steps succeeded.
    /// </summary>
    public bool OverallSuccess
    {
        get
        {
            // If restore failed, build and test are NotRun, so only check restore
            if (RestoreResult.Status == StepStatus.Failed)
            {
                return false;
            }
            
            // If build failed, test is NotRun, so check restore and build
            if (BuildResult.Status == StepStatus.Failed)
            {
                return false;
            }
            
            // All steps that ran succeeded
            return TestResult.Status != StepStatus.Failed;
        }
    }
}
