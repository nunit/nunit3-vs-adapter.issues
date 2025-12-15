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
import platform
from pathlib import Path
from typing import Iterable, Optional, Sequence

LATEST_SDK = "10.0.100"
LEGACY_NET_TO_462 = {
    "net35",
    "net40",
    "net45",
    "net451",
    "net452",
    "net46",
    "net461",
}


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
    """Find a .csproj anywhere under the issue dir, skipping bin/obj output trees.

    Prefer a project that looks like an NUnit test project; otherwise return the first match.
    """
    candidates = []
    for candidate in sorted(issue_dir.rglob("*.csproj")):
        parts = {part.lower() for part in candidate.parts}
        if {"bin", "obj"} & parts:
            continue
        candidates.append(candidate)
    if not candidates:
        return None
    # Prefer the first NUnit-looking project.
    for cand in candidates:
        _, packages = read_tfms_and_packages(cand)
        if is_nunit_project(packages):
            return cand
    return candidates[0]


def find_first_nunit_csproj(issue_dir: Path) -> Optional[Path]:
    """Return the first .csproj that looks like an NUnit test project, or None."""
    for candidate in sorted(issue_dir.rglob("*.csproj")):
        parts = {part.lower() for part in candidate.parts}
        if {"bin", "obj"} & parts:
            continue
        _, packages = read_tfms_and_packages(candidate)
        if is_nunit_project(packages):
            return candidate
    return None


def read_project_references(csproj: Path) -> list[Path]:
    """Return project reference paths resolved relative to the csproj."""
    refs: list[Path] = []
    try:
        tree = ET.parse(csproj)
        root = tree.getroot()
        for pref in root.findall(".//{*}ProjectReference"):
            include = pref.attrib.get("Include")
            if not include:
                continue
            ref_path = (csproj.parent / include).resolve()
            if ref_path.exists():
                refs.append(ref_path)
    except Exception:
        pass
    return refs


def find_runsettings(issue_dir: Path) -> Path | None:
    """Find a .runsettings file in the issue folder (named *.runsettings or .runsettings)."""
    candidates = list(issue_dir.glob("*.runsettings"))
    explicit = issue_dir / ".runsettings"
    if explicit.exists():
        candidates.append(explicit)
    if not candidates:
        return None
    return sorted(candidates)[0]


def find_custom_runners(start_dir: Path, issue_dir: Path) -> list[Path]:
    """Find all custom runner scripts (run_*.sh on Linux, run_*.cmd on Windows).

    Searches from the project directory upward to the issue root, then the issue root itself.
    Returns sorted unique paths.
    """
    is_windows = platform.system().lower().startswith("win")
    pattern = "run_*.cmd" if is_windows else "run_*.sh"
    search_paths: list[Path] = []
    current = start_dir
    while True:
        search_paths.append(current)
        if current == issue_dir:
            break
        if issue_dir in current.parents:
            current = current.parent
        else:
            break
    if issue_dir not in search_paths:
        search_paths.append(issue_dir)

    found: list[Path] = []
    seen: set[Path] = set()
    for folder in search_paths:
        for candidate in sorted(folder.glob(pattern)):
            real = candidate.resolve()
            if real in seen:
                continue
            seen.add(real)
            found.append(candidate)
    return found


def align_target_frameworks_and_references(
    num: int, csproj: Path, rel_proj: Path, root: Path, log_fn, record: dict
) -> None:
    """Upgrade target frameworks in the project and any referenced projects."""
    try:
        tfm_changed = update_target_frameworks(csproj)
        if tfm_changed:
            log_fn(f"[{num}] Updated target framework(s) to net10.0 in {rel_proj}")
    except Exception as exc:  # noqa: BLE001
        record["notes"] = f"Failed to update target frameworks: {exc}"
    try:
        for ref in read_project_references(csproj):
            ref_rel = ref.relative_to(root)
            if update_target_frameworks(ref):
                log_fn(f"[{num}] Updated target framework(s) to net10.0 in {ref_rel}")
    except Exception as exc:  # noqa: BLE001
        record["notes"] = f"Failed to update referenced project frameworks: {exc}"


