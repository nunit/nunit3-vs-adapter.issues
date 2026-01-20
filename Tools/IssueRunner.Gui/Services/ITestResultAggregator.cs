using IssueRunner.Models;
using IssueRunner.Services;

namespace IssueRunner.Gui.Services;

public enum AggregatedIssueStatus
{
    Passed,
    Failed,
    Skipped,
    NotRestored,
    NotCompiling,
    NotTested
}

public sealed class AggregatedIssueResult
{
    public int Number { get; init; }

    public AggregatedIssueStatus Status { get; init; }

    public string? LastRun { get; init; }
}

/// <summary>
/// Aggregates raw <see cref="IssueResult"/> rows into per-issue status values.
/// Ensures a single source of truth for how issues are classified across the UI.
/// </summary>
public interface ITestResultAggregator
{
    IReadOnlyList<AggregatedIssueResult> AggregatePerIssue(
        Dictionary<int, string> folders,
        IReadOnlyList<IssueResult> allResults,
        IMarkerService markerService,
        Action<string>? log = null);
}

