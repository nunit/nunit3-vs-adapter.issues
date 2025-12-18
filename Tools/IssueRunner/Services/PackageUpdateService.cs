using IssueRunner.Models;
using Microsoft.Extensions.Logging;
using NuGet.Versioning;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml.Linq;

namespace IssueRunner.Services;

/// <summary>
/// Implementation of package update service.
/// </summary>
public sealed class PackageUpdateService : IPackageUpdateService
{
    private static readonly string[] NUnitPackages =
    [
        "NUnit",
        "NUnit3TestAdapter",
        "Microsoft.NET.Test.Sdk"
    ];

    private readonly IProcessExecutor _processExecutor;
    private readonly ILogger<PackageUpdateService> _logger;
    private readonly IHttpClientFactory _httpClientFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="PackageUpdateService"/> class.
    /// </summary>
    public PackageUpdateService(
        IProcessExecutor processExecutor,
        IHttpClientFactory httpClientFactory,
        ILogger<PackageUpdateService> logger)
    {
        _processExecutor = processExecutor;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<(bool Success, string Output, string Error)>
        UpdatePackagesAsync(
            string projectPath,
            bool nunitOnly,
            PackageFeed feed,
            int timeoutSeconds,
            CancellationToken cancellationToken)
    {
        // For Alpha feed, ensure MyGet source is configured
        if (feed == PackageFeed.Alpha)
        {
            await EnsureMyGetSourceAsync(cancellationToken);
        }

        if (nunitOnly)
        {
            return await UpdateNUnitPackagesDirectlyAsync(
                projectPath,
                feed,
                cancellationToken);
        }

        // For Stable feed, make sure we can move off prereleases even when the
        // current version is a higher prerelease (dotnet-outdated won't "downgrade").
        if (feed == PackageFeed.Stable)
        {
            try
            {
                await UpdateNUnitPackagesDirectlyAsync(projectPath, feed, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "Stable pre-pass package update failed; continuing with dotnet-outdated.");
            }
        }

        return await UpdateUsingDotnetOutdatedAsync(
            projectPath,
            feed,
            timeoutSeconds,
            cancellationToken);
    }

    private async Task EnsureMyGetSourceAsync(CancellationToken cancellationToken)
    {
        const string mygetSourceName = "nunit-myget";
        const string mygetUrl = "https://www.myget.org/F/nunit/api/v3/index.json";

        try
        {
            // Check if source already exists
            var (exitCode, output, _) = await _processExecutor.ExecuteAsync(
                "dotnet",
                "nuget list source",
                Directory.GetCurrentDirectory(),
                30,
                cancellationToken);

            if (exitCode == 0 && output.Contains(mygetSourceName))
            {
                _logger.LogDebug("MyGet source already configured");
                return;
            }

            // Add MyGet source
            var (addExitCode, addOutput, addError) = await _processExecutor.ExecuteAsync(
                "dotnet",
                $"nuget add source {mygetUrl} --name {mygetSourceName}",
                Directory.GetCurrentDirectory(),
                30,
                cancellationToken);

            if (addExitCode == 0)
            {
                _logger.LogDebug("Added MyGet source for Alpha feed");
            }
            else
            {
                _logger.LogWarning("Failed to add MyGet source: {Error}", addError);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error configuring MyGet source");
        }
    }

    private async Task<(bool Success, string Output, string Error)>
        UpdateNUnitPackagesDirectlyAsync(
            string projectPath,
            PackageFeed feed,
            CancellationToken cancellationToken)
    {
        try
        {
            var doc = XDocument.Load(projectPath);
            var root = doc.Root;

            if (root == null)
            {
                return (false, "", "Invalid project file");
            }

            var latestVersions = await GetLatestNUnitVersionsAsync(
                feed,
                cancellationToken);
            var updated = false;

            foreach (var packageRef in root.Descendants("PackageReference"))
            {
                var name = packageRef.Attribute("Include")?.Value;
                
                if (name != null && latestVersions.TryGetValue(name, out var version))
                {
                    var versionAttr = packageRef.Attribute("Version");
                    if (versionAttr != null)
                    {
                        versionAttr.Value = version;
                        updated = true;
                        _logger.LogDebug(
                            "Updated {Package} to {Version}",
                            name,
                            version);
                    }
                }
            }

            if (updated)
            {
                doc.Save(projectPath);
                return (true, "Updated NUnit packages via text substitution", "");
            }

            return (true, "No NUnit packages to update", "");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating packages directly");
            return (false, "", ex.Message);
        }
    }

    private async Task<Dictionary<string, string>> GetLatestNUnitVersionsAsync(
        PackageFeed feed,
        CancellationToken cancellationToken)
    {
        var includePrerelease = feed is PackageFeed.Beta or PackageFeed.Alpha;

        try
        {
            var client = _httpClientFactory.CreateClient();
            var latest = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (var packageId in NUnitPackages)
            {
                var version = await TryGetLatestNuGetOrgVersionAsync(
                    client,
                    packageId,
                    includePrerelease,
                    cancellationToken);

                if (version != null)
                {
                    latest[packageId] = version.ToNormalizedString();
                }
            }

            if (latest.Count > 0)
            {
                return latest;
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to query NuGet for latest versions; falling back to defaults.");
        }

        // Fallback (best-effort) defaults.
        return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["NUnit"] = "4.4.0",
            ["NUnit3TestAdapter"] = "6.0.0",
            ["Microsoft.NET.Test.Sdk"] = "18.0.1"
        };
    }

    private sealed class FlatContainerIndex
    {
        [JsonPropertyName("versions")]
        public List<string>? Versions { get; set; }
    }

    private static async Task<NuGetVersion?> TryGetLatestNuGetOrgVersionAsync(
        HttpClient client,
        string packageId,
        bool includePrerelease,
        CancellationToken cancellationToken)
    {
        var lowerId = packageId.ToLowerInvariant();
        var url = $"https://api.nuget.org/v3-flatcontainer/{lowerId}/index.json";

        using var response = await client.GetAsync(url, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        var index = await JsonSerializer.DeserializeAsync<FlatContainerIndex>(
            stream,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);

        if (index?.Versions is not { Count: > 0 })
        {
            return null;
        }

        NuGetVersion? best = null;
        foreach (var raw in index.Versions)
        {
            if (!NuGetVersion.TryParse(raw, out var parsed))
            {
                continue;
            }

            if (!includePrerelease && parsed.IsPrerelease)
            {
                continue;
            }

            if (best == null || parsed > best)
            {
                best = parsed;
            }
        }

        return best;
    }

    private async Task<(bool Success, string Output, string Error)>
        UpdateUsingDotnetOutdatedAsync(
            string projectPath,
            PackageFeed feed,
            int timeoutSeconds,
            CancellationToken cancellationToken)
    {
        var workingDir = Path.GetDirectoryName(projectPath)!;

        // Add MyGet source for Alpha feed
        if (feed == PackageFeed.Alpha)
        {
            var addSourceResult = await AddMyGetSourceAsync(workingDir, timeoutSeconds, cancellationToken);
            if (!addSourceResult.Success)
            {
                return addSourceResult;
            }
        }

        // Build command based on feed
        var args = "outdated --upgrade";

        // dotnet-outdated defaults to "--pre-release Auto", which will keep using prereleases
        // if the project is already on a prerelease version. For Stable runs we want to force
        // stable-only resolution.
        args += feed switch
        {
            PackageFeed.Stable => " --pre-release Never",
            PackageFeed.Beta or PackageFeed.Alpha => " --pre-release Always",
            _ => ""
        };
        
        var (exitCode, output, error) = await _processExecutor.ExecuteAsync(
            "dotnet",
            args,
            workingDir,
            timeoutSeconds,
            cancellationToken);

        var success = exitCode == 0;
        
        return (success, output, error);
    }

    private async Task<(bool Success, string Output, string Error)>
        AddMyGetSourceAsync(
            string workingDir,
            int timeoutSeconds,
            CancellationToken cancellationToken)
    {
        const string myGetSource = "https://www.myget.org/F/nunit/api/v3/index.json";
        const string sourceName = "nunit-myget";

        // Check if source already exists
        var (checkCode, checkOutput, checkError) = await _processExecutor.ExecuteAsync(
            "dotnet",
            "nuget list source",
            workingDir,
            timeoutSeconds,
            cancellationToken);

        if (checkCode == 0 && checkOutput.Contains(sourceName))
        {
            _logger.LogDebug("MyGet source already configured");
            return (true, "MyGet source already exists", "");
        }

        // Add the source
        var (exitCode, output, error) = await _processExecutor.ExecuteAsync(
            "dotnet",
            $"nuget add source {myGetSource} --name {sourceName}",
            workingDir,
            timeoutSeconds,
            cancellationToken);

        if (exitCode == 0)
        {
            _logger.LogDebug("Added MyGet source for NUnit packages");
            return (true, "Added MyGet source", "");
        }

        _logger.LogWarning("Failed to add MyGet source: {Error}", error);
        return (false, output, error);
    }
}