def run_tests_for_issue(
    num: int,
    rel_proj: Path,
    target: Path,
    workdir: Path,
    issue_dir: Path,
    timeout: int | None,
    log_fn,
) -> tuple[int, str, str, Path | None, list[str]]:
    """Run tests via custom runners (if any) or dotnet test, returning exit code and output."""
    runsettings_path = find_runsettings(issue_dir)
    runner_scripts = find_custom_runners(workdir, issue_dir)

    combined_stdout: list[str] = []
    combined_stderr: list[str] = []
    test_code = 0
    runner_paths: list[str] = []

    if runner_scripts:
        log_fn(f"[{num}] Found custom test scripts: {', '.join(p.name for p in runner_scripts)}")
        runner_paths = [str(p) for p in runner_scripts]
        for runner_script in runner_scripts:
            log_fn(f"[{num}] Running custom test script {runner_script.name}")
            if platform.system().lower().startswith("win"):
                test_cmd = ["cmd", "/c", runner_script.name]
            else:
                test_cmd = ["bash", runner_script.name]
            out, err, code = run_cmd(test_cmd, runner_script.parent, timeout=timeout)
            combined_stdout.append(out)
            combined_stderr.append(err)
            if code != 0:
                test_code = code
    else:
        log_fn(f"[{num}] Running tests in {rel_proj}")
        test_cmd = ["dotnet", "test", target.name]
        if runsettings_path:
            test_cmd.extend(["--settings", str(runsettings_path)])
        out, err, code = run_cmd(test_cmd, workdir, timeout=timeout)
        combined_stdout.append(out)
        combined_stderr.append(err)
        test_code = code

    test_stdout = "\n".join([part for part in combined_stdout if part])
    test_stderr = "\n".join([part for part in combined_stderr if part])
    return test_code, test_stdout, test_stderr, runsettings_path, runner_paths


def ensure_nugetc_and_myget(root: Path, log_fn) -> None:
    """Ensure nugetc tool is available and myget feed is added."""
    out, err, code = run_cmd(["dotnet", "tool", "update", "-g", "nugetc"], root, None)
    if out.strip():
        log_fn(out.strip())
    if err.strip():
        log_fn(err.strip())
    if code != 0:
        return
    out, err, _ = run_cmd(["nugetc", "add", "myget"], root, None)
    if out.strip():
        log_fn(out.strip())
    if err.strip():
        log_fn(err.strip())


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
    if lower in LEGACY_NET_TO_462:
        return "net462"
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
    """Return target frameworks and all package versions as 'Name=Version' strings.

    For SDK-style projects, we read TargetFramework/TargetFrameworks and PackageReference.
    For classic projects, we also read TargetFrameworkVersion and packages.config if present.
    """
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

    # Classic projects: TargetFrameworkVersion -> net4xx.
    try:
        tree = ET.parse(csproj)
        root = tree.getroot()
        for elem in root.findall(".//{*}TargetFrameworkVersion"):
            if elem.text:
                tfv = elem.text.strip().lstrip("vV")
                # e.g., 4.6.1 -> net461
                digits = "".join(ch for ch in tfv if ch.isdigit())
                if digits:
                    frameworks.append(f"net{digits}")
    except Exception:
        pass

    # Classic projects: pull from packages.config if present.
    pkgs_config = csproj.parent / "packages.config"
    if pkgs_config.exists():
        try:
            tree = ET.parse(pkgs_config)
            root = tree.getroot()
            for pkg in root.findall(".//package"):
                name = pkg.attrib.get("id")
                version = pkg.attrib.get("version")
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


