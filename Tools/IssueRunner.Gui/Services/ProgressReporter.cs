using System.Collections.ObjectModel;
using IssueRunner.Gui.ViewModels;

namespace IssueRunner.Gui.Services;

/// <summary>
/// Implementation of progress reporter that updates the GUI.
/// </summary>
public sealed class ProgressReporter : IProgressReporter
{
    private MainViewModel? _mainViewModel;

    public void SetMainViewModel(MainViewModel mainViewModel)
    {
        _mainViewModel = mainViewModel;
    }

    public void ReportLog(string message)
    {
        _mainViewModel?.AppendLog(message);
    }

    public void ReportProgress(double progress)
    {
        if (_mainViewModel != null)
        {
            _mainViewModel.Progress = progress;
        }
    }

    public void ClearLog()
    {
        _mainViewModel?.ClearLog();
    }
}

