using System.Text.Json.Serialization;

namespace IssueRunner.Models;

/// <summary>
/// Represents a test result entry for tracking pass/fail status.
/// </summary>
public sealed class TestResultEntry
{
    /// <summary>
    /// Gets or sets the issue identifier (e.g., "Issue1015").
    /// </summary>
    [JsonPropertyName("issue")]
    public required string Issue { get; init; }

    /// <summary>
    /// Gets or sets the project path relative to repository root.
    /// </summary>
    [JsonPropertyName("project")]
    public required string Project { get; init; }

    /// <summary>
    /// Gets or sets the timestamp of the last test run (ISO 8601 format).
    /// </summary>
    [JsonPropertyName("last_run")]
    public required string LastRun { get; init; }

    /// <summary>
    /// Gets or sets the test result status ("success" or "fail").
    /// </summary>
    [JsonPropertyName("test_result")]
    public required string TestResult { get; init; }
}

/// <summary>
/// Container for test result lists (pass or fail).
/// </summary>
public sealed class TestResultList
{
    /// <summary>
    /// Gets or sets the list of test results.
    /// </summary>
    [JsonPropertyName("test_results")]
    public required List<TestResultEntry> TestResults { get; init; }
}

