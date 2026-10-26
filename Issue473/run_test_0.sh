#!/bin/bash
# Checks bin/Debug/net10.0/ran.txt: the tests that really ran.
rm -f bin/Debug/net10.0/ran.txt
dotnet test --filter "FullyQualifiedName~Issue473.ExplicitFixture"
rc=0
grep -qF -- 'InExplicitFixture' bin/Debug/net10.0/ran.txt 2>/dev/null || { echo 'Expected to run: InExplicitFixture'; rc=1; }
grep -qF -- 'Normal' bin/Debug/net10.0/ran.txt 2>/dev/null && { echo 'Ran outside the filter: Normal'; rc=1; }
exit $rc
