using IssueRunner.Services;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace IssueRunner.Tests;

[TestFixture]
public class ProjectAnalyzerServiceTests
{
    [Test]
    public void FindProjectFiles_PrefersTestProjectsWhenMultipleExist()
    {
        var tempRoot = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempRoot);

        try
        {
            var appDir = Path.Combine(tempRoot, "App");
            var testDir = Path.Combine(tempRoot, "App.Test");
            Directory.CreateDirectory(appDir);
            Directory.CreateDirectory(testDir);

            File.WriteAllText(Path.Combine(appDir, "App.csproj"), "<Project />");
            File.WriteAllText(Path.Combine(testDir, "App.Test.csproj"), "<Project />");

            var logger = Substitute.For<ILogger<ProjectAnalyzerService>>();
            var analyzer = new ProjectAnalyzerService(logger);

            var projects = analyzer.FindProjectFiles(tempRoot);

            Assert.That(projects[0], Does.Contain("App.Test.csproj"));
        }
        finally
        {
            if (Directory.Exists(tempRoot))
            {
                Directory.Delete(tempRoot, true);
            }
        }
    }
}
