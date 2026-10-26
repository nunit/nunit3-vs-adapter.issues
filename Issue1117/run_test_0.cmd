@echo off
REM Checks bin\Debug\net10.0\ran.txt: the tests that really ran.
if exist bin\Debug\net10.0\ran.txt del bin\Debug\net10.0\ran.txt
dotnet test --filter "FullyQualifiedName~Issue1117.Many"
set rc=0
findstr /C:"MustNotRun" bin\Debug\net10.0\ran.txt >nul 2>&1 && (echo Ran outside the filter: MustNotRun& set rc=1)
exit /b %rc%
