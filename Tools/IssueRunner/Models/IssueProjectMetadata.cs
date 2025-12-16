namespace IssueRunner.Models;

/// <summary>
/// Represents per-issue project metadata combining GitHub info and project details.
/// </summary>
public sealed class IssueProjectMetadata
{
    /// <summary>
    /// Gets or sets the issue number.
    /// </summary>
    [JsonPropertyName("number")]
    public required int Number { get; init; }

    /// <summary>
    /// Gets or sets the issue title.
    /// </summary>
    [JsonPropertyName("title")]
    public required string Title { get; init; }

    /// <summary>
    /// Gets or sets the issue state.
    /// </summary>
    [JsonPropertyName("state")]
    public required string State { get; init; }

    /// <summary>
    /// Gets or sets the milestone.
    /// </summary>
    [JsonPropertyName("milestone")]
    public string? Milestone { get; init; }

    /// <summary>
    /// Gets or sets the labels.
    /// </summary>
    [JsonPropertyName("labels")]
    public required List<string> Labels { get; init; }

    /// <summary>
    /// Gets or sets the issue URL.
    /// </summary>
    [JsonPropertyName("url")]
    public required string Url { get; init; }

    /// <summary>
    /// Gets or sets the relative path to the project file.
    /// </summary>
    [JsonPropertyName("project_path")]
    public required string ProjectPath { get; init; }

    /// <summary>
    /// Gets or sets the target frameworks.
    /// </summary>
    [JsonPropertyName("target_frameworks")]
    public required List<string> TargetFrameworks { get; init; }

    /// <summary>
    /// Gets or sets the package references.
    /// </summary>
    [JsonPropertyName("packages")]
    public required List<PackageInfo> Packages { get; init; }
}
