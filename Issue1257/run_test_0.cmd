@echo off
REM Checks bin\Debug\net10.0\ran.txt: the tests that really ran. The project has more than 2000 test cases, so AssemblySelectLimit must be raised, or the adapter runs all tests.
if exist bin\Debug\net10.0\ran.txt del bin\Debug\net10.0\ran.txt
dotnet test --filter "FullyQualifiedName!~.Multimedia" -- NUnit.AssemblySelectLimit=10000
set rc=0
findstr /C:"NoPlatform" bin\Debug\net10.0\ran.txt >nul 2>&1 && (echo Ran outside the filter: NoPlatform& set rc=1)
findstr /C:"CheckDeviceAddedRemoved" bin\Debug\net10.0\ran.txt >nul 2>&1 && (echo Ran outside the filter: CheckDeviceAddedRemoved& set rc=1)
exit /b %rc%
