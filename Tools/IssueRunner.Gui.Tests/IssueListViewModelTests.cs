using IssueRunner.Gui.ViewModels;
using IssueRunner.Models;
using NUnit.Framework;

namespace IssueRunner.Gui.Tests;

[TestFixture]
public class IssueListViewModelTests
{
    [Test]
    public void ApplyFilters_FiltersIssuesBySelectedTestTypes_WhenTestTypesIsScriptsOnly()
    {
        // Arrange
        var viewModel = new IssueListViewModel();
        var issues = new List<IssueListItem>
        {
            new IssueListItem { Number = 1, TestTypes = "Scripts" },
            new IssueListItem { Number = 2, TestTypes = "DotNet test" },
            new IssueListItem { Number = 3, TestTypes = "Scripts" }
        };
        viewModel.LoadIssues(issues);
        
        // Act
        viewModel.SelectedTestTypes = "Scripts only";
        
        // Assert
        Assert.That(viewModel.Issues.Count, Is.EqualTo(2));
        Assert.That(viewModel.Issues.All(i => i.TestTypes == "Scripts"), Is.True);
        Assert.That(viewModel.Issues.Select(i => i.Number), Is.EquivalentTo(new[] { 1, 3 }));
    }

    [Test]
    public void ApplyFilters_FiltersIssuesBySelectedTestTypes_WhenTestTypesIsDotnetTestOnly()
    {
        // Arrange
        var viewModel = new IssueListViewModel();
        var issues = new List<IssueListItem>
        {
            new IssueListItem { Number = 1, TestTypes = "Scripts" },
            new IssueListItem { Number = 2, TestTypes = "DotNet test" },
            new IssueListItem { Number = 3, TestTypes = "DotNet test" }
        };
        viewModel.LoadIssues(issues);
        
        // Act
        viewModel.SelectedTestTypes = "dotnet test only";
        
        // Assert
        Assert.That(viewModel.Issues.Count, Is.EqualTo(2));
        Assert.That(viewModel.Issues.All(i => i.TestTypes == "DotNet test"), Is.True);
        Assert.That(viewModel.Issues.Select(i => i.Number), Is.EquivalentTo(new[] { 2, 3 }));
    }

    [Test]
    public void ApplyFilters_ShowsAllIssues_WhenTestTypesIsAll()
    {
        // Arrange
        var viewModel = new IssueListViewModel();
        var issues = new List<IssueListItem>
        {
            new IssueListItem { Number = 1, TestTypes = "Scripts" },
            new IssueListItem { Number = 2, TestTypes = "DotNet test" },
            new IssueListItem { Number = 3, TestTypes = "Scripts" }
        };
        viewModel.LoadIssues(issues);
        viewModel.SelectedTestTypes = "Scripts only"; // First filter to Scripts only
        
        // Act
        viewModel.SelectedTestTypes = "All";
        
        // Assert
        Assert.That(viewModel.Issues.Count, Is.EqualTo(3));
    }

    [Test]
    public void TestTypes_PropertyIsPopulatedCorrectly_ForIssueWithCustomScripts()
    {
        // Arrange
        var issue = new IssueListItem
        {
            Number = 1,
            Title = "Test Issue",
            TestTypes = "Scripts"
        };
        
        // Assert
        Assert.That(issue.TestTypes, Is.EqualTo("Scripts"));
    }

    [Test]
    public void TestTypes_PropertyIsPopulatedCorrectly_ForIssueWithoutCustomScripts()
    {
        // Arrange
        var issue = new IssueListItem
        {
            Number = 1,
            Title = "Test Issue",
            TestTypes = "DotNet test"
        };
        
        // Assert
        Assert.That(issue.TestTypes, Is.EqualTo("DotNet test"));
    }

    [Test]
    public void HasActiveFilters_ReturnsTrue_WhenTestTypesFilterIsSet()
    {
        // Arrange
        var viewModel = new IssueListViewModel();
        
        // Act
        viewModel.SelectedTestTypes = "Scripts only";
        
        // Assert
        Assert.That(viewModel.HasActiveFilters, Is.True);
    }

