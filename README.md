# nunit3-vs-adapter.issues
Examples and repros of issues found in the NUnit Adapter

## Regression testing (update_packages_and_tests.py)
- From repo root: `python scripts/update_packages_and_tests.py [--issues 228,343] [--timeout 600] [--pre-release]`
- Pre-release packages are **off by default**; add `--pre-release` to allow prerelease versions, but only for NUnit packages (other packages remain stable).
- Results are written to both `scripts/issues_metadata.json` and each issue’s `issue_metadata.json` entry. The console prints a per-issue conclusion (e.g., “Success: No regression failure”).
- Issues with marker files `ignore`, `ignore.md`, `explicit`, `explicit.md`, `wip`, or `wip.md` are skipped.

## Maintaining metadata
- To regenerate per-issue project metadata: `python scripts/write_issue_project_metadata.py` (creates/updates `issue_metadata.json` in each `Issue*` folder).
- Central metadata lives at `scripts/issues_metadata.json`; keep it in sync with `fetch_issue_metadata.py`/`fetch_issue_titles.py` as needed.
- To ignore a repro folder for test/update runs, drop one of these files into the issue folder: `ignore`, `ignore.md`, `explicit`, `explicit.md`, `wip`, or `wip.md`.
