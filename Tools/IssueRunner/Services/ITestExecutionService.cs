using IssueRunner.Models;

namespace IssueRunner.Services;

/// <summary>
/// Service for executing tests.
/// </summary>
public interface ITestExecutionService
{
    /// <summary>
    /// Executes tests for a project.
    /// </summary>
    /// <param name="projectPath">Path to the project or solution.</param>
    /// <param name="issueFolderPath">Path to the issue folder.</param>
    /// <param name="timeoutSeconds">Command timeout in seconds.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Test result.</returns>
    Task<(bool Success, string Output, string Error, string? RunSettings, List<string>? Scripts)>
        ExecuteTestsAsync(
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
