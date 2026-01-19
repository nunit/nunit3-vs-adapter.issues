@echo off
REM Sync issue metadata from GitHub API to central metadata file
setlocal

set ISSUERUNNER=%~dp0IssueRunner.Cli\bin\Release\net10.0\IssueRunner.Cli.exe

REM Determine repository root
if "%ISSUERUNNER_ROOT%"=="" (
    REM If not set via environment variable, use current directory
    set ROOT=%CD%
) else (
    REM Use environment variable
    set ROOT=%ISSUERUNNER_ROOT%
)

if not exist "%ISSUERUNNER%" (
    echo Error: IssueRunner.Cli.exe not found. Run build.cmd first.
    exit /b 1
)

echo IssueRunner location: %ISSUERUNNER%
echo Target repository: %ROOT%
echo.

"%ISSUERUNNER%" metadata sync-from-github --root "%ROOT%" %*