    [Test]
    public void HasActiveFilters_ReturnsFalse_WhenAllFiltersAreDefault()
    {
        // Arrange
        var viewModel = new IssueListViewModel();
        
        // Assert
        Assert.That(viewModel.SelectedScope, Is.EqualTo(TestScope.All));
        Assert.That(viewModel.SelectedState, Is.EqualTo("All"));
        Assert.That(viewModel.SelectedTestResult, Is.EqualTo("All"));
        Assert.That(viewModel.SelectedTestTypes, Is.EqualTo("All"));
        Assert.That(viewModel.HasActiveFilters, Is.False);
    }

    [Test]
    public void ApplyFilters_FiltersIssuesBySelectedScope_WhenScopeIsRegression()
    {
        // Arrange
        var viewModel = new IssueListViewModel();
        var issues = new List<IssueListItem>
        {
            new IssueListItem { Number = 1, State = "closed" },
            new IssueListItem { Number = 2, State = "open" },
            new IssueListItem { Number = 3, State = "closed" }
        };
        viewModel.LoadIssues(issues);
        
        // Act
        viewModel.SelectedScope = TestScope.Regression;
        
        // Assert
        Assert.That(viewModel.Issues.Count, Is.EqualTo(2));
        Assert.That(viewModel.Issues.All(i => i.State == "closed"), Is.True);
        Assert.That(viewModel.Issues.Select(i => i.Number), Is.EquivalentTo(new[] { 1, 3 }));
    }

    [Test]
    public void ApplyFilters_FiltersIssuesBySelectedScope_WhenScopeIsOpen()
    {
        // Arrange
        var viewModel = new IssueListViewModel();
        var issues = new List<IssueListItem>
        {
            new IssueListItem { Number = 1, State = "closed" },
            new IssueListItem { Number = 2, State = "open" },
            new IssueListItem { Number = 3, State = "open" }
        };
        viewModel.LoadIssues(issues);
        
        // Act
        viewModel.SelectedScope = TestScope.Open;
        
        // Assert
        Assert.That(viewModel.Issues.Count, Is.EqualTo(2));
        Assert.That(viewModel.Issues.All(i => i.State == "open"), Is.True);
        Assert.That(viewModel.Issues.Select(i => i.Number), Is.EquivalentTo(new[] { 2, 3 }));
    }

    [Test]
    public void ApplyFilters_ShowsAllIssues_WhenScopeIsAll()
    {
        // Arrange
        var viewModel = new IssueListViewModel();
        var issues = new List<IssueListItem>
        {
            new IssueListItem { Number = 1, State = "closed" },
            new IssueListItem { Number = 2, State = "open" },
            new IssueListItem { Number = 3, State = "closed" }
        };
        viewModel.LoadIssues(issues);
        viewModel.SelectedScope = TestScope.Regression; // First filter to Regression
        
        // Act
        viewModel.SelectedScope = TestScope.All;
        
        // Assert
        Assert.That(viewModel.Issues.Count, Is.EqualTo(3));
    }

    [Test]
    public void ApplyFilters_FiltersIssuesBySelectedState_WhenStateIsNew()
    {
        // Arrange
        var viewModel = new IssueListViewModel();
        var issues = new List<IssueListItem>
        {
            new IssueListItem { Number = 1, StateValue = IssueState.New },
            new IssueListItem { Number = 2, StateValue = IssueState.Synced },
            new IssueListItem { Number = 3, StateValue = IssueState.New }
        };
        viewModel.LoadIssues(issues);
        
        // Act
        viewModel.SelectedState = "New";
        
        // Assert
        Assert.That(viewModel.Issues.Count, Is.EqualTo(2));
        Assert.That(viewModel.Issues.All(i => i.StateValue == IssueState.New), Is.True);
        Assert.That(viewModel.Issues.Select(i => i.Number), Is.EquivalentTo(new[] { 1, 3 }));
    }

