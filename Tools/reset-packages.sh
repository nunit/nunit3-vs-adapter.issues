#!/bin/bash

# Determine the repository root
if [ -n "$ISSUERUNNER_ROOT" ]; then
    REPO_ROOT="$ISSUERUNNER_ROOT"
else
    REPO_ROOT="$(pwd)"
fi

# Determine IssueRunner location
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ISSUERUNNER="$SCRIPT_DIR/IssueRunner/bin/Release/net10.0/IssueRunner"

# Debug output
echo "IssueRunner location: $ISSUERUNNER"
echo "Target repository: $REPO_ROOT"
echo

# Run the reset command
"$ISSUERUNNER" reset --root "$REPO_ROOT" "$@"
