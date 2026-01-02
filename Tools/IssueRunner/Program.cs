using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;
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

        var rootOption = (Option<string>)rootCommand.Options.Single(o => o.Name == "root");
        var parser = new CommandLineBuilder(rootCommand)
            .AddMiddleware(async (context, next) =>
            {
                // Only initialize environment for commands that actually operate on a repository.
                // Merge operates on artifact folders and does not require repository.json.
                if (!ReferenceEquals(context.ParseResult.CommandResult.Command, rootCommand) &&
                    !string.Equals(context.ParseResult.CommandResult.Command.Name, "merge", StringComparison.OrdinalIgnoreCase))
                {
                    var env = services.GetRequiredService<IEnvironmentService>();
                    var chosenRoot = context.ParseResult.GetValueForOption(rootOption)
                                     ?? env.ResolveRepositoryRoot(Directory.GetCurrentDirectory());
                    env.AddRoot(chosenRoot);
                }

                await next(context);
            })
            .UseDefaults()
            .Build();

        return await parser.InvokeAsync(args);
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

            // Suppress noisy HttpClient logging
            builder.AddFilter("System.Net.Http.HttpClient", LogLevel.Warning);
        });

        services.AddHttpClient<IGitHubApiService, GitHubApiService>();

        services.AddSingleton<IIssueDiscoveryService, IssueDiscoveryService>();
        services.AddSingleton<IProjectAnalyzerService, ProjectAnalyzerService>();
        services.AddSingleton<IFrameworkUpgradeService, FrameworkUpgradeService>();
        services.AddSingleton<IProcessExecutor, ProcessExecutor>();
        services.AddSingleton<IPackageUpdateService, PackageUpdateService>();
        services.AddSingleton<INuGetPackageVersionService, NuGetPackageVersionService>();
        services.AddSingleton<ITestExecutionService, TestExecutionService>();
        services.AddSingleton<IEnvironmentService, EnvironmentService>();
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
            "IssueRunner - NUnit issue test automation tool");

        var rootOption = new Option<string>(
            "--root",
            () => services.GetRequiredService<IEnvironmentService>().ResolveRepositoryRoot(Directory.GetCurrentDirectory()),
            "Repository root path (can also set ISSUERUNNER_ROOT environment variable)");
        rootCommand.AddGlobalOption(rootOption);

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
        syncFromGitHub.SetHandler(async () =>
        {
            var cmd = services.GetRequiredService<SyncFromGitHubCommand>();
            await cmd.ExecuteAsync(null, CancellationToken.None);
        });

        var syncToFolders = new Command("sync-to-folders", "Sync metadata from central file to issue folders");
        syncToFolders.SetHandler(async () =>
        {
            var env = services.GetRequiredService<IEnvironmentService>();
            var cmd = services.GetRequiredService<SyncToFoldersCommand>();
            await cmd.ExecuteAsync(env.Root, null, CancellationToken.None);

        });

        metadataCommand.AddCommand(syncFromGitHub);
        metadataCommand.AddCommand(syncToFolders);

        return metadataCommand;
    }

    private static Command BuildRunCommand(IServiceProvider services)
    {
        var runCommand = new Command("run", "Run tests for issues");

        var scopeOption = new Option<TestScope>(
            "--scope",
            () => TestScope.All,
            "Test scope: All (default), New (untested), NewAndFailed (new or previously failed), RegressionOnly (closed issues), OpenOnly (open issues)");
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
            "Filter by execution method: All (default), Direct (dotnet test only), Custom (custom scripts only)");
        var verbosityOption = new Option<LogVerbosity>(
            "--verbosity",
            () => LogVerbosity.Normal,
            "Logging verbosity (Normal or Verbose)");
        var feedOption = new Option<PackageFeed>(
            "--feed",
            () => PackageFeed.Stable,
            "Package feed: Stable (nuget.org), Beta (nuget.org+prerelease), Alpha (nuget.org+myget+prerelease), Local (C:\\nuget+prerelease)");
        var rerunFailedOption = new Option<bool>(
            "--rerun-failed",
            "Rerun only failed tests from test-fails.json");

        runCommand.AddOption(scopeOption);
        runCommand.AddOption(issuesOption);
        runCommand.AddOption(timeoutOption);
        runCommand.AddOption(skipNetFxOption);
        runCommand.AddOption(onlyNetFxOption);
        runCommand.AddOption(nunitOnlyOption);
        runCommand.AddOption(executionModeOption);
        runCommand.AddOption(verbosityOption);
        runCommand.AddOption(feedOption);
        runCommand.AddOption(rerunFailedOption);

        runCommand.SetHandler(async (context) =>
        {
            var scope = context.ParseResult.GetValueForOption(scopeOption);
            var issues = context.ParseResult.GetValueForOption(issuesOption);
            var timeout = context.ParseResult.GetValueForOption(timeoutOption);
            var skipNetFx = context.ParseResult.GetValueForOption(skipNetFxOption);
            var onlyNetFx = context.ParseResult.GetValueForOption(onlyNetFxOption);
            var nunitOnly = context.ParseResult.GetValueForOption(nunitOnlyOption);
            var executionMode = context.ParseResult.GetValueForOption(executionModeOption);
            var verbosity = context.ParseResult.GetValueForOption(verbosityOption);
            var feed = context.ParseResult.GetValueForOption(feedOption);
            var rerunFailed = context.ParseResult.GetValueForOption(rerunFailedOption);

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
                Feed = feed,
                RerunFailedTests = rerunFailed
            };

            var env = services.GetRequiredService<IEnvironmentService>();
            var cmd = services.GetRequiredService<RunTestsCommand>();
            await cmd.ExecuteAsync(env.Root, options, CancellationToken.None);

        });

        return runCommand;
    }

    private static Command BuildResetCommand(IServiceProvider services)
    {
        var resetCommand = new Command("reset", "Reset package versions to metadata values");

        var issuesOption = new Option<string?>(
            "--issues",
            "Comma-separated issue numbers (null means all)");

        resetCommand.AddOption(issuesOption);

        resetCommand.SetHandler(async issues =>
        {
            var issueNumbers = string.IsNullOrEmpty(issues)
                ? null
                : issues.Split(',').Select(int.Parse).ToList();

            var env = services.GetRequiredService<IEnvironmentService>();
            var cmd = services.GetRequiredService<ResetPackagesCommand>();
            await cmd.ExecuteAsync(env.Root, issueNumbers, CancellationToken.None);
        }, issuesOption);

        return resetCommand;
    }

    private static Command BuildReportCommand(IServiceProvider services)
    {
        var reportCommand = new Command("report", "Generate and check reports");

        var generate = new Command("generate", "Generate test report");
        generate.SetHandler(async () =>
        {
            var cmd = services.GetRequiredService<GenerateReportCommand>();
            await cmd.ExecuteAsync(CancellationToken.None);
        });

        var checkRegressions = new Command("check-regressions", "Check for regression failures");
        checkRegressions.SetHandler(async () =>
        {
            var env = services.GetRequiredService<IEnvironmentService>();
            var cmd = services.GetRequiredService<CheckRegressionsCommand>();
            var exitCode = await cmd.ExecuteAsync(env.Root, CancellationToken.None);
            Environment.Exit(exitCode);
        });

        reportCommand.AddCommand(generate);
        reportCommand.AddCommand(checkRegressions);

        return reportCommand;
    }

    private static Command BuildMergeCommand(IServiceProvider services)
    {
        var mergeCommand = new Command("merge", "Merge results from multiple runs");

        var linuxOption = new Option<string>("--linux", "Path to Linux artifacts");
        var windowsOption = new Option<string>("--windows", "Path to Windows artifacts");
        var outputOption = new Option<string>("--output", Directory.GetCurrentDirectory, "Output path");

        mergeCommand.AddOption(linuxOption);
        mergeCommand.AddOption(windowsOption);
        mergeCommand.AddOption(outputOption);

        mergeCommand.SetHandler(async (linux, windows, output) =>
        {
            var cmd = services.GetRequiredService<MergeResultsCommand>();
            await cmd.ExecuteAsync(linux, windows, output, CancellationToken.None);
        }, linuxOption, windowsOption, outputOption);

        return mergeCommand;
    }
}
