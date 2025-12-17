using IssueRunner.Commands;
using IssueRunner.Services;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;

namespace IssueRunner.Tests;

[TestFixture]
public class ResetPackagesCommandTests
{
    private IIssueDiscoveryService? _issueDiscovery;
    private IProjectAnalyzerService? _projectAnalyzer;
    private ILogger<ResetPackagesCommand>? _logger;
    private ResetPackagesCommand? _command;

    [SetUp]
    public void Setup()
    {
        _issueDiscovery = Substitute.For<IIssueDiscoveryService>();
        _projectAnalyzer = Substitute.For<IProjectAnalyzerService>();
        _logger = Substitute.For<ILogger<ResetPackagesCommand>>();
        _command = new ResetPackagesCommand(_issueDiscovery, _projectAnalyzer, _logger);
    }

    [Test]
    public void Constructor_InitializesSuccessfully()
    {
        Assert.That(_command, Is.Not.Null);
    }

    [Test]
    public async Task ExecuteAsync_WithNoIssues_ReturnsSuccess()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), $"reset-test-{Guid.NewGuid()}");
        Directory.CreateDirectory(tempDir);

        _issueDiscovery!.DiscoverIssueFolders(tempDir)
            .Returns(new Dictionary<int, string>());

        try
        {
            // Act
            var result = await _command!.ExecuteAsync(tempDir, null, CancellationToken.None);

            // Assert
            Assert.That(result, Is.EqualTo(0));
        }
        finally
        {
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, recursive: true);
            }
        }
    }

    [Test]
    public void Constructor_AcceptsDependencies()
    {
        // Verify constructor accepts dependencies
        var command = new ResetPackagesCommand(_issueDiscovery!, _projectAnalyzer!, _logger!);
        Assert.That(command, Is.Not.Null);
    }
}
