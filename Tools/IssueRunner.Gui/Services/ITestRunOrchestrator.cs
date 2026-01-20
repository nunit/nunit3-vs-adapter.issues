using Avalonia.Controls;
using IssueRunner.Gui.ViewModels;
using IssueRunner.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace IssueRunner.Gui.Services;

/// <summary>
/// Service responsible for orchestrating test runs, managing status dialogs, and handling baseline operations.
/// </summary>
public interface ITestRunOrchestrator
{
    /// <summary>
    /// Runs tests with options dialog. Returns true if tests were run, false if cancelled.
    /// </summary>
    Task<bool> RunTestsAsync(
        RunOptions? lastRunOptions,
        Action<string> log,
        Action onRepositoryReload,
        Func<IssueListViewModel, Dictionary<int, string>, string, Task> onIssuesReload,
        Func<Window?> getMainWindow,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Runs tests for filtered issue numbers. Returns true if tests were run, false if cancelled.
    /// </summary>
    Task<bool> RunFilteredTestsAsync(
        List<int> issueNumbers,
        RunOptions? lastRunOptions,
        Action<string> log,
        Action onRepositoryReload,
        Func<IssueListViewModel, Dictionary<int, string>, string, Task> onIssuesReload,
        Func<Window?> getMainWindow,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets the baseline from current test results.
    /// </summary>
    Task SetBaselineAsync(
        Action<string> log,
        Action onRepositoryReload);

    /// <summary>
    /// Cancels the current test run if one is in progress.
    /// </summary>
    void CancelCurrentRun();

    /// <summary>
    /// Gets whether a test run is currently in progress.
    /// </summary>
    bool IsRunning { get; }
}
