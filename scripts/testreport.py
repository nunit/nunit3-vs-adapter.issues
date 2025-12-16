#!/usr/bin/env python
"""
Generate a Markdown test report from scripts/issues_metadata.json (and optional testupdate.json).

The report includes:
- Which package versions we exercised (from testupdate.json if present, otherwise inferred from metadata).
- Regression tests (closed issues): summary table of all, plus detailed table for failures.
- Open issues: successes (candidates to close) and failures (confirmed repros).

Output is written to TestReport.md in the repo root.
"""

from __future__ import annotations

import json
import textwrap
from pathlib import Path
from typing import Dict, List, Optional, Tuple

ROOT = Path(__file__).resolve().parent.parent
METADATA_PATH = ROOT / "scripts" / "issues_metadata.json"
UPDATE_LOG_PATH = ROOT / "testupdate.json"
RESULTS_PATH = ROOT / "results.json"
REPORT_PATH = ROOT / "TestReport.md"


def load_json(path: Path) -> Optional[object]:
    try:
        return json.loads(path.read_text(encoding="utf-8"))
    except Exception:
        return None


def collect_versions() -> List[str]:
    """Return a list of package versions we tested with."""
    versions: List[str] = []
    update_log = load_json(UPDATE_LOG_PATH)
    if isinstance(update_log, list):
        for entry in update_log:
            if entry.get("phase") == "initial-nunit-check":
                for pkg in entry.get("current_versions") or []:
                    if pkg not in versions:
                        versions.append(pkg)
    if versions:
        return versions

    # Fallback: infer from first metadata entry with packages.
    meta = load_json(METADATA_PATH)
    if isinstance(meta, list):
        for item in meta:
            pkgs = item.get("packages") or []
            for pkg in pkgs:
                name = pkg.get("name")
                version = pkg.get("version")
                if name and version and "nunit" in name.lower():
                    entry = f"{name}={version}"
                    if entry not in versions:
                        versions.append(entry)
            if versions:
                break
    return versions


def format_table(headers: List[str], rows: List[Tuple[str, ...]]) -> List[str]:
    lines = []
    if not rows:
        return lines
    lines.append("| " + " | ".join(headers) + " |")
    lines.append("|" + "|".join([" --- " for _ in headers]) + "|")
    for row in rows:
        lines.append("| " + " | ".join(row) + " |")
    return lines


def summarize(text: str, limit: int = 240) -> str:
    text = " ".join((text or "").split())
    if len(text) <= limit:
        return text
    return text[: limit - 3] + "..."


def failure_detail(item: dict) -> str:
    # Prefer test-related details before update noise.
    for key in ("notes", "test_error", "test_output", "update_error", "update_output"):
        val = item.get(key)
        if val:
            summary = summarize(val, limit=800)  # allow more before wrapping
            wrapped = "<br/>".join(textwrap.wrap(summary, width=100))
            return wrapped
    return "n/a"


