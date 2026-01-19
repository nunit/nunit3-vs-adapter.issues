using System.Text.Json.Serialization;

namespace IssueRunner.Models;

/// <summary>
/// Represents NUnit package versions used during a test run.
/// </summary>
public sealed class NUnitPackageVersions
{
    /// <summary>
    /// Gets or sets the dictionary of package names to versions.
    /// </summary>
    [JsonPropertyName("packages")]
    public required Dictionary<string, string> Packages { get; init; }

    /// <summary>
    /// Gets or sets the timestamp when these versions were determined.
    /// </summary>
    [JsonPropertyName("timestamp")]
    public required string Timestamp { get; init; }

    /// <summary>
    /// Gets or sets the package feed used (Stable, Alpha, Local).
    /// </summary>
    [JsonPropertyName("feed")]
    public required string Feed { get; init; }
}

