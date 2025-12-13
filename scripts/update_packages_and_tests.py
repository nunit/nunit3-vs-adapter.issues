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
import urllib.request
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
        if digits is None:
            continue
        try:
            if int(digits) == number:
                if path.name == f"Issue{target}":
                    return path  # Exact match wins immediately.
                candidates.append(path)
        except ValueError:
            continue
    return candidates[0] if candidates else None


def find_csproj(issue_dir: Path) -> Optional[Path]:
    """Find a .csproj anywhere under the issue dir, skipping bin/obj output trees."""
    for candidate in sorted(issue_dir.rglob("*.csproj")):
        parts = {part.lower() for part in candidate.parts}
        if {"bin", "obj"} & parts:
            continue
        return candidate
    return None


def choose_dotnet_target(workdir: Path, csproj: Path) -> Path:
    """
    Prefer a solution file when both a .sln and .csproj exist in the working folder.
    Falls back to the provided csproj otherwise.
    """
    slns = sorted(workdir.glob("*.sln"))
    return slns[0] if slns else csproj


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


def parse_version_parts(version: str) -> tuple:
    base, _, pre = version.partition("-")
    nums = base.split(".")
    while len(nums) < 3:
        nums.append("0")
    major, minor, patch = (int(n) if n.isdigit() else 0 for n in nums[:3])
    pre_key = pre or ""
    return (major, minor, patch, pre_key)


def is_newer_version(a: str, b: str) -> bool:
    """Return True if version a is newer than b."""
    return parse_version_parts(a) > parse_version_parts(b)


_latest_cache: dict[str, str] = {}


def latest_stable_version(package: str) -> Optional[str]:
    """Fetch the highest stable version from NuGet for the given package."""
    name = package.lower()
    if name in _latest_cache:
        return _latest_cache[name]
    try:
        url = f"https://api.nuget.org/v3-flatcontainer/{name}/index.json"
        with urllib.request.urlopen(url, timeout=10) as resp:
            data = json.load(resp)
        versions = data.get("versions") or []
        stable = [v for v in versions if "-" not in v]
        if stable:
            _latest_cache[name] = stable[-1]
            return stable[-1]
    except Exception:
        return None
    return None


def find_nunit_packages(csproj: Path) -> dict[str, str]:
    """Return package refs containing 'nunit' (case-insensitive) and their versions."""
    try:
        tree = ET.parse(csproj)
    except Exception:
        return {}
    root = tree.getroot()
    results: dict[str, str] = {}
    for pr in root.findall(".//{*}PackageReference"):
        name = pr.attrib.get("Include") or pr.attrib.get("Update") or ""
        if "nunit" not in name.lower():
            continue
        version = pr.attrib.get("Version")
        if version is None:
            v_elem = pr.find("{*}Version")
            if v_elem is not None and v_elem.text:
                version = v_elem.text.strip()
        if version:
            results[name] = version
    return results


