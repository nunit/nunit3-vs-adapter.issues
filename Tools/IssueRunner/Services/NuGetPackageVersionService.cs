using IssueRunner.Models;
using Microsoft.Extensions.Logging;
using NuGet.Common;
using NuGet.Configuration;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;

namespace IssueRunner.Services;

public sealed class NuGetPackageVersionService : INuGetPackageVersionService
{
    private readonly ILogger<NuGetPackageVersionService> _logger;

    private static readonly string NugetOrg = "https://api.nuget.org/v3/index.json";
    private static readonly string MyGet = "https://www.myget.org/F/nunit/api/v3/index.json";
    private static readonly string LocalFeed = @"C:\local";

    public NuGetPackageVersionService(ILogger<NuGetPackageVersionService> logger)
    {
        _logger = logger;
    }

    public async Task<Dictionary<string, NuGetVersion>> GetLatestVersionsAsync(
        IEnumerable<string> packageIds,
        PackageFeed feed,
        CancellationToken cancellationToken)
    {
        var includePrerelease = feed != PackageFeed.Stable;

        var sources = new List<string> { NugetOrg };
        if (feed == PackageFeed.Alpha)
        {
            sources.Add(MyGet);
        }
        else if (feed == PackageFeed.Local)
        {
            sources.Add(LocalFeed);
        }

        var repositories = sources
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Select(Repository.Factory.GetCoreV3)
            .ToList();

        var cache = new SourceCacheContext();
        var results = new Dictionary<string, NuGetVersion>(StringComparer.OrdinalIgnoreCase);

        foreach (var packageId in packageIds)
        {
            NuGetVersion? best = null;

            foreach (var repo in repositories)
            {
                try
                {
                    var resource = await repo.GetResourceAsync<FindPackageByIdResource>(cancellationToken);
                    var versions = await resource.GetAllVersionsAsync(
                        packageId,
                        cache,
                        NullLogger.Instance,
                        cancellationToken);

                    if (versions == null)
                    {
                        continue;
                    }

                    var filtered = includePrerelease
                        ? versions
                        : versions.Where(v => !v.IsPrerelease);

                    foreach (var v in filtered)
                    {
                        if (best == null || v > best)
                        {
                            best = v;
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to query {Source} for {Package}", repo.PackageSource.Source, packageId);
                }
            }

            if (best != null)
            {
                results[packageId] = best;
            }
        }

        return results;
    }
}