    [Test]
    public void ApplyFilters_FiltersIssuesBySelectedState_WhenStateIsSkipped()
    {
        // Arrange
        var viewModel = new IssueListViewModel();
        var issues = new List<IssueListItem>
        {
            new IssueListItem { Number = 1, StateValue = IssueState.Skipped },
            new IssueListItem { Number = 2, StateValue = IssueState.Runnable },
            new IssueListItem { Number = 3, StateValue = IssueState.Skipped }
        };
        viewModel.LoadIssues(issues);
        
        // Act
        viewModel.SelectedState = "Skipped";
        
        // Assert
        Assert.That(viewModel.Issues.Count, Is.EqualTo(2));
        Assert.That(viewModel.Issues.All(i => i.StateValue == IssueState.Skipped), Is.True);
        Assert.That(viewModel.Issues.Select(i => i.Number), Is.EquivalentTo(new[] { 1, 3 }));
    }

    [Test]
    public void ApplyFilters_FiltersIssuesBySelectedState_WhenStateIsFailedCompile()
    {
        // Arrange
        var viewModel = new IssueListViewModel();
        var issues = new List<IssueListItem>
        {
            new IssueListItem { Number = 1, StateValue = IssueState.FailedCompile },
            new IssueListItem { Number = 2, StateValue = IssueState.Runnable },
            new IssueListItem { Number = 3, StateValue = IssueState.FailedCompile }
        };
        viewModel.LoadIssues(issues);
        
        // Act
        viewModel.SelectedState = "Failed compile";
        
        // Assert
        Assert.That(viewModel.Issues.Count, Is.EqualTo(2));
        Assert.That(viewModel.Issues.All(i => i.StateValue == IssueState.FailedCompile), Is.True);
        Assert.That(viewModel.Issues.Select(i => i.Number), Is.EquivalentTo(new[] { 1, 3 }));
    }

    [Test]
    public void ApplyFilters_ShowsAllIssues_WhenStateIsAll()
    {
        // Arrange
        var viewModel = new IssueListViewModel();
        var issues = new List<IssueListItem>
        {
            new IssueListItem { Number = 1, StateValue = IssueState.New },
            new IssueListItem { Number = 2, StateValue = IssueState.Synced },
            new IssueListItem { Number = 3, StateValue = IssueState.Runnable }
        };
        viewModel.LoadIssues(issues);
        viewModel.SelectedState = "New"; // First filter to New
        
        // Act
        viewModel.SelectedState = "All";
        
        // Assert
        Assert.That(viewModel.Issues.Count, Is.EqualTo(3));
    }

    [Test]
    public void LoadIssues_ClearsExistingIssues()
    {
        // Arrange
        var viewModel = new IssueListViewModel();
        var initialIssues = new List<IssueListItem>
        {
            new IssueListItem { Number = 1, Title = "Old Title 1" },
            new IssueListItem { Number = 2, Title = "Old Title 2" }
        };
        viewModel.LoadIssues(initialIssues);
        Assert.That(viewModel.AllIssues.Count, Is.EqualTo(2));

        // Act
        var newIssues = new List<IssueListItem>
        {
            new IssueListItem { Number = 3, Title = "New Title 3" }
        };
        viewModel.LoadIssues(newIssues);

        // Assert
        Assert.That(viewModel.AllIssues.Count, Is.EqualTo(1));
        Assert.That(viewModel.AllIssues.First().Number, Is.EqualTo(3));
    }

    [Test]
    public void LoadIssues_AddsAllItemsToCollection()
    {
        // Arrange
        var viewModel = new IssueListViewModel();
        var issues = new List<IssueListItem>
        {
            new IssueListItem { Number = 1, Title = "Title 1" },
            new IssueListItem { Number = 2, Title = "Title 2" },
            new IssueListItem { Number = 3, Title = "Title 3" }
        };

        // Act
        viewModel.LoadIssues(issues);

        // Assert
        Assert.That(viewModel.AllIssues.Count, Is.EqualTo(3));
        Assert.That(viewModel.AllIssues.Select(i => i.Number), Is.EquivalentTo(new[] { 1, 2, 3 }));
    }

