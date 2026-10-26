@echo off
REM Checks bin\Debug\net10.0\ran.txt: the tests that really ran.
if exist bin\Debug\net10.0\ran.txt del bin\Debug\net10.0\ran.txt
dotnet build
dotnet test bin\Debug\net10.0\Issue969.dll --filter "FullyQualifiedName~SampleClass\(\"chrome\"\).SampleMethod"
set rc=0
findstr /C:"chrome" bin\Debug\net10.0\ran.txt >nul 2>&1 || (echo Expected to run: chrome& set rc=1)
findstr /C:"edge" bin\Debug\net10.0\ran.txt >nul 2>&1 && (echo Ran outside the filter: edge& set rc=1)
findstr /C:"chromeheadless" bin\Debug\net10.0\ran.txt >nul 2>&1 && (echo Ran outside the filter: chromeheadless& set rc=1)
exit /b %rc%
