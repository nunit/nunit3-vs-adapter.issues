using IssueRunner.Models;

namespace IssueRunner.Services;

/// <summary>
/// Service for comparing test results between baseline and current.
/// </summary>
public interface ITestResultDiffService
{
    /// <summary>
    /// Compares current test results with baseline and returns a list of changes.
    /// </summary>
    /// <param name="repositoryRoot">Root path of the repository.</param>
    /// <returns>List of test result diffs.</returns>
    Task<List<TestResultDiff>> CompareResultsAsync(string repositoryRoot);
}

