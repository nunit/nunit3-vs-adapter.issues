@echo off
REM Checks bin\Debug\net10.0\ran.txt: the tests that really ran.
if exist bin\Debug\net10.0\ran.txt del bin\Debug\net10.0\ran.txt
dotnet test --filter "FullyQualifiedName~Issue473.ExplicitFixture"
set rc=0
findstr /C:"InExplicitFixture" bin\Debug\net10.0\ran.txt >nul 2>&1 || (echo Expected to run: InExplicitFixture& set rc=1)
findstr /C:"Normal" bin\Debug\net10.0\ran.txt >nul 2>&1 && (echo Ran outside the filter: Normal& set rc=1)
exit /b %rc%
