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
- `--feed <Stable|Beta|Alpha|Local>` - Package feed (default: Stable)
- `--verbosity <Normal|Verbose>` - Logging verbosity

**Test Scope Options:**

- `All` (default): Run all issues
- `New`: Run only issues that haven't been tested yet (no entry in results.json or test_result is null/empty)
- `NewAndFailed`: Run issues that are new OR previously failed (no entry in results.json or test_result == "fail")
- `RegressionOnly`: Run only closed issues (regression tests)
- `OpenOnly`: Run only open issues

**Package Feed Options:**

- `Stable` (default): nuget.org with stable packages only
- `Beta`: nuget.org with prerelease packages enabled
- `Alpha`: nuget.org + MyGet feed with prerelease packages enabled
- `Local`: nuget.org + C:\local feed with prerelease packages enabled

**Execution Methods:**

Issues can be executed in two ways:

1. **Direct execution**: `dotnet test` is run directly on the project file. This is the default method when no custom scripts are found.
2. **Custom script execution**: If `run_*.cmd` (Windows) or `run_*.sh` (Linux/macOS) files exist in the issue folder, those scripts are executed instead. Custom scripts are useful when an issue has multiple projects and you want to limit which ones are tested, or when special test execution logic is required.

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

# Test with local packages from C:\local
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

```bash
cd Tools/IssueRunner/bin/Release/net10.0
./IssueRunner report generate
```

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

### Other maintenance tasks

- To ignore a repro folder for test/update runs, drop one of these files into the issue folder: `ignore`, `explicit`, `wip`, `gui`, or `closedasnotplanned` (with optional `.md` extension).
- To mark a repro as Windows-only (skipped on Linux CI), use `windows` or `windows.md`.
