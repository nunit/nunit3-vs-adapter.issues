using Avalonia;
using Avalonia.ReactiveUI;

namespace IssueRunner.Gui;

internal class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        // Global exception handlers
        AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
        {
            var exception = e.ExceptionObject as Exception;
            LogException("Unhandled Exception", exception);
        };

        try
        {
            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
        }
        catch (Exception ex)
        {
            LogException("Fatal Exception in Main", ex);
            throw;
        }
    }

    private static void LogException(string title, Exception? ex)
    {
        try
        {
            var logPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "IssueRunner", "crash.log");
            Directory.CreateDirectory(Path.GetDirectoryName(logPath)!);
            
            using var writer = new StreamWriter(logPath, append: true);
            writer.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {title}");
            if (ex != null)
            {
                writer.WriteLine($"Message: {ex.Message}");
                writer.WriteLine($"Type: {ex.GetType().FullName}");
                writer.WriteLine($"Stack Trace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    writer.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
            }
            writer.WriteLine(new string('-', 80));
        }
        catch
        {
            // If we can't log, at least try to write to console
            Console.Error.WriteLine($"{title}: {ex?.Message}");
        }
    }

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace()
            .UseReactiveUI();
}

