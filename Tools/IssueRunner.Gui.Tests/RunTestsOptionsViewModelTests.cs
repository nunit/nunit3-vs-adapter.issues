using IssueRunner.Gui.ViewModels;
using IssueRunner.Models;
using NUnit.Framework;
using System.Reflection;

namespace IssueRunner.Gui.Tests;

[TestFixture]
public class RunTestsOptionsViewModelTests
{
    [Test]
    public void ToRunOptions_AlwaysSetsScopeToAll_WhenCreatingRunOptions()
    {
        // Arrange
        var viewModel = new RunTestsOptionsViewModel();
        
        // Act
        var options = viewModel.ToRunOptions();
        
        // Assert
        Assert.That(options.Scope, Is.EqualTo(TestScope.All), "Scope should always be All since it's set in the filter section");
    }

    [Test]
    public void ToRunOptions_IncludesTestTypes_WhenSet()
    {
        // Arrange
        var viewModel = new RunTestsOptionsViewModel();
        viewModel.TestTypes = TestTypes.Custom;
        
        // Act
        var options = viewModel.ToRunOptions();
        
        // Assert
        Assert.That(options.TestTypes, Is.EqualTo(TestTypes.Custom));
    }
}


