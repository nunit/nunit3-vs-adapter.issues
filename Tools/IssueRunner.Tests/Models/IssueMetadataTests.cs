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
        deserialized.Should().NotBeNull();
        deserialized!.Number.Should().Be(228);
        deserialized.Title.Should().Be("Tests inherited from Generic test fixture");
        deserialized.State.Should().Be("closed");
        deserialized.Milestone.Should().Be("No milestone");
        deserialized.Labels.Should().BeEquivalentTo(["is:bug", "pri:normal"]);
        deserialized.Url.Should().Be("https://github.com/nunit/nunit3-vs-adapter/issues/228");
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
        metadata.Should().NotBeNull();
        metadata!.Number.Should().Be(228);
        metadata.Title.Should().Be("Tests inherited from Generic test fixture");
        metadata.State.Should().Be("closed");
        metadata.Labels.Should().HaveCount(3);
    }
}
