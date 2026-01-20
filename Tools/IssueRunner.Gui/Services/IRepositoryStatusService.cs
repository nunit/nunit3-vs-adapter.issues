using IssueRunner.Models;

namespace IssueRunner.Gui.Services;

/// <summary>
/// Loads repository status (summary text, package info, folders, metadata).
/// </summary>
public interface IRepositoryStatusService
{
    Task<RepositoryLoadResult> LoadAsync(string repositoryPath, Action<string> log);
}

