# nunit3-vs-adapter.issues

Examples and repros of issues found in the NUnit Adapter

## Test Automation

### Using IssueRunner GUI (Recommended for Local Development)

The IssueRunner GUI provides a user-friendly interface for managing and running tests locally. It's built with Avalonia for cross-platform support (Windows, Linux, macOS).

**Building the GUI:**

```cmd
# Windows
.\Tools\build.cmd

# Linux/macOS
./Tools/build.sh
```

**Running the GUI:**

```cmd
# Windows
.\Tools\IssueRunner.Gui\bin\Release\net10.0\IssueRunner.Gui.exe

# Linux/macOS
./Tools/IssueRunner.Gui/bin/Release/net10.0/IssueRunner.Gui
```

The GUI provides:
- **Repository Management**: Auto-detection when launched from repository root, or browse to select a repository
- **Issue List View**: Visual display of all issues with filtering capabilities:
  - **Scope Filter**: Filter by test scope (All, Regression, Open) - based on GitHub issue state (closed = Regression, open = Open)
  - **State Filter**: Filter by issue state (All, New, Synced, Failed restore, Failed compile, Runnable, Skipped) - based on metadata presence, build status, and marker files
  - **Test Result Filter**: Filter by test result (All, Success, Fail, Not Tested)
  - Shows detailed state information (not compiling, skipped with marker reason, etc.)
- **Run Tests Options**: Configure test run settings (feed, test types, verbosity, etc.)
  - Options are saved in memory and remembered for subsequent runs
  - Shows issue counts for each scope option
- **Status Dialog**: Real-time progress tracking during test runs:
  - Overall progress bar and statistics (succeeded, failed, skipped, not compiling)
  - Current phase tracking (Updating packages, Running tests, etc.)
  - Phase-specific progress indicators
  - Cancel button to stop running tests
  - Set Baseline button (for stable feeds) to save current results as baseline
  - Close button to dismiss dialog after completion
- **Test Status View**: Compare current test results against baseline:
  - Shows current vs. baseline pass/fail counts
  - Displays improvements (fixed tests, new passes)
  - Displays regressions (new failures, previously passing tests)
- **Navigation**: Switch between Issue List and Test Status views using navigation buttons at the top of the main content area
- **Command Execution**: All CLI commands available through GUI:
  - Run Tests (with options dialog)
  - Sync from GitHub
  - Sync to Folders
  - Reset Packages
  - Generate Report
  - Check Regressions
- **Log Output Viewer**: Real-time console output display
- **Summary Display**: Repository summary with baseline comparison (if baseline exists)

**Note:** The GUI is for local development only. CI/CD pipelines should use the CLI executable.

#### GUI Testing

The GUI includes automated headless testing using Avalonia.Headless.NUnit, enabling automated testing of UI components, windows, buttons, and user interactions without requiring manual testing.

**Running GUI Tests:**

```cmd
cd Tools
dotnet test IssueRunner.Gui.Tests/IssueRunner.Gui.Tests.csproj
```

**Writing Headless Tests:**

Headless tests use the `[AvaloniaTest]` attribute and inherit from `HeadlessTestBase`:

```csharp
[AvaloniaTest]
public void MainWindow_CanBeCreated()
{
    var window = CreateTestWindow();
    Assert.That(window, Is.Not.Null);
}
```

The `HeadlessTestBase` class provides helper methods for creating test windows and service providers with mocked dependencies. Tests run without a display server, making them suitable for CI/CD pipelines.

### Using C# IssueRunner CLI (Recommended for CI/CD)

The new C# tool provides better performance and maintainability.

#### Building IssueRunner

**Using wrapper scripts (recommended):**

```cmd
# Windows
.\Tools\build.cmd

# Linux/macOS
./Tools/build.sh
```

**Or build directly:**

```bash
cd Tools
dotnet build -c Release
```

The build scripts will compile IssueRunner in Release mode and show you where to find the executable.

#### Using IssueRunner Across Repositories

IssueRunner can test issues in any repository, not just nunit3-vs-adapter.issues. This is useful for testing other issue repositories like nunit.issues.

**Three ways to specify the target repository:**

