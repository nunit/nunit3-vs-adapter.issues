using Microsoft.Extensions.Logging;
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

    private readonly ProcessExecutor _processExecutor;
    private readonly ILogger<PackageUpdateService> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="PackageUpdateService"/> class.
    /// </summary>
    public PackageUpdateService(
        ProcessExecutor processExecutor,
        ILogger<PackageUpdateService> logger)
    {
        _processExecutor = processExecutor;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<(bool Success, string Output, string Error)>
        UpdatePackagesAsync(
            string projectPath,
            bool nunitOnly,
            int timeoutSeconds,
            CancellationToken cancellationToken)
    {
        if (nunitOnly)
        {
            return await UpdateNUnitPackagesDirectlyAsync(
                projectPath,
                cancellationToken);
        }

        return await UpdateUsingDotnetOutdatedAsync(
            projectPath,
            timeoutSeconds,
            cancellationToken);
    }

    private async Task<(bool Success, string Output, string Error)>
        UpdateNUnitPackagesDirectlyAsync(
            string projectPath,
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
        CancellationToken cancellationToken)
    {
        // TODO: Query NuGet API for latest versions
        // For now, return hardcoded recent versions
        await Task.CompletedTask;
        
        return new Dictionary<string, string>
        {
            ["NUnit"] = "4.3.1",
            ["NUnit3TestAdapter"] = "4.6.0",
            ["Microsoft.NET.Test.Sdk"] = "17.12.0"
        };
    }

    private async Task<(bool Success, string Output, string Error)>
        UpdateUsingDotnetOutdatedAsync(
            string projectPath,
            int timeoutSeconds,
            CancellationToken cancellationToken)
    {
        var workingDir = Path.GetDirectoryName(projectPath)!;

        var (exitCode, output, error) = await _processExecutor.ExecuteAsync(
            "dotnet",
            "outdated --upgrade",
            workingDir,
            timeoutSeconds,
            cancellationToken);

        var success = exitCode == 0;
        
        return (success, output, error);
    }
}
