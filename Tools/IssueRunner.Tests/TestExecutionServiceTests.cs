using IssueRunner.Services;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;
using System.IO;

namespace IssueRunner.Tests;

[TestFixture]
public class TestExecutionServiceTests
{
    private IProcessExecutor? _processExecutor;
    private IProjectAnalyzerService? _projectAnalyzer;
    private ILogger<TestExecutionService>? _logger;
    private TestExecutionService? _service;
    private string? _tempDir;
    private string? _projectPath;
    private string? _issueFolderPath;

    [SetUp]
    public void Setup()
    {
        _processExecutor = Substitute.For<IProcessExecutor>();
        _projectAnalyzer = Substitute.For<IProjectAnalyzerService>();
        _logger = Substitute.For<ILogger<TestExecutionService>>();
        _service = new TestExecutionService(_processExecutor, _projectAnalyzer, _logger);

        // Create temporary directory structure
        _tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(_tempDir);
        _issueFolderPath = Path.Combine(_tempDir, "Issue1");
        Directory.CreateDirectory(_issueFolderPath);
        _projectPath = Path.Combine(_issueFolderPath, "TestProject.csproj");
        File.WriteAllText(_projectPath, "<Project></Project>");

        // Default mock setup
        _projectAnalyzer.UsesTestingPlatform(Arg.Any<string>()).Returns(false);
    }

    [TearDown]
    public void TearDown()
    {
        if (_tempDir != null && Directory.Exists(_tempDir))
        {
            Directory.Delete(_tempDir, true);
        }
    }

