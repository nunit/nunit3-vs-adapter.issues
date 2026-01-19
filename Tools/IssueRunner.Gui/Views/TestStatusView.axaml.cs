using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace IssueRunner.Gui.Views;

public partial class TestStatusView : UserControl
{
    public TestStatusView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}





