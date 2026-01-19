using IssueRunner.Models;
using System.Text.Json;

namespace IssueRunner.Tests.Models;

[TestFixture]
public class IssueProjectMetadataTests
{
    [Test]
    public void Serialize_AndDeserialize_PreservesAllProperties()
    {
        // Arrange
        var original = new IssueProjectMetadata
        {
            Number = 228,
            Title = "Test Issue",
            State = "open",
            Milestone = "v1.0",
            Labels = new List<string> { "bug", "priority:high" },
            Url = "https://github.com/test/test/issues/228",
            ProjectPath = "Issue228.csproj",
            TargetFrameworks = new List<string> { "net10.0" },
            Packages = new List<PackageInfo>
            {
                new PackageInfo { Name = "NUnit", Version = "4.4.0" }
            }
        };

        // Act
        var json = JsonSerializer.Serialize(original);
        var deserialized = JsonSerializer.Deserialize<IssueProjectMetadata>(json);

        // Assert
        Assert.That(deserialized, Is.Not.Null);
        Assert.That(deserialized!.Number, Is.EqualTo(228));
        Assert.That(deserialized.Title, Is.EqualTo("Test Issue"));
        Assert.That(deserialized.State, Is.EqualTo("open"));
        Assert.That(deserialized.Milestone, Is.EqualTo("v1.0"));
        Assert.That(deserialized.Labels, Is.EquivalentTo(new[] { "bug", "priority:high" }));
        Assert.That(deserialized.ProjectPath, Is.EqualTo("Issue228.csproj"));
        Assert.That(deserialized.Packages, Has.Count.EqualTo(1));
    }

    [Test]
    public void Serialize_HandlesNullMilestone()
    {
        // Arrange
        var original = new IssueProjectMetadata
        {
            Number = 1,
            Title = "Test",
            State = "open",
            Milestone = null,
            Labels = new List<string>(),
            Url = "https://github.com/test/test/issues/1",
            ProjectPath = "Test.csproj",
            TargetFrameworks = new List<string>(),
            Packages = new List<PackageInfo>()
        };

        // Act
        var json = JsonSerializer.Serialize(original);
        var deserialized = JsonSerializer.Deserialize<IssueProjectMetadata>(json);

        // Assert
        Assert.That(deserialized, Is.Not.Null);
        Assert.That(deserialized!.Milestone, Is.Null);
    }
}
