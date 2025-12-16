namespace IssueRunner.Models;

/// <summary>
/// Represents a NuGet package reference.
/// </summary>
public sealed class PackageInfo
{
    /// <summary>
    /// Gets or sets the package name.
    /// </summary>
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    /// <summary>
    /// Gets or sets the package version.
    /// </summary>
    [JsonPropertyName("version")]
    public required string Version { get; init; }
}
