namespace IssueRunner.Models;

/// <summary>
/// Represents a package update operation log entry.
/// </summary>
public sealed class PackageUpdateLog
{
    /// <summary>
    /// Gets or sets the update phase.
    /// </summary>
    [JsonPropertyName("phase")]
    public required string Phase { get; init; }

    /// <summary>
    /// Gets or sets the issue number.
    /// </summary>
    [JsonPropertyName("issue")]
    public required int Issue { get; init; }

    /// <summary>
    /// Gets or sets the target project path.
    /// </summary>
    [JsonPropertyName("target")]
    public required string Target { get; init; }

    /// <summary>
    /// Gets or sets the NuGet feed used.
    /// </summary>
    [JsonPropertyName("feed")]
    public string? Feed { get; init; }

    /// <summary>
    /// Gets or sets the prerelease mode.
    /// </summary>
    [JsonPropertyName("pre_mode")]
    public string? PreMode { get; init; }

    /// <summary>
    /// Gets or sets the current package versions before update.
    /// </summary>
    [JsonPropertyName("current_versions")]
    public List<string>? CurrentVersions { get; init; }

    /// <summary>
    /// Gets or sets the packages being updated.
    /// </summary>
    [JsonPropertyName("packages")]
    public List<PackageInfo>? Packages { get; init; }

    /// <summary>
    /// Gets or sets the target frameworks.
    /// </summary>
    [JsonPropertyName("target_frameworks")]
    public List<string>? TargetFrameworks { get; init; }

    /// <summary>
    /// Gets or sets the version that was updated to.
    /// </summary>
    [JsonPropertyName("to_version")]
    public string? ToVersion { get; init; }
}





