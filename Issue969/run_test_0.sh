#!/bin/bash
# Checks bin/Debug/net10.0/ran.txt: the tests that really ran.
rm -f bin/Debug/net10.0/ran.txt
dotnet build
dotnet test bin/Debug/net10.0/Issue969.dll --filter "FullyQualifiedName~SampleClass\(\"chrome\"\).SampleMethod"
rc=0
grep -qF -- 'chrome' bin/Debug/net10.0/ran.txt 2>/dev/null || { echo 'Expected to run: chrome'; rc=1; }
grep -qF -- 'edge' bin/Debug/net10.0/ran.txt 2>/dev/null && { echo 'Ran outside the filter: edge'; rc=1; }
grep -qF -- 'chromeheadless' bin/Debug/net10.0/ran.txt 2>/dev/null && { echo 'Ran outside the filter: chromeheadless'; rc=1; }
exit $rc
