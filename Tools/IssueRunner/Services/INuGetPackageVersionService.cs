using IssueRunner.Models;
using NuGet.Versioning;

namespace IssueRunner.Services;

public interface INuGetPackageVersionService
{
    Task<Dictionary<string, NuGetVersion>> GetLatestVersionsAsync(
        IEnumerable<string> packageIds,
        PackageFeed feed,
        CancellationToken cancellationToken);
}
