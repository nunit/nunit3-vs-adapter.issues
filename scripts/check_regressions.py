#!/usr/bin/env python
"""Fail if any closed issues have failing tests."""

from __future__ import annotations

import json
from pathlib import Path
import sys


def main() -> int:
    path = Path("scripts/issues_metadata.json")
    try:
        data = json.loads(path.read_text(encoding="utf-8"))
    except Exception as exc:  # noqa: BLE001
        sys.stderr.write(f"Failed to read metadata: {exc}\n")
        return 1

    failed = [
        item
        for item in data
        if (item.get("state") or "").lower() == "closed"
        and item.get("test_result") == "fail"
    ]

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
