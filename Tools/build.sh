#!/bin/bash
# Build IssueRunner projects (Core, CLI, and GUI)

set -e

cd "$(dirname "$0")"

echo "Building IssueRunner.Core..."
dotnet build IssueRunner.Core/IssueRunner.Core.csproj -c Release

echo "Building IssueRunner.Cli..."
dotnet build IssueRunner.Cli/IssueRunner.Cli.csproj -c Release

echo "Building IssueRunner.Gui..."
dotnet build IssueRunner.Gui/IssueRunner.Gui.csproj -c Release

echo ""
echo "Build complete! You can now run:"
echo "  CLI: ./IssueRunner.Cli/bin/Release/net10.0/IssueRunner.Cli --help"
echo "  GUI: ./IssueRunner.Gui/bin/Release/net10.0/IssueRunner.Gui"
echo ""
