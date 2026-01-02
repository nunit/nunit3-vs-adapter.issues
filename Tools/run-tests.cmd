@echo off
REM Run tests on issues and update results
setlocal

set ISSUERUNNER=%~dp0IssueRunner\bin\Release\net10.0\IssueRunner.exe

REM Determine repository root
if "%ISSUERUNNER_ROOT%"=="" (
    REM If not set via environment variable, prefer current directory *if it looks like a repo root*.
    REM This allows running from another repo (e.g. nunit.issues) via a relative path to this script.
    if exist "%CD%\Tools\repository.json" (
        set ROOT=%CD%
    ) else if exist "%CD%\repository.json" (
        set ROOT=%CD%
    ) else if exist "%CD%\.git" (
        set ROOT=%CD%
    ) else (
        REM Fallback: use the parent of this script directory (the repo that contains Tools\run-tests.cmd).
        set ROOT=%~dp0..
    )
) else (
    REM Use environment variable
    set ROOT=%ISSUERUNNER_ROOT%
)

if not exist "%ISSUERUNNER%" (
    echo Error: IssueRunner.exe not found. Run build.cmd first.
    exit /b 1
)

echo Target repository: %ROOT%
echo.

pushd "%ROOT%" >nul

REM If the first arg is a known verb, forward all args as-is.
if /I "%~1"=="run" goto :forward
if /I "%~1"=="report" (
    if "%~2"=="" goto :report_default
    goto :forward
)
if /I "%~1"=="metadata" goto :forward
if /I "%~1"=="reset" goto :forward
if /I "%~1"=="merge" goto :forward

REM No verb specified (or options only): default to "run".
if "%~1"=="" (
    "%ISSUERUNNER%" run
    goto :done
)

REM If options were passed (e.g. --issues, --feed), treat them as "run" options.
if "%~1:~0,2%"=="--" (
    "%ISSUERUNNER%" run %*
    goto :done
)

REM If first arg doesn't start with -- and isn't a known verb, treat as "run" options.
REM This handles cases like "rerun-failed" or "-rerun" where user forgot the -- prefix.
"%ISSUERUNNER%" run %*
goto :done

REM Otherwise forward (lets you call subcommands without modifying this script).
:forward
"%ISSUERUNNER%" %*
goto :done

:report_default
"%ISSUERUNNER%" report generate

:done
popd >nul
