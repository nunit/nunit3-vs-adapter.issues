using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Headless;
using IssueRunner.Gui;
using IssueRunner.Gui.Services;
using IssueRunner.Gui.ViewModels;
using IssueRunner.Gui.Views;
using IssueRunner.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System.IO;

namespace IssueRunner.Gui.Tests;

/// <summary>
/// Base class for headless GUI tests providing common setup and helper methods.
/// </summary>
public abstract class HeadlessTestBase
{
    protected IServiceProvider CreateTestServiceProvider()
    {
        var services = new ServiceCollection();

        // Logging
        services.AddLogging(builder =>
        {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Warning);
        });

        // Create test directory structure for environment service
        var testRepoPath = Path.Combine(Path.GetTempPath(), "IssueRunnerHeadlessTest");
        if (Directory.Exists(testRepoPath))
        {
            Directory.Delete(testRepoPath, true);
        }
        Directory.CreateDirectory(testRepoPath);
        Directory.CreateDirectory(Path.Combine(testRepoPath, "Tools"));
        
        // Create repository.json
        File.WriteAllText(Path.Combine(testRepoPath, "Tools", "repository.json"), 
            "{\"owner\":\"test\",\"name\":\"test\"}");
        
        // Create empty test result files so LoadDataAsync doesn't fail
        File.WriteAllText(Path.Combine(testRepoPath, "test-passes.json"), "{\"testResults\":[]}");
        File.WriteAllText(Path.Combine(testRepoPath, "test-fails.json"), "{\"testResults\":[]}");
        
        // Set up environment service with proper root tracking
        var currentRoot = testRepoPath;
        var envService = Substitute.For<IEnvironmentService>();
        envService.Root.Returns(_ => currentRoot);
        envService.When(x => x.AddRoot(Arg.Any<string>())).Do(callInfo =>
        {
            currentRoot = callInfo.Arg<string>();
        });
        envService.RepositoryConfig.Returns(new IssueRunner.Models.RepositoryConfig("test", "test"));
        
        services.AddSingleton<IEnvironmentService>(envService);
        
        var discoverIssueFolders = Substitute.For<IIssueDiscoveryService>();
        discoverIssueFolders.DiscoverIssueFolders().Returns(new Dictionary<int, string>());
        services.AddSingleton(discoverIssueFolders);
        services.AddSingleton(Substitute.For<IProjectAnalyzerService>());
        services.AddSingleton(Substitute.For<IFrameworkUpgradeService>());
        services.AddSingleton(Substitute.For<IProcessExecutor>());
        services.AddSingleton(Substitute.For<IPackageUpdateService>());
        services.AddSingleton(Substitute.For<INuGetPackageVersionService>());
        var testExecution = Substitute.For<ITestExecutionService>();
        services.AddSingleton(testExecution);
        services.AddSingleton(Substitute.For<IGitHubApiService>());
        var markerService = Substitute.For<IMarkerService>();
        services.AddSingleton(markerService);
        services.AddSingleton<ITestResultAggregator, TestResultAggregator>();
        var diffService = Substitute.For<ITestResultDiffService>();
        diffService.CompareResultsAsync(Arg.Any<string>())
            .Returns(Task.FromResult(new List<IssueRunner.Models.TestResultDiff>()));
        services.AddSingleton(diffService);
        services.AddSingleton<IIssueListLoader>(sp =>
            new IssueListLoader(
                envService,
                testExecution,
                sp.GetRequiredService<IProjectAnalyzerService>(),
                diffService,
                markerService));
        services.AddSingleton<IRepositoryStatusService>(sp =>
            new RepositoryStatusService(
                envService,
                sp.GetRequiredService<IIssueDiscoveryService>(),
                markerService,
                sp.GetRequiredService<ITestResultAggregator>(),
                sp.GetRequiredService<ILogger<RepositoryStatusService>>()));
        services.AddSingleton<ITestRunOrchestrator>(sp =>
            new TestRunOrchestrator(
                sp,
                envService,
                sp.GetRequiredService<IIssueDiscoveryService>()));
        services.AddSingleton<ISyncCoordinator>(sp =>
            new SyncCoordinator(
                sp,
                envService,
                sp.GetRequiredService<IIssueDiscoveryService>()));
        services.AddSingleton<ReportGeneratorService>(sp =>
        {
            var logger = sp.GetRequiredService<ILogger<ReportGeneratorService>>();
            var envService = sp.GetRequiredService<IEnvironmentService>();
            return new ReportGeneratorService(logger, envService);
        });

        // Commands
        services.AddTransient<Commands.SyncFromGitHubCommand>();
        services.AddTransient<Commands.SyncToFoldersCommand>();
        services.AddTransient<Commands.RunTestsCommand>();
        services.AddTransient<Commands.ResetPackagesCommand>();
        services.AddTransient<Commands.GenerateReportCommand>();
        services.AddTransient<Commands.CheckRegressionsCommand>();
        services.AddTransient<Commands.MergeResultsCommand>();

        // ViewModels
        services.AddSingleton<MainViewModel>();
        services.AddSingleton<IssueListViewModel>();
        services.AddTransient<RunTestsOptionsViewModel>();

        // GUI-specific services
        services.AddSingleton<Services.IProgressReporter>(sp =>
        {
            var reporter = new Services.ProgressReporter();
            var mainViewModel = sp.GetRequiredService<MainViewModel>();
            reporter.SetMainViewModel(mainViewModel);
            return reporter;
        });

        return services.BuildServiceProvider();
    }

    protected Window CreateTestWindow(IServiceProvider services)
    {
        var mainViewModel = services.GetRequiredService<MainViewModel>();
        return new MainWindow
        {
            DataContext = mainViewModel
        };
    }

    protected Window CreateTestWindow()
    {
        var services = CreateTestServiceProvider();
        return CreateTestWindow(services);
    }
}

