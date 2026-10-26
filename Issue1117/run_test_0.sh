#!/bin/bash
# Checks bin/Debug/net10.0/ran.txt: the tests that really ran.
rm -f bin/Debug/net10.0/ran.txt
dotnet test --filter "FullyQualifiedName~Issue1117.Many"
rc=0
grep -qF -- 'MustNotRun' bin/Debug/net10.0/ran.txt 2>/dev/null && { echo 'Ran outside the filter: MustNotRun'; rc=1; }
exit $rc
