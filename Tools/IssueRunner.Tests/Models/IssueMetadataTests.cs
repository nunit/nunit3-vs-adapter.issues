using IssueRunner.Models;

namespace IssueRunner.Tests.Models;

/// <summary>
/// Tests for <see cref="IssueMetadata"/> JSON serialization.
/// </summary>
[TestFixture]
public class IssueMetadataTests
{
    [Test]
    public void Serialize_ValidModel_ProducesExpectedJson()
    {
        // Arrange
        var metadata = new IssueMetadata
        {
            Number = 228,
            Title = "Tests inherited from Generic test fixture",
            State = "closed",
            Milestone = "No milestone",
            Labels = ["is:bug", "pri:normal"],
            Url = "https://github.com/nunit/nunit3-vs-adapter/issues/228"
        };

        // Act
        var json = JsonSerializer.Serialize(metadata);
        var deserialized = JsonSerializer.Deserialize<IssueMetadata>(json);

        // Assert
        Assert.That(deserialized, Is.Not.Null);
        Assert.That(deserialized!.Number, Is.EqualTo(228));
        Assert.That(deserialized.Title, Is.EqualTo("Tests inherited from Generic test fixture"));
        Assert.That(deserialized.State, Is.EqualTo("closed"));
        Assert.That(deserialized.Milestone, Is.EqualTo("No milestone"));
        Assert.That(deserialized.Labels, Is.EquivalentTo(new[] { "is:bug", "pri:normal" }));
        Assert.That(deserialized.Url, Is.EqualTo("https://github.com/nunit/nunit3-vs-adapter/issues/228"));
    }

    [Test]
    public void Deserialize_RealGitHubApiResponse_ParsesCorrectly()
    {
        // Arrange
        const string json = """
        {
          "number": 228,
          "title": "Tests inherited from Generic test fixture",
          "state": "closed",
          "milestone": "No milestone",
          "labels": ["is:bug", "pri:normal", "closed:fixedinnewversion"],
          "url": "https://github.com/nunit/nunit3-vs-adapter/issues/228"
        }
        """;

        // Act
        var metadata = JsonSerializer.Deserialize<IssueMetadata>(json);

        // Assert
        Assert.That(metadata, Is.Not.Null);
        Assert.That(metadata!.Number, Is.EqualTo(228));
        Assert.That(metadata.Title, Is.EqualTo("Tests inherited from Generic test fixture"));
        Assert.That(metadata.State, Is.EqualTo("closed"));
        Assert.That(metadata.Labels, Has.Count.EqualTo(3));
    }
}
