using System.CommandLine;
using IssueRunner.Commands;
using IssueRunner.Models;
using IssueRunner.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace IssueRunner;

/// <summary>
/// Entry point for the IssueRunner CLI tool.
/// </summary>
internal static class Program
{
    /// <summary>
    /// Main entry point.
    /// </summary>
    /// <param name="args">Command line arguments.</param>
    /// <returns>Exit code.</returns>
    internal static async Task<int> Main(string[] args)
    {
        var services = ConfigureServices();
        var rootCommand = BuildRootCommand(services);
        
        return await rootCommand.InvokeAsync(args);
    }

    /// <summary>
    /// Configures dependency injection services.
    /// </summary>
    /// <returns>Service provider.</returns>
    private static IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();
        
        services.AddLogging(builder =>
        {
            builder.AddSimpleConsole(options =>
            {
                options.SingleLine = true;
                options.IncludeScopes = false;
                options.TimestampFormat = null;
            });
            builder.SetMinimumLevel(LogLevel.Information);
        });

        services.AddHttpClient<IGitHubApiService, GitHubApiService>();
        
        services.AddSingleton<IIssueDiscoveryService, IssueDiscoveryService>();
        services.AddSingleton<IProjectAnalyzerService, ProjectAnalyzerService>();
        services.AddSingleton<IFrameworkUpgradeService, FrameworkUpgradeService>();
        services.AddSingleton<ProcessExecutor>();
        services.AddSingleton<IPackageUpdateService, PackageUpdateService>();
        services.AddSingleton<ITestExecutionService, TestExecutionService>();
        services.AddSingleton<ReportGeneratorService>();
        
        services.AddTransient<SyncFromGitHubCommand>();
        services.AddTransient<SyncToFoldersCommand>();
        services.AddTransient<RunTestsCommand>();
        services.AddTransient<ResetPackagesCommand>();
        services.AddTransient<GenerateReportCommand>();
        services.AddTransient<CheckRegressionsCommand>();
        services.AddTransient<MergeResultsCommand>();
        
