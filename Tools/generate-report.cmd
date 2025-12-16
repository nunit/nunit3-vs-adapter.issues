@echo off
REM Generate test report from results
setlocal

set ISSUERUNNER=%~dp0IssueRunner\bin\Release\net10.0\IssueRunner.exe
set ROOT=%~dp0..

if not exist "%ISSUERUNNER%" (
    echo Error: IssueRunner.exe not found. Run build.cmd first.
    exit /b 1
)

"%ISSUERUNNER%" report generate --root "%ROOT%" %*
