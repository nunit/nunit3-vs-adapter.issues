#!/usr/bin/env python
"""
Create per-issue JSON files that list project details (one entry per csproj).

For each Issue* folder:
- Look up issue metadata from scripts/issues_metadata.json (or --metadata override).
- Find all csproj files in the folder and one level below.
- For each csproj, record relative path, target frameworks, and package references.
- Write issue_metadata.json inside the issue folder as an array of objects, one per csproj:
  {
    "number": ...,
    "title": ...,
    "state": ...,
    "milestone": ...,
    "labels": [...],
    "url": "...",
    "project_path": "...",
    "target_frameworks": [...],
    "packages": [ { "name": "...", "version": "..." }, ... ]
  }
If an issue has no csproj, it writes an empty array.
"""

from __future__ import annotations

import argparse
import json
import re
from pathlib import Path
from typing import Dict, List, Optional, Tuple
import xml.etree.ElementTree as ET


def load_metadata(path: Path) -> Dict[str, dict]:
    """Load central issues metadata and index by issue number string."""
    data = json.loads(path.read_text(encoding="utf-8"))
    return {str(item.get("number")): item for item in data if "number" in item}


def find_csprojs(issue_dir: Path) -> List[Path]:
    # Walk all subdirectories so projects nested more than one level are still included.
    return list(issue_dir.rglob("*.csproj"))


def parse_csproj(csproj: Path) -> Tuple[List[str], List[Tuple[str, str]]]:
    frameworks: List[str] = []
    packages: List[Tuple[str, str]] = []
    try:
        tree = ET.parse(csproj)
        root = tree.getroot()
        tf_elems = root.findall(".//{*}TargetFramework") + root.findall(".//{*}TargetFrameworks")
        for tf in tf_elems:
            if tf.text:
                for part in tf.text.replace(";", ",").split(","):
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
            if name:
                packages.append((name, version or "<no version found>"))
    except Exception as ex:  # noqa: BLE001
        frameworks.append(f"ERROR parsing csproj: {ex}")
    return frameworks, packages


def first_digits(text: str) -> Optional[str]:
    m = re.search(r"\d+", text)
    return m.group(0) if m else None


def main() -> int:
    parser = argparse.ArgumentParser(description="Write per-issue project metadata JSON files.")
    parser.add_argument(
        "--root",
        type=Path,
        default=Path.cwd(),
        help="Repo root containing Issue* folders (default: current working directory)",
    )
    parser.add_argument(
        "--metadata",
        type=Path,
        default=None,
        help="Central metadata JSON (default: scripts/issues_metadata.json under root)",
    )
    args = parser.parse_args()

    root = args.root.resolve()
    metadata_path = args.metadata or (root / "scripts" / "issues_metadata.json")
    if not metadata_path.exists():
        raise SystemExit(f"Central metadata not found: {metadata_path}")

    meta_index = load_metadata(metadata_path)

    issue_dirs = [p for p in root.iterdir() if p.is_dir() and p.name.startswith("Issue")]
    print(f"Found {len(issue_dirs)} Issue* folders under {root}")
    for issue_dir in issue_dirs:
        num = first_digits(issue_dir.name)
        if not num:
            continue
        issue_info = meta_index.get(num, {})
        url = issue_info.get("url") or f"https://github.com/nunit/nunit3-vs-adapter/issues/{num}"
        csprojs = find_csprojs(issue_dir)
        entries = []
        for csproj in csprojs:
            frameworks, packages = parse_csproj(csproj)
            entries.append(
                {
                    "number": int(num),
                    "title": issue_info.get("title"),
                    "state": issue_info.get("state"),
                    "milestone": issue_info.get("milestone"),
                    "labels": issue_info.get("labels"),
                    "url": url,
                    "project_path": csproj.relative_to(issue_dir).as_posix(),
                    "target_frameworks": frameworks,
                    "packages": [{"name": n, "version": v} for n, v in packages],
                }
            )
        out_path = issue_dir / "issue_metadata.json"
        out_path.write_text(json.dumps(entries, indent=2), encoding="utf-8")
        print(f"Wrote {out_path} ({len(entries)} project entries)")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
