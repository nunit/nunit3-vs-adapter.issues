using Microsoft.Extensions.Logging;
using System.IO;
using System.Text.RegularExpressions;

namespace IssueRunner.Services;

/// <summary>
/// Implementation of issue discovery service.
/// </summary>
public sealed partial class IssueDiscoveryService : IIssueDiscoveryService
{
    private readonly IEnvironmentService _environmentService;

    private static readonly string[] MarkerFiles =
    [
        "ignore", "ignore.md",
        "explicit", "explicit.md",
        "wip", "wip.md",
        "closedasnotplanned", "closedasnotplanned.md"
    ];

    private static readonly string[] WindowsMarkerFiles =
    [
        "windows", "windows.md"
    ];

    private readonly ILogger<IssueDiscoveryService> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="IssueDiscoveryService"/> class.
    /// </summary>
    public IssueDiscoveryService(IEnvironmentService environmentService,ILogger<IssueDiscoveryService> logger)
    {
        _environmentService = environmentService;
        _logger = logger;
    }

    /// <inheritdoc />
    public Dictionary<int, string> DiscoverIssueFolders()
    {
        var issueFolders = new Dictionary<int, string>();
        var rootPath = _environmentService.Root;
        if (!Directory.Exists(rootPath))
        {
            _logger.LogWarning("Root path does not exist: {Path}", rootPath);
            return issueFolders;
        }
        
        var directories = Directory.GetDirectories(rootPath, "Issue*");
        if (directories.Length == 0)
        {
            _logger.LogWarning($"No folders named Issue* under {rootPath}");
            return issueFolders;
        }
        
        foreach (var directory in directories)
        {
            var folderName = Path.GetFileName(directory);
            var match = IssueNumberRegex().Match(folderName);
            
            if (match.Success && int.TryParse(match.Value, out var issueNumber))
            {
                issueFolders[issueNumber] = directory;
                _logger.LogDebug(
                    "Discovered issue {Number} at {Path}",
                    issueNumber,
                    directory);
            }
        }
        
        _logger.LogDebug(
            "Discovered {Count} issue folders",
            issueFolders.Count);
        
        return issueFolders;
    }

    /// <inheritdoc />
    public bool ShouldSkipIssue(string issueFolderPath)
    {
        var files = new HashSet<string>(
            Directory.GetFiles(issueFolderPath)
                .Select(Path.GetFileName)
                .Where(f => f != null)!,
            StringComparer.OrdinalIgnoreCase);

        foreach (var markerFile in MarkerFiles)
        {
            if (files.Contains(markerFile))
            {
                _logger.LogDebug(
                    "Skipping issue at {Path} due to marker file {Marker}",
                    issueFolderPath,
                    markerFile);
                return true;
            }
        }
        
        return false;
    }

    /// <inheritdoc />
    public bool RequiresWindows(string issueFolderPath)
    {
        foreach (var markerFile in WindowsMarkerFiles)
        {
            var markerPath = Path.Combine(issueFolderPath, markerFile);
            if (File.Exists(markerPath))
            {
                _logger.LogDebug(
                    "Issue at {Path} requires Windows due to marker file {Marker}",
                    issueFolderPath,
                    markerFile);
                return true;
            }
        }
        
        return false;
    }

    [GeneratedRegex(@"\d+")]
    private static partial Regex IssueNumberRegex();
}
