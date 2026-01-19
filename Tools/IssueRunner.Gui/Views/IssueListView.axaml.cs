using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using System.Diagnostics;

namespace IssueRunner.Gui.Views;

public partial class IssueListView : UserControl
{
    public IssueListView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void OnIssueNumberClicked(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (sender is Button { DataContext: ViewModels.IssueListItem item })
        {
            if (!string.IsNullOrEmpty(item.GitHubUrl))
            {
                try
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = item.GitHubUrl,
                        UseShellExecute = true
                    });
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Failed to open URL {item.GitHubUrl}: {ex.Message}");
                }
            }
        }
    }
}





