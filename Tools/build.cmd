@echo off
REM Build IssueRunner projects (Core, CLI, and GUI)

cd /d "%~dp0"

echo Building IssueRunner.Core...
dotnet build IssueRunner.Core\IssueRunner.Core.csproj -c Release
if %ERRORLEVEL% neq 0 (
    echo Build failed!
    exit /b %ERRORLEVEL%
)

echo Building IssueRunner.Cli...
dotnet build IssueRunner.Cli\IssueRunner.Cli.csproj -c Release
if %ERRORLEVEL% neq 0 (
    echo Build failed!
    exit /b %ERRORLEVEL%
)

echo Building IssueRunner.Gui...
dotnet build IssueRunner.Gui\IssueRunner.Gui.csproj -c Release
if %ERRORLEVEL% neq 0 (
    echo Build failed!
    exit /b %ERRORLEVEL%
)

echo.
echo Build complete! You can now run:
cd ..
echo   CLI: .\Tools\IssueRunner.Cli\bin\Release\net10.0\IssueRunner.Cli.exe --help
echo   GUI: .Tools\IssueRunner.Gui\bin\Release\net10.0\IssueRunner.Gui.exe
echo.
