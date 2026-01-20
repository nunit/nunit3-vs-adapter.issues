using System.Collections.Generic;
using IssueRunner.Models;

namespace IssueRunner.Gui.ViewModels;

/// <summary>
/// Result of loading issues for the issue list view.
/// </summary>
public sealed class IssueListLoadResult
{
    public required IReadOnlyList<IssueListItem> Issues { get; init; }

    public required Dictionary<string, ChangeType> IssueChanges { get; init; }

    public required string RepositoryBaseUrl { get; init; }
}