def main() -> int:
    metadata = load_json(METADATA_PATH)
    if not isinstance(metadata, list):
        raise SystemExit(f"Could not read metadata from {METADATA_PATH}")
    results = load_json(RESULTS_PATH)
    if not isinstance(results, list):
        results = []
    results_by_issue = {}
    for entry in results:
        num = entry.get("number")
        if num is None:
            continue
        results_by_issue.setdefault(num, []).append(entry)

    versions = collect_versions()

    closed_all: List[Tuple[str, ...]] = []
    closed_fail: List[Tuple[str, ...]] = []
    closed_success_count = 0
    open_success: List[Tuple[str, ...]] = []
    open_fail: List[Tuple[str, ...]] = []

    def project_status(num: int) -> tuple[str, str, str]:
        entries = results_by_issue.get(num, [])
        if not entries:
            return "unknown", "", "n/a"
        # Prefer fail if any.
        for e in entries:
            if e.get("test_result") == "fail":
                return "fail", e.get("test_conclusion") or "Failure", failure_detail(e)
        for e in entries:
            if e.get("test_result") == "success":
                return "success", e.get("test_conclusion") or "Success", failure_detail(e)
        return "unknown", "", "n/a"

    def project_framework(num: int) -> str:
        entries = results_by_issue.get(num, [])
        tfm_set = set()
        for e in entries:
            for tfm in e.get("target_frameworks") or []:
                tfm_set.add((tfm or "").lower())
        if not tfm_set:
            return "unknown"
        is_netfx = any(t.startswith(("net2", "net3", "net4")) for t in tfm_set)
        is_modern = any(t.startswith("net") and not t.startswith(("net2", "net3", "net4")) for t in tfm_set)
        if is_netfx and is_modern:
            return "mixed"
        if is_netfx:
            return "netfx"
        return ".NET"

    for item in metadata:
        number = item.get("number")
        if number is None:
            continue
        issue = f"#{number}"
        url = item.get("url") or ""
        state = (item.get("state") or "").lower()
        result, conclusion, detail = project_status(int(number))
        framework = project_framework(int(number))
        short = f"{result}"
        if conclusion:
            short = f"{result} - {conclusion}"
        detail_row = (f"{issue} {url}".strip(), framework, conclusion or result or "n/a", detail)
        icon = ""
        if state == "closed":
            if result == "success":
                icon = "✅ "
            elif result == "fail":
                icon = "❗ "
        elif state == "open":
            if result == "success":
                icon = "🟢❓ "
        summary_row = (f"{icon}{issue}", result, framework, conclusion or "")

        if state == "closed":
            closed_all.append(summary_row)
            if result == "fail":
                closed_fail.append(detail_row)
            elif result == "success":
                closed_success_count += 1
        elif state == "open":
            if result == "success":
                open_success.append((detail_row[0], detail_row[1]))
            elif result == "fail":
                open_fail.append(detail_row)

    lines: List[str] = []
    lines.append("# Test Report")
    lines.append("")

    # Summary counts
    total_closed = len(closed_all)
    total_open = len(open_success) + len(open_fail)
    lines.append("## Summary")
    lines.append("")
    lines.append(
        f"- Regression tests: total {total_closed}, success {closed_success_count}, fail {len(closed_fail)}"
    )
    lines.append(
        f"- Open issues: total {total_open}, success {len(open_success)}, fail {len(open_fail)}"
    )
    lines.append("")

    # Section: What we are testing
    lines.append("## What we are testing")
    lines.append("")
    if versions:
        lines.append("Package versions under test:")
        lines.append("")
        for v in versions:
            lines.append(f"- {v}")
    else:
        lines.append("- (No version information found)")
    lines.append("")

    # Section: Regression tests (closed issues)
    lines.append("## Regression tests (closed issues)")
    lines.append("")
    lines.append(
        f"- Total: {total_closed}, Success: {closed_success_count}, Fail: {len(closed_fail)}"
    )
    lines.append("")
    if closed_all:
        lines.extend(format_table(["Issue", "Test", "Framework", "Conclusion"], closed_all))
    else:
        lines.append("- None")
    lines.append("")
    lines.append("### Closed failures (details)")
    lines.append("")
    if closed_fail:
        lines.extend(format_table(["Issue", "Framework", "Conclusion", "Details"], closed_fail))
    else:
        lines.append("- None")
    lines.append("")

    # Section: Open issues
    lines.append("## Open issues")
    lines.append("")
    total_open = len(open_success) + len(open_fail)
    lines.append(f"- Total: {total_open}, Success: {len(open_success)}, Fail: {len(open_fail)}")
    lines.append("")
    lines.append("### Succeeded (candidates to close)")
    lines.append("")
    if open_success:
        lines.extend(format_table(["Issue", "Conclusion"], open_success))
    else:
        lines.append("- None")
    lines.append("")
    lines.append("### Failing (confirmed repros)")
    lines.append("")
    if open_fail:
        lines.extend(format_table(["Issue", "Conclusion", "Details"], open_fail))
    else:
        lines.append("- None")
    lines.append("")

    REPORT_PATH.write_text("\n".join(lines), encoding="utf-8")
    print(f"Wrote report to {REPORT_PATH}")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
