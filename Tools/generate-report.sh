#!/bin/bash
# Generate test report from results

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ISSUERUNNER="$SCRIPT_DIR/IssueRunner.Cli/bin/Release/net10.0/IssueRunner.Cli"
ROOT="$SCRIPT_DIR/.."

if [ ! -f "$ISSUERUNNER" ]; then
    echo "Error: IssueRunner not found. Run build.sh first."
    exit 1
fi

"$ISSUERUNNER" report generate --root "$ROOT" "$@"
