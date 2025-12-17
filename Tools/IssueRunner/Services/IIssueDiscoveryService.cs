namespace IssueRunner.Services;

/// <summary>
/// Service for discovering Issue* folders in the repository.
/// </summary>
public interface IIssueDiscoveryService
{
    /// <summary>
    /// Discovers all Issue* folders in the repository.
    /// </summary>
    /// <returns>Dictionary mapping issue number to folder path.</returns>
    Dictionary<int, string> DiscoverIssueFolders();

    /// <summary>
    /// Checks if an issue folder should be skipped based on marker files.
    /// </summary>
    /// <param name="issueFolderPath">Path to the issue folder.</param>
    /// <returns>True if the folder should be skipped.</returns>
    bool ShouldSkipIssue(string issueFolderPath);

    /// <summary>
    /// Checks if an issue requires Windows (netfx workflow) even if it's a .NET Core project.
    /// </summary>
    /// <param name="issueFolderPath">Path to the issue folder.</param>
    /// <returns>True if the issue requires Windows.</returns>
    bool RequiresWindows(string issueFolderPath);
}
