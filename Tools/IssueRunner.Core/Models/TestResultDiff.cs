namespace IssueRunner.Models;

/// <summary>
/// Represents a change in test result between baseline and current.
/// </summary>
public sealed class TestResultDiff
{
    /// <summary>
    /// Gets or sets the issue number.
    /// </summary>
    public required int IssueNumber { get; init; }

    /// <summary>
    /// Gets or sets the project path.
    /// </summary>
    public required string ProjectPath { get; init; }

    /// <summary>
    /// Gets or sets the baseline status (e.g., "success", "fail", "not compile").
    /// </summary>
    public required string BaselineStatus { get; init; }

    /// <summary>
    /// Gets or sets the current status (e.g., "success", "fail", "skipped").
    /// </summary>
    public required string CurrentStatus { get; init; }

    /// <summary>
    /// Gets or sets the type of change.
    /// </summary>
    public required ChangeType ChangeType { get; init; }
}

/// <summary>
/// Type of change between baseline and current.
/// </summary>
public enum ChangeType
{
    /// <summary>No change.</summary>
    None,

    /// <summary>Fixed: Was non-success, now success (Green).</summary>
    Fixed,

    /// <summary>Regression: Was success, now fail (Red).</summary>
    Regression,

    /// <summary>CompileToFail: Was not compile/restore fail, now test fail (Orange).</summary>
    CompileToFail,

    /// <summary>Skipped: Was fail, now skipped (exclude from list).</summary>
    Skipped,

    /// <summary>Other: Any other status change (Grey).</summary>
    Other
}