1. **Navigate to the target repository** (simplest):

   ```cmd
   cd C:\repos\nunit\nunit.issues
   ..\nunit3-vs-adapter.issues\Tools\run-tests.cmd --issues 1
   ```

2. **Set ISSUERUNNER_ROOT environment variable**:

   ```cmd
   # Windows
   set ISSUERUNNER_ROOT=C:\repos\nunit\nunit.issues
   ..\nunit3-vs-adapter.issues\Tools\run-tests.cmd --issues 1
   
   # Linux/macOS
   export ISSUERUNNER_ROOT=/home/user/repos/nunit.issues
   ../nunit3-vs-adapter.issues/Tools/run-tests.sh --issues 1
   ```

3. **Use --root parameter explicitly**:

   ```cmd
   IssueRunner run --root C:\repos\nunit\nunit.issues --issues 1
   ```

All wrapper scripts (run-tests, sync-from-github, sync-to-folders) support these methods.

#### Wrapper Scripts

For convenience, use the wrapper scripts that handle the full path to IssueRunner:

**Windows:**

```cmd
cd /path/to/your/test/repository
..\nunit3-vs-adapter.issues\Tools\run-tests.cmd [options]
..\nunit3-vs-adapter.issues\Tools\generate-report.cmd
..\nunit3-vs-adapter.issues\Tools\sync-from-github.cmd
..\nunit3-vs-adapter.issues\Tools\sync-to-folders.cmd
```

**Linux/macOS:**

```bash
cd /path/to/your/test/repository
../nunit3-vs-adapter.issues/Tools/run-tests.sh [options]
../nunit3-vs-adapter.issues/Tools/generate-report.sh
../nunit3-vs-adapter.issues/Tools/sync-from-github.sh
../nunit3-vs-adapter.issues/Tools/sync-to-folders.sh
```

#### IssueRunner Command Structure

```txt
issuerunner
├── metadata
│   ├── sync-from-github    Sync metadata from GitHub to central file
│   └── sync-to-folders     Sync metadata from central file to issue folders
├── run                      Run tests for issues
├── reset                    Reset package versions to metadata values
├── report
│   ├── generate            Generate test report
│   └── check-regressions   Check for regression failures
└── merge                    Merge multiple results files
```

#### Direct IssueRunner Usage

Alternatively, run IssueRunner directly:

```bash
cd Tools/IssueRunner/bin/Release/net10.0
./IssueRunner run [options]
```

**Options:**

- `--root <path>` - Repository root path (default: current directory, or ISSUERUNNER_ROOT environment variable)
- `--scope <All|Regression|Open>` - Test scope (All = all issues, Regression = closed issues, Open = open issues) (default: All)
- `--issues <numbers>` - Comma-separated issue numbers to run
- `--timeout <seconds>` - Timeout per command (default: 600)
- `--skip-netfx` - Skip .NET Framework tests
- `--only-netfx` - Run only .NET Framework tests
- `--nunit-only` - Update only NUnit packages (faster)
- `--test-types <All|Direct|Custom>` - Filter issues by test types (default: All)
- `--feed <Stable|Beta|Alpha|Local>` - Package feed (default: Stable)
- `--verbosity <Normal|Verbose>` - Logging verbosity
- `--rerun-failed` - Rerun only failed tests from results.json

**Test Scope Options:**

- `All` (default): Run all issues
- `New`: Run only issues that haven't been tested yet (no entry in results.json or test_result is null/empty)
- `NewAndFailed`: Run issues that are new OR previously failed (no entry in results.json or test_result == "fail")
- `RegressionOnly`: Run only closed issues (regression tests)
- `OpenOnly`: Run only open issues

**Rerunning Failed Tests:**

The `--rerun-failed` option allows you to rerun only tests that previously failed. This option:

- Reads the list of failed tests from `results.json` (filters results where `TestResult != "success"`)
- Runs only those specific issues that have failed tests
- Updates `results.json` with new results (incrementally, preserving other results)
- Works independently of the `--scope` option (cannot be combined with scope filters)

**Test Result Statuses:**

When running tests, IssueRunner executes three sequential steps for each issue:

