@echo off
REM Checks bin\Debug\net10.0\ran.txt: the tests that really ran.
if exist bin\Debug\net10.0\ran.txt del bin\Debug\net10.0\ran.txt
dotnet test --filter "TestCategory=MyCustomCategory"
set rc=0
findstr /C:"AnotherTest" bin\Debug\net10.0\ran.txt >nul 2>&1 || (echo Expected to run: AnotherTest& set rc=1)
findstr /C:"MyTest" bin\Debug\net10.0\ran.txt >nul 2>&1 && (echo Ran outside the filter: MyTest& set rc=1)
exit /b %rc%
