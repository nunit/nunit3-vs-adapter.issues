# nunit3-vs-adapter.issues

Examples and repros of issues found in the NUnit Adapter

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

- Generate the Markdown test report: `python scripts/testreport.py` (writes `TestReport.md` at repo root).

## Maintaining metadata

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
   - Updates `readme.md` files in each `Issue*` folder with current GitHub titles

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