1. **Restore**: Runs `dotnet restore` to restore NuGet packages
2. **Build**: Runs `dotnet build` to compile the project
3. **Test**: Runs `dotnet test` to execute the tests

Each step can have one of three statuses:
- **Success**: Step completed successfully
- **Failed**: Step failed (with error output available)
- **Not Run**: Step was not executed (because a previous step failed)

The test execution stops at the first failed step:
- If restore fails, build and test are marked as "Not Run"
- If build fails, test is marked as "Not Run"
- If all steps succeed, the issue is marked as successful

Test result statuses are reported as:
- **Restore failed**: Package restore step failed
- **Build failed**: Compilation step failed
- **Test failed**: Test execution step failed
- **Success**: All steps (restore, build, test) completed successfully

**Test Result Files:**

IssueRunner maintains a single JSON file tracking all test results:

- `results.json`: Contains all test execution results (both passing and failing) with detailed information

This file is incrementally updated after each test run - new results overwrite existing entries for the same issue+project combination, while preserving results for issues not in the current run. Use domain model filtering (e.g., `results.Where(r => r.TestResult == "success")`) to determine pass/fail status.

**Package Feed Options:**

- `Stable` (default): nuget.org with stable packages only
- `Beta`: nuget.org with prerelease packages enabled
- `Alpha`: nuget.org + MyGet feed with prerelease packages enabled
- `Local`: nuget.org + C:\nuget feed with prerelease packages enabled

**Execution Methods:**

Issues can be executed in two ways:

1. **Direct execution**: `dotnet test` is run directly on the project file. This is the default method when no custom scripts are found.
2. **Custom script execution**: If `run_*.cmd` (Windows) or `run_*.sh` (Linux/macOS) files exist in the issue folder, those scripts are executed instead. Custom scripts are useful when an issue has multiple projects and you want to limit which ones are tested, or when special test execution logic is required.

**Test Types Filter:**

The `--test-types` option allows you to filter which issues to run based on their test execution method:

- `All` (default): Run all issues regardless of test execution method
- `Direct`: Run only issues that use direct `dotnet test` execution (issues without custom scripts)
- `Custom`: Run only issues that use custom scripts (`run_*.cmd` or `run_*.sh` files)

This is useful when you want to:

- Test only issues with custom scripts: `--test-types Custom`
- Test only issues without custom scripts: `--test-types Direct`

**Setting Up Custom Test Scripts:**

When an issue has multiple test projects or requires specific test filters, you can create custom runner scripts. IssueRunner will automatically detect and execute any files matching `run_*.cmd` (Windows) or `run_*.sh` (Linux/macOS) in the issue folder.

**Important: Cross-Platform Compatibility**

For issues that use custom test scripts, you **must provide both** `.cmd` (Windows) and `.sh` (Linux/macOS) versions with the same name. This is required because:

- Windows environments (local development, Windows CI) use the `.cmd` files
- Linux environments (GitHub Actions, Linux CI) use the `.sh` files
- If only one version exists, tests will fail on the other platform

Both files should contain equivalent commands - only the comment syntax differs (`REM` for `.cmd`, `#` for `.sh`).

**Example 1: Filtering specific tests (Issue919)**

Windows (`run_test_0.cmd`):

```cmd
REM EXPECT_TESTS=0
dotnet test --filter "FullyQualifiedName~Bar\(1\)"
```

Linux/macOS (`run_test_0.sh`):

```bash
# EXPECT_TESTS=0
dotnet test --filter "FullyQualifiedName~Bar\(1\)"
```

**Example 2: Running with runsettings and filters (Issue1146)**

```cmd
dotnet test NUnitFilterSample.csproj -c Release -s .runsettings --filter "TestCategory!=Sample" --logger "Console;verbosity=normal"
```

**Example 3: Multiple scripts for different test scenarios**

You can create multiple scripts (e.g., `run_test_0.cmd`, `run_test_1.cmd`) to test different scenarios. All matching scripts will be executed in alphabetical order.

**Script Metadata (Optional):**

You can add expectation metadata as comments in the first 10 lines of your script:

