#!/usr/bin/env python
"""
Update Issue*/readme.initialstate.md with the GitHub issue title.

Relies only on the standard library. Optionally uses a GitHub token from
the environment variable GITHUB_TOKEN to avoid rate limits.
"""

from __future__ import annotations

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
USER_AGENT = "issue-title-updater"


def fetch_title(num: str, token: str | None) -> str | None:
    """Return the issue title, or None if not found / error."""
    url = API_URL_FMT.format(num=num)
    headers = {"User-Agent": USER_AGENT}
    if token:
        headers["Authorization"] = f"Bearer {token}"

    req = urllib.request.Request(url, headers=headers)
    try:
        with urllib.request.urlopen(req) as resp:
            data = json.loads(resp.read().decode("utf-8"))
        return data.get("title")
    except urllib.error.HTTPError as exc:
        sys.stderr.write(f"[warn] {num}: HTTP {exc.code} ({exc.reason})\n")
        try:
            body = exc.read().decode("utf-8")
            msg = json.loads(body).get("message")
            if msg:
                sys.stderr.write(f"       message: {msg}\n")
        except Exception:
            pass
    except Exception as exc:  # noqa: BLE001
        sys.stderr.write(f"[warn] {num}: {exc}\n")
    return None


def first_digits(text: str) -> str | None:
    match = re.search(r"\d+", text)
    return match.group(0) if match else None


def main() -> int:
    repo_root = Path(__file__).resolve().parent.parent
    token = os.getenv("GITHUB_TOKEN")

    issue_dirs = [p for p in repo_root.iterdir() if p.is_dir() and p.name.startswith("Issue")]
    if not issue_dirs:
        sys.stderr.write("No Issue* directories found.\n")
        return 1

    for issue_dir in issue_dirs:
        num = first_digits(issue_dir.name)
        if not num:
            continue

        title = fetch_title(num, token)
        issue_url = ISSUE_URL_FMT.format(num=num)
        line = f"Repro for Issue [#{num}]({issue_url})"
        if title:
            line += f" - {title}"

        new_path = issue_dir / "readme.initialstate.md"
        old_path = issue_dir / "readme.md"

        # If an old lowercase readme exists, rename it so we don't leave duplicates around.
        if old_path.exists() and not new_path.exists():
            old_path.rename(new_path)

        new_path.write_text(line, encoding="utf-8")
        print(f"Set {new_path} -> {line}")

    return 0


if __name__ == "__main__":
    raise SystemExit(main())
