namespace IssueRunner.Models;

/// <summary>
/// Represents test execution results for an issue.
/// </summary>
public sealed class IssueResult
{
    /// <summary>
    /// Gets or sets the issue number.
    /// </summary>
    [JsonPropertyName("number")]
    public required int Number { get; init; }

    /// <summary>
    /// Gets or sets the project path.
    /// </summary>
    [JsonPropertyName("project_path")]
    public required string ProjectPath { get; init; }

    /// <summary>
    /// Gets or sets the project style (classic/SDK-style).
    /// </summary>
    [JsonPropertyName("project_style")]
    public string? ProjectStyle { get; init; }

    /// <summary>
    /// Gets or sets the target frameworks.
    /// </summary>
    [JsonPropertyName("target_frameworks")]
    public required List<string> TargetFrameworks { get; init; }

    /// <summary>
    /// Gets or sets the package list (name=version format).
    /// </summary>
    [JsonPropertyName("packages")]
    public required List<string> Packages { get; init; }

    /// <summary>
    /// Gets or sets the package update result (success/fail).
    /// </summary>
    [JsonPropertyName("update_result")]
    public string? UpdateResult { get; init; }

    /// <summary>
    /// Gets or sets the package update output.
    /// </summary>
    [JsonPropertyName("update_output")]
    public string? UpdateOutput { get; init; }

    /// <summary>
    /// Gets or sets the package update error output.
    /// </summary>
    [JsonPropertyName("update_error")]
    public string? UpdateError { get; init; }

    /// <summary>
    /// Gets or sets the restore step result (success/fail/not run).
    /// </summary>
    [JsonPropertyName("restore_result")]
    public string? RestoreResult { get; init; }

    /// <summary>
    /// Gets or sets the restore step output.
    /// </summary>
    [JsonPropertyName("restore_output")]
    public string? RestoreOutput { get; init; }

    /// <summary>
    /// Gets or sets the restore step error output.
    /// </summary>
    [JsonPropertyName("restore_error")]
    public string? RestoreError { get; init; }

    /// <summary>
    /// Gets or sets the build step result (success/fail/not run).
    /// </summary>
    [JsonPropertyName("build_result")]
    public string? BuildResult { get; init; }

    /// <summary>
    /// Gets or sets the build step output.
    /// </summary>
    [JsonPropertyName("build_output")]
    public string? BuildOutput { get; init; }

    /// <summary>
    /// Gets or sets the build step error output.
    /// </summary>
    [JsonPropertyName("build_error")]
    public string? BuildError { get; init; }

    /// <summary>
    /// Gets or sets the test execution result (success/fail/not run).
    /// </summary>
    [JsonPropertyName("test_result")]
    public string? TestResult { get; init; }

    /// <summary>
    /// Gets or sets the test execution output.
    /// </summary>
    [JsonPropertyName("test_output")]
    public string? TestOutput { get; init; }

    /// <summary>
    /// Gets or sets the test execution error output.
    /// </summary>
    [JsonPropertyName("test_error")]
    public string? TestError { get; init; }

    /// <summary>
    /// Gets or sets the test conclusion message.
    /// </summary>
    [JsonPropertyName("test_conclusion")]
    public string? TestConclusion { get; init; }

    /// <summary>
    /// Gets or sets the custom runner scripts used.
    /// </summary>
    [JsonPropertyName("runner_scripts")]
    public List<string>? RunnerScripts { get; init; }

    /// <summary>
    /// Gets or sets the runsettings file path if used.
    /// </summary>
    [JsonPropertyName("runsettings")]
    public string? RunSettings { get; init; }

    /// <summary>
    /// Gets or sets the package feed used for this run.
    /// </summary>
    [JsonPropertyName("feed")]
    public string? Feed { get; init; }

    /// <summary>
    /// Gets or sets the runner expectations parsed from script headers.
    /// </summary>
    [JsonPropertyName("runner_expectations")]
    public List<string>? RunnerExpectations { get; init; }

    /// <summary>
    /// Gets or sets the timestamp when this test was run (ISO 8601 format).
    /// </summary>
    [JsonPropertyName("last_run")]
    public string? LastRun { get; init; }
}




