using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace IssueRunner.Services
{
    public interface IMarkerService
    {
        /// <summary>
        /// Checks if an issue folder should be skipped based on marker files.
        /// </summary>
        /// <param name="issueFolderPath">Path to the issue folder.</param>
        /// <returns>True if the folder should be skipped.</returns>
        bool ShouldSkipIssue(string issueFolderPath);

        /// <summary>
        /// Gets the list of marker files that indicate an issue should be skipped.
        /// </summary>
        /// <returns>Enumerable of marker file names (case-insensitive matching).</returns>
        IEnumerable<string> GetMarkerFiles();

        string GetMarkerReason(string folderPath);
    }

    public class MarkerService(ILogger<MarkerService> logger) : IMarkerService
    {
        private static readonly string[] MarkerFiles =
        [
            "ignore", "ignore.md",
            "explicit", "explicit.md",
            "wip", "wip.md",
            "gui", "gui.md",
            "closedasnotplanned", "closedasnotplanned.md",
            "closednotplanned", "closednotplanned.md"
        ];


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
                    logger.LogDebug(
                        "Skipping issue at {Path} due to marker file {Marker}",
                        issueFolderPath,
                        markerFile);
                    return true;
                }
            }

            return false;
        }

        /// <inheritdoc />
        public IEnumerable<string> GetMarkerFiles()
        {
            return MarkerFiles;
        }

        public string GetMarkerReason(string folderPath)
        {
            var files = Directory.GetFiles(folderPath)
                .Select(Path.GetFileName)
                .Where(f => f != null)
                .Select(f => f!.ToLowerInvariant())
                .ToHashSet();

            // Get marker files from the service (single source of truth)
            var markerFiles = GetMarkerFiles()
                .Select(m => m.ToLowerInvariant())
                .ToList();

            // Find which marker file exists
            string? foundMarker = null;
            foreach (var markerFile in markerFiles)
            {
                if (files.Contains(markerFile))
                {
                    foundMarker = markerFile;
                    break;
                }
            }

            // Map marker file name to display reason
            // State column should just show "skipped", detailed reason goes below (without "Skipped" prefix)
            string markerReason;
            if (foundMarker == null)
            {
                markerReason = "marker file";
            }
            else
            {
                // Remove .md extension for matching
                var markerBase = foundMarker.Replace(".md", "");

                markerReason = markerBase switch
                {
                    "ignore" => "Ignored",
                    "explicit" => "Explicit",
                    "gui" => "GUI",
                    "wip" => "WIP",
                    "closednotplanned" => "Closed Not Planned",
                    "closedasnotplanned" => "Closed As Not Planned",
                    _ => "marker file"
                };
            }

            return markerReason;
        }
    }
}