- `EXPECT_TESTS=N` - Expected total number of tests
- `EXPECT_PASS=N` - Expected number of passing tests
- `EXPECT_FAIL=N` - Expected number of failing tests
- `EXPECT_SKIP=N` - Expected number of skipped tests

If the actual results don't match the expectations, the test run will be marked as failed. This is useful for validating that specific test scenarios behave as expected.

**Example with expectations:**

```cmd
REM EXPECT_TESTS=1
REM EXPECT_PASS=1
dotnet test --filter "FullyQualifiedName~Baz\(1\)"
```

**Examples:**

```bash
# Run all regression tests
./IssueRunner run --scope RegressionOnly

# Run only new issues (never tested before)
./IssueRunner run --scope New

# Run new issues and previously failed tests
./IssueRunner run --scope NewAndFailed

# Run only open issues
./IssueRunner run --scope OpenOnly

# Rerun only failed tests from test-fails.json
./IssueRunner run --rerun-failed

# Run specific issues
./IssueRunner run --issues 228,343,1015

# Run only .NET Core tests (on Linux)
./IssueRunner run --skip-netfx

# Run only custom script tests
./IssueRunner run --test-types Custom

# Test with beta/prerelease packages
./IssueRunner run --feed Beta --issues 1039

# Test with alpha packages from MyGet
./IssueRunner run --feed Alpha --issues 228

# Test with local packages from C:\nuget
./IssueRunner run --feed Local --issues 228
```

**Other Commands:**

```bash
# Sync metadata from GitHub (or use sync-from-github.cmd/sh)
./IssueRunner metadata sync-from-github

# Distribute metadata to issue folders (or use sync-to-folders.cmd/sh)
./IssueRunner metadata sync-to-folders

# Reset packages and frameworks to metadata versions (or use reset-packages.cmd/sh)
./IssueRunner reset                    # Reset all issues
./IssueRunner reset --issues 228,711   # Reset specific issues

# Generate test report
./IssueRunner report generate

# Check for regression failures (CI)
./IssueRunner report check-regressions

# Merge results from multiple runs
./IssueRunner merge --linux <path> --windows <path>
```

#### Reset Command

The `reset` command restores projects to their original state from metadata:

- Resets **TargetFramework(s)** to original values
- Resets **package versions** to original values
- Converts between `<TargetFramework>` (singular) and `<TargetFrameworks>` (plural) as needed
- Useful after testing with different feeds or when projects get out of sync

**Wrapper scripts:**

```cmd
# Windows
.\Tools\reset-packages.cmd

# Linux/macOS
./Tools/reset-packages.sh
```

## Reporting

Generate a test report from your test results:

**Using wrapper scripts (recommended):**

```cmd
# Windows
.\Tools\generate-report.cmd

# Linux/macOS
./Tools/generate-report.sh
```

**Or run IssueRunner directly:**

```bash
cd Tools/IssueRunner/bin/Release/net10.0
./IssueRunner report generate
```

The report will be generated as `TestReport.md` in the repository root. It includes:

- Summary of regression tests (closed issues) and open issues
- Package versions under test
- Test results breakdown

**Note:** The report is generated from `results.json` in the repository root. Make sure you've run tests first to generate this file.

## Maintaining metadata

Use the convenient wrapper scripts:

**Windows:**

```cmd
cd C:\repos\nunit\nunit.issues
..\nunit3-vs-adapter.issues\Tools\sync-from-github.cmd
..\nunit3-vs-adapter.issues\Tools\sync-to-folders.cmd
```

**Linux/macOS:**

```bash
cd /home/user/repos/nunit.issues
../nunit3-vs-adapter.issues/Tools/sync-from-github.sh
../nunit3-vs-adapter.issues/Tools/sync-to-folders.sh
```

Or run IssueRunner directly:

```bash
cd Tools/IssueRunner/bin/Release/net10.0

# Sync from GitHub
./IssueRunner metadata sync-from-github --root /path/to/repo

# Distribute to folders
./IssueRunner metadata sync-to-folders --root /path/to/repo
```

**What the sync commands do:**

