using IssueRunner.Models;
using Microsoft.Extensions.Logging;
using System.Xml.Linq;

namespace IssueRunner.Services;

/// <summary>
/// Implementation of project analyzer service.
/// </summary>
public sealed class ProjectAnalyzerService : IProjectAnalyzerService
{
    private readonly ILogger<ProjectAnalyzerService> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectAnalyzerService"/> class.
    /// </summary>
    public ProjectAnalyzerService(ILogger<ProjectAnalyzerService> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public List<string> FindProjectFiles(string issueFolderPath)
    {
        if (!Directory.Exists(issueFolderPath))
        {
            return [];
        }

        return Directory.GetFiles(
            issueFolderPath,
            "*.csproj",
            SearchOption.AllDirectories).ToList();
    }

    /// <inheritdoc />
    public (List<string> TargetFrameworks, List<PackageInfo> Packages)
        ParseProjectFile(string projectFilePath)
    {
        try
        {
            var doc = XDocument.Load(projectFilePath);
            var root = doc.Root;

            if (root == null)
            {
                return ([], []);
            }

            var frameworks = ParseTargetFrameworks(root);
            var packages = ParsePackageReferences(root, projectFilePath);

            return (frameworks, packages);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error parsing project file {Path}",
                projectFilePath);
            return ([], []);
        }
    }

    /// <inheritdoc />
    public string GetProjectStyle(string projectFilePath)
    {
        try
        {
            var doc = XDocument.Load(projectFilePath);
            var root = doc.Root;

            if (root?.Attribute("Sdk") != null)
            {
                return "SDK-style";
            }

            return "classic";
        }
        catch
        {
            return "unknown";
        }
    }

    /// <inheritdoc />
    public bool UsesTestingPlatform(string projectFilePath)
    {
        try
        {
            var doc = XDocument.Load(projectFilePath);
            var root = doc.Root;

            if (root == null)
            {
                return false;
            }

            var enableNUnitRunner = root.Descendants("EnableNUnitRunner").FirstOrDefault();
            return enableNUnitRunner != null && enableNUnitRunner.Value.Equals("true", StringComparison.OrdinalIgnoreCase);
        }
        catch
        {
            return false;
        }
    }

    private static List<string> ParseTargetFrameworks(XElement root)
    {
        var frameworks = new List<string>();

        var tfmElement = root.Descendants("TargetFramework").FirstOrDefault();
        if (tfmElement != null)
        {
            frameworks.Add(tfmElement.Value);
        }

        var tfmsElement = root.Descendants("TargetFrameworks").FirstOrDefault();
        if (tfmsElement != null)
        {
            frameworks.AddRange(
                tfmsElement.Value.Split(';', StringSplitOptions.RemoveEmptyEntries));
        }

        return frameworks;
    }

    private List<PackageInfo> ParsePackageReferences(
        XElement root,
        string projectFilePath)
    {
        var packages = new List<PackageInfo>();

        foreach (var packageRef in root.Descendants("PackageReference"))
        {
            var name = packageRef.Attribute("Include")?.Value;
            var version = packageRef.Attribute("Version")?.Value;

            if (name != null && version != null)
            {
                packages.Add(new PackageInfo
                {
                    Name = name,
                    Version = version
                });
            }
        }

        var packagesConfigPath = Path.Combine(
            Path.GetDirectoryName(projectFilePath)!,
            "packages.config");

        if (File.Exists(packagesConfigPath))
        {
            packages.AddRange(ParsePackagesConfig(packagesConfigPath));
        }

        return packages;
    }

    private List<PackageInfo> ParsePackagesConfig(string packagesConfigPath)
    {
        var packages = new List<PackageInfo>();

        try
        {
            var doc = XDocument.Load(packagesConfigPath);
            
            foreach (var package in doc.Descendants("package"))
            {
                var id = package.Attribute("id")?.Value;
                var version = package.Attribute("version")?.Value;

                if (id != null && version != null)
                {
                    packages.Add(new PackageInfo
                    {
                        Name = id,
                        Version = version
                    });
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error parsing packages.config at {Path}",
                packagesConfigPath);
        }

        return packages;
    }
}
