using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace IssueRunner.Services;

/// <summary>
/// Service for upgrading target frameworks in projects.
/// </summary>
public sealed partial class FrameworkUpgradeService : IFrameworkUpgradeService
{
    private readonly ILogger<FrameworkUpgradeService> _logger;

    private static readonly HashSet<string> LegacyNetFrameworks = new(StringComparer.OrdinalIgnoreCase)
    {
        "net35", "net40", "net45", "net451", "net452", "net46", "net461"
    };

    /// <summary>
    /// Initializes a new instance of the <see cref="FrameworkUpgradeService"/> class.
    /// </summary>
    public FrameworkUpgradeService(ILogger<FrameworkUpgradeService> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public bool UpgradeAllProjectFrameworks(string issueFolderPath, int issueNumber)
    {
        var modified = false;
        var visited = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var csprojPath in Directory.EnumerateFiles(issueFolderPath, "*.csproj", SearchOption.AllDirectories))
        {
            // Skip bin/obj directories
            var parts = csprojPath.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            if (parts.Any(p => p.Equals("bin", StringComparison.OrdinalIgnoreCase) ||
                              p.Equals("obj", StringComparison.OrdinalIgnoreCase)))
            {
                continue;
            }

            var fullPath = Path.GetFullPath(csprojPath);
            if (!visited.Add(fullPath))
            {
                continue;
            }

            if (UpgradeProjectFrameworks(fullPath, issueNumber))
            {
                modified = true;
                var relativePath = Path.GetRelativePath(issueFolderPath, fullPath);
                Console.WriteLine($"[{issueNumber}] Updated target framework(s) to net10.0 in {relativePath}");
            }
        }

        return modified;
    }

    private bool UpgradeProjectFrameworks(string csprojPath, int issueNumber)
    {
        try
        {
            var doc = XDocument.Load(csprojPath);
            var modified = false;

            // Update <TargetFramework> (singular)
            foreach (var element in doc.Descendants()
                .Where(e => e.Name.LocalName == "TargetFramework"))
            {
                if (UpdateFrameworkElement(element, issueNumber))
                {
                    modified = true;
                }
            }

            // Update <TargetFrameworks> (plural)
            foreach (var element in doc.Descendants()
                .Where(e => e.Name.LocalName == "TargetFrameworks"))
            {
                if (UpdateFrameworksElement(element, issueNumber))
                {
                    modified = true;
                }
            }

            if (modified)
            {
                doc.Save(csprojPath);
            }

            return modified;
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "Failed to upgrade frameworks in {Path}", csprojPath);
            return false;
        }
    }

    private bool UpdateFrameworkElement(XElement element, int issueNumber)
    {
        var original = element.Value.Trim();
        if (string.IsNullOrEmpty(original))
        {
            return false;
        }

        var upgraded = MapFrameworkToLatest(original);
        if (upgraded != original)
        {
            element.Value = upgraded;
            return true;
        }

        return false;
    }

    private bool UpdateFrameworksElement(XElement element, int issueNumber)
    {
        var original = element.Value.Trim();
        if (string.IsNullOrEmpty(original))
        {
            return false;
        }

        // Split by semicolon or comma
        var frameworks = original.Split(new[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(f => f.Trim())
            .ToList();

        if (frameworks.Count == 0)
        {
            return false;
        }

        var upgradedFrameworks = new List<string>();
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var modified = false;

        foreach (var tfm in frameworks)
        {
            var upgraded = MapFrameworkToLatest(tfm);
            
            if (upgraded != tfm)
            {
                modified = true;
            }

            if (seen.Add(upgraded))
            {
                upgradedFrameworks.Add(upgraded);
            }
        }

        if (modified)
        {
            element.Value = string.Join(";", upgradedFrameworks);
        }

        return modified;
    }

    private static string MapFrameworkToLatest(string framework)
    {
        var tfm = framework.Trim().ToLowerInvariant();

        // Legacy .NET Framework -> net462
        if (LegacyNetFrameworks.Contains(tfm))
        {
            return "net462";
        }

        // netcoreapp* -> net10.0
        if (tfm.StartsWith("netcoreapp"))
        {
            return "net10.0";
        }

        // netstandard* -> keep as-is (libraries may need to stay on netstandard)
        if (tfm.StartsWith("netstandard"))
        {
            return framework; // Keep original casing
        }

        // net5.0, net6.0, net7.0, net8.0, net9.0 -> net10.0
        var match = NetVersionRegex().Match(tfm);
        if (match.Success)
        {
            if (int.TryParse(match.Groups[1].Value, out var major))
            {
                if (major >= 10)
                {
                    return framework; // Already net10+ - keep original
                }
                if (major >= 5)
                {
                    return "net10.0";
                }
            }
        }

        // Keep everything else unchanged
        return framework;
    }

    [GeneratedRegex(@"^net(\d+)(?:\.\d+)?$")]
    private static partial Regex NetVersionRegex();
}
