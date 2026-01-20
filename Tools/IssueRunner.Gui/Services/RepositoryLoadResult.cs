using IssueRunner.Models;

namespace IssueRunner.Gui.Services;

/// <summary>
/// DTO returned by repository status service.
/// </summary>
public sealed class RepositoryLoadResult
{
    public required Dictionary<int, string> Folders { get; init; }
    public required int MetadataCount { get; init; }
    public required List<int> FoldersWithoutMetadata { get; init; }
    public required List<int> MetadataWithoutFolders { get; init; }
    public required string SummaryText { get; init; }
    public required string BaselineNUnitPackages { get; init; }
    public required string CurrentNUnitPackages { get; init; }
    public required int PassedCount { get; init; }
    public required int FailedCount { get; init; }
    public required int SkippedCount { get; init; }
    public required int NotRestoredCount { get; init; }
    public required int NotCompilingCount { get; init; }
    public required int NotTestedCount { get; init; }
}

