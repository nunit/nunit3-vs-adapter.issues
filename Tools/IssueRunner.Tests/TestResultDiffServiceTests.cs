using IssueRunner.Models;
using IssueRunner.Services;
using Microsoft.Extensions.Logging;

namespace IssueRunner.Tests;

[TestFixture]
public class TestResultDiffServiceTests
{
    private ILogger<TestResultDiffService> _logger = null!;
    private IEnvironmentService _environmentService = null!;
    private string _testRoot = null!;
    private string _dataDir = null!;

    [SetUp]
    public void SetUp()
    {
        _logger = Substitute.For<ILogger<TestResultDiffService>>();
        _testRoot = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_testRoot);
        _dataDir = Path.Combine(_testRoot, ".nunit", "IssueRunner");
        Directory.CreateDirectory(_dataDir);
        
        _environmentService = Substitute.For<IEnvironmentService>();
        _environmentService.GetDataDirectory(Arg.Any<string>()).Returns(_dataDir);
    }

    [TearDown]
    public void TearDown()
    {
        if (Directory.Exists(_testRoot))
        {
            Directory.Delete(_testRoot, true);
        }
    }

    [Test]
    public async Task CompareResultsAsync_ReturnsEmptyList_WhenNoResultsFiles()
    {
        // Arrange
        var service = new TestResultDiffService(_environmentService, _logger);

        // Act
        var result = await service.CompareResultsAsync(_testRoot);

        // Assert
        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task CompareResultsAsync_DetectsRegression_WhenSuccessBecomesFail()
    {
        // Arrange
        var baseline = new List<IssueResult>
        {
            new IssueResult { Number = 1, ProjectPath = "Test.csproj", TargetFrameworks = [], Packages = [], TestResult = "success" }
        };
        var current = new List<IssueResult>
        {
            new IssueResult { Number = 1, ProjectPath = "Test.csproj", TargetFrameworks = [], Packages = [], TestResult = "fail" }
        };
        
        await WriteResultsFile("results-baseline.json", baseline);
        await WriteResultsFile("results.json", current);
        
        var service = new TestResultDiffService(_environmentService, _logger);

        // Act
        var result = await service.CompareResultsAsync(_testRoot);

        // Assert
        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result[0].ChangeType, Is.EqualTo(ChangeType.Regression));
        Assert.That(result[0].BaselineStatus, Is.EqualTo("success"));
        Assert.That(result[0].CurrentStatus, Is.EqualTo("fail"));
    }

    [Test]
    public async Task CompareResultsAsync_DetectsFixed_WhenFailBecomesSuccess()
    {
        // Arrange
        var baseline = new List<IssueResult>
        {
            new IssueResult { Number = 1, ProjectPath = "Test.csproj", TargetFrameworks = [], Packages = [], TestResult = "fail" }
        };
        var current = new List<IssueResult>
        {
            new IssueResult { Number = 1, ProjectPath = "Test.csproj", TargetFrameworks = [], Packages = [], TestResult = "success" }
        };
        
        await WriteResultsFile("results-baseline.json", baseline);
        await WriteResultsFile("results.json", current);
        
        var service = new TestResultDiffService(_environmentService, _logger);

        // Act
        var result = await service.CompareResultsAsync(_testRoot);

        // Assert
        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result[0].ChangeType, Is.EqualTo(ChangeType.Fixed));
        Assert.That(result[0].BaselineStatus, Is.EqualTo("fail"));
        Assert.That(result[0].CurrentStatus, Is.EqualTo("success"));
    }

    [Test]
    public async Task CompareResultsAsync_DetectsCompileToFail_WhenNotRunBecomesFail()
    {
        // Arrange
        var baseline = new List<IssueResult>
        {
            new IssueResult { Number = 1, ProjectPath = "Test.csproj", TargetFrameworks = [], Packages = [], TestResult = null }
        };
        var current = new List<IssueResult>
        {
            new IssueResult { Number = 1, ProjectPath = "Test.csproj", TargetFrameworks = [], Packages = [], TestResult = "fail" }
        };
        
        await WriteResultsFile("results-baseline.json", baseline);
        await WriteResultsFile("results.json", current);
        
        var service = new TestResultDiffService(_environmentService, _logger);

        // Act
        var result = await service.CompareResultsAsync(_testRoot);

        // Assert
        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result[0].ChangeType, Is.EqualTo(ChangeType.CompileToFail));
    }

    [Test]
    public async Task CompareResultsAsync_SkipsSkipped_WhenFailBecomesSkipped()
    {
        // Arrange
        var baseline = new List<IssueResult>
        {
            new IssueResult { Number = 1, ProjectPath = "Test.csproj", TargetFrameworks = [], Packages = [], TestResult = "fail" }
        };
        var current = new List<IssueResult>
        {
            new IssueResult { Number = 1, ProjectPath = "Test.csproj", TargetFrameworks = [], Packages = [], TestResult = "skipped" }
        };
        
        await WriteResultsFile("results-baseline.json", baseline);
        await WriteResultsFile("results.json", current);
        
        var service = new TestResultDiffService(_environmentService, _logger);

        // Act
        var result = await service.CompareResultsAsync(_testRoot);

        // Assert
        Assert.That(result, Is.Empty, "Skipped changes should be excluded");
    }

    [Test]
    public async Task CompareResultsAsync_DetectsOther_WhenStatusChangesToUnknown()
    {
        // Arrange
        var baseline = new List<IssueResult>
        {
            new IssueResult { Number = 1, ProjectPath = "Test.csproj", TargetFrameworks = [], Packages = [], TestResult = "success" }
        };
        var current = new List<IssueResult>
        {
            new IssueResult { Number = 1, ProjectPath = "Test.csproj", TargetFrameworks = [], Packages = [], TestResult = "unknown" }
        };
        
        await WriteResultsFile("results-baseline.json", baseline);
        await WriteResultsFile("results.json", current);
        
        var service = new TestResultDiffService(_environmentService, _logger);

        // Act
        var result = await service.CompareResultsAsync(_testRoot);

        // Assert
        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result[0].ChangeType, Is.EqualTo(ChangeType.Other));
    }

    [Test]
    public async Task CompareResultsAsync_IgnoresNoChange_WhenStatusUnchanged()
    {
        // Arrange
        var baseline = new List<IssueResult>
        {
            new IssueResult { Number = 1, ProjectPath = "Test.csproj", TargetFrameworks = [], Packages = [], TestResult = "success" }
        };
        var current = new List<IssueResult>
        {
            new IssueResult { Number = 1, ProjectPath = "Test.csproj", TargetFrameworks = [], Packages = [], TestResult = "success" }
        };
        
        await WriteResultsFile("results-baseline.json", baseline);
        await WriteResultsFile("results.json", current);
        
        var service = new TestResultDiffService(_environmentService, _logger);

        // Act
        var result = await service.CompareResultsAsync(_testRoot);

        // Assert
        Assert.That(result, Is.Empty, "No changes should be detected");
    }

    [Test]
    public async Task CompareResultsAsync_HandlesMultipleIssues()
    {
        // Arrange
        var baseline = new List<IssueResult>
        {
            new IssueResult { Number = 1, ProjectPath = "Test1.csproj", TargetFrameworks = [], Packages = [], TestResult = "success" },
            new IssueResult { Number = 2, ProjectPath = "Test2.csproj", TargetFrameworks = [], Packages = [], TestResult = "fail" }
        };
        var current = new List<IssueResult>
        {
            new IssueResult { Number = 1, ProjectPath = "Test1.csproj", TargetFrameworks = [], Packages = [], TestResult = "fail" },
            new IssueResult { Number = 2, ProjectPath = "Test2.csproj", TargetFrameworks = [], Packages = [], TestResult = "success" }
        };
        
        await WriteResultsFile("results-baseline.json", baseline);
        await WriteResultsFile("results.json", current);
        
        var service = new TestResultDiffService(_environmentService, _logger);

        // Act
        var result = await service.CompareResultsAsync(_testRoot);

        // Assert
        Assert.That(result, Has.Count.EqualTo(2));
        Assert.That(result.Any(r => r.ChangeType == ChangeType.Regression), Is.True);
        Assert.That(result.Any(r => r.ChangeType == ChangeType.Fixed), Is.True);
    }

    [Test]
    public async Task CompareResultsAsync_HandlesNewIssues_InCurrent()
    {
        // Arrange
        var baseline = new List<IssueResult>();
        var current = new List<IssueResult>
        {
            new IssueResult { Number = 1, ProjectPath = "Test.csproj", TargetFrameworks = [], Packages = [], TestResult = "success" }
        };
        
        await WriteResultsFile("results-baseline.json", baseline);
        await WriteResultsFile("results.json", current);
        
        var service = new TestResultDiffService(_environmentService, _logger);

        // Act
        var result = await service.CompareResultsAsync(_testRoot);

        // Assert
        // "not run" -> "success" is actually a change (Fixed type), so it should create a diff
        // The test expectation was incorrect - new issues appearing with success status is a change
        Assert.That(result, Has.Count.EqualTo(1), "New issues with success status should create a diff");
        Assert.That(result[0].ChangeType, Is.EqualTo(ChangeType.Fixed), "not run -> success should be Fixed");
    }

    [Test]
    public async Task CompareResultsAsync_HandlesMissingIssues_InCurrent()
    {
        // Arrange
        var baseline = new List<IssueResult>
        {
            new IssueResult { Number = 1, ProjectPath = "Test.csproj", TargetFrameworks = [], Packages = [], TestResult = "success" }
        };
        var current = new List<IssueResult>();
        
        await WriteResultsFile("results-baseline.json", baseline);
        await WriteResultsFile("results.json", current);
        
        var service = new TestResultDiffService(_environmentService, _logger);

        // Act
        var result = await service.CompareResultsAsync(_testRoot);

        // Assert
        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result[0].CurrentStatus, Is.EqualTo("not run"));
    }

    [Test]
    public async Task CompareResultsAsync_NormalizesStatusValues()
    {
        // Arrange - using different status formats
        var baseline = new List<IssueResult>
        {
            new IssueResult { Number = 1, ProjectPath = "Test.csproj", TargetFrameworks = [], Packages = [], TestResult = "Pass" }
        };
        var current = new List<IssueResult>
        {
            new IssueResult { Number = 1, ProjectPath = "Test.csproj", TargetFrameworks = [], Packages = [], TestResult = "Failed" }
        };
        
        await WriteResultsFile("results-baseline.json", baseline);
        await WriteResultsFile("results.json", current);
        
        var service = new TestResultDiffService(_environmentService, _logger);

        // Act
        var result = await service.CompareResultsAsync(_testRoot);

        // Assert
        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result[0].BaselineStatus, Is.EqualTo("success"), "Pass should normalize to success");
        Assert.That(result[0].CurrentStatus, Is.EqualTo("fail"), "Failed should normalize to fail");
    }

    [Test]
    public async Task CompareResultsAsync_HandlesInvalidJson_Gracefully()
    {
        // Arrange
        File.WriteAllText(Path.Combine(_dataDir, "results.json"), "invalid json");
        
        var service = new TestResultDiffService(_environmentService, _logger);

        // Act
        var result = await service.CompareResultsAsync(_testRoot);

        // Assert
        Assert.That(result, Is.Empty, "Should handle invalid JSON gracefully");
    }

    [Test]
    public async Task CompareResultsAsync_HandlesCaseInsensitiveProjectPaths()
    {
        // Arrange
        var baseline = new List<IssueResult>
        {
            new() { Number = 1, ProjectPath = "Test.csproj", TargetFrameworks = [], Packages = [], TestResult = "success" }
        };
        var current = new List<IssueResult>
        {
            new() { Number = 1, ProjectPath = "test.csproj", TargetFrameworks = [], Packages = [], TestResult = "fail" }
        };
        
        await WriteResultsFile("results-baseline.json", baseline);
        await WriteResultsFile("results.json", current);
        
        var service = new TestResultDiffService(_environmentService, _logger);

        // Act
        var result = await service.CompareResultsAsync(_testRoot);

        // Assert
        Assert.That(result, Has.Count.EqualTo(1), "Should match case-insensitively");
    }

    private async Task WriteResultsFile(string fileName, List<IssueResult> results)
    {
        var filePath = Path.Combine(_dataDir, fileName);
        var json = JsonSerializer.Serialize(results);
        await File.WriteAllTextAsync(filePath, json);
    }
}
