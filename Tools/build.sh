#!/bin/bash
# Build and run IssueRunner tool

set -e

cd "$(dirname "$0")"

echo "Building IssueRunner..."
dotnet build -c Release

echo ""
echo "Build complete! You can now run:"
echo "  cd IssueRunner/bin/Release/net10.0"
echo "  ./IssueRunner --help"
echo ""
echo "Or run directly:"
echo "  ./IssueRunner/bin/Release/net10.0/IssueRunner \$@"
