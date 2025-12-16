@echo off
REM Build and run IssueRunner tool

cd /d "%~dp0"

echo Building IssueRunner...
dotnet build -c Release

if %ERRORLEVEL% neq 0 (
    echo Build failed!
    exit /b %ERRORLEVEL%
)

echo.
echo Build complete! You can now run:
echo   cd IssueRunner\bin\Release\net10.0
echo   IssueRunner.exe --help
echo.
echo Or run directly:
echo   IssueRunner\bin\Release\net10.0\IssueRunner.exe %*
