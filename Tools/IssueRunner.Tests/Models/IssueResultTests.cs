using IssueRunner.Models;
using System.Text.Json;

namespace IssueRunner.Tests.Models;

[TestFixture]
public class IssueResultTests
{
    [Test]
    public void Serialize_AndDeserialize_PreservesAllProperties()
    {
        // Arrange
        var original = new IssueResult
        {
            Number = 228,
            ProjectPath = "Issue228.csproj",
            ProjectStyle = "SDK-style",
            TargetFrameworks = new List<string> { "net10.0" },
            Packages = new List<string> { "NUnit=4.4.0" },
            UpdateResult = "success",
            UpdateOutput = "Output",
            UpdateError = "",
            RestoreResult = "success",
            RestoreOutput = "Restored",
            RestoreError = "",
            BuildResult = "success",
            BuildOutput = "Built",
            BuildError = "",
            TestResult = "success",
            TestOutput = "Passed",
            TestError = "",
            TestConclusion = "Success",
            RunnerScripts = new List<string> { "script.sh" },
            RunSettings = "settings.runsettings",
            Feed = "Stable",
            RunnerExpectations = new List<string> { "expect1" },
            LastRun = "2026-01-17T10:26:38Z"
        };

        // Act
        var json = JsonSerializer.Serialize(original);
        var deserialized = JsonSerializer.Deserialize<IssueResult>(json);

        // Assert
        Assert.That(deserialized, Is.Not.Null);
        Assert.That(deserialized!.Number, Is.EqualTo(228));
        Assert.That(deserialized.ProjectPath, Is.EqualTo("Issue228.csproj"));
        Assert.That(deserialized.ProjectStyle, Is.EqualTo("SDK-style"));
        Assert.That(deserialized.TargetFrameworks, Is.EquivalentTo(new[] { "net10.0" }));
        Assert.That(deserialized.Packages, Is.EquivalentTo(new[] { "NUnit=4.4.0" }));
        Assert.That(deserialized.TestResult, Is.EqualTo("success"));
        Assert.That(deserialized.LastRun, Is.EqualTo("2026-01-17T10:26:38Z"));
    }

    [Test]
    public void Serialize_HandlesNullOptionalProperties()
    {
        // Arrange
        var original = new IssueResult
        {
            Number = 1,
            ProjectPath = "Test.csproj",
            TargetFrameworks = new List<string>(),
            Packages = new List<string>(),
            ProjectStyle = null,
            UpdateResult = null,
            TestResult = null
        };

        // Act
        var json = JsonSerializer.Serialize(original);
        var deserialized = JsonSerializer.Deserialize<IssueResult>(json);

        // Assert
        Assert.That(deserialized, Is.Not.Null);
        Assert.That(deserialized!.ProjectStyle, Is.Null);
        Assert.That(deserialized.UpdateResult, Is.Null);
        Assert.That(deserialized.TestResult, Is.Null);
    }

    [Test]
    public void Deserialize_FromRealJsonStructure()
    {
        // Arrange - JSON matching real results.json structure
        var json = @"{
  ""number"": 228,
  ""project_path"": ""Issue228.csproj"",
  ""project_style"": ""SDK-style"",
  ""target_frameworks"": [""net10.0""],
  ""packages"": [""NUnit=4.4.0""],
  ""test_result"": ""success"",
  ""last_run"": ""2026-01-17T10:26:38Z""
}";

        // Act
        var result = JsonSerializer.Deserialize<IssueResult>(json);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Number, Is.EqualTo(228));
        Assert.That(result.ProjectPath, Is.EqualTo("Issue228.csproj"));
        Assert.That(result.TestResult, Is.EqualTo("success"));
    }
}