def is_netfx_tfm(tfm: str) -> bool:
    t = tfm.lower().strip()
    return t.startswith("net2") or t.startswith("net3") or t.startswith("net4")


def is_nunit_project(packages: list[str]) -> bool:
    """Return True if the package list contains NUnit test stack (framework/adapter)."""
    targets = {"nunit", "nunit.framework", "nunit3testadapter"}
    for pkg in packages:
        name = pkg.split("=", 1)[0].lower()
        if name in targets:
            return True
    return False


def detect_project_style(csproj: Path) -> str:
    """Return 'sdk' for SDK-style projects, otherwise 'classic'."""
    try:
        tree = ET.parse(csproj)
        root = tree.getroot()
        if root.attrib.get("Sdk"):
            return "sdk"
    except Exception:
        pass
    return "classic"


def load_json(path: Path) -> object | None:
    try:
        return json.loads(path.read_text(encoding="utf-8"))
    except Exception:
        return None


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
        help="(Deprecated) Allow pre-release updates. Use --feed instead.",
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
    parser.add_argument(
        "--nunit-only",
        action="store_true",
        help="Only update NUnit-related packages (NUnit/NUnit.Framework/NUnit3TestAdapter/Microsoft.NET.Test.Sdk) to the captured target versions; skip other packages.",
    )
    parser.add_argument(
        "--skip-netfx",
        action="store_true",
        help="Skip issues where all target frameworks are classic .NET Framework (net2*/net3*/net4*).",
    )
    parser.add_argument(
        "--only-netfx",
        action="store_true",
        help="Process only issues where all target frameworks are classic .NET Framework (net2*/net3*/net4*).",
    )
    parser.add_argument(
        "--feed",
        choices=["stable", "nuget-prerelease", "myget-alpha"],
        default="stable",
        help="Package feed/versions: stable (nuget.org), nuget-prerelease (nuget.org prerelease), myget-alpha (adds myget feed with prerelease).",
    )
    args = parser.parse_args()

    root = args.root.resolve()
    metadata_path = args.metadata or (root / "scripts" / "issues_metadata.json")
    results_path = root / "results.json"
    log_path = root / "TestResults-consolelog.md"
    log_lines: list[str] = []
    update_records: list[dict] = []
    existing_results = load_json(results_path)
    if not isinstance(existing_results, list):
        existing_results = []

    def log(msg: str) -> None:
        print(msg)
        log_lines.append(msg)

    def persist_logs() -> None:
        try:
            log_path.write_text("\n".join(log_lines) + "\n", encoding="utf-8")
        except Exception:
            pass

    if not metadata_path.exists():
        sys.stderr.write(f"Metadata file not found: {metadata_path}\n")
        return 1

    try:
        records = json.loads(metadata_path.read_text(encoding="utf-8"))
    except Exception as exc:  # noqa: BLE001
        sys.stderr.write(f"Failed to read metadata: {exc}\n")
        return 1

    def persist_metadata() -> None:
        # Do not overwrite central metadata here.
        return

    def persist_update_records() -> None:
        (root / "testupdate.json").write_text(
            json.dumps(update_records, indent=2), encoding="utf-8"
        )

    def upsert_result(entry: dict) -> None:
        key = (entry.get("number"), entry.get("project_path"))
        replaced = False
        for idx, item in enumerate(existing_results):
            if (item.get("number"), item.get("project_path")) == key:
                existing_results[idx] = entry
                replaced = True
                break
        if not replaced:
            existing_results.append(entry)
        results_path.write_text(json.dumps(existing_results, indent=2), encoding="utf-8")
        issue_num = entry.get("number")
        issue_dir = find_issue_dir(root, int(issue_num)) if issue_num is not None else None
        if issue_dir:
            per_issue = [e for e in existing_results if e.get("number") == issue_num]
            (issue_dir / "issue_results.json").write_text(
                json.dumps(per_issue, indent=2), encoding="utf-8"
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
    # Resolve feed/pre-release behavior.
    feed = args.feed
    if args.pre_release and feed == "stable":
        feed = "nuget-prerelease"
    if feed == "stable":
        pre_mode = "Never"
    else:
        pre_mode = "Always"

    # Avoid failing on missing local feeds (e.g., machine-specific paths in NuGet.config).
    os.environ.setdefault("RestoreIgnoreFailedSources", "true")
    os.environ.setdefault("NUGET_RESTORE_IGNORE_FAILED_SOURCES", "true")
    os.environ.setdefault("DOTNET_RESTORE_IGNORE_FAILED_SOURCES", "true")

    if feed == "myget-alpha":
        ensure_nugetc_and_myget(root, log)
    run_nunit_versions: list[str] = []
    run_nunit_map: dict[str, str] = {}

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
        tfms, all_packages = read_tfms_and_packages(csproj)
        if not is_nunit_project(all_packages):
            continue
        if args.skip_netfx and tfms and all(is_netfx_tfm(t) for t in tfms):
            continue
        if args.only_netfx and tfms and not all(is_netfx_tfm(t) for t in tfms):
            continue
        workdir = csproj.parent
        target = choose_dotnet_target(workdir, csproj)
        log(f"[{num_val}] NUnit package version check (pre-release mode {pre_mode})")
        nu_stdout, nu_stderr, _ = run_cmd(
            [
                "dotnet",
                "outdated",
                "--pre-release",
                pre_mode,
                "--include",
                "nunit",
                target.name,
            ],
            workdir,
            timeout=args.timeout,
        )
        current_versions = []
        nunit_versions = find_nunit_packages(csproj)
        # Auto-bump NUnit packages to latest stable before capturing versions.
        if nunit_versions:
            for pkg_name, current_ver in nunit_versions.items():
                latest = latest_stable_version(pkg_name)
                if latest and is_newer_version(latest, current_ver):
                    log(f"[{num_val}] Bumping {pkg_name} from {current_ver} to stable {latest} (pretest)")
                    upd_stdout, upd_stderr, upd_code = run_cmd(
                        ["dotnet", "add", target.name, "package", pkg_name, "--version", latest],
                        workdir,
                        timeout=args.timeout,
                    )
                    update_records.append(
                        {
                            "phase": "manual-nunit-upgrade-pretest",
                            "issue": num_val,
                            "package": pkg_name,
                            "from": current_ver,
                            "to": latest,
                            "target": str(target),
                            "exit_code": upd_code,
                            "stdout": upd_stdout,
                            "stderr": upd_stderr,
                        }
                    )
                    persist_update_records()
            # refresh versions after potential bump
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
                "feed": feed,
                "pre_mode": pre_mode,
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
            if nunit_versions:
                run_nunit_map = {k.lower(): v for k, v in nunit_versions.items()}
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
        tfms, all_packages = read_tfms_and_packages(csproj)
        if not is_nunit_project(all_packages):
            # Try another candidate that does look like an NUnit project before skipping.
            fallback = find_first_nunit_csproj(issue_dir)
            if fallback and fallback != csproj:
                csproj = fallback
                workdir = csproj.parent
                rel_proj = csproj.relative_to(root)
                target = choose_dotnet_target(workdir, csproj)
                tfms, all_packages = read_tfms_and_packages(csproj)
            if not is_nunit_project(all_packages):
                log(f"[{num}] Skipped (not an NUnit test project)")
                continue
        if args.only_netfx and tfms and not all(is_netfx_tfm(t) for t in tfms):
            log(f"[{num}] Skipped (not netfx-only; --only-netfx enabled)")
            continue
        if args.skip_netfx and tfms and all(is_netfx_tfm(t) for t in tfms):
            log(f"[{num}] Skipped (netfx-only project; --skip-netfx enabled)")
            continue
        record["project_style"] = detect_project_style(csproj)
        record_changed = True
        persist_metadata()

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

        align_target_frameworks_and_references(
            num=int(num), csproj=csproj, rel_proj=rel_proj, root=root, log_fn=log, record=record
        )
        record_changed = True

        try:
            global_json = find_global_json(issue_dir, workdir)
            if global_json and update_global_json(global_json):
                log(f"[{num}] Updated SDK version to {LATEST_SDK} in {global_json.relative_to(root)}")
        except Exception as exc:  # noqa: BLE001
            record["notes"] = f"Failed to update global.json: {exc}"
            record_changed = True

        log(f"[{num}] Updating packages in {rel_proj}")

        if args.nunit_only and run_nunit_map:
            outputs = []
            errors = []
            codes = []
            for pkg in all_packages:
                name, _, ver = pkg.partition("=")
                target_ver = run_nunit_map.get(name.lower())
                if not target_ver:
                    continue
                if ver == target_ver:
                    continue
                log(f"[{num}] Bumping {name} from {ver} to {target_ver} (nunit-only)")
                upd_stdout, upd_stderr, upd_code = run_cmd(
                    ["dotnet", "add", target.name, "package", name, "--version", target_ver],
                    workdir,
                    timeout=args.timeout,
                )
                outputs.append(upd_stdout)
                errors.append(upd_stderr)
                codes.append(upd_code)
                update_records.append(
                    {
                        "phase": "issue-update",
                        "scope": "nunit-only",
                        "issue": int(num),
                        "target": str(target),
                        "package": name,
                        "to_version": target_ver,
                        "exit_code": upd_code,
                        "stdout": upd_stdout,
                        "stderr": upd_stderr,
                    }
                )
            if not codes:
                record["update_result"] = "success"
                record["update_output"] = "nunit-only: no changes (already at target versions)"
                record["update_error"] = ""
            else:
                record["update_result"] = "success" if all(c == 0 for c in codes) else "fail"
                record["update_output"] = "\n".join(outputs)
                record["update_error"] = "\n".join(errors)
        else:
            # dotnet outdated (NUnit packages first, optionally with prerelease; then others without)
            nunit_stdout, nunit_stderr, nunit_code = run_cmd(
                [
                    "dotnet",
                    "outdated",
                    "--pre-release",
                    pre_mode,
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
                    "feed": feed,
                    "pre_mode": pre_mode,
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
                    pre_mode,
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
                    "feed": feed,
                    "pre_mode": pre_mode,
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

        test_code, test_stdout, test_stderr, runsettings_path, runner_paths = run_tests_for_issue(
            num=int(num),
            rel_proj=rel_proj,
            target=target,
            workdir=workdir,
            issue_dir=issue_dir,
            timeout=args.timeout,
            log_fn=log,
        )
        record["test_result"] = "success" if test_code == 0 else "fail"
        record["test_output"] = test_stdout
        record["test_error"] = test_stderr
        if runsettings_path:
            record["runsettings"] = str(runsettings_path)
        if runner_paths:
            record["runner_scripts"] = runner_paths

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

        entry = {
            "number": int(num),
            "project_path": csproj.relative_to(issue_dir).as_posix(),
            "project_style": detect_project_style(csproj),
            "target_frameworks": tfms,
            "packages": all_packages,
            "update_result": record["update_result"],
            "update_output": record.get("update_output", ""),
            "update_error": record.get("update_error", ""),
            "test_result": record["test_result"],
            "test_output": record.get("test_output", ""),
            "test_error": record.get("test_error", ""),
            "test_conclusion": record.get("test_conclusion", ""),
            "runner_scripts": record.get("runner_scripts", []),
            "runsettings": record.get("runsettings"),
        }
        upsert_result(entry)

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
        # Persist logs after each issue iteration so partial progress is retained.
        persist_logs()

    persist_logs()
    (root / "testupdate.json").write_text(
        json.dumps(update_records, indent=2), encoding="utf-8"
    )
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