1. **sync-from-github**: Fetches current issue metadata from GitHub API and updates `Tools/issues_metadata.json`
   - Requires `GITHUB_TOKEN` environment variable for higher rate limits
   - Reads repository configuration from `Tools/repository.json` to determine which GitHub repository to query
   
2. **sync-to-folders**: Reads from `Tools/issues_metadata.json` and creates/updates `issue_metadata.json` in each `Issue*` folder
   - Includes project details (csproj files, frameworks, packages)

**Repository Configuration:**

Create a `Tools/repository.json` file in your target repository to specify which GitHub repository to sync from:

```json
{
  "owner": "nunit",
  "name": "nunit"
}
```

If this file doesn't exist, the sync will default to `nunit/nunit3-vs-adapter` and show a warning message.

**What to commit:**
- ✅ Metadata updates from the sync scripts above (central and per-issue `issue_metadata.json` files) should be committed
- ❌ Test results in individual issue folders should normally NOT be committed
- ✅ `TestReport.md` at repo root CAN be committed for documentation purposes

## Utility Scripts

Additional analysis and debugging tools are available in the repository root:

**Build Analysis:**
- `check-builds.py` - Builds all issue projects and reports compilation failures. Can be used with `failed-builds.json` to rerun specific projects.
- `analyze-build-errors.py` - Analyzes and categorizes build errors from `failed-builds.json` and error logs.

**Test Result Analysis:**
- `compare-test-results.ps1` - Compares `test-passes.json` and `test-fails.json` with baseline files to identify changes.

**Dump File Analysis (PowerShell):**
- `check-runningby.ps1` - Analyzes `E*.dump` files to find and count `RunningBy` values.
- `check-settings.ps1` - Checks dump files for `SynchronousEvents`, `ProcessModel`, and `DomainUsage` values.
- `check-testpackage.ps1` - Checks if dump files have more than one DLL in their `TestPackage` element.

These scripts are useful for debugging and analysis but are not part of the core test automation workflow.

### Baseline Management

IssueRunner supports baseline comparison to track changes in test results over time.

**Setting a Baseline:**

After running tests with a stable feed, you can set a baseline from the status dialog:

1. Run tests with `--feed Stable` (or select Stable feed in GUI)
2. When the status dialog shows "Completed", click "Set Baseline"
3. This creates `results-baseline.json` by copying the current `results.json`

**Viewing Baseline Comparison:**

- **CLI**: Use `compare-test-results.ps1` to compare current results with baseline (note: script may need updating to use `results.json`/`results-baseline.json`)
- **GUI**: Click "Test Status" button in the sidebar to view detailed comparison

The comparison shows:

- **Fixed**: Tests that were failing in baseline but now pass
- **New Passes**: Tests that pass but weren't in baseline
- **Regressions**: Tests that were passing in baseline but now fail
- **New Fails**: Tests that fail but weren't in baseline

**Note:** Baselines should only be set when using stable feeds to ensure meaningful comparisons.

### Other maintenance tasks

- To ignore a repro folder for test/update runs, drop one of these files into the issue folder: `ignore`, `explicit`, `wip`, `gui`, or `closedasnotplanned` (with optional `.md` extension).
- To mark a repro as Windows-only (skipped on Linux CI), use `windows` or `windows.md`.

## Design Documentation

### Data Directory Files

All central repository files are stored in the `.nunit/IssueRunner` directory. This section documents each file, what causes it to be created, and what causes it to be updated.

#### repository.json

**Purpose**: Configuration file specifying which GitHub repository to sync metadata from.

**Created**: Manually by the user when setting up a new repository.

**Updated**: Manually by the user if the repository configuration changes.

**Location**: `.nunit/IssueRunner/repository.json` (or legacy locations: `repository.json` or `Tools/repository.json`)

**Format**:

```json
{
  "owner": "nunit",
  "name": "nunit3-vs-adapter"
}
```

**Used by**: `EnvironmentService` to determine repository root, `SyncFromGitHubCommand` to determine which GitHub repository to query.

#### issues_metadata.json

**Purpose**: Central metadata file containing issue information fetched from GitHub (title, state, labels, etc.) and project details (csproj files, frameworks, packages).

