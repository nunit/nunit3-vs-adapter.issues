using IssueRunner.Models;

namespace IssueRunner.Tests.Models;

[TestFixture]
public class TestResultDiffTests
{
    [Test]
    public void Create_WithAllProperties()
    {
        // Arrange & Act
        var diff = new TestResultDiff
        {
            IssueNumber = 228,
            ProjectPath = "Test.csproj",
            BaselineStatus = "success",
            CurrentStatus = "fail",
            ChangeType = ChangeType.Regression
        };

        // Assert
        Assert.That(diff.IssueNumber, Is.EqualTo(228));
        Assert.That(diff.ProjectPath, Is.EqualTo("Test.csproj"));
        Assert.That(diff.BaselineStatus, Is.EqualTo("success"));
        Assert.That(diff.CurrentStatus, Is.EqualTo("fail"));
        Assert.That(diff.ChangeType, Is.EqualTo(ChangeType.Regression));
    }

    [Test]
    public void ChangeType_Enum_HasAllValues()
    {
        // Assert
        Assert.That(Enum.GetValues<ChangeType>(), Contains.Item(ChangeType.None));
        Assert.That(Enum.GetValues<ChangeType>(), Contains.Item(ChangeType.Fixed));
        Assert.That(Enum.GetValues<ChangeType>(), Contains.Item(ChangeType.Regression));
        Assert.That(Enum.GetValues<ChangeType>(), Contains.Item(ChangeType.CompileToFail));
        Assert.That(Enum.GetValues<ChangeType>(), Contains.Item(ChangeType.Skipped));
        Assert.That(Enum.GetValues<ChangeType>(), Contains.Item(ChangeType.Other));
    }
}
