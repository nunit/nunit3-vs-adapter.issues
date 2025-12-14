@echo off
REM Update all metadata from GitHub in two steps:
REM 1. Fetch GitHub metadata to central file
REM 2. Distribute to individual issue folders

echo Fetching metadata from GitHub to central file...
python scripts\update_central_from_github.py
if %ERRORLEVEL% neq 0 (
    echo ERROR: Failed to fetch from GitHub
    exit /b %ERRORLEVEL%
)

echo.
echo Distributing metadata to issue folders...
python scripts\update_folders_from_central.py
if %ERRORLEVEL% neq 0 (
    echo ERROR: Failed to update issue folders
    exit /b %ERRORLEVEL%
)

echo.
echo SUCCESS: All metadata updated from GitHub
echo You can now commit the updated metadata files to the repository.
