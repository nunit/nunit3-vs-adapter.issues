using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Text;

namespace IssueRunner.Services;

/// <summary>
/// Helper service for executing external processes.
/// </summary>
public sealed class ProcessExecutor
{
    private readonly ILogger<ProcessExecutor> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProcessExecutor"/> class.
    /// </summary>
    public ProcessExecutor(ILogger<ProcessExecutor> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Executes a command and returns the result.
    /// </summary>
    public async Task<(int ExitCode, string Output, string Error)> ExecuteAsync(
        string fileName,
        string arguments,
        string workingDirectory,
        int timeoutSeconds,
        CancellationToken cancellationToken)
    {
        var outputBuilder = new StringBuilder();
        var errorBuilder = new StringBuilder();

        var startInfo = new ProcessStartInfo
        {
            FileName = fileName,
            Arguments = arguments,
            WorkingDirectory = workingDirectory,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = new Process { StartInfo = startInfo };
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(
            cancellationToken);

        cts.CancelAfter(TimeSpan.FromSeconds(timeoutSeconds));

        process.OutputDataReceived += (_, e) =>
        {
            if (e.Data != null)
            {
                outputBuilder.AppendLine(e.Data);
            }
        };

        process.ErrorDataReceived += (_, e) =>
        {
            if (e.Data != null)
            {
                errorBuilder.AppendLine(e.Data);
            }
        };

        try
        {
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            await process.WaitForExitAsync(cts.Token);

            return (process.ExitCode, outputBuilder.ToString(), errorBuilder.ToString());
        }
        catch (OperationCanceledException)
        {
            try
            {
                process.Kill(entireProcessTree: true);
            }
            catch
            {
                // Ignore errors when killing
            }

            _logger.LogWarning(
                "Process timed out after {Timeout}s: {FileName} {Arguments}",
                timeoutSeconds,
                fileName,
                arguments);

            return (-1, outputBuilder.ToString(), "Process timed out");
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error executing process: {FileName} {Arguments}",
                fileName,
                arguments);

            return (-1, outputBuilder.ToString(), ex.Message);
        }
    }
}
