@echo off
REM Checks bin\Debug\net10.0\ran.txt: the tests that really ran.
if exist bin\Debug\net10.0\ran.txt del bin\Debug\net10.0\ran.txt
dotnet test --filter "(ClassName=Issue1202.Tests&Name=FailedTestName)"
set rc=0
findstr /C:"FailedTestName" bin\Debug\net10.0\ran.txt >nul 2>&1 || (echo Expected to run: FailedTestName& set rc=1)
findstr /C:"OtherTest" bin\Debug\net10.0\ran.txt >nul 2>&1 && (echo Ran outside the filter: OtherTest& set rc=1)
exit /b %rc%
