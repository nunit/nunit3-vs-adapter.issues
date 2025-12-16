namespace IssueRunner.Models;

/// <summary>
/// Represents issue metadata from GitHub.
/// </summary>
public sealed class IssueMetadata
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
    /// Gets or sets the issue state (open/closed).
    /// </summary>
    [JsonPropertyName("state")]
    public required string State { get; init; }

    /// <summary>
    /// Gets or sets the milestone name.
    /// </summary>
    [JsonPropertyName("milestone")]
    public string? Milestone { get; init; }

    /// <summary>
    /// Gets or sets the issue labels.
    /// </summary>
    [JsonPropertyName("labels")]
    public required List<string> Labels { get; init; }

    /// <summary>
    /// Gets or sets the issue URL.
    /// </summary>
    [JsonPropertyName("url")]
    public required string Url { get; init; }
}