**Created**: When `SyncFromGitHubCommand` is executed for the first time and successfully fetches metadata from GitHub.

**Updated**: Every time `SyncFromGitHubCommand` is executed (via `metadata sync-from-github` command or GUI "Sync from GitHub" button). The file is completely rewritten with the latest metadata from GitHub.

**Location**: `.nunit/IssueRunner/issues_metadata.json`

**Used by**: `SyncToFoldersCommand` to distribute metadata to individual issue folders, `GenerateReportCommand` and `CheckRegressionsCommand` for report generation, `MainViewModel` for displaying issue information in the GUI.

#### results.json

**Purpose**: **Single source of truth** for all test execution results. Contains detailed test execution results for all issues that have ever been run, including restore/build/test step results, outputs, and error messages.

**Created**: When `RunTestsCommand` executes tests for the first time and completes successfully.

**Updated**: Every time `RunTestsCommand` completes a test run (via `run` command or GUI "Run Tests" button). The file is **incrementally updated** - new results overwrite existing entries for the same issue+project combination, while preserving results for issues not in the current run. This ensures historical test result data is preserved across multiple test runs.

**Location**: `.nunit/IssueRunner/results.json`

**Format**: JSON array of `IssueResult` objects containing detailed information about each test execution (restore result, build result, test result, outputs, errors, `last_run` timestamp, etc.).

**Used by**: All components that need test result information:
- `GenerateReportCommand` to generate `TestReport.md`
- `RunTestsCommand` to determine which issues are "New" (not in results.json) and for `--rerun-failed` option
- `MainViewModel` for displaying test results in the Issue List View
- `TestResultDiffService` for comparing current results with baseline
- Domain model filtering (e.g., `results.Where(r => r.TestResult == "success")`) to determine pass/fail status

**Note**: This file is the **single source of truth** for all test results. Use domain model filtering on `IssueResult` objects to determine pass/fail status instead of separate files. Only issues that actually run tests are included. Skipped issues (with marker files) and issues that don't compile are not stored in this file.

#### results-baseline.json

**Purpose**: Baseline snapshot of all test results, used for comparison to detect regressions and improvements.

**Created**: When the user clicks "Set Baseline" button in the GUI status dialog (or manually copies `results.json`).

**Updated**: Every time the user clicks "Set Baseline" button in the GUI status dialog. The file is completely replaced by copying the current `results.json`.

**Location**: `.nunit/IssueRunner/results-baseline.json`

**Format**: Same as `results.json` - JSON array of `IssueResult` objects.

**Used by**: `TestResultDiffService` for comparing current results with baseline to identify fixed tests and regressions, `TestStatusViewModel` for displaying baseline comparison in the GUI.

**Note**: Replaces the old `test-passes.baseline.json` and `test-fails.baseline.json` files. Contains all test results (both passing and failing) in a single file.

#### test-passes.json (DEPRECATED)

**Status**: **DEPRECATED** - No longer created or updated. All test result information is now stored in `results.json`.

**Migration**: If you have existing `test-passes.json` files, they will be ignored. Test results are now loaded exclusively from `results.json`. You can safely delete old `test-passes.json` files.

#### test-fails.json (DEPRECATED)

**Status**: **DEPRECATED** - No longer created or updated. All test result information is now stored in `results.json`.

**Migration**: If you have existing `test-fails.json` files, they will be ignored. Test results are now loaded exclusively from `results.json`. You can safely delete old `test-fails.json` files.

#### test-passes.baseline.json (DEPRECATED)

**Status**: **DEPRECATED** - Replaced by `results-baseline.json`.

**Migration**: If you have existing baseline files, you can set a new baseline using the "Set Baseline" button, which will create `results-baseline.json`. Old baseline files can be safely deleted.

#### test-fails.baseline.json (DEPRECATED)

**Status**: **DEPRECATED** - Replaced by `results-baseline.json`.

**Migration**: If you have existing baseline files, you can set a new baseline using the "Set Baseline" button, which will create `results-baseline.json`. Old baseline files can be safely deleted.

#### failed-builds.json

**Purpose**: List of issues that fail to compile, used to exclude them from test runs.

