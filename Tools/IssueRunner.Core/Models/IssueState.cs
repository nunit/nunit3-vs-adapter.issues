namespace IssueRunner.Models;

/// <summary>
/// Represents the detailed state of an issue.
/// </summary>
public enum IssueState
{
    /// <summary>Issue has metadata but no test results yet.</summary>
    New,
    
    /// <summary>Issue has metadata and has been processed (e.g., has test results).</summary>
    Synced,
    
    /// <summary>Issue failed during the restore step.</summary>
    FailedRestore,
    
    /// <summary>Issue failed during the compile/build step.</summary>
    FailedCompile,
    
    /// <summary>Issue is runnable and passed restore/compile.</summary>
    Runnable,
    
    /// <summary>Issue is skipped due to a marker file.</summary>
    Skipped
}


