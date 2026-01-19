using IssueRunner.Models;
using NUnit.Framework;

namespace IssueRunner.Tests.Models;

[TestFixture]
public class RunOptionsTests
{
    [Test]
    public void TestTypes_Enum_HasCorrectValues()
    {
        // Assert
        Assert.That(Enum.IsDefined(typeof(TestTypes), TestTypes.All), Is.True);
        Assert.That(Enum.IsDefined(typeof(TestTypes), TestTypes.Direct), Is.True);
        Assert.That(Enum.IsDefined(typeof(TestTypes), TestTypes.Custom), Is.True);
    }

    [Test]
    public void RunOptions_TestTypes_PropertyInitializesToAll()
    {
        // Arrange & Act
        var options = new RunOptions();
        
        // Assert
        Assert.That(options.TestTypes, Is.EqualTo(TestTypes.All));
    }

    [Test]
    public void RunOptions_TestTypes_CanBeSetToDirect()
    {
        // Arrange & Act
        var options = new RunOptions
        {
            TestTypes = TestTypes.Direct
        };
        
        // Assert
        Assert.That(options.TestTypes, Is.EqualTo(TestTypes.Direct));
    }

    [Test]
    public void RunOptions_TestTypes_CanBeSetToCustom()
    {
        // Arrange & Act
        var options = new RunOptions
        {
            TestTypes = TestTypes.Custom
        };
        
        // Assert
        Assert.That(options.TestTypes, Is.EqualTo(TestTypes.Custom));
    }

    [Test]
    public void RunOptions_ExecutionMode_PropertyDoesNotExist()
    {
        // Arrange
        var runOptionsType = typeof(RunOptions);
        
        // Act & Assert
        var executionModeProperty = runOptionsType.GetProperty("ExecutionMode");
        Assert.That(executionModeProperty, Is.Null, "ExecutionMode property should not exist - it was renamed to TestTypes");
    }
}


