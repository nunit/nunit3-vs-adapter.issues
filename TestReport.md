# Test Report

## Summary

- Regression tests: total 95, success 69, fail 25
- Open issues: total 21, success 14, fail 7

## What we are testing

Package versions under test:

- NUnit=4.4.0
- NUnit.Analyzers=4.11.2
- NUnit3TestAdapter=6.0.0

## Regression tests (closed issues)

- Total: 95, Success: 69, Fail: 25

| Issue | Test | Conclusion |
| --- | --- | --- |
| ✅ #228 | success | Success: No regression failure |
| ✅ #241 | success | Success: No regression failure |
| ✅ #261 | success | Success: No regression failure |
| ✅ #343 | success | Success: No regression failure |
| ✅ #364 | success | Success: No regression failure |
| ✅ #425 | success | Success: No regression failure |
| ✅ #484 | success | Success: No regression failure |
| ✅ #497 | success | Success: No regression failure |
| ✅ #516 | success | Success: No regression failure |
| ✅ #530 | success | Success: No regression failure |
| ✅ #535 | success | Success: No regression failure |
| ✅ #545 | success | Success: No regression failure |
| ✅ #618 | success | Success: No regression failure |
| ✅ #640 | success | Success: No regression failure |
| ✅ #658 | success | Success: No regression failure |
| ❗ #671 | fail | Failure: Regression failure. |
| ✅ #687 | success | Success: No regression failure |
| ❗ #691 | fail | Failure: Regression failure. |
| ✅ #709 | success | Success: No regression failure |
| ✅ #711 | success | Success: No regression failure |
| ✅ #735 | success | Success: No regression failure |
| ✅ #740 | success | Success: No regression failure |
| ❗ #744 | fail | Failure: Regression failure. |
| ✅ #758 | success | Success: No regression failure |
| ✅ #760 | success | Success: No regression failure |
| ❗ #765 | fail | Failure: Regression failure. |
| ❗ #768 | fail | Failure: Regression failure. |
| ✅ #770 | success | Success: No regression failure |
| ✅ #774 | success | Success: No regression failure |
| ✅ #779 | success | Success: No regression failure |
| ✅ #780 | success | Success: No regression failure |
| ✅ #781 | success | Success: No regression failure |
| ✅ #784 | success | Success: No regression failure |
| ✅ #786 | success | Success: No regression failure |
| ✅ #811 | success | Success: No regression failure |
| ✅ #817 | success | Success: No regression failure |
| #822 | skipped | Skipped |
| ✅ #824 | success | Success: No regression failure |
| ❗ #833 | fail |  |
| ✅ #843 | success | Success: No regression failure |
| ❗ #846 | fail | Failure: Regression failure. |
| ✅ #847 | success | Success: No regression failure |
| ✅ #854 | success | Success: No regression failure |
| ❗ #873 | fail | Failure: Regression failure. |
| ✅ #874 | success | Success: No regression failure |
| ✅ #878 | success | Success: No regression failure |
| ✅ #881 | success | Success: No regression failure |
| ✅ #891 | success | Success: No regression failure |
| ✅ #909 | success | Success: No regression failure |
| ✅ #912 | success | Success: No regression failure |
| ✅ #914 | success | Success: No regression failure |
| ✅ #918 | success | Success: No regression failure |
| ❗ #919 | fail | Failure: Regression failure. |
| ✅ #941 | success | Success: No regression failure |
| ❗ #972 | fail | Failure: Regression failure. |
| ✅ #973 | success | Success: No regression failure |
| ✅ #987 | success | Success: No regression failure |
| ✅ #1027 | success | Success: No regression failure |
| ❗ #1039 | fail | Failure: Regression failure. |
| ❗ #1041 | fail | Failure: Regression failure. |
| ❗ #1044 | fail | Failure: Regression failure. |
| ✅ #1053 | success | Success: No regression failure |
| ✅ #1065 | success | Success: No regression failure |
| ✅ #1078 | success | Success: No regression failure |
| ✅ #1083 | success | Success: No regression failure |
| ✅ #1089 | success | Success: No regression failure |
| ❗ #1096 | fail | Failure: Regression failure. |
| ✅ #1102 | success | Success: No regression failure |
| ✅ #1111 | success | Success: No regression failure |
| ❗ #1127 | fail | Failure: Regression failure. |
| ❗ #1132 | fail | Failure: Regression failure. |
| ✅ #1144 | success | Success: No regression failure |
| ❗ #1146 | fail | Failure: Regression failure. |
| ❗ #1152 | fail | Failure: Regression failure. |
| ✅ #1166 | success | Success: No regression failure |
| ✅ #1182 | success | Success: No regression failure |
| ✅ #1183 | success | Success: No regression failure |
| ✅ #1186 | success | Success: No regression failure |
| ✅ #1213 | success | Success: No regression failure |
| ✅ #1224 | success | Success: No regression failure |
| ✅ #1225 | success | Success: No regression failure |
| ✅ #1231 | success | Success: No regression failure |
| ✅ #1240 | success | Success: No regression failure |
| ❗ #1241 | fail | Failure: Regression failure. |
| ✅ #1242 | success | Success: No regression failure |
| ✅ #1243 | success | Success: No regression failure |
| ✅ #1254 | success | Success: No regression failure |
| ❗ #1258 | fail | Failure: Regression failure. |
| ✅ #1261 | success | Success: No regression failure |
| ❗ #1265 | fail | Failure: Regression failure. |
| ❗ #1290 | fail | Failure: Regression failure. |
| ❗ #1332 | fail | Failure: Regression failure. |
| ❗ #1348 | fail | Failure: Regression failure. |
| ✅ #1357 | success | Success: No regression failure |
| ❗ #1360 | fail | Failure: Regression failure. |

