namespace IssueRunner.Gui.Services;

/// <summary>
/// Interface for reporting progress and log messages to the GUI.
/// </summary>
public interface IProgressReporter
{
    /// <summary>
    /// Reports a log message.
    /// </summary>
    void ReportLog(string message);

    /// <summary>
    /// Reports progress (0.0 to 1.0).
    /// </summary>
    void ReportProgress(double progress);

    /// <summary>
    /// Clears the log output.
    /// </summary>
    void ClearLog();
}