def read_tfms_and_packages(csproj: Path) -> tuple[list[str], list[str]]:
    """Return target frameworks and all PackageReference versions as 'Name=Version' strings."""
    frameworks: list[str] = []
    packages: list[str] = []
    try:
        tree = ET.parse(csproj)
        root = tree.getroot()
        for elem in root.findall(".//{*}TargetFramework"):
            if elem.text:
                frameworks.append(elem.text.strip())
        for elem in root.findall(".//{*}TargetFrameworks"):
            if elem.text:
                for part in elem.text.replace(";", ",").split(","):
                    part = part.strip()
                    if part:
                        frameworks.append(part)
        for pr in root.findall(".//{*}PackageReference"):
            name = pr.attrib.get("Include") or pr.attrib.get("Update")
            version = pr.attrib.get("Version")
            if version is None:
                v_elem = pr.find("{*}Version")
                if v_elem is not None and v_elem.text:
                    version = v_elem.text.strip()
            if name and version:
                packages.append(f"{name}={version}")
    except Exception:
        pass
    # Deduplicate while preserving order
    seen = set()
    frameworks = [f for f in frameworks if not (f in seen or seen.add(f))]
    seen.clear()
    packages = [p for p in packages if not (p in seen or seen.add(p))]
    return frameworks, packages


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
    parser.add_argument(
        "--scope",
        choices=["all", "new", "new-and-failed", "regression-only", "open-only"],
        default="all",
        help=(
            "Which issues to process: all (default), new (no test_result), new-and-failed (no test_result or previous failure), "
            "regression-only (state closed), or open-only (state open)"
        ),
    )
    args = parser.parse_args()

    root = args.root.resolve()
    metadata_path = args.metadata or (root / "scripts" / "issues_metadata.json")
    log_lines: list[str] = []
    update_records: list[dict] = []

    def log(msg: str) -> None:
        print(msg)
        log_lines.append(msg)

    if not metadata_path.exists():
        sys.stderr.write(f"Metadata file not found: {metadata_path}\n")
        return 1

    try:
        records = json.loads(metadata_path.read_text(encoding="utf-8"))
    except Exception as exc:  # noqa: BLE001
        sys.stderr.write(f"Failed to read metadata: {exc}\n")
        return 1

    def persist_metadata() -> None:
        metadata_path.write_text(json.dumps(records, indent=2), encoding="utf-8")

    def persist_update_records() -> None:
        (root / "testupdate.json").write_text(
            json.dumps(update_records, indent=2), encoding="utf-8"
        )

    # Seed metadata entries for Issue* folders that are missing.
    meta_numbers = {int(it["number"]) for it in records if isinstance(it, dict) and "number" in it}
    issue_dirs = sorted([p for p in root.iterdir() if p.is_dir() and p.name.startswith("Issue")])
    added = 0
    for d in issue_dirs:
        digits = first_digits(d.name)
        if not digits:
            continue
        num = int(digits)
        if num in meta_numbers:
            continue
        records.append({"number": num})
        meta_numbers.add(num)
        added += 1
    if added:
        persist_metadata()
        log(f"Seeded {added} missing metadata entries from Issue* folders.")

    issue_filter = None
    if args.issues:
        try:
            issue_filter = {int(part.strip()) for part in args.issues.split(",") if part.strip()}
        except ValueError:
            sys.stderr.write("Could not parse --issues (expected comma-separated integers)\n")
            return 1
    nunit_pre_mode = "Always" if args.pre_release else "Auto"
    run_nunit_versions: list[str] = []

    # One-time NUnit version check on the lowest-numbered eligible issue (by folder) to capture versions up front.
    checked_nunit_versions = False
    for issue_dir in issue_dirs:
        digits = first_digits(issue_dir.name)
        if not digits:
            continue
        try:
            num_val = int(digits)
        except ValueError:
            continue
        if issue_filter is not None and num_val not in issue_filter:
            continue
        if should_skip_issue(issue_dir):
            continue
        csproj = find_csprojj(issue_dir) if False else find_csproj(issue_dir)
        if csproj is None:
            continue
        workdir = csproj.parent
        target = choose_dotnet_target(workdir, csproj)
        log(
            f"[{num_val}] NUnit package version check (pre-release mode {nunit_pre_mode})"
        )
        tfms, all_packages = read_tfms_and_packages(csproj)
        nu_stdout, nu_stderr, _ = run_cmd(
            [
                "dotnet",
                "outdated",
                "--pre-release",
                nunit_pre_mode,
                "--include",
                "nunit",
                target.name,
            ],
            workdir,
            timeout=args.timeout,
        )
        current_versions = []
        nunit_versions = find_nunit_packages(csproj)
        if nunit_versions:
            current_versions = [f"{k}={v}" for k, v in nunit_versions.items()]
        else:
            try:
                issue_meta = issue_dir / "issue_metadata.json"
                if issue_meta.exists():
                    entries = json.loads(issue_meta.read_text(encoding="utf-8"))
                    if isinstance(entries, list) and entries:
                        packages = entries[0].get("packages") or []
                        for pkg in packages:
                            name = (pkg.get("name") or "").lower()
                            version = pkg.get("version")
                            if name and "nunit" in name and version:
                                current_versions.append(f"{pkg.get('name')}={version}")
            except Exception:
                current_versions = []
        update_records.append(
            {
                "phase": "initial-nunit-check",
                "issue": num_val,
                "target": str(target),
                "pre_release": args.pre_release,
                "stdout": nu_stdout,
                "stderr": nu_stderr,
                "current_versions": current_versions,
                "packages": all_packages,
                "target_frameworks": tfms,
            }
        )
        persist_update_records()
        if nu_stdout.strip():
            log(nu_stdout.strip())
        if nu_stderr.strip():
            log(nu_stderr.strip())
        if current_versions:
            run_nunit_versions = current_versions
            log(f"[{num_val}] Testing with NUnit package versions: {', '.join(current_versions)}")
            update_records.append(
                {
                    "phase": "nunit-versions",
                    "issue": num_val,
                    "versions": list(current_versions),
                    "packages": all_packages,
                    "target_frameworks": tfms,
                }
            )
            persist_update_records()
        checked_nunit_versions = True
        break
    if not checked_nunit_versions:
        log("No eligible issue found for initial NUnit version check.")

    for record in records:
        num = record.get("number")
        if num is None:
            continue
        if issue_filter is not None and int(num) not in issue_filter:
            continue
        record_changed = False
        state_val = (record.get("state") or "").lower()
        if args.scope == "regression-only" and state_val != "closed":
            continue
        if args.scope == "open-only" and state_val != "open":
            continue
        existing_result = record.get("test_result")
        if args.scope == "new" and existing_result:
            log(f"[{num}] Skipped (scope=new; already has test_result={existing_result})")
            continue
        if args.scope == "new-and-failed" and existing_result and existing_result != "fail":
            log(f"[{num}] Skipped (scope=new-and-failed; test_result={existing_result})")
            continue
        issue_dir = find_issue_dir(root, int(num))
        if issue_dir is None:
            record["update_result"] = "fail"
            record["test_result"] = "fail"
            record["notes"] = "Issue directory not found"
            record_changed = True
            persist_metadata()
            continue

        if should_skip_issue(issue_dir):
            record["update_result"] = "skipped"
            record["test_result"] = "skipped"
            record["notes"] = "Skipped due to marker file (ignore/explicit/wip)"
            record["test_conclusion"] = "Skipped"
            log(f"[{num}] Skipped")
            record_changed = True
            persist_metadata()
            continue

        csproj = find_csproj(issue_dir)
        if csproj is None:
            record["update_result"] = "fail"
            record["test_result"] = "fail"
            record["notes"] = "No csproj found"
            record_changed = True
            persist_metadata()
            continue

        workdir = csproj.parent
        rel_proj = csproj.relative_to(root)
        target = choose_dotnet_target(workdir, csproj)

        # Force upgrade NUnit packages to latest stable if current is older or pre-release.
        nunit_packages = find_nunit_packages(csproj)
        for pkg_name, current_ver in nunit_packages.items():
            latest = latest_stable_version(pkg_name)
            if latest and is_newer_version(latest, current_ver):
                log(f"[{num}] Bumping {pkg_name} from {current_ver} to stable {latest}")
                upd_stdout, upd_stderr, upd_code = run_cmd(
                    ["dotnet", "add", target.name, "package", pkg_name, "--version", latest],
                    workdir,
                    timeout=args.timeout,
                )
                update_records.append(
                    {
                        "phase": "manual-nunit-upgrade",
                        "issue": int(num),
                        "package": pkg_name,
                        "from": current_ver,
                        "to": latest,
                        "target": str(target),
                        "exit_code": upd_code,
                        "stdout": upd_stdout,
                        "stderr": upd_stderr,
                    }
                )

        try:
            tfm_changed = update_target_frameworks(csproj)
            if tfm_changed:
                log(f"[{num}] Updated target framework(s) to net10.0 in {rel_proj}")
        except Exception as exc:  # noqa: BLE001
            record["notes"] = f"Failed to update target frameworks: {exc}"
            record_changed = True

        try:
            global_json = find_global_json(issue_dir, workdir)
            if global_json and update_global_json(global_json):
                log(f"[{num}] Updated SDK version to {LATEST_SDK} in {global_json.relative_to(root)}")
        except Exception as exc:  # noqa: BLE001
            record["notes"] = f"Failed to update global.json: {exc}"
            record_changed = True

        log(f"[{num}] Updating packages in {rel_proj}")

        # dotnet outdated (NUnit packages first, optionally with prerelease; then others without)
        nunit_stdout, nunit_stderr, nunit_code = run_cmd(
            [
                "dotnet",
                "outdated",
                "--pre-release",
                nunit_pre_mode,
                "--upgrade",
                "--include",
                "nunit",
                target.name,
            ],
            workdir,
            timeout=args.timeout,
        )
        update_records.append(
            {
                "phase": "issue-update",
                "scope": "nunit",
                "issue": int(num),
                "target": str(target),
                "pre_release": args.pre_release,
                "exit_code": nunit_code,
                "stdout": nunit_stdout,
                "stderr": nunit_stderr,
            }
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
                target.name,
            ],
            workdir,
            timeout=args.timeout,
        )
        update_records.append(
            {
                "phase": "issue-update",
                "scope": "other",
                "issue": int(num),
                "target": str(target),
                "pre_release": False,
                "exit_code": other_code,
                "stdout": other_stdout,
                "stderr": other_stderr,
            }
        )

        record["update_result"] = "success" if (nunit_code == 0 and other_code == 0) else "fail"
        record["update_output"] = (
            "[nunit]\n" + nunit_stdout + "\n[other]\n" + other_stdout
        )
        record["update_error"] = "[nunit]\n" + nunit_stderr + "\n[other]\n" + other_stderr
        record_changed = True

        log(f"[{num}] Running tests in {rel_proj}")

        # dotnet test
        test_stdout, test_stderr, test_code = run_cmd(
            ["dotnet", "test", target.name], workdir, timeout=args.timeout
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
        log(f"[{num}] {test_conclusion}")
        record_changed = True

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
            record_changed = True

        if test_code != 0:
            # On failure, surface both stdout and stderr for visibility.
            if test_stdout:
                for line in test_stdout.rstrip().splitlines():
                    log(line)
            if test_stderr:
                for line in test_stderr.rstrip().splitlines():
                    log(line)

        if record_changed:
            persist_metadata()

    # Write back updated metadata
    metadata_path.write_text(json.dumps(records, indent=2), encoding="utf-8")
    log_path = root / "TestResults-consolelog.md"
    log_path.write_text("\n".join(log_lines) + "\n", encoding="utf-8")
    (root / "testupdate.json").write_text(
        json.dumps(update_records, indent=2), encoding="utf-8"
    )
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
