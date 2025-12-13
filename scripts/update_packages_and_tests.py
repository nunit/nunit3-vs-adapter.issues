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
import re
import subprocess
import sys
import xml.etree.ElementTree as ET
from pathlib import Path
from typing import Iterable, Optional, Sequence

LATEST_SDK = "10.0.100"


def first_digits(text: str) -> Optional[str]:
    """Return the first run of digits in a string (e.g., Issue343B -> '343')."""
    current = []
    for ch in text:
        if ch.isdigit():
            current.append(ch)
        elif current:
            break
    return "".join(current) if current else None


def find_issue_dir(root: Path, number: int) -> Optional[Path]:
    """Return the Issue* directory whose digits match the number, preferring exact names."""
    candidates = []
    target = str(number)
    for path in root.iterdir():
        if not path.is_dir() or not path.name.startswith("Issue"):
            continue
        digits = first_digits(path.name)
        if digits == target:
            if path.name == f"Issue{target}":
                return path  # Exact match wins immediately.
            candidates.append(path)
    return candidates[0] if candidates else None


def find_csproj(issue_dir: Path) -> Optional[Path]:
    """Find a .csproj anywhere under the issue dir, skipping bin/obj output trees."""
    for candidate in sorted(issue_dir.rglob("*.csproj")):
        parts = {part.lower() for part in candidate.parts}
        if {"bin", "obj"} & parts:
            continue
        return candidate
    return None


def run_cmd(cmd: Sequence[str], cwd: Path, timeout: Optional[int]) -> tuple[str, str, int]:
    """Run a command, returning (stdout, stderr, returncode)."""
    try:
        proc = subprocess.run(
            list(cmd),
            cwd=str(cwd),
            stdout=subprocess.PIPE,
            stderr=subprocess.PIPE,
            text=True,
            timeout=timeout,
        )
        return proc.stdout, proc.stderr, proc.returncode
    except subprocess.TimeoutExpired as exc:
        stdout = exc.stdout or ""
        stderr = (exc.stderr or "") + f"\nTimed out after {timeout} seconds."
        return stdout, stderr, 124


def should_skip_issue(issue_dir: Path) -> bool:
    """Skip issues that are marked with ignore/explicit/wip files."""
    markers = {"ignore", "ignore.md", "explicit", "explicit.md", "wip", "wip.md"}
    for child in issue_dir.iterdir():
        if not child.is_file():
            continue
        if child.name.lower() in markers:
            return True
    return False


def map_tfm_to_net10(tfm: str) -> str:
    """Return net10.0 for modern TFMs that should be upgraded, otherwise the original."""
    raw = tfm.strip()
    lower = raw.lower()
    if lower.startswith("netcoreapp"):
        return "net10.0"
    m = re.match(r"net(\d+)(?:\.\d+)?", lower)
    if m:
        major = int(m.group(1))
        if major >= 10:
            return raw  # already new enough
        if major >= 5:
            return "net10.0"
    return raw


def update_target_frameworks(csproj: Path) -> bool:
    """Upgrade eligible target frameworks to net10.0. Returns True if file changed."""
    try:
        tree = ET.parse(csproj)
    except Exception:
        return False
    root = tree.getroot()
    changed = False

    def update_elem(elem: ET.Element) -> bool:
        text = elem.text or ""
        frameworks = [part for part in re.split(r"[;,\\s]+", text) if part]
        if not frameworks:
            return False
        updated = []
        seen = set()
        modified = False
        for tfm in frameworks:
            new_tfm = map_tfm_to_net10(tfm)
            if new_tfm != tfm:
                modified = True
            if new_tfm not in seen:
                seen.add(new_tfm)
                updated.append(new_tfm)
        if modified:
            elem.text = ";".join(updated)
        return modified

    for elem in root.findall(".//{*}TargetFramework"):
        if update_elem(elem):
            changed = True
    for elem in root.findall(".//{*}TargetFrameworks"):
        if update_elem(elem):
            changed = True

    if changed:
        tree.write(csproj, encoding="utf-8", xml_declaration=True)
    return changed


def find_global_json(issue_dir: Path, workdir: Path) -> Optional[Path]:
    """Search upwards from workdir to the issue root for a global.json."""
    current = workdir
    while True:
        candidate = current / "global.json"
        if candidate.exists():
            return candidate
        if current == issue_dir or current.parent == current:
            break
        current = current.parent
    return None


def update_global_json(path: Path) -> bool:
    """Set sdk.version to LATEST_SDK. Returns True if changed."""
    try:
        data = json.loads(path.read_text(encoding="utf-8"))
    except Exception:
        return False
    sdk = data.get("sdk")
    if not isinstance(sdk, dict):
        return False
    if sdk.get("version") == LATEST_SDK:
        return False
    sdk["version"] = LATEST_SDK
    path.write_text(json.dumps(data, indent=2), encoding="utf-8")
    return True