### Closed failures (details)

| Issue | Conclusion | Details |
| --- | --- | --- |
| #671 https://github.com/nunit/nunit3-vs-adapter/issues/671 | Failure: Regression failure. | Setup failed for test fixture Issue671.Tests SetUp : System.Exception : oops Deep Down StackTrace: --SetUp at Issue671.SomeWhereDeepDown.WhatDoWeDoHere() in C:\repos\nunit\nunit3-vs-adapter.issues\Issue671\UnitTest1.cs:line 44 at Issue67... |
| #691 https://github.com/nunit/nunit3-vs-adapter/issues/691 | Failure: Regression failure. | [nunit] [other] |
| #744 https://github.com/nunit/nunit3-vs-adapter/issues/744 | Failure: Regression failure. | [nunit] [other] |
| #765 https://github.com/nunit/nunit3-vs-adapter/issues/765 | Failure: Regression failure. | [nunit] [other] |
| #768 https://github.com/nunit/nunit3-vs-adapter/issues/768 | Failure: Regression failure. | [nunit] [other] |
| #833 https://github.com/nunit/nunit3-vs-adapter/issues/833 | fail | No csproj found |
| #846 https://github.com/nunit/nunit3-vs-adapter/issues/846 | Failure: Regression failure. | [nunit] [other] |
| #873 https://github.com/nunit/nunit3-vs-adapter/issues/873 | Failure: Regression failure. | [nunit] [other] |
| #919 https://github.com/nunit/nunit3-vs-adapter/issues/919 | Failure: Regression failure. | [nunit] [other] |
| #972 https://github.com/nunit/nunit3-vs-adapter/issues/972 | Failure: Regression failure. | [nunit] [other] |
| #1039 https://github.com/nunit/nunit3-vs-adapter/issues/1039 | Failure: Regression failure. | [nunit] [other] |
| #1041 https://github.com/nunit/nunit3-vs-adapter/issues/1041 | Failure: Regression failure. | [nunit] [other] |
| #1044 https://github.com/nunit/nunit3-vs-adapter/issues/1044 | Failure: Regression failure. | [nunit] [other] |
| #1096 https://github.com/nunit/nunit3-vs-adapter/issues/1096 | Failure: Regression failure. | [nunit] [other] |
| #1127 https://github.com/nunit/nunit3-vs-adapter/issues/1127 | Failure: Regression failure. | [xUnit.net 00:00:02.61] Issue1127.X.Tests.Test1(a: " ", b: False, c: True) [FAIL] [xUnit.net 00:00:02.63] Issue1127.X.Tests.Test1(a: "", b: True, c: False) [FAIL] [xUnit.net 00:00:02.66] Issue1127.X.Tests.Test1(a: " ", b: True, c: False)... |
| #1132 https://github.com/nunit/nunit3-vs-adapter/issues/1132 | Failure: Regression failure. | [nunit] Unable to process the project `C:\repos\nunit\nunit3-vs-adapter.issues\Issue1132\NUnitAdapterIssue.sln. Are you sure this is a valid .NET Core or .NET Standard project type? Here is the full error message returned from the Micros... |
| #1146 https://github.com/nunit/nunit3-vs-adapter/issues/1146 | Failure: Regression failure. | [nunit] [other] |
| #1152 https://github.com/nunit/nunit3-vs-adapter/issues/1152 | Failure: Regression failure. | [nunit] [other] |
| #1241 https://github.com/nunit/nunit3-vs-adapter/issues/1241 | Failure: Regression failure. | Specifying a solution for 'dotnet test' should be via '--solution'. |
| #1258 https://github.com/nunit/nunit3-vs-adapter/issues/1258 | Failure: Regression failure. | [nunit] [other] |
| #1265 https://github.com/nunit/nunit3-vs-adapter/issues/1265 | Failure: Regression failure. | [nunit] [other] |
| #1290 https://github.com/nunit/nunit3-vs-adapter/issues/1290 | Failure: Regression failure. | TearDown failed for test fixture Issue1290.Tests TearDown : System.NullReferenceException : Object reference not set to an instance of an object. StackTrace: --TearDown at Issue1290.Tests.OneTimeTearDown() in C:\repos\nunit\nunit3-vs-ada... |
| #1332 https://github.com/nunit/nunit3-vs-adapter/issues/1332 | Failure: Regression failure. | Specifying a solution for 'dotnet test' should be via '--solution'. |
| #1348 https://github.com/nunit/nunit3-vs-adapter/issues/1348 | Failure: Regression failure. | Specifying a solution for 'dotnet test' should be via '--solution'. |
| #1360 https://github.com/nunit/nunit3-vs-adapter/issues/1360 | Failure: Regression failure. | [nunit] [other] |

