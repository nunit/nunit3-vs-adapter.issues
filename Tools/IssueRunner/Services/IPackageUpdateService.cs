using IssueRunner.Models;

namespace IssueRunner.Services;

/// <summary>
/// Service for executing package updates.
/// </summary>
public interface IPackageUpdateService
{
    /// <summary>
    /// Updates NuGet packages in a project.
    /// </summary>
    /// <param name="projectPath">Path to the project file.</param>
    /// <param name="nunitOnly">Whether to update only NUnit packages.</param>
    /// <param name="feed">Package feed option.</param>
    /// <param name="timeoutSeconds">Command timeout in seconds.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Tuple of (success, output, error).</returns>
    Task<(bool Success, string Output, string Error)> UpdatePackagesAsync(
        string projectPath,
        bool nunitOnly,
        PackageFeed feed,
        int timeoutSeconds,
        CancellationToken cancellationToken);
}
