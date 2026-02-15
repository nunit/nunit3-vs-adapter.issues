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
| #671 https://github.com/nunit/nunit3-vs-adapter/issues/671 | Failure: Regression failure. | Setup failed for test fixture Issue671.Tests SetUp : System.Exception : oops Deep Down StackTrace:<br>--SetUp at Issue671.SomeWhereDeepDown.WhatDoWeDoHere() in C:\repos\nunit\nunit3-vs-<br>adapter.issues\Issue671\UnitTest1.cs:line 44 at Issue671.Tests.OneTimeSetup() in<br>C:\repos\nunit\nunit3-vs-adapter.issues\Issue671\UnitTest1.cs:line 12 at<br>System.Reflection.MethodBaseInvoker.InterpretedInvoke_Method(Object obj, IntPtr* args) at<br>System.Reflection.MethodBaseInvoker.InvokeWithNoArgs(Object obj, BindingFlags invokeAttr) |
| #691 https://github.com/nunit/nunit3-vs-adapter/issues/691 | Failure: Regression failure. | Determining projects to restore... Restored C:\repos\nunit\nunit3-vs-<br>adapter.issues\Issue691\Issue691\Issue691\Issue691.csproj (in 457 ms). C:\repos\nunit\nunit3-vs-<br>adapter.issues\Issue691\Issue691\Issue691\UnitTest1.cs(31,20): error CS0117: 'Assert' does not<br>contain a definition for 'AreEqual' [C:\repos\nunit\nunit3-vs-<br>adapter.issues\Issue691\Issue691\Issue691\Issue691.csproj] |
| #744 https://github.com/nunit/nunit3-vs-adapter/issues/744 | Failure: Regression failure. | Determining projects to restore... Restored C:\repos\nunit\nunit3-vs-<br>adapter.issues\Issue744\Issue744TestName\Issue744TestName.csproj (in 542 ms). C:\Users\TerjeSandstro<br>m\.nuget\packages\system.runtime.compilerservices.unsafe\6.1.2\buildTransitive\net461\System.Runtime<br>.CompilerServices.Unsafe.targets(4,5): warning : System.Runtime.CompilerServices.Unsafe 6.1.2<br>doesn't support net461 and has not been tested with it. Consider upgrading your TargetFramework to<br>net462 or later. You may also set<br><SuppressTfmSupportBuildWarnings>true</SuppressTfmSupportBuildWarnings> in the project file to<br>ignore this warning and attempt to run in this unsupported configuration at your own risk.<br>[C:\repos\nunit\nunit3-vs-adapter.issues\Issue744\Issue744TestName\Issue744TestName.csproj]<br>C:\Users\TerjeSandstrom... |
| #765 https://github.com/nunit/nunit3-vs-adapter/issues/765 | Failure: Regression failure. | Determining projects to restore... Restored C:\repos\nunit\nunit3-vs-adapter.issues\Issue765\NSubsti<br>tuteSkippedTestDemo\NSubstituteSkippedTestDemo\NSubstituteSkippedTestDemo.csproj (in 523 ms).<br>C:\repos\nunit\nunit3-vs-<br>adapter.issues\Issue765\NSubstituteSkippedTestDemo\NSubstituteSkippedTestDemo\Tests.cs(68,20): error<br>CS0117: 'Assert' does not contain a definition for 'AreEqual' [C:\repos\nunit\nunit3-vs-adapter.issu<br>es\Issue765\NSubstituteSkippedTestDemo\NSubstituteSkippedTestDemo\NSubstituteSkippedTestDemo.csproj]<br>C:\repos\nunit\nunit3-vs-<br>adapter.issues\Issue765\NSubstituteSkippedTestDemo\NSubstituteSkippedTestDemo\Tests.cs(38,20): error<br>CS0117: 'Assert' does not contain a definition for 'AreEqual' [C:\repos\nunit\nunit3-vs-<br>adapter.issues\Issue765\NSubstituteSkippedTestDemo\NSubstitute... |
| #768 https://github.com/nunit/nunit3-vs-adapter/issues/768 | Failure: Regression failure. | Determining projects to restore... Restored C:\repos\nunit\nunit3-vs-<br>adapter.issues\Issue768\NRENUnitEventListner_v2\NRENUnitEventListner_v2.csproj (in 536 ms).<br>C:\repos\nunit\nunit3-vs-adapter.issues\Issue768\NRENUnitEventListner_v2\TestNRE.cs(11,20): error<br>CS0117: 'Assert' does not contain a definition for 'True' [C:\repos\nunit\nunit3-vs-<br>adapter.issues\Issue768\NRENUnitEventListner_v2\NRENUnitEventListner_v2.csproj] |
| #833 https://github.com/nunit/nunit3-vs-adapter/issues/833 | fail | No csproj found |
| #846 https://github.com/nunit/nunit3-vs-adapter/issues/846 | Failure: Regression failure. | Determining projects to restore... Restored C:\repos\nunit\nunit3-vs-<br>adapter.issues\Issue846\Issue846\Issue846.csproj (in 357 ms). Issue846 -> C:\repos\nunit\nunit3-vs-<br>adapter.issues\Issue846\Issue846\bin\Debug\net10.0\Issue846.dll Test run for<br>C:\repos\nunit\nunit3-vs-adapter.issues\Issue846\Issue846\bin\Debug\net10.0\Issue846.dll<br>(.NETCoreApp,Version=v10.0) VSTest version 18.0.1 (x64) Starting test execution, please wait... A<br>total of 1 test files matched the specified pattern. Failed TestCulture("sv-SE") [72 ms] Error<br>Message: Has not been sorted at all Assert.That(result, Is.EqualTo(input).AsCollection) Expected and<br>actual are both <System.Collections.Generic.List`1[System.String]> with 5 elements Values differ at<br>index [0] String lengths are both 3. Strings differ at index 0. Expec... |
| #873 https://github.com/nunit/nunit3-vs-adapter/issues/873 | Failure: Regression failure. | Determining projects to restore... Restored C:\repos\nunit\nunit3-vs-<br>adapter.issues\Issue873\Issue873.csproj (in 533 ms). C:\repos\nunit\nunit3-vs-<br>adapter.issues\Issue873\UnitTest1.cs(16,16): error CS0117: 'Assert' does not contain a definition<br>for 'AreEqual' [C:\repos\nunit\nunit3-vs-adapter.issues\Issue873\Issue873.csproj] |
| #919 https://github.com/nunit/nunit3-vs-adapter/issues/919 | Failure: Regression failure. | Determining projects to restore... Restored C:\repos\nunit\nunit3-vs-<br>adapter.issues\Issue919\Issue919.csproj (in 530 ms). C:\repos\nunit\nunit3-vs-<br>adapter.issues\Issue919\UnitTest1.cs(23,2): error CS1513: } expected [C:\repos\nunit\nunit3-vs-<br>adapter.issues\Issue919\Issue919.csproj] |
| #972 https://github.com/nunit/nunit3-vs-adapter/issues/972 | Failure: Regression failure. | Determining projects to restore... C:\Program Files\dotnet\sdk\10.0.100\Sdks\Microsoft.NET.Sdk\targe<br>ts\Microsoft.NET.EolTargetFrameworks.targets(32,5): warning NETSDK1138: The target framework<br>'net6.0' is out of support and will not receive security updates in the future. Please refer to<br>https://aka.ms/dotnet-core-support for more information about the support policy.<br>[C:\repos\nunit\nunit3-vs-adapter.issues\Issue972\Issue_972\Issue_972.API\Issue_972.API.csproj]<br>C:\repos\nunit\nunit3-vs-adapter.issues\Issue972\Issue_972\Issue_972\Issue_972.csproj : warning<br>NU1902: Package 'Azure.Identity' 1.10.3 has a known moderate severity vulnerability,<br>https://github.com/advisories/GHSA-m5vv-6r4h-3vj9 C:\repos\nunit\nunit3-vs-<br>adapter.issues\Issue972\Issue_972\Issue_972\Issue_972.csproj : warning NU1... |
| #1039 https://github.com/nunit/nunit3-vs-adapter/issues/1039 | Failure: Regression failure. | Determining projects to restore... Restored C:\repos\nunit\nunit3-vs-<br>adapter.issues\Issue1039\LibraryToTest.Tests\LibraryToTest.Tests.csproj (in 441 ms).<br>C:\repos\nunit\nunit3-vs-adapter.issues\Issue1039\LibraryToTest.Tests\LibraryTests.cs(24,20): error<br>CS0117: 'Assert' does not contain a definition for 'IsTrue' [C:\repos\nunit\nunit3-vs-<br>adapter.issues\Issue1039\LibraryToTest.Tests\LibraryToTest.Tests.csproj] C:\repos\nunit\nunit3-vs-<br>adapter.issues\Issue1039\LibraryToTest.Tests\LibraryTests.cs(13,13): warning NUnit1033: The Write<br>methods are wrappers on TestContext.Out<br>(https://github.com/nunit/nunit.analyzers/tree/master/documentation/NUnit1033.md)<br>[C:\repos\nunit\nunit3-vs-adapter.issues\Issue1039\LibraryToTest.Tests\LibraryToTest.Tests.csproj]<br>C:\repos\nunit\nunit3-vs-adapter.issues\... |
| #1041 https://github.com/nunit/nunit3-vs-adapter/issues/1041 | Failure: Regression failure. | Determining projects to restore... Restored C:\repos\nunit\nunit3-vs-<br>adapter.issues\Issue1041\Issue1041.csproj (in 505 ms). C:\repos\nunit\nunit3-vs-<br>adapter.issues\Issue1041\UnitTest1.cs(11,21): error NUnit1029: The TestCaseSource provides '>0'<br>parameter(s), but the Test method expects '0' parameter(s)<br>(https://github.com/nunit/nunit.analyzers/tree/master/documentation/NUnit1029.md)<br>[C:\repos\nunit\nunit3-vs-adapter.issues\Issue1041\Issue1041.csproj] C:\repos\nunit\nunit3-vs-<br>adapter.issues\Issue1041\UnitTest1.cs(5,21): error NUnit1029: The TestCaseSource provides '>0'<br>parameter(s), but the Test method expects '0' parameter(s)<br>(https://github.com/nunit/nunit.analyzers/tree/master/documentation/NUnit1029.md)<br>[C:\repos\nunit\nunit3-vs-adapter.issues\Issue1041\Issue1041.csproj] C:\repos\nun... |
| #1044 https://github.com/nunit/nunit3-vs-adapter/issues/1044 | Failure: Regression failure. | Determining projects to restore... Restored C:\repos\nunit\nunit3-vs-<br>adapter.issues\Issue1044\Issue1044.csproj (in 459 ms). C:\repos\nunit\nunit3-vs-<br>adapter.issues\Issue1044\UnitTest1.cs(12,9): warning NUnit1033: The Write methods are wrappers on<br>TestContext.Out (https://github.com/nunit/nunit.analyzers/tree/master/documentation/NUnit1033.md)<br>[C:\repos\nunit\nunit3-vs-adapter.issues\Issue1044\Issue1044.csproj] C:\repos\nunit\nunit3-vs-<br>adapter.issues\Issue1044\UnitTest1.cs(17,21): error NUnit1029: The TestCaseSource provides '>0'<br>parameter(s), but the Test method expects '0' parameter(s)<br>(https://github.com/nunit/nunit.analyzers/tree/master/documentation/NUnit1029.md)<br>[C:\repos\nunit\nunit3-vs-adapter.issues\Issue1044\Issue1044.csproj] C:\repos\nunit\nunit3-vs-<br>adapter.issues\Issue1044\Un... |
| #1096 https://github.com/nunit/nunit3-vs-adapter/issues/1096 | Failure: Regression failure. | Determining projects to restore... Restored C:\repos\nunit\nunit3-vs-<br>adapter.issues\Issue1096\NUnitParseIssue.csproj (in 457 ms). C:\repos\nunit\nunit3-vs-<br>adapter.issues\Issue1096\S0\Issue.cs(11,20): error CS0117: 'Assert' does not contain a definition<br>for 'AreEqual' [C:\repos\nunit\nunit3-vs-adapter.issues\Issue1096\NUnitParseIssue.csproj] |
| #1127 https://github.com/nunit/nunit3-vs-adapter/issues/1127 | Failure: Regression failure. | [xUnit.net 00:00:02.61] Issue1127.X.Tests.Test1(a: " ", b: False, c: True) [FAIL] [xUnit.net<br>00:00:02.63] Issue1127.X.Tests.Test1(a: "", b: True, c: False) [FAIL] [xUnit.net 00:00:02.66]<br>Issue1127.X.Tests.Test1(a: " ", b: True, c: False) [FAIL] [xUnit.net 00:00:02.67]<br>Issue1127.X.Tests.Test1(a: "", b: False, c: True) [FAIL] |
| #1132 https://github.com/nunit/nunit3-vs-adapter/issues/1132 | Failure: Regression failure. | Determining projects to restore... C:\Program Files\dotnet\sdk\10.0.100\NuGet.targets(196,5): error<br>: Operation is not valid due to the current state of the object. [C:\repos\nunit\nunit3-vs-<br>adapter.issues\Issue1132\NUnitAdapterIssue.sln] |
| #1146 https://github.com/nunit/nunit3-vs-adapter/issues/1146 | Failure: Regression failure. | Determining projects to restore... Restored C:\repos\nunit\nunit3-vs-<br>adapter.issues\Issue1146\NUnitFilterSample\src\NUnitFilterSample.csproj (in 523 ms).<br>NUnitFilterSample -> C:\repos\nunit\nunit3-vs-<br>adapter.issues\Issue1146\NUnitFilterSample\src\bin\Debug\net10.0\NUnitFilterSample.dll Test run for<br>C:\repos\nunit\nunit3-vs-<br>adapter.issues\Issue1146\NUnitFilterSample\src\bin\Debug\net10.0\NUnitFilterSample.dll<br>(.NETCoreApp,Version=v10.0) VSTest version 18.0.1 (x64) Starting test execution, please wait... A<br>total of 1 test files matched the specified pattern. Failed Test('"') [57 ms] Error Message: in<br>sample category Stack Trace: at NUnitFilterSample.FixtureWithCategory.Test(Char c) in<br>C:\repos\nunit\nunit3-vs-adapter.issues\Issue1146\NUnitFilterSample\src\FixtureWithCategory.cs:line<br>16 1)... |
| #1152 https://github.com/nunit/nunit3-vs-adapter/issues/1152 | Failure: Regression failure. | Determining projects to restore... Restored C:\repos\nunit\nunit3-vs-<br>adapter.issues\Issue1152\1-nunit-runner\nunit-runner.csproj (in 570 ms). nunit-runner -><br>C:\repos\nunit\nunit3-vs-adapter.issues\Issue1152\1-nunit-runner\bin\Debug\net10.0\nunit-runner.dll<br>EnableNUnitRunner is set to: true EnableNUnitRunner is set to: true C:\Users\TerjeSandstrom\.nuget\p<br>ackages\microsoft.testing.platform.msbuild\2.0.2\buildMultiTargeting\Microsoft.Testing.Platform.MSBu<br>ild.targets(263,5): error : Testing with VSTest target is no longer supported by<br>Microsoft.Testing.Platform on .NET 10 SDK and later. If you use dotnet test, you should opt-in to<br>the new dotnet test experience. For more information, see https://aka.ms/dotnet-test-mtp-error<br>[C:\repos\nunit\nunit3-vs-adapter.issues\Issue1152\1-nunit-runner... |
| #1241 https://github.com/nunit/nunit3-vs-adapter/issues/1241 | Failure: Regression failure. | Specifying a solution for 'dotnet test' should be via '--solution'. |
| #1258 https://github.com/nunit/nunit3-vs-adapter/issues/1258 | Failure: Regression failure. | Determining projects to restore... Restored C:\repos\nunit\nunit3-vs-<br>adapter.issues\Issue1258\Issue1258.csproj (in 618 ms). C:\Users\TerjeSandstrom\.nuget\packages\micro<br>soft.testing.platform.msbuild\2.0.2\buildMultiTargeting\Microsoft.Testing.Platform.MSBuild.targets(2<br>63,5): error : Testing with VSTest target is no longer supported by Microsoft.Testing.Platform on<br>.NET 10 SDK and later. If you use dotnet test, you should opt-in to the new dotnet test experience.<br>For more information, see https://aka.ms/dotnet-test-mtp-error [C:\repos\nunit\nunit3-vs-<br>adapter.issues\Issue1258\Issue1258.csproj] |
| #1265 https://github.com/nunit/nunit3-vs-adapter/issues/1265 | Failure: Regression failure. | Determining projects to restore... Restored C:\repos\nunit\nunit3-vs-<br>adapter.issues\Issue1265\mtp\mtp.csproj (in 610 ms). C:\Users\TerjeSandstrom\.nuget\packages\microso<br>ft.testing.platform.msbuild\2.0.2\buildMultiTargeting\Microsoft.Testing.Platform.MSBuild.targets(263<br>,5): error : Testing with VSTest target is no longer supported by Microsoft.Testing.Platform on .NET<br>10 SDK and later. If you use dotnet test, you should opt-in to the new dotnet test experience. For<br>more information, see https://aka.ms/dotnet-test-mtp-error [C:\repos\nunit\nunit3-vs-<br>adapter.issues\Issue1265\mtp\mtp.csproj] |
| #1290 https://github.com/nunit/nunit3-vs-adapter/issues/1290 | Failure: Regression failure. | TearDown failed for test fixture Issue1290.Tests TearDown : System.NullReferenceException : Object<br>reference not set to an instance of an object. StackTrace: --TearDown at<br>Issue1290.Tests.OneTimeTearDown() in C:\repos\nunit\nunit3-vs-<br>adapter.issues\Issue1290\UnitTest1.cs:line 27 at<br>System.Reflection.MethodBaseInvoker.InterpretedInvoke_Method(Object obj, IntPtr* args) at<br>System.Reflection.MethodBaseInvoker.InvokeWithNoArgs(Object obj, BindingFlags invokeAttr) |
| #1332 https://github.com/nunit/nunit3-vs-adapter/issues/1332 | Failure: Regression failure. | Specifying a solution for 'dotnet test' should be via '--solution'. |
| #1348 https://github.com/nunit/nunit3-vs-adapter/issues/1348 | Failure: Regression failure. | Specifying a solution for 'dotnet test' should be via '--solution'. |
| #1360 https://github.com/nunit/nunit3-vs-adapter/issues/1360 | Failure: Regression failure. | Determining projects to restore... Restored C:\repos\nunit\nunit3-vs-<br>adapter.issues\Issue1360\Issue1360.csproj (in 526 ms). C:\Users\TerjeSandstrom\.nuget\packages\micro<br>soft.testing.platform.msbuild\2.0.2\buildMultiTargeting\Microsoft.Testing.Platform.MSBuild.targets(2<br>63,5): error : Testing with VSTest target is no longer supported by Microsoft.Testing.Platform on<br>.NET 10 SDK and later. If you use dotnet test, you should opt-in to the new dotnet test experience.<br>For more information, see https://aka.ms/dotnet-test-mtp-error [C:\repos\nunit\nunit3-vs-<br>adapter.issues\Issue1360\Issue1360.csproj] |

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
| #782 https://github.com/nunit/nunit3-vs-adapter/issues/782 | Failure: Open issue, repro fails. | Determining projects to restore... Restored C:\repos\nunit\nunit3-vs-<br>adapter.issues\Issue782\UnitTestBug\UnitTestBug.csproj (in 210 ms). C:\repos\nunit\nunit3-vs-<br>adapter.issues\Issue782\UnitTestBugTests\UnitTestBugTests.csproj : error NU1201: Project UnitTestBug<br>is not compatible with netcoreapp3.1 (.NETCoreApp,Version=v3.1). Project UnitTestBug supports:<br>net10.0 (.NETCoreApp,Version=v10.0) [C:\repos\nunit\nunit3-vs-<br>adapter.issues\Issue782\UnitTestBug\UnitTestBug.sln] Failed to restore C:\repos\nunit\nunit3-vs-<br>adapter.issues\Issue782\UnitTestBugTests\UnitTestBugTests.csproj (in 683 ms). |
| #871 https://github.com/nunit/nunit3-vs-adapter/issues/871 | Failure: Open issue, repro fails. | Determining projects to restore... Restored C:\repos\nunit\nunit3-vs-<br>adapter.issues\Issue871\Issue871.csproj (in 502 ms). Issue871 -> C:\repos\nunit\nunit3-vs-<br>adapter.issues\Issue871\bin\Debug\net10.0\Issue871.dll Test run for C:\repos\nunit\nunit3-vs-<br>adapter.issues\Issue871\bin\Debug\net10.0\Issue871.dll (.NETCoreApp,Version=v10.0) VSTest version<br>18.0.1 (x64) Starting test execution, please wait... A total of 1 test files matched the specified<br>pattern. Failed NUnitTest [75 ms] Error Message: Test failed! Stack Trace: at<br>Issue871.Tests.TearDownTest() in C:\repos\nunit\nunit3-vs-adapter.issues\Issue871\UnitTest1.cs:line<br>23 1) at Issue871.Tests.TearDownTest() in C:\repos\nunit\nunit3-vs-<br>adapter.issues\Issue871\UnitTest1.cs:line 23 Failed! - Failed: 1, Passed: 0, Skipped: 0, Total: 1,<br>Dura... |
| #1336 https://github.com/nunit/nunit3-vs-adapter/issues/1336 | Failure: Open issue, repro fails. | Specifying a solution for 'dotnet test' should be via '--solution'. |
| #1349 https://github.com/nunit/nunit3-vs-adapter/issues/1349 | Failure: Open issue, repro fails. | Specifying a solution for 'dotnet test' should be via '--solution'. |
| #1351 https://github.com/nunit/nunit3-vs-adapter/issues/1351 | Failure: Open issue, repro fails. | Specifying a solution for 'dotnet test' should be via '--solution'. |
| #1374 https://github.com/nunit/nunit3-vs-adapter/issues/1374 | Failure: Open issue, repro fails. | The active test run was aborted. Reason: Test host process crashed : Process terminated. Assertion<br>Failed false at<br>Issue1374.UnitTest1.<>c.<TraceAssert_WithFalseCondition_ShouldThrowException>b__1_0() in<br>C:\repos\nunit\nunit3-vs-adapter.issues\Issue1374\UnitTest1.cs:line 21 at<br>NUnit.Framework.Assert.Throws(IResolveConstraint expression, TestDelegate code, String message,<br>Object[] args) at NUnit.Framework.Assert.Throws(IResolveConstraint expression, TestDelegate code) at<br>NUnit.Framework.Assert.Catch(TestDelegate code) at<br>Issue1374.UnitTest1.TraceAssert_WithFalseCondition_ShouldThrowException() in<br>C:\repos\nunit\nunit3-vs-adapter.issues\Issue1374\UnitTest1.cs:line 21 at<br>System.Reflection.MethodBaseInvoker.InterpretedInvoke_Method(Object obj, IntPtr* args) at<br>System.Reflection.MethodBaseIn... |
| #1375 https://github.com/nunit/nunit3-vs-adapter/issues/1375 | Failure: Open issue, repro fails. | Determining projects to restore... Restored C:\repos\nunit\nunit3-vs-<br>adapter.issues\Issue1375\Issue1375.csproj (in 543 ms). Issue1375 -> C:\repos\nunit\nunit3-vs-<br>adapter.issues\Issue1375\bin\Debug\net10.0\Issue1375.dll Test run for C:\repos\nunit\nunit3-vs-<br>adapter.issues\Issue1375\bin\Debug\net10.0\Issue1375.dll (.NETCoreApp,Version=v10.0) VSTest version<br>18.0.1 (x64) Starting test execution, please wait... A total of 1 test files matched the specified<br>pattern. Failed TestCSScript [2 s] Error Message:<br>Microsoft.CSharp.RuntimeBinder.RuntimeBinderException : Cannot implicitly convert type<br>'TestCase.Point' to 'TestCase.Point' Stack Trace: at CallSite.Target(Closure, CallSite, Object) at<br>System.Dynamic.UpdateDelegates.UpdateAndExecute1[T0,TRet](CallSite site, T0 arg0) at<br>TestCase.DynamicEval... |
