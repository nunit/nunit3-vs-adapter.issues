# IssueRunner Quick Reference

## Build
```bash
cd Tools
dotnet build -c Release
```

## Common Commands

### Metadata Management
```bash
# Sync from GitHub
./IssueRunner metadata sync-from-github --root /repo/path

# Update issue folders
./IssueRunner metadata sync-to-folders --root /repo/path
```

### Running Tests
```bash
# All regression tests
./IssueRunner run --scope RegressionOnly

# All open issues
./IssueRunner run --scope OpenOnly

# Specific issues
./IssueRunner run --issues 228,343,1015

# Linux - skip .NET Framework
./IssueRunner run --skip-netfx

# Windows - only .NET Framework
./IssueRunner run --only-netfx

# Fast mode - NUnit packages only
./IssueRunner run --nunit-only

# Custom scripts only
./IssueRunner run --execution-mode Custom
```

### Reporting
```bash
# Generate report
./IssueRunner report generate

# Check regressions (CI gate)
./IssueRunner report check-regressions
```

### Multi-OS Merge
```bash
./IssueRunner merge --linux ./linux-artifacts --windows ./windows-artifacts
```

## Python to C# Command Map

| Python | C# |
|--------|-----|
| `python scripts/update_central_from_github.py` | `./IssueRunner metadata sync-from-github` |
| `python scripts/update_folders_from_central.py` | `./IssueRunner metadata sync-to-folders` |
| `python scripts/run_tests.py --scope regression-only` | `./IssueRunner run --scope RegressionOnly` |
| `python scripts/run_tests.py --skip-netfx` | `./IssueRunner run --skip-netfx` |
| `python scripts/testreport.py` | `./IssueRunner report generate` |
| `python scripts/check_regressions.py` | `./IssueRunner report check-regressions` |
| `python scripts/merge_results.py --linux A --windows B` | `./IssueRunner merge --linux A --windows B` |

## Environment Variables

- **GITHUB_TOKEN** - GitHub API token for higher rate limits

```bash
export GITHUB_TOKEN=ghp_yourtoken
```

## Output Files

- **results.json** - Aggregated test results
- **TestReport.md** - Markdown report
- **TestResults-consolelog.md** - Console output log
- **scripts/issues_metadata.json** - Central metadata (GitHub)
- **Issue*/issue_metadata.json** - Per-issue metadata

## Marker Files (Skip Issues)

Create in issue folder to skip:
- `ignore` or `ignore.md` - Skip completely
- `explicit` or `explicit.md` - Explicit run only
- `wip` or `wip.md` - Work in progress

## Performance Tips

1. Use `--nunit-only` for faster package updates (~80% speedup)
2. Use `--execution-mode Direct` to skip custom script issues
3. Use `--issues` to test specific issues during development
4. Split by platform (`--skip-netfx` / `--only-netfx`) for parallel CI

## Troubleshooting

### Build fails
```bash
dotnet --version  # Ensure .NET 10 SDK installed
```

### GitHub API rate limit
```bash
export GITHUB_TOKEN=ghp_yourtoken
```

### Timeout errors
```bash
./IssueRunner run --timeout 1200  # Increase to 20 minutes
```

### Tests not found
Check for marker files (ignore/explicit/wip) in issue folders

## CI Workflows

- **regression-tests-dotnet.yml** - .NET Core regressions (Linux)
- **regression-tests-netfx.yml** - .NET Framework regressions (Windows)
- **open-issues-dotnet.yml** - Open issues .NET Core (weekly)
- **open-issues-netfx.yml** - Open issues .NET Framework (weekly)
- **aggregate-report.yml** - Merge and report

## Getting Help

```bash
./IssueRunner --help
./IssueRunner run --help
./IssueRunner metadata --help
./IssueRunner report --help
```

## Documentation

- **Tools/README.md** - Complete guide
- **Tools/IMPLEMENTATION_SUMMARY.md** - Migration details
- **Tools/FILES_CREATED.md** - File inventory
