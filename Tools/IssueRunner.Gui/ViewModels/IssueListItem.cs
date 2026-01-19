using IssueRunner.Models;

namespace IssueRunner.Gui.ViewModels;

/// <summary>
/// Represents an issue in the list.
/// </summary>
public class IssueListItem
{
    public int Number { get; set; }
    public string Title { get; set; } = "";
    public string State { get; set; } = ""; // Original state from metadata (open/closed)
    public IssueState StateValue { get; set; } = IssueState.New; // New state enum value
    public string DetailedState { get; set; } = ""; // Enhanced state (includes not compiling, skipped, etc.) - for display
    public string TestResult { get; set; } = "";
    public string LastRun { get; set; } = "";
    public string? NotTestedReason { get; set; } // Why it's not tested (marker reason, not compiling, etc.)
    public string TestTypes { get; set; } = ""; // "Scripts" or "DotNet test"
    public string GitHubUrl { get; set; } = ""; // GitHub issue URL
    public string Framework { get; set; } = ""; // ".Net" or ".Net Framework"
    public ChangeType ChangeType { get; set; } = ChangeType.None; // Type of change from baseline
    public string? StatusDisplay { get; set; } = null; // Display text for status (e.g., "=> fail" for regressions). Null means use TestResult.
}