**Created**: By external Python script `check-builds.py` (not part of IssueRunner). The script builds all issue projects and records compilation failures.

**Updated**: When `check-builds.py` is run manually. IssueRunner does not update this file.

**Location**: `.nunit/IssueRunner/failed-builds.json`

**Format**: JSON file containing an array of failed build entries with issue numbers and error details.

**Used by**: `RunTestsCommand` to exclude non-compiling issues from test runs (unless `--rerun-failed` is used), `MainViewModel` to display "not compiling" status in the GUI.

**Note**: If this file doesn't exist, `RunTestsCommand` will attempt to run all issues, including potentially non-compiling ones. The file is optional but recommended for efficient test runs.

#### nunit-packages-current.json

**Purpose**: Tracks the NUnit package versions that were used in the most recent test run.

**Created**: When `RunTestsCommand` starts a test run and determines target NUnit package versions (via `PrintTargetNUnitVersionsAsync`).

**Updated**: Every time `RunTestsCommand` starts a test run (via `run` command or GUI "Run Tests" button), before any tests are executed. The file is completely rewritten with the current package versions from the selected feed.

**Location**: `.nunit/IssueRunner/nunit-packages-current.json`

**Format**: JSON object with `packages` (dictionary of package name to version), `timestamp`, and `feed` (Stable/Beta/Alpha/Local).

**Used by**: `MainViewModel` to display current NUnit package versions in the GUI repository status section, `SetBaselineAsync` to copy to baseline when user sets baseline.

#### nunit-packages-baseline.json

**Purpose**: Baseline snapshot of NUnit package versions, used for comparison to see what packages were used when baseline was set.

**Created**: When the user clicks "Set Baseline" button in the GUI status dialog.

**Updated**: Every time the user clicks "Set Baseline" button in the GUI status dialog. The file is completely replaced by copying the current `nunit-packages-current.json`.

**Location**: `.nunit/IssueRunner/nunit-packages-baseline.json`

**Format**: Same as `nunit-packages-current.json`.

**Used by**: `MainViewModel` to display baseline NUnit package versions in the GUI repository status section for comparison with current versions.

#### TestReport.md

**Purpose**: Human-readable test report summarizing test results, package versions, and issue status.

**Created**: When `GenerateReportCommand` is executed for the first time (via `report generate` command or GUI "Generate Report" button).

**Updated**: Every time `GenerateReportCommand` is executed. The file is completely rewritten with a new report based on current `results.json` and `issues_metadata.json`.

**Location**: `.nunit/IssueRunner/TestReport.md`

**Format**: Markdown file with sections for regression tests, open issues, package versions, and test results breakdown.

**Used by**: Human review, can be committed to repository for documentation purposes.

**Note**: The report is generated from `results.json`, so it only includes issues that have been run. Skipped issues and issues that don't compile are not included unless they were attempted in a test run.

### File Relationships

- **Test Results Flow**: `RunTestsCommand` → `results.json` (incrementally updated, single source of truth)
- **Baseline Flow**: User action → `results-baseline.json` + `nunit-packages-baseline.json` (copies of current files)
- **Metadata Flow**: GitHub API → `issues_metadata.json` → individual issue folders (`issue_metadata.json`)
- **Report Flow**: `results.json` + `issues_metadata.json` → `TestReport.md`

### Missing States

The following issue states are **not** stored in any of the data directory files:

- **Skipped Issues**: Issues with marker files (`ignore`, `explicit`, `wip`, `gui`, `closednotplanned`, etc.) are excluded from test runs and do not appear in `results.json`. Their status is determined at runtime by checking for marker files.

- **Not Compiling Issues**: Issues listed in `failed-builds.json` are excluded from test runs (unless `--rerun-failed` is used) and do not appear in `results.json`. Their status is determined at runtime by checking `failed-builds.json`.

- **Restore Failures**: Issues that fail during package restore are included in `results.json` with `restore_result: "fail"` and `test_result: "not run"`.

- **Not Tested Issues**: Issues that have never been run do not appear in `results.json`. Their status is determined at runtime by checking if they exist in `results.json`.
