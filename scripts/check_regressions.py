#!/usr/bin/env python
"""Fail if any closed issues have failing tests."""

from __future__ import annotations

import json
from pathlib import Path
import sys


def main() -> int:
    meta_path = Path("scripts/issues_metadata.json")
    results_path = Path("results.json")
    try:
        metadata = json.loads(meta_path.read_text(encoding="utf-8"))
    except Exception as exc:  # noqa: BLE001
        sys.stderr.write(f"Failed to read metadata: {exc}\n")
        return 1
    try:
        results = json.loads(results_path.read_text(encoding="utf-8"))
    except Exception as exc:  # noqa: BLE001
        sys.stderr.write(f"Failed to read results: {exc}\n")
        return 1

    state_map = {int(m["number"]): (m.get("state") or "").lower() for m in metadata if "number" in m}
    failed = []
    for entry in results:
        num = entry.get("number")
        if num is None:
            continue
        try:
            num_int = int(num)
        except Exception:
            continue
        if state_map.get(num_int) != "closed":
            continue
        if entry.get("test_result") == "fail":
            failed.append(entry)

    if failed:
        print("Regression failures detected:")
        for item in failed:
            num = item.get("number")
            url = item.get("url") or ""
            print(f"- Issue #{num} {url}")
        return 1

    print("No regression failures detected.")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
