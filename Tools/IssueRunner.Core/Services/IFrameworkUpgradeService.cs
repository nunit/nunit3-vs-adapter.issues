namespace IssueRunner.Services;

/// <summary>
/// Service for upgrading target frameworks in projects.
/// </summary>
public interface IFrameworkUpgradeService
{
    /// <summary>
    /// Upgrades all target frameworks in the issue folder to latest versions.
    /// </summary>
    /// <param name="issueFolderPath">Path to the issue folder.</param>
    /// <param name="issueNumber">Issue number for logging.</param>
    /// <returns>True if any files were modified.</returns>
    bool UpgradeAllProjectFrameworks(string issueFolderPath, int issueNumber);
}

