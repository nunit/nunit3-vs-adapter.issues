#!/usr/bin/env python
"""
Fetch issue metadata (state, milestone, labels) from GitHub and write JSON.

- Reads issue numbers from folders named Issue*/ under the repo root.
- Uses optional GITHUB_TOKEN for higher rate limits.
- Output JSON is a list of issue dicts with keys:
  number, title, state, milestone, labels, url
"""

from __future__ import annotations

import argparse
import json
import os
import re
import sys
import urllib.error
import urllib.request
from pathlib import Path

REPO = "nunit/nunit3-vs-adapter"
ISSUE_URL_FMT = f"https://github.com/{REPO}/issues/{{num}}"
API_URL_FMT = f"https://api.github.com/repos/{REPO}/issues/{{num}}"
USER_AGENT = "issue-metadata-fetcher"


def first_digits(text: str) -> str | None:
    match = re.search(r"\d+", text)
    return match.group(0) if match else None


def fetch_issue(num: str, token: str | None) -> dict | None:
    url = API_URL_FMT.format(num=num)
    headers = {"User-Agent": USER_AGENT}
    if token:
        headers["Authorization"] = f"Bearer {token}"
    req = urllib.request.Request(url, headers=headers)
    try:
        with urllib.request.urlopen(req) as resp:
            data = json.loads(resp.read().decode("utf-8"))
    except urllib.error.HTTPError as exc:
        sys.stderr.write(f"[warn] {num}: HTTP {exc.code} ({exc.reason})\n")
        try:
            body = exc.read().decode("utf-8")
            msg = json.loads(body).get("message")
            if msg:
                sys.stderr.write(f"       message: {msg}\n")
        except Exception:
            pass
        return None
    except Exception as exc:  # noqa: BLE001
        sys.stderr.write(f"[warn] {num}: {exc}\n")
        return None

    return {
        "number": int(num),
        "title": data.get("title"),
        "state": data.get("state"),
        "milestone": (data.get("milestone") or {}).get("title") or "No milestone",
        "labels": [lbl.get("name") for lbl in data.get("labels", []) if lbl.get("name")],
        "url": ISSUE_URL_FMT.format(num=num),
    }


def gather_issue_numbers(root: Path) -> list[str]:
    nums: list[str] = []
    for path in root.iterdir():
        if not path.is_dir() or not path.name.startswith("Issue"):
            continue
        num = first_digits(path.name)
        if num:
            nums.append(num)
    return nums


def main() -> int:
    parser = argparse.ArgumentParser(description="Fetch GitHub issue metadata for Issue* folders.")
    parser.add_argument("--root", type=Path, default=Path(__file__).resolve().parent.parent,
                        help="Repo root containing Issue* folders (default: repo root)")
    parser.add_argument("--output", type=Path, default=None,
                        help="Output JSON path (default: scripts/issues_metadata.json)")
    args = parser.parse_args()

    root = args.root.resolve()
    if args.output is None:
        args.output = root / "scripts" / "issues_metadata.json"
    output_path: Path = args.output

    token = os.getenv("GITHUB_TOKEN")
    nums = sorted(set(gather_issue_numbers(root)), key=lambda x: int(x))
    if not nums:
        sys.stderr.write("No Issue* directories found.\n")
        return 1

    results: list[dict] = []
    for num in nums:
        info = fetch_issue(num, token)
        if info:
            results.append(info)
            print(f"Fetched #{num}: {info['state']}, milestone={info['milestone']}, labels={info['labels']}")
        else:
            print(f"Skipped #{num} (not fetched)")

    output_path.parent.mkdir(parents=True, exist_ok=True)
    output_path.write_text(json.dumps(results, indent=2), encoding="utf-8")
    print(f"Wrote {len(results)} issues to {output_path}")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())