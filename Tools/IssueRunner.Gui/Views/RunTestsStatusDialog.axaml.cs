using Avalonia.Controls;
using Avalonia.ReactiveUI;
using IssueRunner.Gui.ViewModels;

namespace IssueRunner.Gui.Views;

public partial class RunTestsStatusDialog : ReactiveWindow<RunTestsStatusViewModel>
{
    public RunTestsStatusDialog() : this(new RunTestsStatusViewModel())
    {
    }

    public RunTestsStatusDialog(RunTestsStatusViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}

