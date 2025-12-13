#!/usr/bin/env python
"""
Update packages and run tests for each Issue* repro, recording results back into the metadata JSON.

Steps per issue:
1) Locate the corresponding Issue* folder and its .csproj (same folder or one level down).
2) Run `dotnet outdated -pre -u` in the csproj directory.
3) Run `dotnet test` in the same directory.
4) Write success/fail results into scripts/issues_metadata.json (or a provided --metadata path).

Only the standard library is used.
"""

from __future__ import annotations

import argparse
import json
import subprocess
import sys
from pathlib import Path
from typing import Iterable, Optional


def find_issue_dir(root: Path, number: int) -> Optional[Path]:
    """Return the Issue* directory that matches the number, or None."""
    for path in root.iterdir():
        if path.is_dir() and path.name.startswith("Issue") and str(number) in path.name:
            return path
    return None


def find_csproj(issue_dir: Path) -> Optional[Path]:
    """Find a .csproj in the issue dir, or one level below."""
    for candidate in issue_dir.glob("*.csproj"):
        return candidate
    for sub in issue_dir.iterdir():
        if sub.is_dir():
            for candidate in sub.glob("*.csproj"):
                return candidate
    return None


def run_cmd(cmd: Iterable[str], cwd: Path) -> tuple[str, str, int]:
    """Run a command, returning (stdout, stderr, returncode)."""
    proc = subprocess.run(
        list(cmd),
        cwd=str(cwd),
        stdout=subprocess.PIPE,
        stderr=subprocess.PIPE,
        text=True,
    )
    return proc.stdout, proc.stderr, proc.returncode


def main() -> int:
    parser = argparse.ArgumentParser(description="Update packages and run tests for Issue* repros.")
    parser.add_argument(
        "--root",
        type=Path,
        default=Path(__file__).resolve().parent.parent,
        help="Repo root containing Issue* folders (default: repo root)",
    )
    parser.add_argument(
        "--metadata",
        type=Path,
        default=None,
        help="Path to issues_metadata.json (default: scripts/issues_metadata.json under root)",
    )
    args = parser.parse_args()

    root = args.root.resolve()
    metadata_path = args.metadata or (root / "scripts" / "issues_metadata.json")

    if not metadata_path.exists():
        sys.stderr.write(f"Metadata file not found: {metadata_path}\n")
        return 1

    try:
        records = json.loads(metadata_path.read_text(encoding="utf-8"))
    except Exception as exc:  # noqa: BLE001
        sys.stderr.write(f"Failed to read metadata: {exc}\n")
        return 1

    for record in records:
        num = record.get("number")
        if num is None:
            continue
        issue_dir = find_issue_dir(root, int(num))
        if issue_dir is None:
            record["update_result"] = "fail"
            record["test_result"] = "fail"
            record["notes"] = "Issue directory not found"
            continue

        csproj = find_csproj(issue_dir)
        if csproj is None:
            record["update_result"] = "fail"
            record["test_result"] = "fail"
            record["notes"] = "No csproj found"
            continue

        workdir = csproj.parent

        # dotnet outdated
        out_stdout, out_stderr, out_code = run_cmd(["dotnet", "outdated", "-pre", "-u"], workdir)
        record["update_result"] = "success" if out_code == 0 else "fail"
        record["update_output"] = out_stdout
        record["update_error"] = out_stderr

        # dotnet test
        test_stdout, test_stderr, test_code = run_cmd(["dotnet", "test"], workdir)
        record["test_result"] = "success" if test_code == 0 else "fail"
        record["test_output"] = test_stdout
        record["test_error"] = test_stderr

    # Write back updated metadata
    metadata_path.write_text(json.dumps(records, indent=2), encoding="utf-8")
    print(f"Updated metadata written to {metadata_path}")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
