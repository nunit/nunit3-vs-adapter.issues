using ReactiveUI;
using System.Reactive;
using Avalonia.Controls;

namespace IssueRunner.Gui.ViewModels;

/// <summary>
/// ViewModel for the Sync from GitHub dialog.
/// </summary>
public class SyncFromGitHubViewModel : ViewModelBase
{
    private string _statusText = "Ready to sync...";
    private int _issuesSynced = 0;
    private int _issuesFound = 0;
    private int _issuesNotFound = 0;
    private bool _isRunning = false;
    private bool _canCancel = true;
    private string _syncMode = "All";
    private bool _updateExisting = false;
    private double _progress = 0.0;
    private int _totalIssues = 0;
    private bool _syncCompleted = false;
    private bool _showProgress = false;
    private Avalonia.Controls.Window? _dialogWindow;

    public SyncFromGitHubViewModel()
    {
        // Commands will be set by MainViewModel before showing dialog
        StartCommand = ReactiveCommand.Create(() => { });
        CancelCommand = ReactiveCommand.Create(() => { });
        SyncToFoldersCommand = ReactiveCommand.Create(() => { });
        CloseCommand = ReactiveCommand.Create(() => CloseDialog());
        ToggleProgressViewCommand = ReactiveCommand.Create(() => ToggleProgressView());
    }

    /// <summary>
    /// Sets the dialog window reference for closing.
    /// </summary>
    public void SetDialogWindow(Avalonia.Controls.Window window)
    {
        _dialogWindow = window;
    }

    public ReactiveCommand<Unit, Unit> StartCommand { get; set; }
    public ReactiveCommand<Unit, Unit> CancelCommand { get; set; }
    public ReactiveCommand<Unit, Unit> SyncToFoldersCommand { get; set; }
    public ReactiveCommand<Unit, Unit> CloseCommand { get; set; }
    public ReactiveCommand<Unit, Unit> ToggleProgressViewCommand { get; set; }

    public string StatusText
    {
        get => _statusText;
        set => SetProperty(ref _statusText, value);
    }

    public int IssuesSynced
    {
        get => _issuesSynced;
        set
        {
            if (SetProperty(ref _issuesSynced, value))
            {
                UpdateProgress();
            }
        }
    }

    public int IssuesFound
    {
        get => _issuesFound;
        set => SetProperty(ref _issuesFound, value);
    }

    public int IssuesNotFound
    {
        get => _issuesNotFound;
        set => SetProperty(ref _issuesNotFound, value);
    }

    public bool IsRunning
    {
        get => _isRunning;
        set
        {
            if (SetProperty(ref _isRunning, value))
            {
                CanCancel = value;
                OnPropertyChanged(nameof(CanStart));
                OnPropertyChanged(nameof(CanSyncToFolders));
            }
        }
    }

    public bool CanCancel
    {
        get => _canCancel;
        set => SetProperty(ref _canCancel, value);
    }

    public bool CanStart => !IsRunning;

    public string SyncMode
    {
        get => _syncMode;
        set => SetProperty(ref _syncMode, value);
    }

    public bool UpdateExisting
    {
        get => _updateExisting;
        set => SetProperty(ref _updateExisting, value);
    }

    public double Progress
    {
        get => _progress;
        set => SetProperty(ref _progress, value);
    }

    public int TotalIssues
    {
        get => _totalIssues;
        set
        {
            if (SetProperty(ref _totalIssues, value))
            {
                UpdateProgress();
            }
        }
    }

    public bool SyncCompleted
    {
        get => _syncCompleted;
        set
        {
            if (SetProperty(ref _syncCompleted, value))
            {
                // SyncCompleted no longer affects CanSyncToFolders or CanStart
                // Both buttons are independent now
            }
        }
    }

    public bool CanSyncToFolders => !IsRunning;

    public bool ShowProgress
    {
        get => _showProgress;
        set
        {
            if (SetProperty(ref _showProgress, value))
            {
                OnPropertyChanged(nameof(ProgressViewButtonText));
            }
        }
    }

    public string ProgressViewButtonText => ShowProgress ? "Hide Progress" : "Show Progress";

    public string ProgressText
    {
        get
        {
            if (TotalIssues > 0)
            {
                return $"{IssuesSynced} / {TotalIssues} issues synced";
            }
            return IssuesSynced > 0 ? $"{IssuesSynced} issues synced" : "";
        }
    }

    private void UpdateProgress()
    {
        if (TotalIssues > 0)
        {
            Progress = (double)IssuesSynced / TotalIssues * 100.0;
        }
        else
        {
            Progress = 0.0;
        }
        OnPropertyChanged(nameof(ProgressText));
    }

    public void Reset()
    {
        StatusText = "Ready to sync...";
        IssuesSynced = 0;
        IssuesFound = 0;
        IssuesNotFound = 0;
        Progress = 0.0;
        TotalIssues = 0;
        IsRunning = false;
        SyncCompleted = false;
        ShowProgress = false;
    }

    public void ToggleProgressView()
    {
        ShowProgress = !ShowProgress;
    }

    private void CloseDialog()
    {
        if (_dialogWindow != null)
        {
            _dialogWindow.Close();
        }
        else if (Avalonia.Application.Current?.ApplicationLifetime is Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop)
        {
            var window = desktop.Windows.OfType<Avalonia.Controls.Window>()
                .FirstOrDefault(w => w.DataContext == this);
            window?.Close();
        }
    }
}

