@echo off
REM Run tests on issues and update results
setlocal

set ISSUERUNNER=%~dp0IssueRunner\bin\Release\net10.0\IssueRunner.exe

REM Determine repository root
if "%ISSUERUNNER_ROOT%"=="" (
    REM If not set via environment variable, use current directory
    set ROOT=%CD%
) else (
    REM Use environment variable
    set ROOT=%ISSUERUNNER_ROOT%
)

if not exist "%ISSUERUNNER%" (
    echo Error: IssueRunner.exe not found. Run build.cmd first.
    exit /b 1
)

echo IssueRunner location: %ISSUERUNNER%
echo Target repository: %ROOT%
echo.

"%ISSUERUNNER%" run --root "%ROOT%" %*