    [Test]
    public void LoadIssues_PreservesTitleProperty_WhenLoadingItems()
    {
        // Arrange
        var viewModel = new IssueListViewModel();
        var issues = new List<IssueListItem>
        {
            new IssueListItem { Number = 1, Title = "Test Title 1" },
            new IssueListItem { Number = 228, Title = "Tests inherited from Generic test fixture" },
            new IssueListItem { Number = 999, Title = "Issue 999" }
        };

        // Act
        viewModel.LoadIssues(issues);

        // Assert
        Assert.That(viewModel.AllIssues.Count, Is.EqualTo(3));
        Assert.That(viewModel.AllIssues.First(i => i.Number == 1).Title, Is.EqualTo("Test Title 1"));
        Assert.That(viewModel.AllIssues.First(i => i.Number == 228).Title, Is.EqualTo("Tests inherited from Generic test fixture"));
        Assert.That(viewModel.AllIssues.First(i => i.Number == 999).Title, Is.EqualTo("Issue 999"));
    }

    [Test]
    public void LoadIssues_CallsApplyFilters_AfterLoading()
    {
        // Arrange
        var viewModel = new IssueListViewModel();
        var issues = new List<IssueListItem>
        {
            new IssueListItem { Number = 1, Title = "Title 1", State = "open" },
            new IssueListItem { Number = 2, Title = "Title 2", State = "closed" }
        };

        // Act
        viewModel.LoadIssues(issues);

        // Assert - Filters should be applied, so Issues collection should be populated
        Assert.That(viewModel.Issues.Count, Is.EqualTo(2));
    }

    [Test]
    public void ApplyFilters_PreservesTitle_WhenNoFiltersApplied()
    {
        // Arrange
        var viewModel = new IssueListViewModel();
        var issues = new List<IssueListItem>
        {
            new IssueListItem { Number = 1, Title = "Title 1" },
            new IssueListItem { Number = 2, Title = "Title 2" }
        };
        viewModel.LoadIssues(issues);

        // Act - No filter changes (all defaults)

        // Assert
        Assert.That(viewModel.Issues.Count, Is.EqualTo(2));
        Assert.That(viewModel.Issues.First(i => i.Number == 1).Title, Is.EqualTo("Title 1"));
        Assert.That(viewModel.Issues.First(i => i.Number == 2).Title, Is.EqualTo("Title 2"));
    }

    [Test]
    public void ApplyFilters_PreservesTitle_WhenFilteringByScope()
    {
        // Arrange
        var viewModel = new IssueListViewModel();
        var issues = new List<IssueListItem>
        {
            new IssueListItem { Number = 1, Title = "Title 1", State = "open" },
            new IssueListItem { Number = 2, Title = "Title 2", State = "closed" }
        };
        viewModel.LoadIssues(issues);

        // Act
        viewModel.SelectedScope = TestScope.Regression;

        // Assert
        Assert.That(viewModel.Issues.Count, Is.EqualTo(1));
        Assert.That(viewModel.Issues.First().Number, Is.EqualTo(2));
        Assert.That(viewModel.Issues.First().Title, Is.EqualTo("Title 2"));
    }

    [Test]
    public void ApplyFilters_PreservesTitle_WhenFilteringByState()
    {
        // Arrange
        var viewModel = new IssueListViewModel();
        var issues = new List<IssueListItem>
        {
            new IssueListItem { Number = 1, Title = "Title 1", StateValue = IssueState.New },
            new IssueListItem { Number = 2, Title = "Title 2", StateValue = IssueState.Synced }
        };
        viewModel.LoadIssues(issues);

        // Act
        viewModel.SelectedState = "New";

        // Assert
        Assert.That(viewModel.Issues.Count, Is.EqualTo(1));
        Assert.That(viewModel.Issues.First().Number, Is.EqualTo(1));
        Assert.That(viewModel.Issues.First().Title, Is.EqualTo("Title 1"));
    }

