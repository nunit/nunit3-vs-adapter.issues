@echo off
setlocal

REM Determine the repository root
if defined ISSUERUNNER_ROOT (
    set "REPO_ROOT=%ISSUERUNNER_ROOT%"
) else (
    set "REPO_ROOT=%CD%"
)

REM Determine IssueRunner location
set "SCRIPT_DIR=%~dp0"
set "ISSUERUNNER=%SCRIPT_DIR%IssueRunner.Cli\bin\Release\net10.0\IssueRunner.Cli.exe"

REM Debug output
echo IssueRunner location: %ISSUERUNNER%
echo Target repository: %REPO_ROOT%
echo.

REM Run the reset command
"%ISSUERUNNER%" reset --root "%REPO_ROOT%" %*
