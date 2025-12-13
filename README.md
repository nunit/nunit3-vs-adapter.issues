# nunit3-vs-adapter.issues

Examples and repros of issues found in the NUnit Adapter

## Regression testing (update_packages_and_tests.py)

- From repo root: `python scripts/update_packages_and_tests.py [--issues 228,343] [--timeout 600] [--pre-release] [--scope all|new|new-and-failed|regression-only|open-only]`
- Pre-release packages are **off by default**; add `--pre-release` to allow prerelease versions (NUnit packages only; others stay stable).
- Timeout applies per external command (`dotnet outdated` and `dotnet test`) per issue.
- Scope options:
  - `all` (default) – process every issue.
  - `new` – only issues without `test_result`.
  - `new-and-failed` – issues without `test_result` or with a previous `test_result == fail`.
  - `regression-only` – only closed issues.
  - `open-only` – only open issues.
- Results go to `scripts/issues_metadata.json` and each issue's `issue_metadata.json`; console conclusions are mirrored to `TestResults-consolelog.md`. Package versions and framework info are captured in `testupdate.json`.
- Issues with marker files `ignore`, `ignore.md`, `explicit`, `explicit.md`, `wip`, or `wip.md` are skipped.

Examples:
- Run everything with defaults: `python scripts/update_packages_and_tests.py`
- Run specific issues: `python scripts/update_packages_and_tests.py --issues 1,228,343`
- Only new tests: `python scripts/update_packages_and_tests.py --scope new`
- New or previously failing: `python scripts/update_packages_and_tests.py --scope new-and-failed`
- Only closed (regressions): `python scripts/update_packages_and_tests.py --scope regression-only`
- Only open issues: `python scripts/update_packages_and_tests.py --scope open-only`
- Allow prerelease for NUnit packages: add `--pre-release`

## Maintaining metadata

- To regenerate per-issue project metadata: `python scripts/write_issue_project_metadata.py` (creates/updates `issue_metadata.json` in each `Issue*` folder).
- Central metadata lives at `scripts/issues_metadata.json`; keep it in sync with `fetch_issue_metadata.py`/`fetch_issue_titles.py` as needed.
- To ignore a repro folder for test/update runs, drop one of these files into the issue folder: `ignore`, `ignore.md`, `explicit`, `explicit.md`, `wip`, or `wip.md`.

## Reporting

- Generate the Markdown test report: `python scripts/testreport.py` (writes `TestReport.md` at repo root).
