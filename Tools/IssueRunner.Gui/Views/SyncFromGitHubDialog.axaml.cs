using Avalonia.Controls;
using Avalonia.ReactiveUI;
using IssueRunner.Gui.ViewModels;

namespace IssueRunner.Gui.Views;

/// <summary>
/// Dialog for syncing metadata from GitHub.
/// </summary>
public partial class SyncFromGitHubDialog : ReactiveWindow<SyncFromGitHubViewModel>
{
    public SyncFromGitHubDialog() : this(new SyncFromGitHubViewModel())
    {
    }

    public SyncFromGitHubDialog(SyncFromGitHubViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}