        return services.BuildServiceProvider();
    }

    private static RootCommand BuildRootCommand(IServiceProvider services)
    {
        var rootCommand = new RootCommand(
            "IssueRunner - NUnit VS Adapter issue test automation tool");

        rootCommand.AddCommand(BuildMetadataCommand(services));
        rootCommand.AddCommand(BuildRunCommand(services));
        rootCommand.AddCommand(BuildResetCommand(services));
        rootCommand.AddCommand(BuildReportCommand(services));
        rootCommand.AddCommand(BuildMergeCommand(services));

        return rootCommand;
    }

    private static Command BuildMetadataCommand(IServiceProvider services)
    {
        var metadataCommand = new Command("metadata", "Manage issue metadata");

        var syncFromGitHub = new Command("sync-from-github", "Sync metadata from GitHub to central file");
        var rootOption = new Option<string>(
            "--root",
            () => Directory.GetCurrentDirectory(),
            "Repository root path");
        syncFromGitHub.AddOption(rootOption);
        syncFromGitHub.SetHandler(async (string root) =>
        {
            var cmd = services.GetRequiredService<SyncFromGitHubCommand>();
            await cmd.ExecuteAsync(root, null, CancellationToken.None);
        }, rootOption);

        var syncToFolders = new Command("sync-to-folders", "Sync metadata from central file to issue folders");
        var rootOption2 = new Option<string>(
            "--root",
            () => Directory.GetCurrentDirectory(),
            "Repository root path");
        syncToFolders.AddOption(rootOption2);
        syncToFolders.SetHandler(async (string root) =>
        {
            var cmd = services.GetRequiredService<SyncToFoldersCommand>();
            await cmd.ExecuteAsync(root, null, CancellationToken.None);
        }, rootOption2);

        metadataCommand.AddCommand(syncFromGitHub);
        metadataCommand.AddCommand(syncToFolders);

        return metadataCommand;
    }

    private static Command BuildRunCommand(IServiceProvider services)
    {
        var runCommand = new Command("run", "Run tests for issues");

        var rootOption = new Option<string>(
            "--root",
            () => Directory.GetCurrentDirectory(),
            "Repository root path");
        var scopeOption = new Option<TestScope>(
            "--scope",
            () => TestScope.All,
            "Test scope");
        var issuesOption = new Option<string?>(
            "--issues",
            "Comma-separated issue numbers");
        var timeoutOption = new Option<int>(
            "--timeout",
            () => 600,
            "Timeout in seconds per command");
        var skipNetFxOption = new Option<bool>(
            "--skip-netfx",
            "Skip .NET Framework tests");
        var onlyNetFxOption = new Option<bool>(
            "--only-netfx",
            "Run only .NET Framework tests");
        var nunitOnlyOption = new Option<bool>(
            "--nunit-only",
            "Update only NUnit packages");
        var executionModeOption = new Option<ExecutionMode>(
            "--execution-mode",
            () => ExecutionMode.All,
            "Execution mode filter");
        var verbosityOption = new Option<LogVerbosity>(
            "--verbosity",
            () => LogVerbosity.Normal,
            "Logging verbosity (Normal or Verbose)");
        var feedOption = new Option<PackageFeed>(
            "--feed",
            () => PackageFeed.Stable,
            "Package feed (Stable=nuget.org, Beta=nuget.org+prerelease, Alpha=nuget.org+myget+prerelease)");

        runCommand.AddOption(rootOption);
        runCommand.AddOption(scopeOption);
        runCommand.AddOption(issuesOption);
        runCommand.AddOption(timeoutOption);
        runCommand.AddOption(skipNetFxOption);
        runCommand.AddOption(onlyNetFxOption);
        runCommand.AddOption(nunitOnlyOption);
        runCommand.AddOption(executionModeOption);
        runCommand.AddOption(verbosityOption);
        runCommand.AddOption(feedOption);

        runCommand.SetHandler(async (context) =>
        {
            var root = context.ParseResult.GetValueForOption(rootOption)!;
            var scope = context.ParseResult.GetValueForOption(scopeOption);
            var issues = context.ParseResult.GetValueForOption(issuesOption);
            var timeout = context.ParseResult.GetValueForOption(timeoutOption);
            var skipNetFx = context.ParseResult.GetValueForOption(skipNetFxOption);
            var onlyNetFx = context.ParseResult.GetValueForOption(onlyNetFxOption);
            var nunitOnly = context.ParseResult.GetValueForOption(nunitOnlyOption);
            var executionMode = context.ParseResult.GetValueForOption(executionModeOption);
            var verbosity = context.ParseResult.GetValueForOption(verbosityOption);
            var feed = context.ParseResult.GetValueForOption(feedOption);

            var options = new RunOptions
            {
                Scope = scope,
                IssueNumbers = string.IsNullOrEmpty(issues)
                    ? null
                    : issues.Split(',').Select(int.Parse).ToList(),
                TimeoutSeconds = timeout,
                SkipNetFx = skipNetFx,
                OnlyNetFx = onlyNetFx,
                NUnitOnly = nunitOnly,
                ExecutionMode = executionMode,
                Verbosity = verbosity,
                Feed = feed
            };

            var cmd = services.GetRequiredService<RunTestsCommand>();
            await cmd.ExecuteAsync(root, options, CancellationToken.None);
        });

        return runCommand;
    }

    private static Command BuildResetCommand(IServiceProvider services)
    {
        var resetCommand = new Command("reset", "Reset package versions to metadata values");

        var rootOption = new Option<string>(
            "--root",
            () => Directory.GetCurrentDirectory(),
            "Repository root path");
        var issuesOption = new Option<string?>(
            "--issues",
            "Comma-separated issue numbers (null means all)");

        resetCommand.AddOption(rootOption);
        resetCommand.AddOption(issuesOption);

        resetCommand.SetHandler(async (string root, string? issues) =>
        {
            var issueNumbers = string.IsNullOrEmpty(issues)
                ? null
                : issues.Split(',').Select(int.Parse).ToList();

            var cmd = services.GetRequiredService<ResetPackagesCommand>();
            await cmd.ExecuteAsync(root, issueNumbers, CancellationToken.None);
        }, rootOption, issuesOption);

        return resetCommand;
    }

    private static Command BuildReportCommand(IServiceProvider services)
    {
        var reportCommand = new Command("report", "Generate and check reports");

        var generate = new Command("generate", "Generate test report");
        var rootOption = new Option<string>(
            "--root",
            () => Directory.GetCurrentDirectory(),
            "Repository root path");
        generate.AddOption(rootOption);
        generate.SetHandler(async (string root) =>
        {
            var cmd = services.GetRequiredService<GenerateReportCommand>();
            await cmd.ExecuteAsync(root, CancellationToken.None);
        }, rootOption);

        var checkRegressions = new Command("check-regressions", "Check for regression failures");
        var rootOption2 = new Option<string>(
            "--root",
            () => Directory.GetCurrentDirectory(),
            "Repository root path");
        checkRegressions.AddOption(rootOption2);
        checkRegressions.SetHandler(async (string root) =>
        {
            var cmd = services.GetRequiredService<CheckRegressionsCommand>();
            var exitCode = await cmd.ExecuteAsync(root, CancellationToken.None);
            Environment.Exit(exitCode);
        }, rootOption2);

        reportCommand.AddCommand(generate);
        reportCommand.AddCommand(checkRegressions);

        return reportCommand;
    }

    private static Command BuildMergeCommand(IServiceProvider services)
    {
        var mergeCommand = new Command("merge", "Merge results from multiple runs");

        var linuxOption = new Option<string>(
            "--linux",
            "Path to Linux artifacts");
        var windowsOption = new Option<string>(
            "--windows",
            "Path to Windows artifacts");
        var outputOption = new Option<string>(
            "--output",
            () => Directory.GetCurrentDirectory(),
            "Output path");

        mergeCommand.AddOption(linuxOption);
        mergeCommand.AddOption(windowsOption);
        mergeCommand.AddOption(outputOption);

        mergeCommand.SetHandler(async (string linux, string windows, string output) =>
        {
            var cmd = services.GetRequiredService<MergeResultsCommand>();
            await cmd.ExecuteAsync(linux, windows, output, CancellationToken.None);
        }, linuxOption, windowsOption, outputOption);

        return mergeCommand;
    }
}
