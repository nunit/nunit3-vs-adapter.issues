using Avalonia.Threading;
using ReactiveUI;
using System;
using System.Reactive;

namespace IssueRunner.Gui.ViewModels;

/// <summary>
/// ViewModel for the Run Tests status dialog.
/// </summary>
public class RunTestsStatusViewModel : ViewModelBase
{
    private string _currentStatus = "Initializing...";
    private string _currentIssue = "";
    private int _totalIssues = 0;
    private int _processedIssues = 0;
    private int _succeeded = 0;
    private int _failed = 0;
    private int _skipped = 0;
    private int _notCompiling = 0;
    private double _progress = 0.0;
    private bool _isRunning = false;
    private bool _canSetBaseline = false;
    private bool _isStableFeed = false;
    private string _currentPhase = "";
    private int _currentPhaseProgress = 0;
    private int _currentPhaseTotal = 0;
    private DateTime? _startTime;
    private DateTime? _endTime;
    private DispatcherTimer? _durationTimer;

    public RunTestsStatusViewModel()
    {
        CancelCommand = ReactiveCommand.Create(() => { });
        SetBaselineCommand = ReactiveCommand.Create(() => { });
        CloseCommand = ReactiveCommand.Create(() => { });
    }

    public ReactiveCommand<Unit, Unit> CancelCommand { get; set; }
    public ReactiveCommand<Unit, Unit> SetBaselineCommand { get; set; }
    public ReactiveCommand<Unit, Unit> CloseCommand { get; set; }

    public bool CanSetBaseline
    {
        get => _canSetBaseline;
        set => SetProperty(ref _canSetBaseline, value);
    }

    public bool IsStableFeed
    {
        get => _isStableFeed;
        set
        {
            if (SetProperty(ref _isStableFeed, value))
            {
                UpdateCanSetBaseline();
            }
        }
    }

    public string CurrentStatus
    {
        get => _currentStatus;
        set => SetProperty(ref _currentStatus, value);
    }

    public string CurrentIssue
    {
        get => _currentIssue;
        set => SetProperty(ref _currentIssue, value);
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

    public int ProcessedIssues
    {
        get => _processedIssues;
        set
        {
            if (SetProperty(ref _processedIssues, value))
            {
                UpdateProgress();
            }
        }
    }

    public int Succeeded
    {
        get => _succeeded;
        set => SetProperty(ref _succeeded, value);
    }

    public int Failed
    {
        get => _failed;
        set => SetProperty(ref _failed, value);
    }

    public int Skipped
    {
        get => _skipped;
        set => SetProperty(ref _skipped, value);
    }

    public int NotCompiling
    {
        get => _notCompiling;
        set => SetProperty(ref _notCompiling, value);
    }

    public double Progress
    {
        get => _progress;
        private set => SetProperty(ref _progress, value);
    }

    public bool IsRunning
    {
        get => _isRunning;
        set
        {
            if (SetProperty(ref _isRunning, value))
            {
                UpdateCanSetBaseline();
                UpdateDurationTimer();
            }
        }
    }

    private void UpdateCanSetBaseline()
    {
        // Can set baseline when run is complete and feed is stable
        CanSetBaseline = !IsRunning && IsStableFeed;
    }

    public string ProgressText => $"{ProcessedIssues} / {TotalIssues} issues processed";

    public string CurrentPhase
    {
        get => _currentPhase;
        set => SetProperty(ref _currentPhase, value);
    }

    public int CurrentPhaseProgress
    {
        get => _currentPhaseProgress;
        set
        {
            if (SetProperty(ref _currentPhaseProgress, value))
            {
                OnPropertyChanged(nameof(CurrentPhaseProgressText));
            }
        }
    }

    public int CurrentPhaseTotal
    {
        get => _currentPhaseTotal;
        set
        {
            if (SetProperty(ref _currentPhaseTotal, value))
            {
                OnPropertyChanged(nameof(CurrentPhaseProgressText));
            }
        }
    }

    public string CurrentPhaseProgressText
    {
        get
        {
            if (CurrentPhaseTotal > 0)
            {
                return $"{CurrentPhaseProgress} / {CurrentPhaseTotal}";
            }
            return CurrentPhaseProgress > 0 ? $"{CurrentPhaseProgress}" : "";
        }
    }

    private void UpdateProgress()
    {
        if (TotalIssues > 0)
        {
            Progress = (double)ProcessedIssues / TotalIssues * 100.0;
        }
        else
        {
            Progress = 0.0;
        }
        OnPropertyChanged(nameof(ProgressText));
    }

    public DateTime? StartTime
    {
        get => _startTime;
        set
        {
            if (SetProperty(ref _startTime, value))
            {
                OnPropertyChanged(nameof(Duration));
                OnPropertyChanged(nameof(DurationText));
                UpdateDurationTimer();
            }
        }
    }

    public DateTime? EndTime
    {
        get => _endTime;
        set
        {
            if (SetProperty(ref _endTime, value))
            {
                OnPropertyChanged(nameof(Duration));
                OnPropertyChanged(nameof(DurationText));
            }
        }
    }

    public TimeSpan? Duration
    {
        get
        {
            if (_startTime.HasValue && _endTime.HasValue)
            {
                return _endTime.Value - _startTime.Value;
            }
            if (_startTime.HasValue)
            {
                // If we have a start time, calculate elapsed time (even while running)
                return DateTime.Now - _startTime.Value;
            }
            return null;
        }
    }

    public string DurationText
    {
        get
        {
            var duration = Duration;
            if (duration.HasValue)
            {
                var totalSeconds = (int)duration.Value.TotalSeconds;
                var hours = totalSeconds / 3600;
                var minutes = (totalSeconds % 3600) / 60;
                var seconds = totalSeconds % 60;

                if (hours > 0)
                {
                    return $"{hours}h {minutes}m {seconds}s";
                }
                if (minutes > 0)
                {
                    return $"{minutes}m {seconds}s";
                }
                return $"{seconds}s";
            }
            return "";
        }
    }

    private void UpdateDurationTimer()
    {
        if (_isRunning && _startTime.HasValue)
        {
            // Start timer to update duration every second
            if (_durationTimer == null)
            {
                _durationTimer = new DispatcherTimer
                {
                    Interval = TimeSpan.FromSeconds(1)
                };
                _durationTimer.Tick += (s, e) =>
                {
                    OnPropertyChanged(nameof(Duration));
                    OnPropertyChanged(nameof(DurationText));
                };
                _durationTimer.Start();
            }
        }
        else
        {
            // Stop timer when not running
            if (_durationTimer != null)
            {
                _durationTimer.Stop();
                _durationTimer = null;
            }
        }
    }

    public void Reset()
    {
        CurrentStatus = "Initializing...";
        CurrentIssue = "";
        TotalIssues = 0;
        ProcessedIssues = 0;
        Succeeded = 0;
        Failed = 0;
        Skipped = 0;
        NotCompiling = 0;
        Progress = 0.0;
        IsRunning = true;
        StartTime = null;
        EndTime = null;
        if (_durationTimer != null)
        {
            _durationTimer.Stop();
            _durationTimer = null;
        }
    }
}

