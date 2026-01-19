#!/bin/bash
# Sync metadata from central file to individual issue folders

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ISSUERUNNER="$SCRIPT_DIR/IssueRunner.Cli/bin/Release/net10.0/IssueRunner.Cli"

# Determine repository root
if [ -z "$ISSUERUNNER_ROOT" ]; then
    # If not set via environment variable, use current directory
    ROOT=$(pwd)
else
    # Use environment variable
    ROOT="$ISSUERUNNER_ROOT"
fi

if [ ! -f "$ISSUERUNNER" ]; then
    echo "Error: IssueRunner not found. Run build.sh first."
    exit 1
fi

echo "IssueRunner location: $ISSUERUNNER"
echo "Target repository: $ROOT"
echo

"$ISSUERUNNER" metadata sync-to-folders --root "$ROOT" "$@"
