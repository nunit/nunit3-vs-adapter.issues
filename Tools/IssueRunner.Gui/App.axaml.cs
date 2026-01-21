using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using IssueRunner.Commands;
using IssueRunner.Gui.ViewModels;
using IssueRunner.Gui.Views;
using IssueRunner.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.IO;

namespace IssueRunner.Gui;

public partial class App : Application
{
    public static IServiceProvider? Services { get; private set; }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            try
            {
                Services = ConfigureServices();
                var mainViewModel = Services.GetRequiredService<MainViewModel>();
                desktop.MainWindow = new MainWindow
                {
                    DataContext = mainViewModel
                };
            }
            catch (Exception ex)
            {
                // Log to file since UI might not be ready
                var logPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "IssueRunner", "crash.log");
                Directory.CreateDirectory(Path.GetDirectoryName(logPath)!);
                File.AppendAllText(logPath, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Error in OnFrameworkInitializationCompleted: {ex.Message}\n{ex.StackTrace}\n");
                
                // Try to show error in a message box if possible
                try
                {
                    var errorWindow = new Avalonia.Controls.Window
                    {
                        Title = "Fatal Error",
                        Width = 600,
                        Height = 400,
                        Content = new Avalonia.Controls.TextBlock
                        {
                            Text = $"Failed to initialize application:\n\n{ex.Message}\n\n{ex.StackTrace}",
                            TextWrapping = Avalonia.Media.TextWrapping.Wrap,
                            Margin = new Avalonia.Thickness(20)
                        }
                    };
                    desktop.MainWindow = errorWindow;
                }
                catch
                {
                    // If we can't even show error window, just throw
                    throw;
                }
            }
        }

        base.OnFrameworkInitializationCompleted();
    }

    private static IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        // Logging
        services.AddLogging(builder =>
        {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Information);
        });

        // Core services (same as CLI)
        services.AddHttpClient<IGitHubApiService, GitHubApiService>();
        services.AddSingleton<IIssueDiscoveryService, IssueDiscoveryService>();
        services.AddSingleton<IProjectAnalyzerService, ProjectAnalyzerService>();
        services.AddSingleton<IFrameworkUpgradeService, FrameworkUpgradeService>();
        services.AddSingleton<IProcessExecutor, ProcessExecutor>();
        services.AddSingleton<IPackageUpdateService, PackageUpdateService>();
        services.AddSingleton<INuGetPackageVersionService, NuGetPackageVersionService>();
        services.AddSingleton<ITestExecutionService, TestExecutionService>();
        services.AddSingleton<IEnvironmentService, EnvironmentService>();
        services.AddSingleton<ITestResultDiffService, TestResultDiffService>();
        services.AddSingleton<IMarkerService, MarkerService>();
        services.AddSingleton<ReportGeneratorService>();

        // Commands
        services.AddTransient<SyncFromGitHubCommand>();
        services.AddTransient<SyncToFoldersCommand>();
        services.AddTransient<RunTestsCommand>();
        services.AddTransient<ResetPackagesCommand>();
        services.AddTransient<GenerateReportCommand>();
        services.AddTransient<CheckRegressionsCommand>();
        services.AddTransient<MergeResultsCommand>();

        // ViewModels
        services.AddSingleton<MainViewModel>();
        services.AddSingleton<ViewModels.IssueListViewModel>();
        services.AddTransient<ViewModels.RunTestsOptionsViewModel>();

        // GUI-specific services (set MainViewModel after creation)
        services.AddSingleton<Services.IProgressReporter>(sp =>
        {
            var reporter = new Services.ProgressReporter();
            var mainViewModel = sp.GetRequiredService<MainViewModel>();
            reporter.SetMainViewModel(mainViewModel);
            return reporter;
        });

        return services.BuildServiceProvider();
    }
}
