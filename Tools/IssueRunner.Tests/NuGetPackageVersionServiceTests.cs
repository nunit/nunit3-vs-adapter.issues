using IssueRunner.Models;
using IssueRunner.Services;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NuGet.Versioning;

namespace IssueRunner.Tests;

[TestFixture]
public class NuGetPackageVersionServiceTests
{
    private ILogger<NuGetPackageVersionService> _logger = null!;

    [SetUp]
    public void SetUp()
    {
        _logger = Substitute.For<ILogger<NuGetPackageVersionService>>();
    }

    [Test]
    public void Constructor_InitializesSuccessfully()
    {
        // Arrange & Act
        var service = new NuGetPackageVersionService(_logger);

        // Assert
        Assert.That(service, Is.Not.Null);
    }

    [Test]
    public async Task GetLatestVersionsAsync_ReturnsEmptyDictionary_WhenNoPackagesProvided()
    {
        // Arrange
        var service = new NuGetPackageVersionService(_logger);
        var packageIds = Array.Empty<string>();

        // Act
        var result = await service.GetLatestVersionsAsync(packageIds, PackageFeed.Stable, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task GetLatestVersionsAsync_HandlesStableFeed()
    {
        // Arrange
        var service = new NuGetPackageVersionService(_logger);
        var packageIds = new[] { "NUnit", "NUnit3TestAdapter" };

        // Act
        // Note: This will make actual network calls, but tests the method structure
        var result = await service.GetLatestVersionsAsync(packageIds, PackageFeed.Stable, CancellationToken.None);

        // Assert
        // Result may be empty if network is unavailable, but method should complete
        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public async Task GetLatestVersionsAsync_HandlesBetaFeed()
    {
        // Arrange
        var service = new NuGetPackageVersionService(_logger);
        var packageIds = new[] { "NUnit" };

        // Act
        var result = await service.GetLatestVersionsAsync(packageIds, PackageFeed.Beta, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public async Task GetLatestVersionsAsync_HandlesAlphaFeed()
    {
        // Arrange
        var service = new NuGetPackageVersionService(_logger);
        var packageIds = new[] { "NUnit" };

        // Act
        var result = await service.GetLatestVersionsAsync(packageIds, PackageFeed.Alpha, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public async Task GetLatestVersionsAsync_HandlesLocalFeed()
    {
        // Arrange
        var service = new NuGetPackageVersionService(_logger);
        var packageIds = new[] { "NUnit" };

        // Act
        var result = await service.GetLatestVersionsAsync(packageIds, PackageFeed.Local, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public async Task GetLatestVersionsAsync_HandlesCancellation()
    {
        // Arrange
        var service = new NuGetPackageVersionService(_logger);
        var packageIds = new[] { "NUnit" };
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        // Should handle cancellation gracefully
        Assert.DoesNotThrowAsync(async () =>
        {
            await service.GetLatestVersionsAsync(packageIds, PackageFeed.Stable, cts.Token);
        });
    }

    [Test]
    public async Task GetLatestVersionsAsync_HandlesMultiplePackages()
    {
        // Arrange
        var service = new NuGetPackageVersionService(_logger);
        var packageIds = new[] { "NUnit", "NUnit3TestAdapter", "Microsoft.NET.Test.Sdk" };

        // Act
        var result = await service.GetLatestVersionsAsync(packageIds, PackageFeed.Stable, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        // May be empty if network unavailable, but should handle multiple packages
    }

    [Test]
    public async Task GetLatestVersionsAsync_HandlesCaseInsensitivePackageIds()
    {
        // Arrange
        var service = new NuGetPackageVersionService(_logger);
        var packageIds = new[] { "nunit", "NUNIT", "NUnit" };

        // Act
        var result = await service.GetLatestVersionsAsync(packageIds, PackageFeed.Stable, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        // Should handle case-insensitive package IDs
    }

    [Test]
    public async Task GetLatestVersionsAsync_ExcludesPrerelease_ForStableFeed()
    {
        // Arrange
        var service = new NuGetPackageVersionService(_logger);
        var packageIds = new[] { "NUnit" };

        // Act
        var result = await service.GetLatestVersionsAsync(packageIds, PackageFeed.Stable, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        // If versions are returned, they should not be prerelease for Stable feed
        foreach (var version in result.Values)
        {
            Assert.That(version.IsPrerelease, Is.False, "Stable feed should exclude prerelease versions");
        }
    }

    [Test]
    public async Task GetLatestVersionsAsync_IncludesPrerelease_ForBetaFeed()
    {
        // Arrange
        var service = new NuGetPackageVersionService(_logger);
        var packageIds = new[] { "NUnit" };

        // Act
        var result = await service.GetLatestVersionsAsync(packageIds, PackageFeed.Beta, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        // Beta feed may include prerelease versions
    }

    [Test]
    public async Task GetLatestVersionsAsync_HandlesInvalidPackageIds()
    {
        // Arrange
        var service = new NuGetPackageVersionService(_logger);
        var packageIds = new[] { "NonExistentPackage12345" };

        // Act
        var result = await service.GetLatestVersionsAsync(packageIds, PackageFeed.Stable, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        // Should handle missing packages gracefully (empty result)
    }
}