    [Test]
    public void ApplyFilters_PreservesTitle_WhenFilteringByTestResult()
    {
        // Arrange
        var viewModel = new IssueListViewModel();
        var issues = new List<IssueListItem>
        {
            new IssueListItem { Number = 1, Title = "Title 1", TestResult = "success" },
            new IssueListItem { Number = 2, Title = "Title 2", TestResult = "fail" }
        };
        viewModel.LoadIssues(issues);

        // Act
        viewModel.SelectedTestResult = "Success";

        // Assert
        Assert.That(viewModel.Issues.Count, Is.EqualTo(1));
        Assert.That(viewModel.Issues.First().Number, Is.EqualTo(1));
        Assert.That(viewModel.Issues.First().Title, Is.EqualTo("Title 1"));
    }

    [Test]
    public void ApplyFilters_PreservesTitle_WhenFilteringByTestTypes()
    {
        // Arrange
        var viewModel = new IssueListViewModel();
        var issues = new List<IssueListItem>
        {
            new IssueListItem { Number = 1, Title = "Title 1", TestTypes = "Scripts" },
            new IssueListItem { Number = 2, Title = "Title 2", TestTypes = "DotNet test" }
        };
        viewModel.LoadIssues(issues);

        // Act
        viewModel.SelectedTestTypes = "Scripts only";

        // Assert
        Assert.That(viewModel.Issues.Count, Is.EqualTo(1));
        Assert.That(viewModel.Issues.First().Number, Is.EqualTo(1));
        Assert.That(viewModel.Issues.First().Title, Is.EqualTo("Title 1"));
    }

    [Test]
    public void ApplyFilters_PreservesTitle_WhenFilteringByFramework()
    {
        // Arrange
        var viewModel = new IssueListViewModel();
        var issues = new List<IssueListItem>
        {
            new IssueListItem { Number = 1, Title = "Title 1", Framework = ".Net" },
            new IssueListItem { Number = 2, Title = "Title 2", Framework = ".Net Framework" }
        };
        viewModel.LoadIssues(issues);

        // Act
        viewModel.SelectedFramework = ".Net";

        // Assert
        Assert.That(viewModel.Issues.Count, Is.EqualTo(1));
        Assert.That(viewModel.Issues.First().Number, Is.EqualTo(1));
        Assert.That(viewModel.Issues.First().Title, Is.EqualTo("Title 1"));
    }

    [Test]
    public void ApplyFilters_PreservesTitle_WhenFilteringByDiff()
    {
        // Arrange
        var viewModel = new IssueListViewModel();
        var issues = new List<IssueListItem>
        {
            new IssueListItem { Number = 1, Title = "Title 1" },
            new IssueListItem { Number = 2, Title = "Title 2" }
        };
        viewModel.LoadIssues(issues);
        viewModel.IssueChanges = new Dictionary<string, ChangeType>
        {
            { "Issue1|test.csproj", ChangeType.Regression }
        };

        // Act
        viewModel.ShowDiffOnly = true;

        // Assert
        Assert.That(viewModel.Issues.Count, Is.EqualTo(1));
        Assert.That(viewModel.Issues.First().Number, Is.EqualTo(1));
        Assert.That(viewModel.Issues.First().Title, Is.EqualTo("Title 1"));
    }

    [Test]
    public void ApplyFilters_PreservesTitle_ThroughMultipleFilterChanges()
    {
        // Arrange
        var viewModel = new IssueListViewModel();
        var issues = new List<IssueListItem>
        {
            new IssueListItem { Number = 1, Title = "Title 1", State = "open", TestResult = "success" },
            new IssueListItem { Number = 2, Title = "Title 2", State = "closed", TestResult = "fail" }
        };
        viewModel.LoadIssues(issues);

        // Act - Apply multiple filters
        viewModel.SelectedScope = TestScope.Open;
        viewModel.SelectedTestResult = "Success";

        // Assert
        Assert.That(viewModel.Issues.Count, Is.EqualTo(1));
        Assert.That(viewModel.Issues.First().Number, Is.EqualTo(1));
        Assert.That(viewModel.Issues.First().Title, Is.EqualTo("Title 1"));
    }
}

