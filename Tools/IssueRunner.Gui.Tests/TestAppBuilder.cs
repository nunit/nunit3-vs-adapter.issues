using Avalonia;
using Avalonia.Headless;
using Avalonia.ReactiveUI;

[assembly: AvaloniaTestApplication(typeof(IssueRunner.Gui.Tests.TestAppBuilder))]

namespace IssueRunner.Gui.Tests;

public class TestAppBuilder
{
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<IssueRunner.Gui.App>()
            .UseHeadless(new AvaloniaHeadlessPlatformOptions())
            .WithInterFont()
            .LogToTrace()
            .UseReactiveUI();
}


