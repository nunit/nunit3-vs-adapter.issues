namespace IssueRunner.Models;

/// <summary>
/// Options for running tests.
/// </summary>
public sealed class RunOptions
{
    /// <summary>
    /// Gets or sets the scope of tests to run.
    /// </summary>
    public TestScope Scope { get; init; } = TestScope.All;

    /// <summary>
    /// Gets or sets specific issue numbers to run (null means all).
    /// </summary>
    public List<int>? IssueNumbers { get; init; }

    /// <summary>
    /// Gets or sets the timeout in seconds per command.
    /// </summary>
    public int TimeoutSeconds { get; init; } = 600;

    /// <summary>
    /// Gets or sets whether to skip .NET Framework tests.
    /// </summary>
    public bool SkipNetFx { get; init; }

    /// <summary>
    /// Gets or sets whether to run only .NET Framework tests.
    /// </summary>
    public bool OnlyNetFx { get; init; }

    /// <summary>
    /// Gets or sets whether to update only NUnit packages.
    /// </summary>
    public bool NUnitOnly { get; init; }

    /// <summary>
    /// Gets or sets the package feed option.
    /// </summary>
    public PackageFeed Feed { get; init; } = PackageFeed.Stable;

    /// <summary>
    /// Gets or sets the execution mode filter.
    /// </summary>
    public ExecutionMode ExecutionMode { get; init; } = ExecutionMode.All;

    /// <summary>
    /// Gets or sets the logging verbosity.
    /// </summary>
    public LogVerbosity Verbosity { get; init; } = LogVerbosity.Normal;

    /// <summary>
    /// Gets or sets whether to rerun only failed tests from test-fails.json.
    /// </summary>
    public bool RerunFailedTests { get; init; }
}

/// <summary>
/// Test scope options.
/// </summary>
public enum TestScope
{
    /// <summary>All issues.</summary>
    All,
    
    /// <summary>Only issues without test_result.</summary>
    New,
    
    /// <summary>Issues without test_result or with previous failures.</summary>
    NewAndFailed,
    
    /// <summary>Only closed issues (regression tests).</summary>
    RegressionOnly,
    
    /// <summary>Only open issues.</summary>
    OpenOnly
}

/// <summary>
/// Execution mode filter.
/// </summary>
public enum ExecutionMode
{
    /// <summary>All execution modes.</summary>
    All,
    
    /// <summary>Only direct dotnet test execution.</summary>
    Direct,
    
    /// <summary>Only custom script execution.</summary>
    Custom
}

/// <summary>
/// Logging verbosity options.
/// </summary>
public enum LogVerbosity
{
    /// <summary>Normal output - key steps only.</summary>
    Normal,
    
    /// <summary>Verbose output - detailed diagnostic info.</summary>
    Verbose
}

/// <summary>
/// Package feed options.
/// </summary>
public enum PackageFeed
{
    /// <summary>Stable packages from nuget.org only.</summary>
    Stable,
    
    /// <summary>Beta packages - nuget.org with prerelease enabled.</summary>
    Beta,
    
    /// <summary>Alpha packages - nuget.org + myget with prerelease enabled.</summary>
    Alpha,
    
    /// <summary>Local feed at C:\local with prerelease enabled.</summary>
    Local
}
