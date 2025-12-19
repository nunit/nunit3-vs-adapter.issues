# nunit3-vs-adapter.issues

Examples and repros of issues found in the NUnit Adapter

## Test Automation

### Using C# IssueRunner (Recommended)

The new C# tool provides better performance and maintainability. Build it first:

```bash
cd Tools
dotnet build -c Release
```

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
..\nunit3-vs-adapter.issues\Tools\sync-from-github.cmd
..\nunit3-vs-adapter.issues\Tools\sync-to-folders.cmd
```

**Linux/macOS:**
```bash
cd /path/to/your/test/repository
../nunit3-vs-adapter.issues/Tools/run-tests.sh [options]
../nunit3-vs-adapter.issues/Tools/sync-from-github.sh
../nunit3-vs-adapter.issues/Tools/sync-to-folders.sh
```

#### IssueRunner Command Structure

```
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
- `--scope <All|New|NewAndFailed|RegressionOnly|OpenOnly>` - Test scope (default: All)
- `--issues <numbers>` - Comma-separated issue numbers to run
- `--timeout <seconds>` - Timeout per command (default: 600)
- `--skip-netfx` - Skip .NET Framework tests
- `--only-netfx` - Run only .NET Framework tests
- `--nunit-only` - Update only NUnit packages (faster)
- `--execution-mode <All|Direct|Custom>` - Filter by execution method
- `--feed <Stable|Beta|Alpha>` - Package feed (default: Stable)
- `--verbosity <Normal|Verbose>` - Logging verbosity

**Package Feed Options:**
- `Stable` (default): nuget.org with stable packages only
- `Beta`: nuget.org with prerelease packages enabled
- `Alpha`: nuget.org + MyGet feed with prerelease packages enabled

**Examples:**
```bash
# Run all regression tests
./IssueRunner run --scope RegressionOnly

# Run specific issues
./IssueRunner run --issues 228,343,1015

# Run only .NET Core tests (on Linux)
./IssueRunner run --skip-netfx

# Run only custom script tests
./IssueRunner run --execution-mode Custom

# Test with beta/prerelease packages
./IssueRunner run --feed Beta --issues 1039

# Test with alpha packages from MyGet
./IssueRunner run --feed Alpha --issues 228
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

### Using Python scripts (Legacy)

The Python scripts are still available but will be deprecated:

## Regression testing (run_tests.py)

- From repo root: `python scripts/run_tests.py [--issues 228,343] [--timeout 600] [--scope all|new|new-and-failed|regression-only|open-only] [--skip-netfx|--only-netfx] [--nunit-only] [--feed stable|nuget-prerelease|myget-alpha]`
- Package feed options (`--feed`, default `stable`):
  - `stable` – nuget.org stable only (pre-release disabled)
  - `nuget-prerelease` – nuget.org with pre-release enabled
  - `myget-alpha` – adds myget via `nugetc add myget`, uses pre-release
- Timeout applies per external command (`dotnet outdated` and `dotnet test`) per issue.
- Scope options:
  - `all` (default) - process every issue.
  - `new` - only issues without `test_result`.
  - `new-and-failed` - issues without `test_result` or with a previous `test_result == fail`.
  - `regression-only` - only closed issues.
  - `open-only` - only open issues.
- NetFx filters: `--skip-netfx` (skip netfx-only projects) or `--only-netfx` (process only netfx-only projects).
- NUnit-only updates: add `--nunit-only` to bump only NUnit/NUnit.Framework/NUnit3TestAdapter/Microsoft.NET.Test.Sdk to the captured target versions; other packages are left as-is.
- Results go to `results.json` (plus per-issue `issue_results.json`); console conclusions are mirrored to `TestResults-consolelog.md`. Package versions and framework info are captured in `testupdate.json`. The central metadata files are read-only during test runs.
- Issues with marker files `ignore`, `ignore.md`, `explicit`, `explicit.md`, `wip`, or `wip.md` are skipped.
- Test execution:
  - If a `.runsettings` (or `*.runsettings`) file exists in the issue folder, it is passed to `dotnet test` via `--settings`.
  - If a custom runner script is present (Linux: `run_*.sh`; Windows: `run_*.cmd`) in the project folder or its parents up to the issue root, that script is executed instead of `dotnet test`. The script runs from its own folder.
  - Optional expectations per runner script (add as header lines in the script; examples: `# EXPECT_TESTS=0`, `REM EXPECT_PASS=1`):
    - `EXPECT_TESTS=<number>`
    - `EXPECT_PASS=<number>`
    - `EXPECT_FAIL=<number>`
    - `EXPECT_SKIP=<number>`
    Mismatches will fail the run and are recorded in results, including expected/actual counts.

Examples:

- Run everything with defaults: `python scripts/run_tests.py`
- Run specific issues: `python scripts/run_tests.py --issues 1,228,343`
- Only new tests: `python scripts/run_tests.py --scope new`
- New or previously failing: `python scripts/run_tests.py --scope new-and-failed`
- Only closed (regressions): `python scripts/run_tests.py --scope regression-only`
- Only open issues: `python scripts/run_tests.py --scope open-only`
- Skip netfx-only on Linux: `python scripts/run_tests.py --skip-netfx`
- Only netfx (e.g., on Windows): `python scripts/run_tests.py --only-netfx`
- NUnit-only package updates: `python scripts/run_tests.py --nunit-only`
- Allow prerelease for NUnit packages: add `--pre-release`

## Reporting

### C# Tool
```bash
cd Tools/IssueRunner/bin/Release/net10.0
./IssueRunner report generate
```

### Python (Legacy)
- Generate the Markdown test report: `python scripts/testreport.py` (writes `TestReport.md` at repo root).

## Maintaining metadata

### Using C# Tool (Recommended)

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

### Using Python Scripts (Legacy)

### Syncing metadata from GitHub

When issues have been updated on GitHub (state, labels, milestone changes), run these scripts in sequence:

1. **Fetch from GitHub to central file**: `python scripts/update_central_from_github.py`
   - Fetches current issue metadata from GitHub API
   - Updates `scripts/issues_metadata.json`
   - Requires `GITHUB_TOKEN` environment variable for higher rate limits

2. **Distribute to issue folders**: `python scripts/update_folders_from_central.py`
   - Reads from `scripts/issues_metadata.json`
   - Creates/updates `issue_metadata.json` in each `Issue*` folder
   - Includes project details (csproj files, frameworks, packages)

3. **Optional - Update readme titles**: `python scripts/fetch_issue_titles.py`
   - Updates `readme.initialstate.md` files in each `Issue*` folder with current GitHub titles

**Quick sync workflow:**
```bash
python scripts/update_central_from_github.py
python scripts/update_folders_from_central.py
```

Or use the convenience script:
```bash
scripts\Update_all_from_github.cmd
```

**What to commit:**
- ✅ Metadata updates from the sync scripts above (central and per-issue `issue_metadata.json` files) should be committed
- ❌ Test results in individual issue folders should normally NOT be committed
- ✅ `TestReport.md` at repo root CAN be committed for documentation purposes

### Other maintenance tasks

- To ignore a repro folder for test/update runs, drop one of these files into the issue folder: `ignore`, `ignore.md`, `explicit`, `explicit.md`, `wip`, or `wip.md`.
