using IssueRunner.Models;
using IssueRunner.Services;
using Microsoft.Extensions.Logging;

namespace IssueRunner.Tests;

[TestFixture]
public class ReportGeneratorServiceTests
{
    [Test]
    public void RegressionSuccess_IncludesPassCount_omitsFailZero()
    {
        var logger = Substitute.For<ILogger<ReportGeneratorService>>();
        var env = Substitute.For<IEnvironmentService>();
        env.RepositoryConfig.Returns(new RepositoryConfig("owner", "repo"));

        var service = new ReportGeneratorService(logger, env);

        var results = new List<IssueResult>
        {
            new IssueResult
            {
                Number = 1,
                ProjectPath = "proj.csproj",
                ProjectStyle = "SDK-style",
                TargetFrameworks = new List<string> { "net10.0" },
                Packages = new List<string> { "NUnit=4.4.0" },
                UpdateResult = "success",
                TestResult = "success",
                TestOutput = "Passed!  - Failed:     0, Passed:     3, Skipped:     0, Total:     3",
                TestConclusion = "Success: No regression failure."
            }
        };

        var metadata = new List<IssueMetadata>
        {
            new IssueMetadata
            {
                Number = 1,
                Title = "Regression fix",
                State = "closed",
                Milestone = null,
                Labels = new List<string>(),
                Url = "https://example.com"
            }
        };

        var report = service.GenerateReport(results, metadata);

        Assert.That(report, Does.Contain("Success: No regression failure (Pass 3)"));
        Assert.That(report, Does.Not.Contain("Fail 0"));
    }

    [Test]
    public void RegressionFailure_IncludesPassAndFailCounts()
    {
        var logger = Substitute.For<ILogger<ReportGeneratorService>>();
        var env = Substitute.For<IEnvironmentService>();
        env.RepositoryConfig.Returns(new RepositoryConfig("owner", "repo"));

        var service = new ReportGeneratorService(logger, env);

        var results = new List<IssueResult>
        {
            new IssueResult
            {
                Number = 2,
                ProjectPath = "proj.csproj",
                ProjectStyle = "SDK-style",
                TargetFrameworks = new List<string> { "net10.0" },
                Packages = new List<string> { "NUnit=4.4.0" },
                UpdateResult = "success",
                TestResult = "fail",
                TestOutput = "Failed!  - Failed:     2, Passed:     1, Skipped:     0, Total:     3",
                TestConclusion = "Failure: Regression failure."
            }
        };

        var metadata = new List<IssueMetadata>
        {
            new IssueMetadata
            {
                Number = 2,
                Title = "Regression repro",
                State = "closed",
                Milestone = null,
                Labels = new List<string>(),
                Url = "https://example.com"
            }
        };

        var report = service.GenerateReport(results, metadata);

        Assert.That(report, Does.Contain("Failure: Regression failure. (Pass 1, Fail 2)"));
    }
}
