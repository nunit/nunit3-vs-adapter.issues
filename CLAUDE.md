# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Repository Overview

This repository contains examples and repros of issues found in the NUnit Adapter. Each `Issue*` folder contains a minimal reproduction case for a specific issue. The `Tools/` directory contains the IssueRunner C# tool for test automation.

## Building and Testing

### IssueRunner Tool (Tools/)

Build the C# tool:
```bash
cd Tools
dotnet build -c Release
```

Run all tests:
```bash
dotnet test Tools/Tools.sln
```

Run a single test file:
```bash
dotnet test Tools/IssueRunner.Tests/IssueRunner.Tests.csproj --filter "FullyQualifiedName~ClassName"
```

### Running Issue Tests

Using wrapper scripts (from repo root):
```cmd
# Windows
Tools\run-tests.cmd --issues 228,343

# All issues
Tools\run-tests.cmd --scope All
```

Direct IssueRunner usage:
```bash
cd Tools/IssueRunner/bin/Release/net10.0
./IssueRunner run --issues 228,343
./IssueRunner run --scope RegressionOnly
./IssueRunner run --skip-netfx  # Skip .NET Framework tests (Linux)
```

### Key IssueRunner Commands

```
issuerunner run                      # Run tests for issues
issuerunner metadata sync-from-github  # Sync metadata from GitHub
issuerunner metadata sync-to-folders   # Distribute metadata to issue folders
issuerunner reset                    # Reset packages to metadata versions
issuerunner report generate          # Generate test report
issuerunner report check-regressions # Check for regressions (CI)
issuerunner merge                    # Merge results from multiple runs
```

## Architecture

### Tools/ Directory Structure

- **IssueRunner/** - Main CLI tool (.NET 10, C# 14)
  - `Commands/` - System.CommandLine command handlers
  - `Services/` - Business logic (DI-registered, interface-based)
  - `Models/` - Data models and options
- **IssueRunner.Tests/** - Unit tests (NUnit, NSubstitute)
- **IssueRunner.ComponentTests/** - Integration tests

### Issue Folder Structure

Each `Issue*/` folder contains:
- `issue_metadata.json` - Issue state, labels, project info
- `.csproj` files - Test projects reproducing the issue
- Optional: `.runsettings`, custom runner scripts (`run_*.cmd`/`run_*.sh`)
- Marker files to skip: `ignore`, `explicit`, `wip` (with optional `.md` extension)

### Metadata Flow

1. GitHub → `Tools/issues_metadata.json` (via `sync-from-github`)
2. Central file → per-issue `issue_metadata.json` (via `sync-to-folders`)
3. Repository config in `Tools/repository.json` determines which GitHub repo to sync from

## Development Guidelines (Tools/)

### Coding Standards
- .NET 10 with C# 14 syntax
- Dependency injection for all services (interfaces required)
- System.CommandLine 2.0+ for argument parsing
- Async/await for I/O operations
- XML documentation on public members
- Methods should not exceed 25 lines

### Testing Requirements
- Use NUnit constraint syntax: `Assert.That(actual, Is.EqualTo(expected))`
- Use NSubstitute for mocking
- **Never use** FluentAssertions, MSTest, XUnit, Moq, or Shouldly
- Build and run tests after each significant change

### Test Types
- **Unit tests** - Single class/component focus
- **Component tests** - File I/O, multi-class interactions, test data from files
- **Integration tests** - End-to-end scenarios with actual Issue projects
