# IssueRunner - NUnit VS Adapter Issue Test Automation

C# replacement for Python test automation scripts, providing better performance and maintainability.

## Features

- **Metadata Management**: Sync issue metadata from GitHub to local files
- **Test Execution**: Run tests with flexible filtering and options
- **Report Generation**: Generate markdown reports from test results
- **Regression Checking**: Validate that closed issues still pass
- **Multi-platform**: Supports Windows (.NET Framework) and Linux/macOS (.NET Core)
- **Fast Package Updates**: Optional NUnit-only update mode using text substitution

## Building

```bash
cd Tools
dotnet build
```

For release builds:
```bash
dotnet build -c Release
```

## Running Tests

```bash
cd Tools
dotnet test
```

## Usage

### Quick Start

```bash
cd Tools/IssueRunner/bin/Release/net10.0

# Run all regression tests
./IssueRunner run --scope RegressionOnly

# Generate report
./IssueRunner report generate

# Check for regressions
./IssueRunner report check-regressions
```

### Command Reference

#### Metadata Commands

```bash
# Sync from GitHub to central metadata file
./IssueRunner metadata sync-from-github --root /path/to/repo

# Distribute from central file to issue folders
./IssueRunner metadata sync-to-folders --root /path/to/repo
```

Requires `GITHUB_TOKEN` environment variable for higher API rate limits.

#### Run Command

```bash
./IssueRunner run [options]
```

**Options:**
- `--root <path>` - Repository root (default: current directory)
- `--scope <All|New|NewAndFailed|RegressionOnly|OpenOnly>` - Test scope
- `--issues <numbers>` - Comma-separated issue numbers (e.g., `228,343,1015`)
- `--timeout <seconds>` - Command timeout (default: 600)
- `--skip-netfx` - Skip .NET Framework tests
- `--only-netfx` - Run only .NET Framework tests
- `--nunit-only` - Update only NUnit packages (faster, uses text substitution)
- `--execution-mode <All|Direct|Custom>` - Filter by execution method
  - `Direct`: Only `dotnet test` execution
  - `Custom`: Only custom runner scripts
  - `All`: Both methods (default)

**Marker Files** - Issues with these files are skipped:
- `ignore`, `ignore.md` - Skip completely
- `explicit`, `explicit.md` - Skip (manual only)
- `wip`, `wip.md` - Work in progress

#### Report Commands

```bash
# Generate TestReport.md
./IssueRunner report generate --root /path/to/repo

# Check for regression failures (exits with code 1 if found)
./IssueRunner report check-regressions --root /path/to/repo
```

#### Merge Command

For CI scenarios where tests run on multiple OS:

```bash
./IssueRunner merge --linux <linux-artifacts> --windows <windows-artifacts> --output <path>
```

## Architecture

### Project Structure

- **IssueRunner** - Main console application
  - `Commands/` - CLI command implementations
  - `Services/` - Business logic services
  - `Models/` - Data models for JSON serialization
  - `Program.cs` - Entry point and DI configuration

- **IssueRunner.Tests** - Unit tests with mocking

- **IssueRunner.ComponentTests** - Integration tests with real data

### Key Services

- **GitHubApiService** - Fetches issue metadata from GitHub API
- **IssueDiscoveryService** - Finds Issue* folders and checks marker files
- **ProjectAnalyzerService** - Parses csproj files for frameworks and packages
- **PackageUpdateService** - Updates NuGet packages (dotnet-outdated or text substitution)
- **TestExecutionService** - Runs tests (dotnet test or custom scripts)
- **ReportGeneratorService** - Generates markdown reports
- **ProcessExecutor** - Helper for running external processes with timeouts

### Design Principles

- Dependency injection for testability
- Async/await throughout
- Methods under 25 lines (per coding guidelines)
- XML documentation on all public members
- Separation of concerns (commands, services, models)

### GUI Architecture (IssueRunner.Gui)

The GUI application follows a **ViewModel/Service separation pattern** to keep ViewModels focused on UI concerns while delegating business logic to services.

**ViewModels** (`ViewModels/`):
- `MainViewModel` - Main application view model, coordinates UI state and delegates to services
- `IssueListViewModel` - Manages issue list display and filtering
- `TestStatusViewModel` - Displays test status and results
- `RunTestsOptionsViewModel` - Manages test run options dialog

**Services** (`Services/`):
- `IRepositoryStatusService` - Loads repository status, summary text, and package versions
- `IIssueListLoader` - Loads issue list data with metadata and test results
- `ITestRunOrchestrator` - Orchestrates test runs, manages status dialogs, and handles cancellation
- `ISyncCoordinator` - Coordinates GitHub sync and folder sync operations

**Key Principles**:
- ViewModels are thin - they primarily update bindable properties and delegate to services
- Business logic lives in services, not ViewModels
- Services are testable independently of UI
- All services are registered in the DI container (`App.axaml.cs`)
- ViewModels receive services via constructor injection

**Testing**:
- ViewModels are tested using Avalonia headless testing (`Avalonia.Headless.NUnit`)
- Services are mocked in ViewModel tests using NSubstitute
- All new services should have corresponding interface registrations in test setup

## CI/CD Integration

The tool is used in split GitHub Actions workflows:

- `regression-tests-dotnet.yml` - .NET Core regression tests (Linux)
- `regression-tests-netfx.yml` - .NET Framework regression tests (Windows)
- `open-issues-dotnet.yml` - Open issue tests (Linux)
- `open-issues-netfx.yml` - Open issue tests (Windows)
- `aggregate-report.yml` - Merges results and generates report

Benefits of splitting:
- Parallel execution (faster CI)
- Separate timeout handling
- Clear visibility into platform-specific failures
- Independent retry capability

## Migration from Python

### Equivalent Commands

| Python | C# |
|--------|-----|
| `python scripts/update_central_from_github.py` | `./IssueRunner metadata sync-from-github` |
| `python scripts/update_folders_from_central.py` | `./IssueRunner metadata sync-to-folders` |
| `python scripts/run_tests.py` | `./IssueRunner run` |
| `python scripts/testreport.py` | `./IssueRunner report generate` |
| `python scripts/check_regressions.py` | `./IssueRunner report check-regressions` |
| `python scripts/merge_results.py` | `./IssueRunner merge` |

### Key Differences

1. **Performance**: NUnit-only mode uses text substitution instead of `dotnet-outdated` (~80% faster)
2. **Type Safety**: Strongly-typed models with compile-time checking
3. **Testing**: Comprehensive unit and component tests
4. **Logging**: Structured logging with Microsoft.Extensions.Logging
5. **Platform**: Native .NET tool, no Python dependency

## Development

### Adding a New Command

1. Create command class in `Commands/`
2. Inject required services via constructor
3. Implement `ExecuteAsync` method
4. Register in `Program.cs` DI container
5. Add command to CLI in `BuildXXXCommand` method
6. Write unit tests in `IssueRunner.Tests`

### Adding a New Service

1. Create interface in `Services/IXXXService.cs`
2. Implement in `Services/XXXService.cs`
3. Register in `Program.cs` DI container
4. Write unit tests with mocked dependencies
5. Add component tests if needed

## Troubleshooting

### Build Errors

Ensure you have .NET 10 SDK installed:
```bash
dotnet --version
```

### GitHub API Rate Limiting

Set `GITHUB_TOKEN` environment variable:
```bash
export GITHUB_TOKEN=ghp_yourtoken
```

### Timeout Issues

Increase timeout for slow issues:
```bash
./IssueRunner run --timeout 1200
```

### Missing Results

Check for marker files (ignore/explicit/wip) in issue folders.
