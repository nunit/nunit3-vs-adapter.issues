#!/bin/bash
# Checks bin/Debug/net10.0/ran.txt: the tests that really ran.
rm -f bin/Debug/net10.0/ran.txt
dotnet test --filter "TestCategory=MyCustomCategory"
rc=0
grep -qF -- 'AnotherTest' bin/Debug/net10.0/ran.txt 2>/dev/null || { echo 'Expected to run: AnotherTest'; rc=1; }
grep -qF -- 'MyTest' bin/Debug/net10.0/ran.txt 2>/dev/null && { echo 'Ran outside the filter: MyTest'; rc=1; }
exit $rc
