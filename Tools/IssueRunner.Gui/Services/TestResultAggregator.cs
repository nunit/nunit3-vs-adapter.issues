using IssueRunner.Models;
using IssueRunner.Services;

namespace IssueRunner.Gui.Services;

/// <summary>
/// Default implementation that aggregates raw <see cref="IssueResult"/> rows into
/// a single status per issue, matching the rules used in the repository summary.
/// </summary>
public sealed class TestResultAggregator : ITestResultAggregator
{
    public IReadOnlyList<AggregatedIssueResult> AggregatePerIssue(
        Dictionary<int, string> folders,
        IReadOnlyList<IssueResult> allResults,
        IMarkerService markerService,
        Action<string>? log = null)
    {
        var resultsByIssue = allResults
            .GroupBy(r => r.Number)
            .ToDictionary(g => g.Key, g => g.ToList());

        var aggregated = new List<AggregatedIssueResult>();

        foreach (var (issueNumber, folderPath) in folders)
        {
            var status = AggregatedIssueStatus.NotTested;
            string? lastRun = null;

            try
            {
                if (markerService.ShouldSkipIssue(folderPath))
                {
                    status = AggregatedIssueStatus.Skipped;
                }
                else if (resultsByIssue.TryGetValue(issueNumber, out var issueResults))
                {
                    // Restore failure has highest priority after skipped
                    var restoreFailure = issueResults.FirstOrDefault(r =>
                        r.RestoreResult == "fail" ||
                        !string.IsNullOrWhiteSpace(r.RestoreError));

                    if (restoreFailure != null)
                    {
                        status = AggregatedIssueStatus.NotRestored;
                        lastRun = restoreFailure.LastRun;
                    }
                    else if (issueResults.Any(r => r.BuildResult == "fail" || r.BuildResult == "not compile"))
                    {
                        status = AggregatedIssueStatus.NotCompiling;
                        lastRun = issueResults
                            .Where(r => r.LastRun != null)
                            .Select(r => r.LastRun!)
                            .DefaultIfEmpty()
                            .Max();
                    }
                    else
                    {
                        // Determine status from test_result on the most recent row
                        var mostRecent = issueResults
                            .OrderByDescending(r => r.LastRun)
                            .FirstOrDefault();

                        lastRun = mostRecent?.LastRun;

                        if (mostRecent?.TestResult == "success")
                        {
                            status = AggregatedIssueStatus.Passed;
                        }
                        else if (mostRecent?.TestResult == "fail")
                        {
                            status = AggregatedIssueStatus.Failed;
                        }
                        else
                        {
                            status = AggregatedIssueStatus.NotTested;
                        }
                    }
                }
                else
                {
                    // No entry in results.json
                    status = AggregatedIssueStatus.NotTested;
                }
            }
            catch (Exception ex)
            {
                log?.Invoke($"Warning: Failed to aggregate results for Issue {issueNumber}: {ex.Message}");
                status = AggregatedIssueStatus.NotTested;
            }

            aggregated.Add(new AggregatedIssueResult
            {
                Number = issueNumber,
                Status = status,
                LastRun = lastRun
            });
        }

        return aggregated;
    }
}

