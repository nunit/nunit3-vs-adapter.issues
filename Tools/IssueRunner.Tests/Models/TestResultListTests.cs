using IssueRunner.Models;
using System.Text.Json;

namespace IssueRunner.Tests.Models;

[TestFixture]
public class TestResultListTests
{
    [Test]
    public void Serialize_AndDeserialize_PreservesAllProperties()
    {
        // Arrange
        var original = new TestResultList
        {
            TestResults = new List<TestResultEntry>
            {
                new TestResultEntry
                {
                    Issue = "Issue228",
                    Project = "Issue228.csproj",
                    LastRun = "2026-01-17T10:26:38Z",
                    TestResult = "success"
                },
                new TestResultEntry
                {
                    Issue = "Issue229",
                    Project = "Issue229.csproj",
                    LastRun = "2026-01-17T10:27:00Z",
                    TestResult = "fail"
                }
            }
        };

        // Act
        var json = JsonSerializer.Serialize(original);
        var deserialized = JsonSerializer.Deserialize<TestResultList>(json);

        // Assert
        Assert.That(deserialized, Is.Not.Null);
        Assert.That(deserialized!.TestResults, Has.Count.EqualTo(2));
        Assert.That(deserialized.TestResults[0].Issue, Is.EqualTo("Issue228"));
        Assert.That(deserialized.TestResults[0].TestResult, Is.EqualTo("success"));
        Assert.That(deserialized.TestResults[1].TestResult, Is.EqualTo("fail"));
    }

    [Test]
    public void Deserialize_FromRealJsonStructure()
    {
        // Arrange - JSON matching real test-passes.json structure
        var json = @"{
  ""test_results"": [
    {
      ""issue"": ""Issue228"",
      ""project"": ""Issue228.csproj"",
      ""last_run"": ""2026-01-17T10:26:38Z"",
      ""test_result"": ""success""
    }
  ]
}";

        // Act
        var result = JsonSerializer.Deserialize<TestResultList>(json);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.TestResults, Has.Count.EqualTo(1));
        Assert.That(result.TestResults[0].Issue, Is.EqualTo("Issue228"));
        Assert.That(result.TestResults[0].TestResult, Is.EqualTo("success"));
    }
}
