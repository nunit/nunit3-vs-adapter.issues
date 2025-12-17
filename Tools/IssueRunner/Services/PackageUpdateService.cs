using IssueRunner.Models;
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

    private readonly IProcessExecutor _processExecutor;
    private readonly ILogger<PackageUpdateService> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="PackageUpdateService"/> class.
    /// </summary>
    public PackageUpdateService(
        IProcessExecutor processExecutor,
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
        // TODO: Query NuGet API (and MyGet for Alpha) for latest versions based on feed
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
        
        if (feed == PackageFeed.Beta || feed == PackageFeed.Alpha)
        {
            args += " --pre-release Always";
        }
        
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