## Open issues

- Total: 21, Success: 14, Fail: 7

### Succeeded (candidates to close)

| Issue | Conclusion |
| --- | --- |
| #266 https://github.com/nunit/nunit3-vs-adapter/issues/266 | Open issue, but test succeeds. |
| #667 https://github.com/nunit/nunit3-vs-adapter/issues/667 | Open issue, but test succeeds. |
| #718 https://github.com/nunit/nunit3-vs-adapter/issues/718 | Open issue, but test succeeds. |
| #729 https://github.com/nunit/nunit3-vs-adapter/issues/729 | Open issue, but test succeeds. |
| #775 https://github.com/nunit/nunit3-vs-adapter/issues/775 | Open issue, but test succeeds. |
| #894 https://github.com/nunit/nunit3-vs-adapter/issues/894 | Open issue, but test succeeds. |
| #935 https://github.com/nunit/nunit3-vs-adapter/issues/935 | Open issue, but test succeeds. |
| #954 https://github.com/nunit/nunit3-vs-adapter/issues/954 | Open issue, but test succeeds. |
| #1015 https://github.com/nunit/nunit3-vs-adapter/issues/1015 | Open issue, but test succeeds. |
| #1097 https://github.com/nunit/nunit3-vs-adapter/issues/1097 | Open issue, but test succeeds. |
| #1167 https://github.com/nunit/nunit3-vs-adapter/issues/1167 | Open issue, but test succeeds. |
| #1264 https://github.com/nunit/nunit3-vs-adapter/issues/1264 | Open issue, but test succeeds. |
| #1367 https://github.com/nunit/nunit3-vs-adapter/issues/1367 | Open issue, but test succeeds. |
| #1371 https://github.com/nunit/nunit3-vs-adapter/issues/1371 | Open issue, but test succeeds. |

### Failing (confirmed repros)

| Issue | Conclusion | Details |
| --- | --- | --- |
| #782 https://github.com/nunit/nunit3-vs-adapter/issues/782 | Failure: Open issue, repro fails. | [nunit] Unable to process the project `C:\repos\nunit\nunit3-vs-adapter.issues\Issue782\UnitTestBug\UnitTestBug.sln. Are you sure this is a valid .NET Core or .NET Standard project type? Here is the full error message returned from the M... |
| #871 https://github.com/nunit/nunit3-vs-adapter/issues/871 | Failure: Open issue, repro fails. | [nunit] [other] |
| #1336 https://github.com/nunit/nunit3-vs-adapter/issues/1336 | Failure: Open issue, repro fails. | Specifying a solution for 'dotnet test' should be via '--solution'. |
| #1349 https://github.com/nunit/nunit3-vs-adapter/issues/1349 | Failure: Open issue, repro fails. | Specifying a solution for 'dotnet test' should be via '--solution'. |
| #1351 https://github.com/nunit/nunit3-vs-adapter/issues/1351 | Failure: Open issue, repro fails. | Specifying a solution for 'dotnet test' should be via '--solution'. |
| #1374 https://github.com/nunit/nunit3-vs-adapter/issues/1374 | Failure: Open issue, repro fails. | The active test run was aborted. Reason: Test host process crashed : Process terminated. Assertion Failed false at Issue1374.UnitTest1.<>c.<TraceAssert_WithFalseCondition_ShouldThrowException>b__1_0() in C:\repos\nunit\nunit3-vs-adapter.... |
| #1375 https://github.com/nunit/nunit3-vs-adapter/issues/1375 | Failure: Open issue, repro fails. | [nunit] [other] |
