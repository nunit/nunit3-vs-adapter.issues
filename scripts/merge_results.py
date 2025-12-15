#!/usr/bin/env python
"""
Merge per-OS test artifacts into unified outputs:
- results.json (deduped by number + project_path)
- testupdate.json (deduped by phase/issue/target/package/to_version)
- TestResults-consolelog.md (concatenated)

Usage:
    python scripts/merge_results.py --linux artifacts-linux --windows artifacts-windows
"""

from __future__ import annotations

import argparse
import json
from pathlib import Path
from typing import Iterable


def load_json_list(path: Path) -> list:
    try:
        data = json.loads(path.read_text(encoding="utf-8"))
        return data if isinstance(data, list) else []
    except Exception:
        return []


def merge_results(paths: Iterable[Path]) -> list:
    merged: list[dict] = []
    seen: set[tuple] = set()
    for p in paths:
        for item in load_json_list(p):
            if not isinstance(item, dict):
                continue
            key = (item.get("number"), item.get("project_path"))
            if key in seen:
                continue
            seen.add(key)
            merged.append(item)
    return merged


def merge_updates(paths: Iterable[Path]) -> list:
    merged: list[dict] = []
    seen: set[tuple] = set()
    for p in paths:
        for item in load_json_list(p):
            if not isinstance(item, dict):
                continue
            key = (
                item.get("phase"),
                item.get("issue"),
                item.get("target"),
                item.get("package"),
                item.get("to") or item.get("to_version"),
            )
            if key in seen:
                continue
            seen.add(key)
            merged.append(item)
    return merged


def merge_logs(paths: Iterable[Path]) -> str:
    parts: list[str] = []
    for p in paths:
        if p.exists():
            try:
                parts.append(p.read_text(encoding="utf-8"))
            except Exception:
                continue
    return "\n\n".join(parts)


def main() -> int:
    parser = argparse.ArgumentParser(description="Merge per-OS test artifacts.")
    parser.add_argument("--linux", type=Path, required=False, help="Path to Linux artifacts folder")
    parser.add_argument("--windows", type=Path, required=False, help="Path to Windows artifacts folder")
    parser.add_argument("--out-results", type=Path, default=Path("results.json"))
    parser.add_argument("--out-updates", type=Path, default=Path("testupdate.json"))
    parser.add_argument("--out-log", type=Path, default=Path("TestResults-consolelog.md"))
    args = parser.parse_args()

    artifact_dirs = [p for p in [args.linux, args.windows] if p]

    result_paths = [p / "results.json" for p in artifact_dirs]
    update_paths = [p / "testupdate.json" for p in artifact_dirs]
    log_paths = [p / "TestResults-consolelog.md" for p in artifact_dirs]

    merged_results = merge_results(result_paths)
    merged_updates = merge_updates(update_paths)
    merged_logs = merge_logs(log_paths)

    args.out_results.write_text(json.dumps(merged_results, indent=2), encoding="utf-8")
    args.out_updates.write_text(json.dumps(merged_updates, indent=2), encoding="utf-8")
    args.out_log.write_text(merged_logs, encoding="utf-8")

    return 0


if __name__ == "__main__":
    raise SystemExit(main())
