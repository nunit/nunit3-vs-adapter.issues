using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using IssueRunner.Gui.ViewModels;

namespace IssueRunner.Gui.Views;

public partial class RunTestsOptionsDialog : Window
{
    public RunTestsOptionsDialog()
    {
        InitializeComponent();
    }

    public RunTestsOptionsDialog(RunTestsOptionsViewModel viewModel) : this()
    {
        DataContext = viewModel;
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}