def update_issue_metadata_file(
    issue_dir: Path,
    project_rel: str,
    number: int,
    results: dict,
) -> None:
    """
    Mirror results into issue_dir/issue_metadata.json alongside existing data.
    If an entry with project_path exists, update it; otherwise append a new entry.
    """
    path = issue_dir / "issue_metadata.json"
    try:
        data = json.loads(path.read_text(encoding="utf-8")) if path.exists() else []
    except Exception:
        data = []

    if not isinstance(data, list):
        data = []

    matched = False
    for entry in data:
        if entry.get("project_path") == project_rel:
            entry.update(results)
            matched = True
    if not matched:
        entry = {"number": number, "project_path": project_rel}
        entry.update(results)
        data.append(entry)

    path.write_text(json.dumps(data, indent=2), encoding="utf-8")


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
    parser.add_argument(
        "--issues",
        type=str,
        default=None,
        help="Comma-separated issue numbers to process (default: all in metadata)",
    )
    parser.add_argument(
        "--timeout",
        type=int,
        default=600,
        help="Seconds to wait for each external command before timing out (default: 600)",
    )
    parser.add_argument(
        "--pre-release",
        action="store_true",
        help="Allow pre-release updates (default: disabled)",
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

    issue_filter = None
    if args.issues:
        try:
            issue_filter = {int(part.strip()) for part in args.issues.split(",") if part.strip()}
        except ValueError:
            sys.stderr.write("Could not parse --issues (expected comma-separated integers)\n")
            return 1

    for record in records:
        num = record.get("number")
        if num is None:
            continue
        if issue_filter is not None and int(num) not in issue_filter:
            continue
        if not any(k for k in record.keys() if k != "number"):
            record["update_result"] = "skipped"
            record["test_result"] = "skipped"
            record["notes"] = "Skipped: empty metadata record"
            record["test_conclusion"] = "Skipped"
            print(f"[{num}] Skipped", flush=True)
            continue
        issue_dir = find_issue_dir(root, int(num))
        if issue_dir is None:
            record["update_result"] = "fail"
            record["test_result"] = "fail"
            record["notes"] = "Issue directory not found"
            continue

        if should_skip_issue(issue_dir):
            record["update_result"] = "skipped"
            record["test_result"] = "skipped"
            record["notes"] = "Skipped due to marker file (ignore/explicit/wip)"
            record["test_conclusion"] = "Skipped"
            print(f"[{num}] Skipped", flush=True)
            continue

        csproj = find_csproj(issue_dir)
        if csproj is None:
            record["update_result"] = "fail"
            record["test_result"] = "fail"
            record["notes"] = "No csproj found"
            continue

        workdir = csproj.parent
        rel_proj = csproj.relative_to(root)

        try:
            tfm_changed = update_target_frameworks(csproj)
            if tfm_changed:
                print(f"[{num}] Updated target framework(s) to net10.0 in {rel_proj}", flush=True)
        except Exception as exc:  # noqa: BLE001
            record["notes"] = f"Failed to update target frameworks: {exc}"

        try:
            global_json = find_global_json(issue_dir, workdir)
            if global_json and update_global_json(global_json):
                print(f"[{num}] Updated SDK version to {LATEST_SDK} in {global_json.relative_to(root)}", flush=True)
        except Exception as exc:  # noqa: BLE001
            record["notes"] = f"Failed to update global.json: {exc}"

        print(f"[{num}] Updating packages in {rel_proj}", flush=True)

        # dotnet outdated (NUnit packages first, optionally with prerelease; then others without)
        nunit_stdout, nunit_stderr, nunit_code = run_cmd(
            [
                "dotnet",
                "outdated",
                "--pre-release",
                "Always" if args.pre_release else "Never",
                "--upgrade",
                "--include",
                "nunit",
            ],
            workdir,
            timeout=args.timeout,
        )
        other_stdout, other_stderr, other_code = run_cmd(
            [
                "dotnet",
                "outdated",
                "--pre-release",
                "Never",
                "--upgrade",
                "--exclude",
                "nunit",
            ],
            workdir,
            timeout=args.timeout,
        )

        record["update_result"] = "success" if (nunit_code == 0 and other_code == 0) else "fail"
        record["update_output"] = (
            "[nunit]\n" + nunit_stdout + "\n[other]\n" + other_stdout
        )
        record["update_error"] = "[nunit]\n" + nunit_stderr + "\n[other]\n" + other_stderr

        print(f"[{num}] Running tests in {rel_proj}", flush=True)

        # dotnet test
        test_stdout, test_stderr, test_code = run_cmd(
            ["dotnet", "test"], workdir, timeout=args.timeout
        )
        record["test_result"] = "success" if test_code == 0 else "fail"
        record["test_output"] = test_stdout
        record["test_error"] = test_stderr

        state = (record.get("state") or "").lower()
        if state == "closed":
            test_conclusion = (
                "Success: No regression failure" if test_code == 0 else "Regression failure."
            )
        elif state == "open":
            test_conclusion = (
                "Open issue, but test succeeds."
                if test_code == 0
                else "Open issue, repro fails."
            )
        else:
            test_conclusion = "Test succeeded." if test_code == 0 else "Test failed."

        if test_code != 0 and not test_conclusion.lower().startswith("failure:"):
            test_conclusion = f"Failure: {test_conclusion}"
        record["test_conclusion"] = test_conclusion
        print(f"[{num}] {test_conclusion}", flush=True)

        # Mirror results into the per-issue metadata file.
        try:
            update_issue_metadata_file(
                issue_dir,
                csproj.relative_to(issue_dir).as_posix(),
                int(num),
                {
                    "update_result": record["update_result"],
                    "update_output": record["update_output"],
                    "update_error": record["update_error"],
                    "test_result": record["test_result"],
                    "test_output": record["test_output"],
                    "test_error": record["test_error"],
                    "test_conclusion": record["test_conclusion"],
                },
            )
        except Exception as exc:  # noqa: BLE001
            record["notes"] = f"Failed to write issue_metadata.json: {exc}"

        if test_code != 0:
            # On failure, surface both stdout and stderr for visibility.
            if test_stdout:
                print(test_stdout, end="")
            if test_stderr:
                print(test_stderr, end="")

    # Write back updated metadata
    metadata_path.write_text(json.dumps(records, indent=2), encoding="utf-8")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
