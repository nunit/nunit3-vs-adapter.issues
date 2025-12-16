#!/bin/bash
# Run tests on issues and update results

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ISSUERUNNER="$SCRIPT_DIR/IssueRunner/bin/Release/net10.0/IssueRunner"
ROOT="$SCRIPT_DIR/.."

if [ ! -f "$ISSUERUNNER" ]; then
    echo "Error: IssueRunner not found. Run build.sh first."
    exit 1
fi

"$ISSUERUNNER" run --root "$ROOT" "$@"
