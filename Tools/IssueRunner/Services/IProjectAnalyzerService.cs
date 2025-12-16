using IssueRunner.Models;

namespace IssueRunner.Services;

/// <summary>
/// Service for analyzing project files and extracting metadata.
/// </summary>
public interface IProjectAnalyzerService
{
    /// <summary>
    /// Finds all csproj files in an issue folder recursively.
    /// </summary>
    /// <param name="issueFolderPath">Path to the issue folder.</param>
    /// <returns>List of csproj file paths.</returns>
    List<string> FindProjectFiles(string issueFolderPath);

    /// <summary>
    /// Parses a csproj file to extract target frameworks and package references.
    /// </summary>
    /// <param name="projectFilePath">Path to the csproj file.</param>
    /// <returns>Tuple of (target frameworks, packages).</returns>
    (List<string> TargetFrameworks, List<PackageInfo> Packages) ParseProjectFile(
        string projectFilePath);

    /// <summary>
    /// Determines if a project is SDK-style or classic.
    /// </summary>
    /// <param name="projectFilePath">Path to the csproj file.</param>
    /// <returns>"SDK-style" or "classic".</returns>
    string GetProjectStyle(string projectFilePath);
}