    [Test]
    public async Task ExecuteTestsAsync_RunsRestoreBuildThenTest_InSequence()
    {
        // Arrange
        _processExecutor!.ExecuteAsync("dotnet", Arg.Is<string>(s => s.Contains("restore")), Arg.Any<string>(), Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns((0, "Restore output", ""));
        _processExecutor.ExecuteAsync("dotnet", Arg.Is<string>(s => s.Contains("build")), Arg.Any<string>(), Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns((0, "Build output", ""));
        _processExecutor.ExecuteAsync("dotnet", Arg.Is<string>(s => s.Contains("test")), Arg.Any<string>(), Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns((0, "Test output", ""));

        // Act
        var result = await _service!.ExecuteTestsAsync(_projectPath!, _issueFolderPath!, 60, CancellationToken.None);

        // Assert
        Assert.That(result.RestoreResult.Status, Is.EqualTo(StepStatus.Success));
        Assert.That(result.BuildResult.Status, Is.EqualTo(StepStatus.Success));
        Assert.That(result.TestResult.Status, Is.EqualTo(StepStatus.Success));
        Assert.That(result.OverallSuccess, Is.True);
        
        // Verify restore was called first
        await _processExecutor.Received(1).ExecuteAsync("dotnet", Arg.Is<string>(s => s.Contains("restore")), Arg.Any<string>(), Arg.Any<int>(), Arg.Any<CancellationToken>());
        // Verify build was called second
        await _processExecutor.Received(1).ExecuteAsync("dotnet", Arg.Is<string>(s => s.Contains("build")), Arg.Any<string>(), Arg.Any<int>(), Arg.Any<CancellationToken>());
        // Verify test was called third
        await _processExecutor.Received(1).ExecuteAsync("dotnet", Arg.Is<string>(s => s.Contains("test")), Arg.Any<string>(), Arg.Any<int>(), Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task ExecuteTestsAsync_ReturnsRestoreFailed_WhenRestoreFails()
    {
        // Arrange
        _processExecutor!.ExecuteAsync("dotnet", Arg.Is<string>(s => s.Contains("restore")), Arg.Any<string>(), Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns((1, "Restore output", "Restore error"));

        // Act
        var result = await _service!.ExecuteTestsAsync(_projectPath!, _issueFolderPath!, 60, CancellationToken.None);

        // Assert
        Assert.That(result.RestoreResult.Status, Is.EqualTo(StepStatus.Failed));
        Assert.That(result.RestoreResult.Output, Is.EqualTo("Restore output"));
        Assert.That(result.RestoreResult.Error, Is.EqualTo("Restore error"));
        Assert.That(result.BuildResult.Status, Is.EqualTo(StepStatus.NotRun));
        Assert.That(result.TestResult.Status, Is.EqualTo(StepStatus.NotRun));
        Assert.That(result.OverallSuccess, Is.False);
        
        // Verify build and test were NOT called
        await _processExecutor.DidNotReceive().ExecuteAsync("dotnet", Arg.Is<string>(s => s.Contains("build")), Arg.Any<string>(), Arg.Any<int>(), Arg.Any<CancellationToken>());
        await _processExecutor.DidNotReceive().ExecuteAsync("dotnet", Arg.Is<string>(s => s.Contains("test")), Arg.Any<string>(), Arg.Any<int>(), Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task ExecuteTestsAsync_ReturnsBuildFailed_WhenBuildFails()
    {
        // Arrange
        _processExecutor!.ExecuteAsync("dotnet", Arg.Is<string>(s => s.Contains("restore")), Arg.Any<string>(), Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns((0, "Restore output", ""));
        _processExecutor.ExecuteAsync("dotnet", Arg.Is<string>(s => s.Contains("build")), Arg.Any<string>(), Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns((1, "Build output", "Build error"));

        // Act
        var result = await _service!.ExecuteTestsAsync(_projectPath!, _issueFolderPath!, 60, CancellationToken.None);

        // Assert
        Assert.That(result.RestoreResult.Status, Is.EqualTo(StepStatus.Success));
        Assert.That(result.BuildResult.Status, Is.EqualTo(StepStatus.Failed));
        Assert.That(result.BuildResult.Output, Is.EqualTo("Build output"));
        Assert.That(result.BuildResult.Error, Is.EqualTo("Build error"));
        Assert.That(result.TestResult.Status, Is.EqualTo(StepStatus.NotRun));
        Assert.That(result.OverallSuccess, Is.False);
        
        // Verify test was NOT called
        await _processExecutor.DidNotReceive().ExecuteAsync("dotnet", Arg.Is<string>(s => s.Contains("test")), Arg.Any<string>(), Arg.Any<int>(), Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task ExecuteTestsAsync_ReturnsTestFailed_WhenTestFails()
    {
        // Arrange
        _processExecutor!.ExecuteAsync("dotnet", Arg.Is<string>(s => s.Contains("restore")), Arg.Any<string>(), Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns((0, "Restore output", ""));
        _processExecutor.ExecuteAsync("dotnet", Arg.Is<string>(s => s.Contains("build")), Arg.Any<string>(), Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns((0, "Build output", ""));
        _processExecutor.ExecuteAsync("dotnet", Arg.Is<string>(s => s.Contains("test")), Arg.Any<string>(), Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns((1, "Test output", "Test error"));

        // Act
        var result = await _service!.ExecuteTestsAsync(_projectPath!, _issueFolderPath!, 60, CancellationToken.None);

        // Assert
        Assert.That(result.RestoreResult.Status, Is.EqualTo(StepStatus.Success));
        Assert.That(result.BuildResult.Status, Is.EqualTo(StepStatus.Success));
        Assert.That(result.TestResult.Status, Is.EqualTo(StepStatus.Failed));
        Assert.That(result.TestResult.Output, Is.EqualTo("Test output"));
        Assert.That(result.TestResult.Error, Is.EqualTo("Test error"));
        Assert.That(result.OverallSuccess, Is.False);
    }

    [Test]
    public async Task ExecuteTestsAsync_ReturnsAllStepsSuccess_WhenAllStepsPass()
    {
        // Arrange
        _processExecutor!.ExecuteAsync("dotnet", Arg.Is<string>(s => s.Contains("restore")), Arg.Any<string>(), Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns((0, "Restore output", ""));
        _processExecutor.ExecuteAsync("dotnet", Arg.Is<string>(s => s.Contains("build")), Arg.Any<string>(), Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns((0, "Build output", ""));
        _processExecutor.ExecuteAsync("dotnet", Arg.Is<string>(s => s.Contains("test")), Arg.Any<string>(), Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns((0, "Test output", ""));

        // Act
        var result = await _service!.ExecuteTestsAsync(_projectPath!, _issueFolderPath!, 60, CancellationToken.None);

        // Assert
        Assert.That(result.RestoreResult.Status, Is.EqualTo(StepStatus.Success));
        Assert.That(result.BuildResult.Status, Is.EqualTo(StepStatus.Success));
        Assert.That(result.TestResult.Status, Is.EqualTo(StepStatus.Success));
        Assert.That(result.OverallSuccess, Is.True);
    }
}


