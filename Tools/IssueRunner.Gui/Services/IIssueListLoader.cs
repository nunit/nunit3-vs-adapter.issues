using IssueRunner.Gui.ViewModels;

namespace IssueRunner.Gui.Services;

/// <summary>
/// Service responsible for loading issues and their state for the issue list view.
/// </summary>
public interface IIssueListLoader
{
    /// <summary>
    /// Loads issues for the issue list, including metadata, test results, diffs, and marker state.
    /// </summary>
    /// <param name="repositoryRoot">Repository root path.</param>
    /// <param name="folders">Discovered issue folders (issue number -> folder path).</param>
    /// <param name="log">Optional logging callback.</param>
    /// <returns>Issue list load result.</returns>
    Task<IssueListLoadResult> LoadIssuesAsync(
        string repositoryRoot,
        Dictionary<int, string> folders,
        Action<string>? log = null);
}

