using System.Text;

namespace IssueRunner.Gui.ViewModels;

/// <summary>
/// TextWriter that writes to the log in real-time.
/// </summary>
internal class RealTimeLogWriter(Action<string> appendLog, Action<string>? updateStatus = null)
    : TextWriter
{
    private readonly StringBuilder _currentLine = new();
    private readonly object _lock = new();

    public override Encoding Encoding => Encoding.UTF8;

    public override void Write(char value)
    {
        lock (_lock)
        {
            if (value == '\n')
            {
                FlushLine();
            }
            else if (value != '\r')
            {
                _currentLine.Append(value);
            }
        }
    }

    public override void Write(string? value)
    {
        if (value == null) return;
        
        lock (_lock)
        {
            foreach (var c in value)
            {
                if (c == '\n')
                {
                    FlushLine();
                }
                else if (c != '\r')
                {
                    _currentLine.Append(c);
                }
            }
        }
    }

    public override void Flush()
    {
        lock (_lock)
        {
            if (_currentLine.Length > 0)
            {
                FlushLine();
            }
        }
    }

    private void FlushLine()
    {
        if (_currentLine.Length > 0)
        {
            var line = _currentLine.ToString().TrimEnd();
            if (!string.IsNullOrWhiteSpace(line))
            {
                // Dispatch to UI thread
                Avalonia.Threading.Dispatcher.UIThread.Post(() =>
                {
                    try
                    {
                        appendLog(line);
                        updateStatus?.Invoke(line);
                    }
                    catch
                    {
                        // Ignore errors in logging
                    }
                });
            }
            _currentLine.Clear();
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Flush();
        }
        base.Dispose(disposing);
    }
}