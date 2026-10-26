#!/bin/bash
# Checks bin/Debug/net10.0/ran.txt: the tests that really ran.
rm -f bin/Debug/net10.0/ran.txt
dotnet test --filter "(ClassName=Issue1202.Tests&Name=FailedTestName)"
rc=0
grep -qF -- 'FailedTestName' bin/Debug/net10.0/ran.txt 2>/dev/null || { echo 'Expected to run: FailedTestName'; rc=1; }
grep -qF -- 'OtherTest' bin/Debug/net10.0/ran.txt 2>/dev/null && { echo 'Ran outside the filter: OtherTest'; rc=1; }
exit $rc
