#!/bin/bash
# Checks bin/Debug/net10.0/ran.txt: the tests that really ran. The project has more than 2000 test cases, so AssemblySelectLimit must be raised, or the adapter runs all tests.
rm -f bin/Debug/net10.0/ran.txt
dotnet test --filter "FullyQualifiedName!~.Multimedia" -- NUnit.AssemblySelectLimit=10000
rc=0
grep -qF -- 'NoPlatform' bin/Debug/net10.0/ran.txt 2>/dev/null && { echo 'Ran outside the filter: NoPlatform'; rc=1; }
grep -qF -- 'CheckDeviceAddedRemoved' bin/Debug/net10.0/ran.txt 2>/dev/null && { echo 'Ran outside the filter: CheckDeviceAddedRemoved'; rc=1; }
exit $rc
