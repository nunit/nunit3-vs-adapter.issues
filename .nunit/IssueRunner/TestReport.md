# Test Report

## Summary

- Regression tests: total 104, success 85, fail 2
- Open issues: total 16, success 13, fail 1

## What we are testing

- Repository: https://github.com/nunit/nunit3-vs-adapter

Package versions under test:

- NUnit: 4.4.0
- NUnit.Analyzers: 4.3.0
- NUnit3TestAdapter: 6.1.0

## Regression tests (closed issues)

- Total: 104, Success: 85, Fail: 2

| Issue | Title | Test | Conclusion |
| --- | --- | --- | --- |
| ✅ [#1](https://github.com/nunit/nunit3-vs-adapter/issues/1) | Upgrade vsix file to the 2.0 format | success | Success: No regression failure (Pass 1) |
| ✅ [#228](https://github.com/nunit/nunit3-vs-adapter/issues/228) | Tests inherited from Generic test fixture | success | Success: No regression failure (Pass 2) |
| ✅ [#241](https://github.com/nunit/nunit3-vs-adapter/issues/241) | Nunit test runner picking up ignored tests (ish) in tfs | success | Success: No regression failure (Pass 0) |
| ❗ [#261](https://github.com/nunit/nunit3-vs-adapter/issues/261) | NUnit3TestDiscoverer unable to load nunit.engine | not run | Failure: Regression failure. |
| ✅ [#343](https://github.com/nunit/nunit3-vs-adapter/issues/343) | Console.WriteLine() does not write to console when running`dotnet test` | success | Success: No regression failure (Pass 1) |
| ✅ [#364](https://github.com/nunit/nunit3-vs-adapter/issues/364) | Inherited Testcase does not open source file on double click | success | Success: No regression failure (Pass 2) |
| ✅ [#425](https://github.com/nunit/nunit3-vs-adapter/issues/425) | Run only specific tests when using dotnet test? | success | Success: No regression failure (Pass 6) |
| ✅ [#484](https://github.com/nunit/nunit3-vs-adapter/issues/484) | Cannot run an individual test whose TestCase parameter contains characters from the range [U+0001..U+001F] | success | Success: No regression failure (Pass 7) |
| ✅ [#497](https://github.com/nunit/nunit3-vs-adapter/issues/497) | dotnet test with category filter is slow with a lot of tests | success | Success: No regression failure (Pass 20002) |
| ✅ [#516](https://github.com/nunit/nunit3-vs-adapter/issues/516) | ArgumentException when whitespace sent to logger | success | Success: No regression failure (Pass 7) |
| ✅ [#530](https://github.com/nunit/nunit3-vs-adapter/issues/530) | Adapter discovers explicit TestCase without skip details | success | Success: No regression failure (Pass 0) |
| ✅ [#535](https://github.com/nunit/nunit3-vs-adapter/issues/535) | Test list not refreshing if test project is not rebuilt | success | Success: No regression failure |
| ❗ [#545](https://github.com/nunit/nunit3-vs-adapter/issues/545) | Setting [TestCaseSource] to [Explicit] makes other tests in fixture explicit. | not run | Failure: Regression failure. |
| ✅ [#618](https://github.com/nunit/nunit3-vs-adapter/issues/618) | Description from TestContext.AddTestAttachment does not displays in TEst Output window | success | Success: No regression failure (Pass 1) |
| ✅ [#640](https://github.com/nunit/nunit3-vs-adapter/issues/640) | Allow use of FullName as the DisplayName for Converted Tests | success | Success: No regression failure (Pass 9) |
| ❗ [#658](https://github.com/nunit/nunit3-vs-adapter/issues/658) | Explicit tests are automatically run in Visual Studio 2019 | fail | Failure: Regression failure. (Pass 1, Fail 2) |
| ❗ [#671](https://github.com/nunit/nunit3-vs-adapter/issues/671) | Exception in OneTimeSetUp has no stack trace | fail | Failure: Regression failure. (Pass 0, Fail 2) |
| ✅ [#687](https://github.com/nunit/nunit3-vs-adapter/issues/687) | Test discovery fails when using TestCaseSource in RIDER and VS2017 with Resharper Unit Tests | success | Success: No regression failure |
| ✅ [#691](https://github.com/nunit/nunit3-vs-adapter/issues/691) | Incorrect format for TestCaseFilter Error: Missing '(' | success | Success: No regression failure (Pass 12) |
| ✅ [#709](https://github.com/nunit/nunit3-vs-adapter/issues/709) | TestCases not grouped using TestCaseSource "SetName" property in v3.16.1 | success | Success: No regression failure (Pass 5) |
| ✅ [#711](https://github.com/nunit/nunit3-vs-adapter/issues/711) | Incorrect format for TestCaseFilter Error: Invalid Condition - Using = in SetName | success | Success: No regression failure (Pass 62) |
| ✅ [#735](https://github.com/nunit/nunit3-vs-adapter/issues/735) | TestCaseData: Missing Feature "Jump to File" | success | Success: No regression failure (Pass 4) |
| ✅ [#740](https://github.com/nunit/nunit3-vs-adapter/issues/740) | Allow Warnings to be mapped to any outcome | success | Success: No regression failure (Pass 2) |
| ✅ [#744](https://github.com/nunit/nunit3-vs-adapter/issues/744) | Setting TestCaseAttribute.TestName in .NET Core project causes not running tests | success | Success: No regression failure (Pass 3) |
| ✅ [#758](https://github.com/nunit/nunit3-vs-adapter/issues/758) | Feature Request: Adapter to support NonTestAssemblyAttribute | success | Success: No regression failure (Pass 1) |
| ✅ [#760](https://github.com/nunit/nunit3-vs-adapter/issues/760) | Syntax for auto-removable test suffix | success | Success: No regression failure (Pass 3) |
| ✅ [#765](https://github.com/nunit/nunit3-vs-adapter/issues/765) | TestCaseSource + NSubstitute + Async = Skipped Tests in VS 2019 | success | Success: No regression failure (Pass 4) |
| ✅ [#768](https://github.com/nunit/nunit3-vs-adapter/issues/768) | NRE at NUnit.VisualStudio.TestAdapter.NUnitEventListener.TestFinished(INUnitTestEventTestCase resultNode) When Referencing NUnit 3.11 | success | Success: No regression failure (Pass 1) |
| ✅ [#770](https://github.com/nunit/nunit3-vs-adapter/issues/770) | "Not a TestFixture, but TestSuite" error when using un-namespaced SetupFixture | success | Success: No regression failure (Pass 1) |
| ✅ [#774](https://github.com/nunit/nunit3-vs-adapter/issues/774) | Tests not executed if Console.WriteLine() is used | success | Success: No regression failure (Pass 3) |
| ✅ [#775](https://github.com/nunit/nunit3-vs-adapter/issues/775) | When group of tests contains sub-group, VS 2019 Test Explorer shows Not Run, while all tests pass separately | success | Success: No regression failure (Pass 3) |
| ✅ [#779](https://github.com/nunit/nunit3-vs-adapter/issues/779) | Filtering tests with any "PropertyAttribute" | success | Success: No regression failure (Pass 2) |
| ✅ [#780](https://github.com/nunit/nunit3-vs-adapter/issues/780) | NUnit3TestAdapter 3.17.0 empty output file regression? | success | Success: No regression failure (Pass 1) |
| ❗ [#781](https://github.com/nunit/nunit3-vs-adapter/issues/781) | An exception occurred while test discoverer 'NUnit3TestDiscoverer' was loading tests. Exception: Object reference not set to an instance of an object, with VS2015 | not run | Failure: Regression failure. |
| ✅ [#784](https://github.com/nunit/nunit3-vs-adapter/issues/784) | Explicit tests are automatically run in Visual Studio 2019 after upgrading from 3.17.0 to 4.0.0-alpha.1 | success | Success: No regression failure (Pass 1) |
| ✅ [#786](https://github.com/nunit/nunit3-vs-adapter/issues/786) | When using TRX logger, should warn about incompatible test adapter across .NET Framework and .NET Core and/or log where an adapter is located | success | Success: No regression failure (Pass 1) |
| ❗ [#811](https://github.com/nunit/nunit3-vs-adapter/issues/811) | "System.FormatException: The UTC representation of the date falls outside the year range 1-9999" from skipped test in Eastern European time zone | not run | Failure: Regression failure. |
| ❗ [#817](https://github.com/nunit/nunit3-vs-adapter/issues/817) | [Azure Devops Pipeline]: Error re-running test with TestCase Attribute | not run | Failure: Regression failure. |
| ❗ [#822](https://github.com/nunit/nunit3-vs-adapter/issues/822) | dotnet test not working for net35 | skipped | Failure: Regression failure. |
| ✅ [#824](https://github.com/nunit/nunit3-vs-adapter/issues/824) | "Not a TestFixture or TestSuite, but SetUpFixture" exception is being thrown in case of more than one [SetUpFixture] | success | Success: No regression failure (Pass 2) |
| ✅ [#843](https://github.com/nunit/nunit3-vs-adapter/issues/843) | Reporting random seed for a test case | success | Success: No regression failure (Pass 1) |
| ✅ [#846](https://github.com/nunit/nunit3-vs-adapter/issues/846) | NUnit3TestAdapter 4.0.0-beta.2 fails with Resharper | success | Success: No regression failure (Pass 5) |
| ✅ [#847](https://github.com/nunit/nunit3-vs-adapter/issues/847) | Adapter doesn't report TestCase properties | success | Success: No regression failure (Pass 1) |
| ✅ [#854](https://github.com/nunit/nunit3-vs-adapter/issues/854) | [BUG] incorrect grouping of ValueTuple<> array TestCase parameters | success | Success: No regression failure (Pass 6) |
| ✅ [#873](https://github.com/nunit/nunit3-vs-adapter/issues/873) | Parametrized test not run from TestCaseSource when one argument is an array of tuple | success | Success: No regression failure (Pass 1) |
| ✅ [#874](https://github.com/nunit/nunit3-vs-adapter/issues/874) | Explicit tests executed when running with dotnet test and a filter | success | Success: No regression failure (Pass 2) |
| ✅ [#878](https://github.com/nunit/nunit3-vs-adapter/issues/878) | NUnit3TestAdapter 4.0.0: test owner no longer is written to .trx file | success | Success: No regression failure (Pass 1) |
| ✅ [#881](https://github.com/nunit/nunit3-vs-adapter/issues/881) | Cannot run tests where test name gets changed by code. Test run is just skipped | success | Success: No regression failure (Pass 2) |
| ✅ [#891](https://github.com/nunit/nunit3-vs-adapter/issues/891) | The same object instance is used when running tests in parallel | success | Success: No regression failure (Pass 3) |
| ✅ [#909](https://github.com/nunit/nunit3-vs-adapter/issues/909) | Tests marked as "Explicit" are reported as "Inconclusive" | success | Success: No regression failure (Pass 1) |
| ✅ [#912](https://github.com/nunit/nunit3-vs-adapter/issues/912) | Explicit runs when using NUnit-filters 'cat!=FOO' | success | Success: No regression failure (Pass 2) |
| ✅ [#914](https://github.com/nunit/nunit3-vs-adapter/issues/914) | AddTestAttachment does not work anymore in VS2022 | success | Success: No regression failure (Pass 1) |
| ✅ [#918](https://github.com/nunit/nunit3-vs-adapter/issues/918) | New DiscoveryMode doesn't play nicely with `TestFixtureSource` - Missing GenericFixture | success | Success: No regression failure (Pass 1) |
| ✅ [#919](https://github.com/nunit/nunit3-vs-adapter/issues/919) | VSTest test case filter does not work with parenthesis | success | Success: No regression failure (Pass 1) |
| ✅ [#941](https://github.com/nunit/nunit3-vs-adapter/issues/941) | When running dotnet test with filter, all tests are run and all console output is logged. | success | Success: No regression failure (Pass 3) |
| ❗ [#972](https://github.com/nunit/nunit3-vs-adapter/issues/972) | Nunit won't discover working non explicit tests Visual studio 2022 | not run | Failure: Regression failure. |
| ✅ [#973](https://github.com/nunit/nunit3-vs-adapter/issues/973) | Unable to cast transparent proxy to type System.Web.UI.ICallbackEventHandler | success | Success: No regression failure (Pass 1) |
| ✅ [#987](https://github.com/nunit/nunit3-vs-adapter/issues/987) | System.ArgumentException: Unknown framework version 7.0 | success | Success: No regression failure (Pass 1) |
| ✅ [#1027](https://github.com/nunit/nunit3-vs-adapter/issues/1027) | Test Explorer is finding tests, but not running them after upgrading to NUnit3TestAdapter v4.3.0 | success | Success: No regression failure (Pass 1) |
| ✅ [#1039](https://github.com/nunit/nunit3-vs-adapter/issues/1039) | Exception while loading tests; tests don't run. Since 4.3.1 | success | Success: No regression failure (Pass 1) |
| ✅ [#1040](https://github.com/nunit/nunit3-vs-adapter/issues/1040) | net461 tests don't run with Nunit3TestAdapter 4.3.x | success | Success: No regression failure (Pass 1) |
| ❗ [#1041](https://github.com/nunit/nunit3-vs-adapter/issues/1041) | How to append date_timestamp in the test result file name? | not run | Failure: Regression failure. |
| ❗ [#1044](https://github.com/nunit/nunit3-vs-adapter/issues/1044) | Explicit tests executed when TestName is set on TestCaseData | not run | Failure: Regression failure. |
| ✅ [#1053](https://github.com/nunit/nunit3-vs-adapter/issues/1053) | First unit test console entry causes VS Test Explorer Warning | success | Success: No regression failure (Pass 1) |
| ✅ [#1065](https://github.com/nunit/nunit3-vs-adapter/issues/1065) | WebHostBuilder.ConfigureServices method not found when using nunit3testadapter 4.4.0  | success | Success: No regression failure (Pass 1) |
| ✅ [#1078](https://github.com/nunit/nunit3-vs-adapter/issues/1078) | Tests with the Explicit attribute are run when all test case filters are using the operator !~ | success | Success: No regression failure (Pass 1) |
| ✅ [#1083](https://github.com/nunit/nunit3-vs-adapter/issues/1083) | Abstract Fixture with Tests and Derived Fixture without Tests | success | Success: No regression failure (Pass 1) |
| ✅ [#1089](https://github.com/nunit/nunit3-vs-adapter/issues/1089) | TextContext.Progress.Writeline does not work in 4.4.2 when running dotnet test | success | Success: No regression failure (Pass 1) |
| ❗ [#1096](https://github.com/nunit/nunit3-vs-adapter/issues/1096) | Parsing error when namespace contains 0 | not run | Failure: Regression failure. |
| ✅ [#1102](https://github.com/nunit/nunit3-vs-adapter/issues/1102) | Console output from application is logged as warnings | success | Success: No regression failure (Pass 1) |
| ✅ [#1111](https://github.com/nunit/nunit3-vs-adapter/issues/1111) | New Mode that completely excludes explicit tests | success | Success: No regression failure (Pass 0) |
| ❗ [#1127](https://github.com/nunit/nunit3-vs-adapter/issues/1127) | Unable to work with Test Driven tests after latest Visual Studio Preview. | skipped | Failure: Regression failure. |
| ❗ [#1132](https://github.com/nunit/nunit3-vs-adapter/issues/1132) | VS22 seems to be ignoring settings defined in the settings file | not run | Failure: Regression failure. |
| ✅ [#1144](https://github.com/nunit/nunit3-vs-adapter/issues/1144) | dotnet test with .NET 8 SDK installed | success | Success: No regression failure (Pass 1) |
| ✅ [#1152](https://github.com/nunit/nunit3-vs-adapter/issues/1152) | Microsoft Testing Platform for NUnit | success | Success: No regression failure (Pass 0) |
| ✅ [#1166](https://github.com/nunit/nunit3-vs-adapter/issues/1166) | TestAdapter and overloaded parametrized test methods creates warnings and marks wrong status | success | Success: No regression failure (Pass 4) |
| ✅ [#1182](https://github.com/nunit/nunit3-vs-adapter/issues/1182) | Category (Traits) should differentiate test cases even when they have same data name | success | Success: No regression failure (Pass 8) |
| ✅ [#1183](https://github.com/nunit/nunit3-vs-adapter/issues/1183) | dotnet test -- NUnit.Where works but dotnet test --list-tests -- NUnit.Where does not | success | Success: No regression failure (Pass 3) |
| ✅ [#1186](https://github.com/nunit/nunit3-vs-adapter/issues/1186) | Failure during unit test discovery doesn't cause entire test suite to fail | success | Success: No regression failure (Pass 2) |
| ✅ [#1213](https://github.com/nunit/nunit3-vs-adapter/issues/1213) | Test discovery fails with exception when test project uses .NET 6+ but references a project using .NET Standard | success | Success: No regression failure |
| ✅ [#1224](https://github.com/nunit/nunit3-vs-adapter/issues/1224) | TestCaseSource generates incorrect tests when used with tuples within tuples in Visual Studio Test Explorer | success | Success: No regression failure (Pass 4) |
| ✅ [#1225](https://github.com/nunit/nunit3-vs-adapter/issues/1225) | Wish: Ability to get the test cases selected for execution | success | Success: No regression failure (Pass 3) |
| ✅ [#1231](https://github.com/nunit/nunit3-vs-adapter/issues/1231) | /TestCaseFilter does not support ClassName property | success | Success: No regression failure (Pass 4) |
| ❗ [#1240](https://github.com/nunit/nunit3-vs-adapter/issues/1240) | NUnit3TestAdapter v5 launches executables in the background | skipped | Failure: Regression failure. |
| ✅ [#1241](https://github.com/nunit/nunit3-vs-adapter/issues/1241) | VS Test Explorer NUnit error: "TestPlatformFormatException: Filter string ... includes unrecognized escape sequence" | success | Success: No regression failure (Pass 0) |
| ✅ [#1242](https://github.com/nunit/nunit3-vs-adapter/issues/1242) | Visual Studio Test Explorer doesnt show all tests | success | Success: No regression failure (Pass 6) |
| ✅ [#1243](https://github.com/nunit/nunit3-vs-adapter/issues/1243) | NUnit3TestAdapter v5 Does Not Display Console.WriteLine in Azure Pipelines | success | Success: No regression failure (Pass 1) |
| ✅ [#1254](https://github.com/nunit/nunit3-vs-adapter/issues/1254) | Apparent test duplication when renaming tests | success | Success: No regression failure (Pass 1) |
| ✅ [#1258](https://github.com/nunit/nunit3-vs-adapter/issues/1258) | Categories aren't shown in Visual Studio when MTP is on | success | Success: No regression failure (Pass 0) |
| ✅ [#1261](https://github.com/nunit/nunit3-vs-adapter/issues/1261) | Retry extension - no tests discovered with NUnit runner | success | Success: No regression failure |
| ❗ [#1265](https://github.com/nunit/nunit3-vs-adapter/issues/1265) | In Microsoft.Testing.Platform  , using dotnet test, the testResults folder is not created, but it is created when run test from the  TestExplorer of vs 2022 | skipped | Failure: Regression failure. |
| ❗ [#1290](https://github.com/nunit/nunit3-vs-adapter/issues/1290) | OneTimeTeardown failures don't show up in results   (formerly: Include TearDown errors on stdout) | skipped | Failure: Regression failure. |
| ✅ [#1332](https://github.com/nunit/nunit3-vs-adapter/issues/1332) | [MTP] OutOfMemoryException in VS Test Explorer from TestFilterConverter.Tokenizer | success | Success: No regression failure (Pass 0) |
| ✅ [#1347](https://github.com/nunit/nunit3-vs-adapter/issues/1347) | Test Cases With Strings That Contain Escaped Double-Quotes are Not Run | success | Success: No regression failure (Pass 0) |
| ✅ [#1348](https://github.com/nunit/nunit3-vs-adapter/issues/1348) | 🧪 [MTP] AwesomeAssertions throws different NUnit.Framework.AssertionException than expected at runtime | success | Success: No regression failure (Pass 0) |
| ✅ [#1357](https://github.com/nunit/nunit3-vs-adapter/issues/1357) | Test cases with ArgDisplayNames containing closing braces `)` are recognized as a different test and names are truncated after first closing brace | success | Success: No regression failure (Pass 32) |
| ❗ [#1360](https://github.com/nunit/nunit3-vs-adapter/issues/1360) | Cannot run (and debug) NUnit tests in Visual Studio 2022 after upgrade NUnit3TestAdapter to version 5.0.0 when using .NET 4.7.2 Framework | skipped | Failure: Regression failure. |
| ✅ [#1367](https://github.com/nunit/nunit3-vs-adapter/issues/1367) | 🧪  v10 of Microsoft.* nugets makes it more difficult to support older nugets and runtime | success | Success: No regression failure (Pass 1) |
| ✅ [#1371](https://github.com/nunit/nunit3-vs-adapter/issues/1371) | :test_tube: V6 - assembly loading issue | success | Success: No regression failure (Pass 0, Fail 1) |
| ✅ [#1374](https://github.com/nunit/nunit3-vs-adapter/issues/1374) | :test_tube: System.Diagnostics.Trace.Assert crashes the test process after updating to NUnit3TestAdapter 6.0.0 | success | Success: No regression failure (Pass 1) |
| ✅ [#1375](https://github.com/nunit/nunit3-vs-adapter/issues/1375) | :test_tube: New failures in dynamic code evaluation in v6 | success | Success: No regression failure |
| ✅ [#1376](https://github.com/nunit/nunit3-vs-adapter/issues/1376) | Parent: Assemblyloading failures in Version 6.0 | success | Success: No regression failure (Pass 1) |
| ❗ [#1391](https://github.com/nunit/nunit3-vs-adapter/issues/1391) | NUnit3TestAdapter V6.0.1 doesn't work with Resharpers Unit Tests explorer | skipped | Failure: Regression failure. |
| ✅ [#1396](https://github.com/nunit/nunit3-vs-adapter/issues/1396) | Resharper test runner fails with NUnit3TestAdapter 6.0.1 | success | Success: No regression failure (Pass 1) |

### Closed failures (details)

#### Issue #658: Explicit tests are automatically run in Visual Studio 2019

**Link**: [#658](https://github.com/nunit/nunit3-vs-adapter/issues/658)

**Repro folder**: [Issue658](https://github.com/nunit/nunit3-vs-adapter/tree/master/Issue658)

**Labels**: is:bug, closed:done, pri:high, VS Issue

**Conclusion**: Failure: Regression failure. (Pass 1, Fail 2)

**Details**:

```
=== Issue658\Explicit\ExplicitCore\ExplicitCore.csproj ===
=== Restore ===
  Determining projects to restore...
  Restored C:\repos\nunit\nunit3-vs-adapter.issues\Issue658\Explicit\ExplicitCore\ExplicitCore.csproj (in 477 ms).

=== Build ===
  Determining projects to restore...
  All projects are up-to-date for restore.
  ExplicitCore -> C:\repos\nunit\nunit3-vs-adapter.issues\Issue658\Explicit\ExplicitCore\bin\Debug\net10.0\ExplicitCore.dll

Build succeeded.
    0 Warning(s)
    0 Error(s)

Time Elapsed 00:00:02.95

=== Test ===
  Determining projects to restore...
  All projects are up-to-date for restore.
  ExplicitCore -> C:\repos\nunit\nunit3-vs-adapter.issues\Issue658\Explicit\ExplicitCore\bin\Debug\net10.0\ExplicitCore.dll
Test run for C:\repos\nunit\nunit3-vs-adapter.issues\Issue658\Explicit\ExplicitCore\bin\Debug\net10.0\ExplicitCore.dll (.NETCoreApp,Version=v10.0)
VSTest version 18.0.1 (x64)

Starting test execution, please wait...
A total of 1 test files matched the specified pattern.
  Skipped AnIgnoredTest [5 ms]
  Failed FailsTestMethod [69 ms]
  Error Message:
     Assert.That(1, Is.EqualTo(2))
  Expected: 2
  But was:  1

  Stack Trace:
     at ExplicitCore.TestClassCore.FailsTestMethod() in C:\repos\nunit\nunit3-vs-adapter.issues\Issue658\Explicit\ExplicitCore\UnitTest1.cs:line 46

1)    at ExplicitCore.TestClassCore.FailsTestMethod() in C:\repos\nunit\nunit3-vs-adapter.issues\Issue658\Explicit\ExplicitCore\UnitTest1.cs:line 46


  Failed SlowAndFailsTestMethod [< 1 ms]
  Stack Trace:
     at ExplicitCore.TestClassCore.SlowAndFailsTestMethod() in C:\repos\nunit\nunit3-vs-adapter.issues\Issue658\Explicit\ExplicitCore\UnitTest1.cs:line 40

1)    at ExplicitCore.TestClassCore.SlowAndFailsTestMethod() in C:\repos\nunit\nunit3-vs-adapter.issues\Issue658\Explicit\ExplicitCore\UnitTest1.cs:line 40


  Skipped TestMethodCoreIsExplicit [< 1 ms]

Failed!  - Failed:     2, Passed:     1, Skipped:     1, Total:     4, Duration: 80 ms - ExplicitCore.dll (net10.0)


=== Issue658\Explicit\Explicit\Explicit.csproj ===
=== Restore ===
  Determining projects to restore...
  Restored C:\repos\nunit\nunit3-vs-adapter.issues\Issue658\Explicit\Explicit\Explicit.csproj (in 515 ms).

=== Build ===
  Determining projects to restore...
  All projects are up-to-date for restore.
C:\repos\nunit\nunit3-vs-adapter.issues\Issue658\Explicit\Explicit\TestClass.cs(5,7): error CS0246: The type or namespace name 'NUnit' could not be found (are you missing a using directive or an assembly reference?) [C:\repos\nunit\nunit3-vs-adapter.issues\Issue658\Explicit\Explicit\Explicit.csproj]
C:\repos\nunit\nunit3-vs-adapter.issues\Issue658\Explicit\Explicit\TestClass.cs(36,6): error CS0616: 'Explicit' is not an attribute class [C:\repos\nunit\nunit3-vs-adapter.issues\Issue658\Explicit\Explicit\Explicit.csproj]
C:\repos\nunit\nunit3-vs-adapter.issues\Issue658\Explicit\Explicit\TestClass.cs(37,6): error CS0246: The type or namespace name 'TestFixtureAttribute' could not be found (are you missing a using directive or an assembly reference?) [C:\repos\nunit\nunit3-vs-adapter.issues\Issue658\Explicit\Explicit\Explicit.csproj]
C:\repos\nunit\nunit3-vs-adapter.issues\Issue658\Explicit\Explicit\TestClass.cs(37,6): error CS0246: The type or namespace name 'TestFixture' could not be found (are you missing a using directive or an assembly reference?) [C:\repos\nunit\nunit3-vs-adapter.issues\Issue658\Explicit\Explicit\Explicit.csproj]
C:\repos\nunit\nunit3-vs-adapter.issues\Issue658\Explicit\Explicit\TestClass.cs(9,6): error CS0246: The type or namespace name 'TestFixtureAttribute' could not be found (are you missing a using directive or an assembly reference?) [C:\repos\nunit\nunit3-vs-adapter.issues\Issue658\Explicit\Explicit\Explicit.csproj]
C:\repos\nunit\nunit3-vs-adapter.issues\Issue658\Explicit\Explicit\TestClass.cs(9,6): error CS0246: The type or namespace name 'TestFixture' could not be found (are you missing a using directive or an assembly reference?) [C:\repos\nunit\nunit3-vs-adapter.issues\Issue658\Explicit\Explicit\Explicit.csproj]
C:\repos\nunit\nunit3-vs-adapter.issues\Issue658\Explicit\Explicit\TestClass.cs(12,10): error CS0616: 'Explicit' is not an attribute class [C:\repos\nunit\nunit3-vs-adapter.issues\Issue658\Explicit\Explicit\Explicit.csproj]
C:\repos\nunit\nunit3-vs-adapter.issues\Issue658\Explicit\Explicit\TestClass.cs(13,10): error CS0246: The type or namespace name 'TestAttribute' could not be found (are you missing a using directive or an assembly reference?) [C:\repos\nunit\nunit3-vs-adapter.issues\Issue658\Explicit\Explicit\Explicit.csproj]
C:\repos\nunit\nunit3-vs-adapter.issues\Issue658\Explicit\Explicit\TestClass.cs(13,10): error CS0246: The type or namespace name 'Test' could not be found (are you missing a using directive or an assembly reference?) [C:\repos\nunit\nunit3-vs-adapter.issues\Issue658\Explicit\Explicit\Explicit.csproj]
C:\repos\nunit\nunit3-vs-adapter.issues\Issue658\Explicit\Explicit\TestClass.cs(21,10): error CS0246: The type or namespace name 'TestAttribute' could not be found (are you missing a using directive or an assembly reference?) [C:\repos\nunit\nunit3-vs-adapter.issues\Issue658\Explicit\Explicit\Explicit.csproj]
C:\repos\nunit\nunit3-vs-adapter.issues\Issue658\Explicit\Explicit\TestClass.cs(21,10): error CS0246: The type or namespace name 'Test' could not be found (are you missing a using directive or an assembly reference?) [C:\repos\nunit\nunit3-vs-adapter.issues\Issue658\Explicit\Explicit\Explicit.csproj]
C:\repos\nunit\nunit3-vs-adapter.issues\Issue658\Explicit\Explicit\TestClass.cs(41,10): error CS0246: The type or namespace name 'TestAttribute' could not be found (are you missing a using directive or an assembly reference?) [C:\repos\nunit\nunit3-vs-adapter.issues\Issue658\Explicit\Explicit\Explicit.csproj]
C:\repos\nunit\nunit3-vs-adapter.issues\Issue658\Explicit\Explicit\TestClass.cs(41,10): error CS0246: The type or namespace name 'Test' could not be found (are you missing a using directive or an assembly reference?) [C:\repos\nunit\nunit3-vs-adapter.issues\Issue658\Explicit\Explicit\Explicit.csproj]
C:\repos\nunit\nunit3-vs-adapter.issues\Issue658\Explicit\Explicit\TestClass.cs(29,10): error CS0246: The type or namespace name 'TestAttribute' could not be found (are you missing a using directive or an assembly reference?) [C:\repos\nunit\nunit3-vs-adapter.issues\Issue658\Explicit\Explicit\Explicit.csproj]
C:\repos\nunit\nunit3-vs-adapter.issues\Issue658\Explicit\Explicit\TestClass.cs(29,10): error CS0246: The type or namespace name 'Test' could not be found (are you missing a using directive or an assembly reference?) [C:\repos\nunit\nunit3-vs-adapter.issues\Issue658\Explicit\Explicit\Explicit.csproj]
C:\repos\nunit\nunit3-vs-adapter.issues\Issue658\Explicit\Explicit\TestClass.cs(29,16): error CS0246: The type or namespace name 'CategoryAttribute' could not be found (are you missing a using directive or an assembly reference?) [C:\repos\nunit\nunit3-vs-adapter.issues\Issue658\Explicit\Explicit\Explicit.csproj]
C:\repos\nunit\nunit3-vs-adapter.issues\Issue658\Explicit\Explicit\TestClass.cs(29,16): error CS0246: The type or namespace name 'Category' could not be found (are you missing a using directive or an assembly reference?) [C:\repos\nunit\nunit3-vs-adapter.issues\Issue658\Explicit\Explicit\Explicit.csproj]
C:\repos\nunit\nunit3-vs-adapter.issues\Issue658\Explicit\Explicit\TestClass.cs(49,10): error CS0246: The type or namespace name 'TestAttribute' could not be found (are you missing a using directive or an assembly reference?) [C:\repos\nunit\nunit3-vs-adapter.issues\Issue658\Explicit\Explicit\Explicit.csproj]
C:\repos\nunit\nunit3-vs-adapter.issues\Issue658\Explicit\Explicit\TestClass.cs(49,10): error CS0246: The type or namespace name 'Test' could not be found (are you missing a using directive or an assembly reference?) [C:\repos\nunit\nunit3-vs-adapter.issues\Issue658\Explicit\Explicit\Explicit.csproj]

Build FAILED.

C:\repos\nunit\nunit3-vs-adapter.issues\Issue658\Explicit\Explicit\TestClass.cs(5,7): error CS0246: The type or namespace name 'NUnit' could not be found (are you missing a using directive or an assembly reference?) [C:\repos\nunit\nunit3-vs-adapter.issues\Issue658\Explicit\Explicit\Explicit.csproj]
C:\repos\nunit\nunit3-vs-adapter.issues\Issue658\Explicit\Explicit\TestClass.cs(36,6): error CS0616: 'Explicit' is not an attribute class [C:\repos\nunit\nunit3-vs-adapter.issues\Issue658\Explicit\Explicit\Explicit.csproj]
C:\repos\nunit\nunit3-vs-adapter.issues\Issue658\Explicit\Explicit\TestClass.cs(37,6): error CS0246: The type or namespace name 'TestFixtureAttribute' could not be found (are you missing a using directive or an assembly reference?) [C:\repos\nunit\nunit3-vs-adapter.issues\Issue658\Explicit\Explicit\Explicit.csproj]
C:\repos\nunit\nunit3-vs-adapter.issues\Issue658\Explicit\Explicit\TestClass.cs(37,6): error CS0246: The type or namespace name 'TestFixture' could not be found (are you missing a using directive or an assembly reference?) [C:\repos\nunit\nunit3-vs-adapter.issues\Issue658\Explicit\Explicit\Explicit.csproj]
C:\repos\nunit\nunit3-vs-adapter.issues\Issue658\Explicit\Explicit\TestClass.cs(9,6): error CS0246: The type or namespace name 'TestFixtureAttribute' could not be found (are you missing a using directive or an assembly reference?) [C:\repos\nunit\nunit3-vs-adapter.issues\Issue658\Explicit\Explicit\Explicit.csproj]
C:\repos\nunit\nunit3-vs-adapter.issues\Issue658\Explicit\Explicit\TestClass.cs(9,6): error CS0246: The type or namespace name 'TestFixture' could not be found (are you missing a using directive or an assembly reference?) [C:\repos\nunit\nunit3-vs-adapter.issues\Issue658\Explicit\Explicit\Explicit.csproj]
C:\repos\nunit\nunit3-vs-adapter.issues\Issue658\Explicit\Explicit\TestClass.cs(12,10): error CS0616: 'Explicit' is not an attribute class [C:\repos\nunit\nunit3-vs-adapter.issues\Issue658\Explicit\Explicit\Explicit.csproj]
C:\repos\nunit\nunit3-vs-adapter.issues\Issue658\Explicit\Explicit\TestClass.cs(13,10): error CS0246: The type or namespace name 'TestAttribute' could not be found (are you missing a using directive or an assembly reference?) [C:\repos\nunit\nunit3-vs-adapter.issues\Issue658\Explicit\Explicit\Explicit.csproj]
C:\repos\nunit\nunit3-vs-adapter.issues\Issue658\Explicit\Explicit\TestClass.cs(13,10): error CS0246: The type or namespace name 'Test' could not be found (are you missing a using directive or an assembly reference?) [C:\repos\nunit\nunit3-vs-adapter.issues\Issue658\Explicit\Explicit\Explicit.csproj]
C:\repos\nunit\nunit3-vs-adapter.issues\Issue658\Explicit\Explicit\TestClass.cs(21,10): error CS0246: The type or namespace name 'TestAttribute' could not be found (are you missing a using directive or an assembly reference?) [C:\repos\nunit\nunit3-vs-adapter.issues\Issue658\Explicit\Explicit\Explicit.csproj]
C:\repos\nunit\nunit3-vs-adapter.issues\Issue658\Explicit\Explicit\TestClass.cs(21,10): error CS0246: The type or namespace name 'Test' could not be found (are you missing a using directive or an assembly reference?) [C:\repos\nunit\nunit3-vs-adapter.issues\Issue658\Explicit\Explicit\Explicit.csproj]
C:\repos\nunit\nunit3-vs-adapter.issues\Issue658\Explicit\Explicit\TestClass.cs(41,10): error CS0246: The type or namespace name 'TestAttribute' could not be found (are you missing a using directive or an assembly reference?) [C:\repos\nunit\nunit3-vs-adapter.issues\Issue658\Explicit\Explicit\Explicit.csproj]
C:\repos\nunit\nunit3-vs-adapter.issues\Issue658\Explicit\Explicit\TestClass.cs(41,10): error CS0246: The type or namespace name 'Test' could not be found (are you missing a using directive or an assembly reference?) [C:\repos\nunit\nunit3-vs-adapter.issues\Issue658\Explicit\Explicit\Explicit.csproj]
C:\repos\nunit\nunit3-vs-adapter.issues\Issue658\Explicit\Explicit\TestClass.cs(29,10): error CS0246: The type or namespace name 'TestAttribute' could not be found (are you missing a using directive or an assembly reference?) [C:\repos\nunit\nunit3-vs-adapter.issues\Issue658\Explicit\Explicit\Explicit.csproj]
C:\repos\nunit\nunit3-vs-adapter.issues\Issue658\Explicit\Explicit\TestClass.cs(29,10): error CS0246: The type or namespace name 'Test' could not be found (are you missing a using directive or an assembly reference?) [C:\repos\nunit\nunit3-vs-adapter.issues\Issue658\Explicit\Explicit\Explicit.csproj]
C:\repos\nunit\nunit3-vs-adapter.issues\Issue658\Explicit\Explicit\TestClass.cs(29,16): error CS0246: The type or namespace name 'CategoryAttribute' could not be found (are you missing a using directive or an assembly reference?) [C:\repos\nunit\nunit3-vs-adapter.issues\Issue658\Explicit\Explicit\Explicit.csproj]
C:\repos\nunit\nunit3-vs-adapter.issues\Issue658\Explicit\Explicit\TestClass.cs(29,16): error CS0246: The type or namespace name 'Category' could not be found (are you missing a using directive or an assembly reference?) [C:\repos\nunit\nunit3-vs-adapter.issues\Issue658\Explicit\Explicit\Explicit.csproj]
C:\repos\nunit\nunit3-vs-adapter.issues\Issue658\Explicit\Explicit\TestClass.cs(49,10): error CS0246: The type or namespace name 'TestAttribute' could not be found (are you missing a using directive or an assembly reference?) [C:\repos\nunit\nunit3-vs-adapter.issues\Issue658\Explicit\Explicit\Explicit.csproj]
C:\repos\nunit\nunit3-vs-adapter.issues\Issue658\Explicit\Explicit\TestClass.cs(49,10): error CS0246: The type or namespace name 'Test' could not be found (are you missing a using directive or an assembly reference?) [C:\repos\nunit\nunit3-vs-adapter.issues\Issue658\Explicit\Explicit\Explicit.csproj]
    0 Warning(s)
    19 Error(s)

Time Elapsed 00:00:01.33


=== Issue658\StopOnError\StopOnError.csproj ===
=== Restore ===
  Determining projects to restore...
  Restored C:\repos\nunit\nunit3-vs-adapter.issues\Issue658\StopOnError\StopOnError.csproj (in 464 ms).

=== Build ===
  Determining projects to restore...
  All projects are up-to-date for restore.
  StopOnError -> C:\repos\nunit\nunit3-vs-adapter.issues\Issue658\StopOnError\bin\Debug\net10.0\StopOnError.dll

Build succeeded.
    0 Warning(s)
    0 Error(s)

Time Elapsed 00:00:03.43

=== Test ===
  Determining projects to restore...
  All projects are up-to-date for restore.
  StopOnError -> C:\repos\nunit\nunit3-vs-adapter.issues\Issue658\StopOnError\bin\Debug\net10.0\StopOnError.dll
Test run for C:\repos\nunit\nunit3-vs-adapter.issues\Issue658\StopOnError\bin\Debug\net10.0\StopOnError.dll (.NETCoreApp,Version=v10.0)
VSTest version 18.0.1 (x64)

Starting test execution, please wait...
A total of 1 test files matched the specified pattern.
  Failed Test1 [97 ms]
  Stack Trace:
     at StopOnError.Tests.Test1() in C:\repos\nunit\nunit3-vs-adapter.issues\Issue658\StopOnError\UnitTest1.cs:line 15

1)    at StopOnError.Tests.Test1() in C:\repos\nunit\nunit3-vs-adapter.issues\Issue658\StopOnError\UnitTest1.cs:line 15


  Failed Test3 [1 ms]
  Stack Trace:
     at StopOnError.Tests.Test3() in C:\repos\nunit\nunit3-vs-adapter.issues\Issue658\StopOnError\UnitTest1.cs:line 27

1)    at StopOnError.Tests.Test3() in C:\repos\nunit\nunit3-vs-adapter.issues\Issue658\StopOnError\UnitTest1.cs:line 27



Failed!  - Failed:     2, Passed:     1, Skipped:     0, Total:     3, Duration: 104 ms - StopOnError.dll (net10.0)
```

#### Issue #671: Exception in OneTimeSetUp has no stack trace

**Link**: [#671](https://github.com/nunit/nunit3-vs-adapter/issues/671)

**Repro folder**: [Issue671](https://github.com/nunit/nunit3-vs-adapter/tree/master/Issue671)

**Labels**: is:enhancement, closed:done

**Conclusion**: Failure: Regression failure. (Pass 0, Fail 2)

**Details**:

```
=== Issue671\Issue671.csproj ===
=== Test Error ===
Setup failed for test fixture Issue671.Tests
SetUp : System.Exception : oops Deep Down
StackTrace: --SetUp
   at Issue671.SomeWhereDeepDown.WhatDoWeDoHere() in C:\repos\nunit\nunit3-vs-adapter.issues\Issue671\UnitTest1.cs:line 44
   at Issue671.Tests.OneTimeSetup() in C:\repos\nunit\nunit3-vs-adapter.issues\Issue671\UnitTest1.cs:line 12
   at System.Reflection.MethodBaseInvoker.InterpretedInvoke_Method(Object obj, IntPtr* args)
   at System.Reflection.MethodBaseInvoker.InvokeWithNoArgs(Object obj, BindingFlags invokeAttr)

=== Issue671\Issue671.csproj ===
=== Restore ===
  Determining projects to restore...
  Restored C:\repos\nunit\nunit3-vs-adapter.issues\Issue671\Issue671.csproj (in 478 ms).

=== Build ===
  Determining projects to restore...
  All projects are up-to-date for restore.
  Issue671 -> C:\repos\nunit\nunit3-vs-adapter.issues\Issue671\bin\Debug\net10.0\Issue671.dll

Build succeeded.
    0 Warning(s)
    0 Error(s)

Time Elapsed 00:00:03.10

=== Test ===
  Determining projects to restore...
  All projects are up-to-date for restore.
  Issue671 -> C:\repos\nunit\nunit3-vs-adapter.issues\Issue671\bin\Debug\net10.0\Issue671.dll
Test run for C:\repos\nunit\nunit3-vs-adapter.issues\Issue671\bin\Debug\net10.0\Issue671.dll (.NETCoreApp,Version=v10.0)
VSTest version 18.0.1 (x64)

Starting test execution, please wait...
A total of 1 test files matched the specified pattern.
  Failed Test1 [23 ms]
  Error Message:
   OneTimeSetUp: SetUp : System.Exception : oops Deep Down
  Stack Trace:
  --SetUp
   at Issue671.SomeWhereDeepDown.WhatDoWeDoHere() in C:\repos\nunit\nunit3-vs-adapter.issues\Issue671\UnitTest1.cs:line 44
   at Issue671.Tests.OneTimeSetup() in C:\repos\nunit\nunit3-vs-adapter.issues\Issue671\UnitTest1.cs:line 12
   at System.Reflection.MethodBaseInvoker.InterpretedInvoke_Method(Object obj, IntPtr* args)
   at System.Reflection.MethodBaseInvoker.InvokeWithNoArgs(Object obj, BindingFlags invokeAttr)

  Failed Test2 [24 ms]
  Error Message:
   OneTimeSetUp: SetUp : System.Exception : oops Deep Down
  Stack Trace:
  --SetUp
   at Issue671.SomeWhereDeepDown.WhatDoWeDoHere() in C:\repos\nunit\nunit3-vs-adapter.issues\Issue671\UnitTest1.cs:line 44
   at Issue671.Tests.OneTimeSetup() in C:\repos\nunit\nunit3-vs-adapter.issues\Issue671\UnitTest1.cs:line 12
   at System.Reflection.MethodBaseInvoker.InterpretedInvoke_Method(Object obj, IntPtr* args)
   at System.Reflection.MethodBaseInvoker.InvokeWithNoArgs(Object obj, BindingFlags invokeAttr)


Failed!  - Failed:     2, Passed:     0, Skipped:     0, Total:     2, Duration: 24 ms - Issue671.dll (net10.0)
```

## Open issues

- Total: 16, Success: 13, Fail: 1

### Succeeded (candidates to close)

| Issue | Title | Conclusion |
| --- | --- | --- |
| [#266](https://github.com/nunit/nunit3-vs-adapter/issues/266) | Console.WriteLine statements in "OneTimeSetUp" and "OneTimeTearDown" is not captured | Open issue, but test succeeds. |
| [#718](https://github.com/nunit/nunit3-vs-adapter/issues/718) | Trace and Debug output is not displayed | Open issue, but test succeeds. |
| [#729](https://github.com/nunit/nunit3-vs-adapter/issues/729) | TestFixture with TestName displayed twice in TestExplorer | Open issue, but test succeeds. |
| [#782](https://github.com/nunit/nunit3-vs-adapter/issues/782) | Test not run with IEnumerable tuple arguments from TestCaseSource | Open issue, but test succeeds. |
| [#935](https://github.com/nunit/nunit3-vs-adapter/issues/935) | An exception occurred while invoking executor 'executor://nunit3testexecutor/': Incorrect format for TestCaseFilter Error when using Real Time Discovery | Open issue, but test succeeds. |
| [#954](https://github.com/nunit/nunit3-vs-adapter/issues/954) | Test generated by IFixtureBuilder is skipped the first time when its name changes | Open issue, but test succeeds. |
| [#1097](https://github.com/nunit/nunit3-vs-adapter/issues/1097) | NUnit test adapter does not detect test cases that contain ")." in a string | Open issue, but test succeeds. |
| [#1167](https://github.com/nunit/nunit3-vs-adapter/issues/1167) | Console output handling reimagined | Open issue, but test succeeds. |
| [#1227](https://github.com/nunit/nunit3-vs-adapter/issues/1227) |  Listing the discovered tests doesn't respect the filter | Open issue, but test succeeds. |
| [#1264](https://github.com/nunit/nunit3-vs-adapter/issues/1264) | Tests are not discovered in VS when the NUnit version is 3.11 or less | Open issue, but test succeeds. |
| [#1336](https://github.com/nunit/nunit3-vs-adapter/issues/1336) | [MTP] No output from `TestContext.Progress` | Open issue, but test succeeds. |
| [#1349](https://github.com/nunit/nunit3-vs-adapter/issues/1349) | [MTP] Filter string includes unrecognized escape sequence | Open issue, but test succeeds. |
| [#1351](https://github.com/nunit/nunit3-vs-adapter/issues/1351) | Test class name missing in TRX report for parametrized fixture when running through MTP | Open issue, but test succeeds. |

### Failing (confirmed repros)

#### Issue #1377: failed to run tests after migration to MTP and newset nunit

**Link**: [#1377](https://github.com/nunit/nunit3-vs-adapter/issues/1377)

**Labels**: is:bug, MTP

**Conclusion**: Failure: Open issue, repro fails.

**Details**:

```
=== Issue1377\NunitTests.csproj ===
=== Restore ===
  Determining projects to restore...
  Restored C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\NunitTests.csproj (in 484 ms).

=== Build ===
  Determining projects to restore...
  All projects are up-to-date for restore.
C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs(226,90): warning CS8625: Cannot convert null literal to non-nullable reference type. [C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\NunitTests.csproj]
  NunitTests -> C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\bin\Debug\net10.0\NunitTests.dll

Build succeeded.

C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs(226,90): warning CS8625: Cannot convert null literal to non-nullable reference type. [C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\NunitTests.csproj]
    1 Warning(s)
    0 Error(s)

Time Elapsed 00:00:03.16

=== Test ===
Running tests from C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\bin\Debug\net10.0\NunitTests.dll (net10.0|x64)
failed ConvertBsQRCodeToOriginal_Valid("+.Å¡.Ã©Â°Ä›Å™Ã©Ä›+Ã½Ã©Ã©Ã©Å¡ÄÅ¾Ã½Ã©Ã­+Â°Ä›Å¯Â°+Â´Ä›Ã©+Å¾+Ã©Ã©+Ã©Ã©ÄÃ½+Ã­'Ä›Â´+'Å¡Â´Ã©'ÄÂ´3ABCDEFGHIJKLMNOPQRSTUWVX'Å™Â´Ã©'Å¾Â´Ä›Ã©Ä›Å™Ã©Ä›+Ã½'Ã½Â´'Ã¡Â´Ã©'Ã­Â´3TBÄCDBAFAÅ™Å™Ã­Ã­CEÃ¡Ä+Ã­ÄEFÃ½Ã©Ã¡Ä›Ä›FAFÅ™DÄ'+Ã©Â´Ã©'++Â´Ã©'+Ä›Â´Ã©'+Å¡Â´Ä›Ã©+Ã½Ã©Å¾Ä›Ã½+Å¡Ã©Å¾Å¡Å¡'+ÄÂ´+'+Å™Â´Ä›Ã©Ä›Å™Ã©Ä›+Ã½Ã©Ã¡+Ä›Å™Ä›'+Å¾Â´Å¾+Å¾'+Ã½Â´Ã©'+Ã¡Â´3TBDÅ™DÃ¡AÅ¾BEÄÅ¡Å™EFÃ½B'+Ã­Â´3T+ÄÅ¡DÅ™FEDÅ¡BÄECÃ¡Ã¡Ã©'Â°Ä›Ã©Ä›Å™Ã©+Å¡Ã©+Å¾ÄÃ­Å™Å¾Å¯Å¡Å¾Å¾Å¯+Ã©ÄÃ©Ã©Ã¡Å¡Ä›Å¯Å¯Å¯+Â°Ä›Ã©Ä›Å™Ã©+Å¡Ã©+Å¾Å¡Å¡Ã©Å™Å¯Å¡Å¾Å¾Å¯+Ã©ÄÃ©Ã©Ã¡Å¡Ä›Å¯Å¯Å¯+Â°Ä›Ã©Ä›Å™Ã©+Ä›Ã©Ã©Ã­Å™Å¾+Ã©Å¯Å¡Å¡Å¡Å¯+Ã©Å¡Ã­Ã­Ã¡Å™Ä›Å¯Å¯Å¯+Â°Ã©Ã¡BDÅ™AC+CÅ¡Ã©FÃ©Ã­Ä+Ã©Ã¡Ã½Ã¡D+Ã­ÄÄÃ¡Ã­BBÃ½+Ã½Ã©+Ã©CÅ™Å¡Ã­Ã©DÃ­EBCÃ½+DÄ›Ã½Ä›Ä›Å¾Å™Å¾Ã¡Å¡DÄÅ™Å¡ÄÅ¾C","1.3.0~2502170003467091~2;~1=20161001004719|2=1|3=0|4=#ABCDEFGHIJKLMNOPQRSTUWVX|5=0|6=20250217|7=|8=0|9=#TB4CDBAFA5599CE84194EF70822FAF5D4|10=0|11=0|12=0|13=20170627130633|14=1|15=20250217081252|16=616|17=0|18=#TBD5D8A6BE435EF7B|19=#T143D5FED3B4EC880|~20250130164956;366;10400832;;;1~20250130163305;366;10400832;;;1~20250120095610;333;10399852;;;1~08BD5AC1C30F09410878D194489BB717010C5390D9EBC71D272265683D45346C") (97ms)
    Assert.That(input, Is.EqualTo(originalInput))
  String lengths are both 411. Strings differ at index 0.
  Expected: "1.3.0~2502170003467091~2;~1=20161001004719|2=1|3=0|4=#ABCDEFG..."
  But was:  "+.Å¡.Ã©Â°Ä›Å™Ã©Ä›+Ã½Ã©Ã©Ã©Å¡ÄÅ¾Ã½Ã©Ã­+Â°Ä›Å¯Â°+Â´Ä›Ã©+Å¾+Ã©Ã©+Ã©Ã©ÄÃ½+Ã­'Ä›Â´+'Å¡Â´Ã©'ÄÂ´3ABCDEFG..."
  -----------^

  from C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\bin\Debug\net10.0\NunitTests.dll (net10.0|x64)
    Assert.That(input, Is.EqualTo(originalInput))
    String lengths are both 411. Strings differ at index 0.
    Expected: "1.3.0~2502170003467091~2;~1=20161001004719|2=1|3=0|4=#ABCDEFG..."
    But was:  "+.Å¡.Ã©Â°Ä›Å™Ã©Ä›+Ã½Ã©Ã©Ã©Å¡ÄÅ¾Ã½Ã©Ã­+Â°Ä›Å¯Â°+Â´Ä›Ã©+Å¾+Ã©Ã©+Ã©Ã©ÄÃ½+Ã­'Ä›Â´+'Å¡Â´Ã©'ÄÂ´3ABCDEFG..."
    -----------^
  
    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    1)    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:line 229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    
failed ConvertBsQRCodeToOriginal_Valid("+.Å¡.Ã©Â°Ä›Å™Ã©Ä›+Ã½Ã©Ã©Ã©Å¡ÄÅ¾Ã½Ã©Ã­+Â°Ä›Å¯Â°+Â´Ä›Ã©+Å¾+Ã©Ã©+Ã©Ã©ÄÃ½+Ã­'Ä›Â´+'Å¡Â´Ã©'ÄÂ´3ABCDEFGHIJKLMNOPQRSTUWVX'Å™Â´Ã©'Å¾Â´Ä›Ã©Ä›Å™Ã©Ä›+Ã½'Ã½Â´'Ã¡Â´Ã©'Ã­Â´3TBÄCDBAFAÅ™Å™Ã­Ã­CEÃ¡Ä+Ã­ÄEFÃ½Ã©Ã¡Ä›Ä›FAFÅ™DÄ'+Ã©Â´Ã©'++Â´Ã©'+Ä›Â´Ã©'+Å¡Â´Ä›Ã©+Ã½Ã©Å¾Ä›Ã½+Å¡Ã©Å¾Å¡Å¡'+ÄÂ´+'+Å™Â´Ä›Ã©Ä›Å™Ã©Ä›+Ã½Ã©Ã¡+Ä›Å™Ä›'+Å¾Â´Å¾+Å¾'+Ã½Â´Ã©'+Ã¡Â´3TBDÅ™DÃ¡AÅ¾BEÄÅ¡Å™EFÃ½B'+Ã­Â´3T+ÄÅ¡DÅ™FEDÅ¡BÄECÃ¡Ã¡Ã©'Â°Ä›Ã©Ä›Å™Ã©+Å¡Ã©+Å¾ÄÃ­Å™Å¾Å¯Å¡Å¾Å¾Å¯+Ã©ÄÃ©Ã©Ã¡Å¡Ä›Å¯Å¯Å¯+Â°Ä›Ã©Ä›Å™Ã©+Å¡Ã©+Å¾Å¡Å¡Ã©Å™Å¯Å¡Å¾Å¾Å¯+Ã©ÄÃ©Ã©Ã¡Å¡Ä›Å¯Å¯Å¯+Â°Ä›Ã©Ä›Å™Ã©+Ä›Ã©Ã©Ã­Å™Å¾+Ã©Å¯Å¡Å¡Å¡Å¯+Ã©Å¡Ã­Ã­Ã¡Å™Ä›Å¯Å¯Å¯+Â°Ã©Ã¡BDÅ™AC+CÅ¡Ã©FÃ©Ã­Ä+Ã©Ã¡Ã½Ã¡D+Ã­ÄÄÃ¡Ã­BBÃ½+Ã½Ã©+Ã©CÅ™Å¡Ã­Ã©DÃ­EBCÃ½+DÄ›Ã½Ä›Ä›Å¾Å™Å¾Ã¡Å¡DÄÅ™Å¡ÄÅ¾C","1.3.0~2502170003467091~2;~1=20161001004719|2=1|3=0|4=#ABCDEFGHIJKLMNOPQRSTUWVX|5=0|6=20250217|7=|8=0|9=#TB4CDBAFA5599CE84194EF70822FAF5D4|10=0|11=0|12=0|13=20170627130633|14=1|15=20250217081252|16=616|17=0|18=#TBD5D8A6BE435EF7B|19=#T143D5FED3B4EC880|~20250130164956;366;10400832;;;1~20250130163305;366;10400832;;;1~20250120095610;333;10399852;;;1~08BD5AC1C30F09410878D194489BB717010C5390D9EBC71D272265683D45346C") (0ms)
    Assert.That(input, Is.EqualTo(originalInput))
  String lengths are both 411. Strings differ at index 0.
  Expected: "1.3.0~2502170003467091~2;~1=20161001004719|2=1|3=0|4=#ABCDEFG..."
  But was:  "+.Å¡.Ã©Â°Ä›Å™Ã©Ä›+Ã½Ã©Ã©Ã©Å¡ÄÅ¾Ã½Ã©Ã­+Â°Ä›Å¯Â°+Â´Ä›Ã©+Å¾+Ã©Ã©+Ã©Ã©ÄÃ½+Ã­'Ä›Â´+'Å¡Â´Ã©'ÄÂ´3ABCDEFG..."
  -----------^

  from C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\bin\Debug\net10.0\NunitTests.dll (net10.0|x64)
    Assert.That(input, Is.EqualTo(originalInput))
    String lengths are both 411. Strings differ at index 0.
    Expected: "1.3.0~2502170003467091~2;~1=20161001004719|2=1|3=0|4=#ABCDEFG..."
    But was:  "+.Å¡.Ã©Â°Ä›Å™Ã©Ä›+Ã½Ã©Ã©Ã©Å¡ÄÅ¾Ã½Ã©Ã­+Â°Ä›Å¯Â°+Â´Ä›Ã©+Å¾+Ã©Ã©+Ã©Ã©ÄÃ½+Ã­'Ä›Â´+'Å¡Â´Ã©'ÄÂ´3ABCDEFG..."
    -----------^
  
    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    1)    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:line 229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    
failed ConvertBsQRCodeToOriginal_Valid("+.Å¡.Ã©Â°Ä¾Å¥Ã©Ä¾+Ã½Ã©Ã©Ã©Å¡ÄÅ¾Ã½Ã©Ã­+Â°Ä¾Ã´Â°+Â´Ä¾Ã©+Å¾+Ã©Ã©+Ã©Ã©ÄÃ½+Ã­)Ä¾Â´+)Å¡Â´Ã©)ÄÂ´3ABCDEFGHIJKLMNOPQRSTUWVX)Å¥Â´Ã©)Å¾Â´Ä¾Ã©Ä¾Å¥Ã©Ä¾+Ã½)Ã½Â´)Ã¡Â´Ã©)Ã­Â´3TBÄCDBAFAÅ¥Å¥Ã­Ã­CEÃ¡Ä+Ã­ÄEFÃ½Ã©Ã¡Ä¾Ä¾FAFÅ¥DÄ)+Ã©Â´Ã©)++Â´Ã©)+Ä¾Â´Ã©)+Å¡Â´Ä¾Ã©+Ã½Ã©Å¾Ä¾Ã½+Å¡Ã©Å¾Å¡Å¡)+ÄÂ´+)+Å¥Â´Ä¾Ã©Ä¾Å¥Ã©Ä¾+Ã½Ã©Ã¡+Ä¾Å¥Ä¾)+Å¾Â´Å¾+Å¾)+Ã½Â´Ã©)+Ã¡Â´3TBDÅ¥DÃ¡AÅ¾BEÄÅ¡Å¥EFÃ½B)+Ã­Â´3T+ÄÅ¡DÅ¥FEDÅ¡BÄECÃ¡Ã¡Ã©)Â°Ä¾Ã©Ä¾Å¥Ã©+Å¡Ã©+Å¾ÄÃ­Å¥Å¾Ã´Å¡Å¾Å¾Ã´+Ã©ÄÃ©Ã©Ã¡Å¡Ä¾Ã´Ã´Ã´+Â°Ä¾Ã©Ä¾Å¥Ã©+Å¡Ã©+Å¾Å¡Å¡Ã©Å¥Ã´Å¡Å¾Å¾Ã´+Ã©ÄÃ©Ã©Ã¡Å¡Ä¾Ã´Ã´Ã´+Â°Ä¾Ã©Ä¾Å¥Ã©+Ä¾Ã©Ã©Ã­Å¥Å¾+Ã©Ã´Å¡Å¡Å¡Ã´+Ã©Å¡Ã­Ã­Ã¡Å¥Ä¾Ã´Ã´Ã´+Â°Ã©Ã¡BDÅ¥AC+CÅ¡Ã©FÃ©Ã­Ä+Ã©Ã¡Ã½Ã¡D+Ã­ÄÄÃ¡Ã­BBÃ½+Ã½Ã©+Ã©CÅ¥Å¡Ã­Ã©DÃ­EBCÃ½+DÄ¾Ã½Ä¾Ä¾Å¾Å¥Å¾Ã¡Å¡DÄÅ¥Å¡ÄÅ¾C","1.3.0~2502170003467091~2;~1=20161001004719|2=1|3=0|4=#ABCDEFGHIJKLMNOPQRSTUWVX|5=0|6=20250217|7=|8=0|9=#TB4CDBAFA5599CE84194EF70822FAF5D4|10=0|11=0|12=0|13=20170627130633|14=1|15=20250217081252|16=616|17=0|18=#TBD5D8A6BE435EF7B|19=#T143D5FED3B4EC880|~20250130164956;366;10400832;;;1~20250130163305;366;10400832;;;1~20250120095610;333;10399852;;;1~08BD5AC1C30F09410878D194489BB717010C5390D9EBC71D272265683D45346C") (0ms)
    Assert.That(input, Is.EqualTo(originalInput))
  String lengths are both 411. Strings differ at index 0.
  Expected: "1.3.0~2502170003467091~2;~1=20161001004719|2=1|3=0|4=#ABCDEFG..."
  But was:  "+.Å¡.Ã©Â°Ä¾Å¥Ã©Ä¾+Ã½Ã©Ã©Ã©Å¡ÄÅ¾Ã½Ã©Ã­+Â°Ä¾Ã´Â°+Â´Ä¾Ã©+Å¾+Ã©Ã©+Ã©Ã©ÄÃ½+Ã­)Ä¾Â´+)Å¡Â´Ã©)ÄÂ´3ABCDEFG..."
  -----------^

  from C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\bin\Debug\net10.0\NunitTests.dll (net10.0|x64)
    Assert.That(input, Is.EqualTo(originalInput))
    String lengths are both 411. Strings differ at index 0.
    Expected: "1.3.0~2502170003467091~2;~1=20161001004719|2=1|3=0|4=#ABCDEFG..."
    But was:  "+.Å¡.Ã©Â°Ä¾Å¥Ã©Ä¾+Ã½Ã©Ã©Ã©Å¡ÄÅ¾Ã½Ã©Ã­+Â°Ä¾Ã´Â°+Â´Ä¾Ã©+Å¾+Ã©Ã©+Ã©Ã©ÄÃ½+Ã­)Ä¾Â´+)Å¡Â´Ã©)ÄÂ´3ABCDEFG..."
    -----------^
  
    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    1)    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:line 229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    
failed ConvertBsQRCodeToOriginal_Valid("1.3.0Ð®2502170003467091Ð®2;Ð®1=20161001004719Ñ2=1Ñ3=0Ñ4=â„–ÐÐ‘Ð¦Ð”Ð•Ð¤Ð“Ð¥Ð˜Ð™ÐšÐ›ÐœÐÐžÐŸÐ§Ð Ð¡Ð¢Ð£Ð¨Ð’Ð–Ñ5=0Ñ6=20250217Ñ7=Ñ8=0Ñ9=â„–Ð¢Ð‘4Ð¦Ð”Ð‘ÐÐ¤Ð5599Ð¦Ð•84194Ð•Ð¤70822Ð¤ÐÐ¤5Ð”4Ñ10=0Ñ11=0Ñ12=0Ñ13=20170627130633Ñ14=1Ñ15=20250217081252Ñ16=616Ñ17=0Ñ18=â„–Ð¢Ð‘Ð”5Ð”8Ð6Ð‘Ð•435Ð•Ð¤7Ð‘Ñ19=â„–Ð¢143Ð”5Ð¤Ð•Ð”3Ð‘4Ð•Ð¦880ÑÐ®20250130164956;366;10400832;;;1Ð®20250130163305;366;10400832;;;1Ð®20250120095610;333;10399852;;;1Ð®08Ð‘Ð”5ÐÐ¦1Ð¦30Ð¤09410878Ð”194489Ð‘Ð‘717010Ð¦5390Ð”9Ð•Ð‘Ð¦71Ð”272265683Ð”45346Ð¦","1.3.0~2502170003467091~2;~1=20161001004719|2=1|3=0|4=#ABCDEFGHIJKLMNOPQRSTUWVX|5=0|6=20250217|7=|8=0|9=#TB4CDBAFA5599CE84194EF70822FAF5D4|10=0|11=0|12=0|13=20170627130633|14=1|15=20250217081252|16=616|17=0|18=#TBD5D8A6BE435EF7B|19=#T143D5FED3B4EC880|~20250130164956;366;10400832;;;1~20250130163305;366;10400832;;;1~20250120095610;333;10399852;;;1~08BD5AC1C30F09410878D194489BB717010C5390D9EBC71D272265683D45346C") (0ms)
    Assert.That(input, Is.EqualTo(originalInput))
  String lengths are both 411. Strings differ at index 5.
  Expected: "1.3.0~2502170003467091~2;~1=20161001004719|2=1|3=0|4=#ABCDEFG..."
  But was:  "1.3.0Ð®2502170003467091Ð®2;Ð®1=20161001004719Ñ2=1Ñ3=0Ñ4=â„–ÐÐ‘Ð¦Ð”Ð•Ð¤Ð“..."
  ----------------^

  from C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\bin\Debug\net10.0\NunitTests.dll (net10.0|x64)
    Assert.That(input, Is.EqualTo(originalInput))
    String lengths are both 411. Strings differ at index 5.
    Expected: "1.3.0~2502170003467091~2;~1=20161001004719|2=1|3=0|4=#ABCDEFG..."
    But was:  "1.3.0Ð®2502170003467091Ð®2;Ð®1=20161001004719Ñ2=1Ñ3=0Ñ4=â„–ÐÐ‘Ð¦Ð”Ð•Ð¤Ð“..."
    ----------------^
  
    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    1)    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:line 229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    
failed ConvertBsQRCodeToOriginal_Valid("1.3.0Ð§2502170003467091Ð§2;Ð§1=20161001004719Ð®2=1Ð®3=0Ð®4=â„–ÐÐ‘Ð¦Ð”Ð•Ð¤Ð“Ð¥Ð˜Ð™ÐšÐ›ÐœÐÐžÐŸÐ¯Ð Ð¡Ð¢Ð£Ð’Ð–ÑÐ®5=0Ð®6=20250217Ð®7=Ð®8=0Ð®9=â„–Ð¢Ð‘4Ð¦Ð”Ð‘ÐÐ¤Ð5599Ð¦Ð•84194Ð•Ð¤70822Ð¤ÐÐ¤5Ð”4Ð®10=0Ð®11=0Ð®12=0Ð®13=20170627130633Ð®14=1Ð®15=20250217081252Ð®16=616Ð®17=0Ð®18=â„–Ð¢Ð‘Ð”5Ð”8Ð6Ð‘Ð•435Ð•Ð¤7Ð‘Ð®19=â„–Ð¢143Ð”5Ð¤Ð•Ð”3Ð‘4Ð•Ð¦880Ð®Ð§20250130164956;366;10400832;;;1Ð§20250130163305;366;10400832;;;1Ð§20250120095610;333;10399852;;;1Ð§08Ð‘Ð”5ÐÐ¦1Ð¦30Ð¤09410878Ð”194489Ð‘Ð‘717010Ð¦5390Ð”9Ð•Ð‘Ð¦71Ð”272265683Ð”45346Ð¦","1.3.0~2502170003467091~2;~1=20161001004719|2=1|3=0|4=#ABCDEFGHIJKLMNOPQRSTUWVX|5=0|6=20250217|7=|8=0|9=#TB4CDBAFA5599CE84194EF70822FAF5D4|10=0|11=0|12=0|13=20170627130633|14=1|15=20250217081252|16=616|17=0|18=#TBD5D8A6BE435EF7B|19=#T143D5FED3B4EC880|~20250130164956;366;10400832;;;1~20250130163305;366;10400832;;;1~20250120095610;333;10399852;;;1~08BD5AC1C30F09410878D194489BB717010C5390D9EBC71D272265683D45346C") (0ms)
    Assert.That(input, Is.EqualTo(originalInput))
  String lengths are both 411. Strings differ at index 5.
  Expected: "1.3.0~2502170003467091~2;~1=20161001004719|2=1|3=0|4=#ABCDEFG..."
  But was:  "1.3.0Ð§2502170003467091Ð§2;Ð§1=20161001004719Ð®2=1Ð®3=0Ð®4=â„–ÐÐ‘Ð¦Ð”Ð•Ð¤Ð“..."
  ----------------^

  from C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\bin\Debug\net10.0\NunitTests.dll (net10.0|x64)
    Assert.That(input, Is.EqualTo(originalInput))
    String lengths are both 411. Strings differ at index 5.
    Expected: "1.3.0~2502170003467091~2;~1=20161001004719|2=1|3=0|4=#ABCDEFG..."
    But was:  "1.3.0Ð§2502170003467091Ð§2;Ð§1=20161001004719Ð®2=1Ð®3=0Ð®4=â„–ÐÐ‘Ð¦Ð”Ð•Ð¤Ð“..."
    ----------------^
  
    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    1)    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:line 229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    
failed ConvertBsQRCodeToOriginal_Valid("1Ð»3Ð»0~2502170003467091~2Ð¼~1.20161001004719)2.1)3.0)4.+Ð¬Ð¤ÐªÐÐ•ÐžÐ–Ð“Ð¡Ð¢ÐÐ’ÐŸÐ¥Ð”Ð—Ñ‹Ð˜Ð¯Ð¨ÐšÐ£Ð­Ð™)5.0)6.20250217)7.)8.0)9.+Ð¨Ð¤4ÐªÐÐ¤Ð¬ÐžÐ¬5599ÐªÐ•84194Ð•Ðž70822ÐžÐ¬Ðž5Ð4)10.0)11.0)12.0)13.20170627130633)14.1)15.20250217081252)16.616)17.0)18.+Ð¨Ð¤Ð5Ð8Ð¬6Ð¤Ð•435Ð•Ðž7Ð¤)19.+Ð¨143Ð5ÐžÐ•Ð3Ð¤4Ð•Ðª880)~20250130164956Ð¼366Ð¼10400832Ð¼Ð¼Ð¼1~20250130163305Ð¼366Ð¼10400832Ð¼Ð¼Ð¼1~20250120095610Ð¼333Ð¼10399852Ð¼Ð¼Ð¼1~08Ð¤Ð5Ð¬Ðª1Ðª30Ðž09410878Ð194489Ð¤Ð¤717010Ðª5390Ð9Ð•Ð¤Ðª71Ð272265683Ð45346Ðª","1.3.0~2502170003467091~2;~1=20161001004719|2=1|3=0|4=#ABCDEFGHIJKLMNOPQRSTUWVX|5=0|6=20250217|7=|8=0|9=#TB4CDBAFA5599CE84194EF70822FAF5D4|10=0|11=0|12=0|13=20170627130633|14=1|15=20250217081252|16=616|17=0|18=#TBD5D8A6BE435EF7B|19=#T143D5FED3B4EC880|~20250130164956;366;10400832;;;1~20250130163305;366;10400832;;;1~20250120095610;333;10399852;;;1~08BD5AC1C30F09410878D194489BB717010C5390D9EBC71D272265683D45346C") (0ms)
    Assert.That(input, Is.EqualTo(originalInput))
  String lengths are both 411. Strings differ at index 1.
  Expected: "1.3.0~2502170003467091~2;~1=20161001004719|2=1|3=0|4=#ABCDEFG..."
  But was:  "1Ð»3Ð»0~2502170003467091~2Ð¼~1.20161001004719)2.1)3.0)4.+Ð¬Ð¤ÐªÐÐ•ÐžÐ–..."
  ------------^

  from C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\bin\Debug\net10.0\NunitTests.dll (net10.0|x64)
    Assert.That(input, Is.EqualTo(originalInput))
    String lengths are both 411. Strings differ at index 1.
    Expected: "1.3.0~2502170003467091~2;~1=20161001004719|2=1|3=0|4=#ABCDEFG..."
    But was:  "1Ð»3Ð»0~2502170003467091~2Ð¼~1.20161001004719)2.1)3.0)4.+Ð¬Ð¤ÐªÐÐ•ÐžÐ–..."
    ------------^
  
    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    1)    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:line 229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    
failed ConvertBsQRCodeToOriginal_Valid("1.3.0Â¨2502170003467091Â¨2ÄÂ¨1+20161001004719Å½2+1Å½3+0Å½4+#ABCDEFGHIJKLMNOPQRSTUWVXÅ½5+0Å½6+20250217Å½7+Å½8+0Å½9+#TB4CDBAFA5599CE84194EF70822FAF5D4Å½10+0Å½11+0Å½12+0Å½13+20170627130633Å½14+1Å½15+20250217081252Å½16+616Å½17+0Å½18+#TBD5D8A6BE435EF7BÅ½19+#T143D5FED3B4EC880Å½Â¨20250130164956Ä366Ä10400832ÄÄÄ1Â¨20250130163305Ä366Ä10400832ÄÄÄ1Â¨20250120095610Ä333Ä10399852ÄÄÄ1Â¨08BD5AC1C30F09410878D194489BB717010C5390D9EBC71D272265683D45346C","1.3.0~2502170003467091~2;~1=20161001004719|2=1|3=0|4=#ABCDEFGHIJKLMNOPQRSTUWVX|5=0|6=20250217|7=|8=0|9=#TB4CDBAFA5599CE84194EF70822FAF5D4|10=0|11=0|12=0|13=20170627130633|14=1|15=20250217081252|16=616|17=0|18=#TBD5D8A6BE435EF7B|19=#T143D5FED3B4EC880|~20250130164956;366;10400832;;;1~20250130163305;366;10400832;;;1~20250120095610;333;10399852;;;1~08BD5AC1C30F09410878D194489BB717010C5390D9EBC71D272265683D45346C") (0ms)
    Assert.That(input, Is.EqualTo(originalInput))
  String lengths are both 411. Strings differ at index 5.
  Expected: "1.3.0~2502170003467091~2;~1=20161001004719|2=1|3=0|4=#ABCDEFG..."
  But was:  "1.3.0Â¨2502170003467091Â¨2ÄÂ¨1+20161001004719Å½2+1Å½3+0Å½4+#ABCDEFG..."
  ----------------^

  from C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\bin\Debug\net10.0\NunitTests.dll (net10.0|x64)
    Assert.That(input, Is.EqualTo(originalInput))
    String lengths are both 411. Strings differ at index 5.
    Expected: "1.3.0~2502170003467091~2;~1=20161001004719|2=1|3=0|4=#ABCDEFG..."
    But was:  "1.3.0Â¨2502170003467091Â¨2ÄÂ¨1+20161001004719Å½2+1Å½3+0Å½4+#ABCDEFG..."
    ----------------^
  
    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    1)    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:line 229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    
failed ConvertBsQRCodeToOriginal_Valid("1.3.0Â¨2511070058875610Â¨2ÄÂ¨1+20250923164601Å½2+1Å½3+0Å½4+#TB1AE461F44417AD65A21E0535741C31EÅ½5+0Å½6+20251107Å½8+0Å½9+344796270974Å½10+0Å½11+0Å½12+0Å½13+20250923164601Å½14+1Å½15+20251107151918Å½16+616Å½17+0Å½20+268Å½Â¨20251107142824Ä10001592Ä43583ÄÄÄ1Â¨20251107121516Ä10001592Ä43583ÄÄÄ1Ã„11759E2212357F73BC37EF20A79CA8A9C8EC5F803B77F09289D781A1581FF31","1.3.0~2511070058875610~2;~1=20250923164601|2=1|3=0|4=#TB1AE461F44417AD65A21E0535741C31E|5=0|6=20251107|8=0|9=344796270974|10=0|11=0|12=0|13=20250923164601|14=1|15=20251107151918|16=616|17=0|20=268|~20251107142824;10001592;43583;;;1~20251107121516;10001592;43583;;;1~A11759E2212357F73BC37EF20A79CA8A9C8EC5F803B77F09289D781A1581FF31") (0ms)
    Assert.That(input, Is.EqualTo(originalInput))
  Expected string length 330 but was 329. Strings differ at index 5.
  Expected: "1.3.0~2511070058875610~2;~1=20250923164601|2=1|3=0|4=#TB1AE46..."
  But was:  "1.3.0Â¨2511070058875610Â¨2ÄÂ¨1+20250923164601Å½2+1Å½3+0Å½4+#TB1AE46..."
  ----------------^

  from C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\bin\Debug\net10.0\NunitTests.dll (net10.0|x64)
    Assert.That(input, Is.EqualTo(originalInput))
    Expected string length 330 but was 329. Strings differ at index 5.
    Expected: "1.3.0~2511070058875610~2;~1=20250923164601|2=1|3=0|4=#TB1AE46..."
    But was:  "1.3.0Â¨2511070058875610Â¨2ÄÂ¨1+20250923164601Å½2+1Å½3+0Å½4+#TB1AE46..."
    ----------------^
  
    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    1)    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:line 229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    
failed ConvertBsQRCodeToOriginal_Valid("1.3.0Â¨2511120058876746Â¨2ÄÂ¨1+20250923164601Å½2+1Å½3+0Å½4+#T8CB5ABC7900BBFB4261780660CBF532DÅ½5+0Å½6+20251112Å½8+0Å½9+344796207979Å½10+0Å½11+0Å½12+0Å½13+20250923164601Å½14+1Å½15+20251112150314Å½16+616Å½17+0Å½20+289Å½Â¨20251112113438Ä10001592Ä43580ÄÄÄ1Â¨20251105083708Ä10001592Ä43583ÄÄÄ1Ã‹5EB1E5EEDB22B2A754978CF3B1655AE48","1.3.0~2511120058876746~2;~1=20250923164601|2=1|3=0|4=#T8CB5ABC7900BBFB4261780660CBF532D|5=0|6=20251112|8=0|9=344796207979|10=0|11=0|12=0|13=20250923164601|14=1|15=20251112150314|16=616|17=0|20=289|~20251112113438;10001592;43580;;;1~20251105083708;10001592;43583;;;1~E5EB1E5EEDB22B2A754978CF3B1655AE48") (0ms)
    Assert.That(input, Is.EqualTo(originalInput))
  Expected string length 300 but was 299. Strings differ at index 5.
  Expected: "1.3.0~2511120058876746~2;~1=20250923164601|2=1|3=0|4=#T8CB5AB..."
  But was:  "1.3.0Â¨2511120058876746Â¨2ÄÂ¨1+20250923164601Å½2+1Å½3+0Å½4+#T8CB5AB..."
  ----------------^

  from C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\bin\Debug\net10.0\NunitTests.dll (net10.0|x64)
    Assert.That(input, Is.EqualTo(originalInput))
    Expected string length 300 but was 299. Strings differ at index 5.
    Expected: "1.3.0~2511120058876746~2;~1=20250923164601|2=1|3=0|4=#T8CB5AB..."
    But was:  "1.3.0Â¨2511120058876746Â¨2ÄÂ¨1+20250923164601Å½2+1Å½3+0Å½4+#T8CB5AB..."
    ----------------^
  
    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    1)    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:line 229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    
failed ConvertBsQRCodeToOriginal_Valid("1Ã§3Ã§0Ã©2502170003467091Ã©2ÅŸÃ©1-20161001004719;2-1;3-0;4-Ã‚BCDEFGHIJKLMNOPQRSTUWVX;5-0;6-20250217;7-;8-0;9-^TB4CDBAFA5599CE84194EF70822FAF5D4;10-0;11-0;12-0;13-20170627130633;14-1;15-20250217081252;16-616;17-0;18-^TBD5D8A6BE435EF7B;19-^T143D5FED3B4EC880;Ã©20250130164956ÅŸ366ÅŸ10400832ÅŸÅŸÅŸ1Ã©20250130163305ÅŸ366ÅŸ10400832ÅŸÅŸÅŸ1Ã©20250120095610ÅŸ333ÅŸ10399852ÅŸÅŸÅŸ1Ã©08BD5AC1C30F09410878D194489BB717010C5390D9EBC71D272265683D45346C","1.3.0~2502170003467091~2;~1=20161001004719|2=1|3=0|4=#ABCDEFGHIJKLMNOPQRSTUWVX|5=0|6=20250217|7=|8=0|9=#TB4CDBAFA5599CE84194EF70822FAF5D4|10=0|11=0|12=0|13=20170627130633|14=1|15=20250217081252|16=616|17=0|18=#TBD5D8A6BE435EF7B|19=#T143D5FED3B4EC880|~20250130164956;366;10400832;;;1~20250130163305;366;10400832;;;1~20250120095610;333;10399852;;;1~08BD5AC1C30F09410878D194489BB717010C5390D9EBC71D272265683D45346C") (0ms)
    Assert.That(input, Is.EqualTo(originalInput))
  Expected string length 411 but was 410. Strings differ at index 1.
  Expected: "1.3.0~2502170003467091~2;~1=20161001004719|2=1|3=0|4=#ABCDEFG..."
  But was:  "1Ã§3Ã§0Ã©2502170003467091Ã©2ÅŸÃ©1-20161001004719;2-1;3-0;4-Ã‚BCDEFGH..."
  ------------^

  from C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\bin\Debug\net10.0\NunitTests.dll (net10.0|x64)
    Assert.That(input, Is.EqualTo(originalInput))
    Expected string length 411 but was 410. Strings differ at index 1.
    Expected: "1.3.0~2502170003467091~2;~1=20161001004719|2=1|3=0|4=#ABCDEFG..."
    But was:  "1Ã§3Ã§0Ã©2502170003467091Ã©2ÅŸÃ©1-20161001004719;2-1;3-0;4-Ã‚BCDEFGH..."
    ------------^
  
    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    1)    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:line 229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    
failed ConvertBsQRCodeToOriginal_Valid("1Ã§3Ã§0Ã©2502170003467091Ã©2ÅŸÃ©1-20161001004719;2-1;3-0;4-^C^DÃ‚^BÃŠ^F^G^HÃŽ^J^K^L^M^NÃ”^P^Q^R^S^TÃ›^W^V^X;5-0;6-20250217;7-;8-0;9-^TB4CDBAFA5599CE84194EF70822FAF5D4;10-0;11-0;12-0;13-20170627130633;14-1;15-20250217081252;16-616;17-0;18-^TBD5D8A6BE435EF7B;19-^T143D5FED3B4EC880;Ã©20250130164956ÅŸ366ÅŸ10400832ÅŸÅŸÅŸ1Ã©20250130163305ÅŸ366ÅŸ10400832ÅŸÅŸÅŸ1Ã©20250120095610ÅŸ333ÅŸ10399852ÅŸÅŸÅŸ1Ã©08BD5AC1C30F09410878D194489BB717010C5390D9EBC71D272265683D45346C","1.3.0~2502170003467091~2;~1=20161001004719|2=1|3=0|4=#C#D#A#B#E#F#G#H#I#J#K#L#M#N#O#P#Q#R#S#T#U#W#V#X|5=0|6=20250217|7=|8=0|9=#TB4CDBAFA5599CE84194EF70822FAF5D4|10=0|11=0|12=0|13=20170627130633|14=1|15=20250217081252|16=616|17=0|18=#TBD5D8A6BE435EF7B|19=#T143D5FED3B4EC880|~20250130164956;366;10400832;;;1~20250130163305;366;10400832;;;1~20250120095610;333;10399852;;;1~08BD5AC1C30F09410878D194489BB717010C5390D9EBC71D272265683D45346C") (0ms)
    Assert.That(input, Is.EqualTo(originalInput))
  Expected string length 434 but was 429. Strings differ at index 1.
  Expected: "1.3.0~2502170003467091~2;~1=20161001004719|2=1|3=0|4=#C#D#A#B..."
  But was:  "1Ã§3Ã§0Ã©2502170003467091Ã©2ÅŸÃ©1-20161001004719;2-1;3-0;4-^C^DÃ‚^BÃŠ..."
  ------------^

  from C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\bin\Debug\net10.0\NunitTests.dll (net10.0|x64)
    Assert.That(input, Is.EqualTo(originalInput))
    Expected string length 434 but was 429. Strings differ at index 1.
    Expected: "1.3.0~2502170003467091~2;~1=20161001004719|2=1|3=0|4=#C#D#A#B..."
    But was:  "1Ã§3Ã§0Ã©2502170003467091Ã©2ÅŸÃ©1-20161001004719;2-1;3-0;4-^C^DÃ‚^BÃŠ..."
    ------------^
  
    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    1)    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:line 229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    
failed ConvertBsQRCodeToOriginal_Valid("1.3.0*2502170003467091*2y*1-20161001004719X2-1X3-0X4-Ã›Ã‡VEÄžAÃœTNKMLSZHPFIÄ°ORGCÃ–X5-0X6-20250217X7-X8-0X9-Ã”Ã‡4VEÃ‡UAU5599VÄž84194ÄžA70822AUA5E4X10-0X11-0X12-0X13-20170627130633X14-1X15-20250217081252X16-616X17-0X18-Ã”Ã‡E5E8U6Ã‡Äž435ÄžA7Ã‡X19-Ã”143E5AÄžE3Ã‡4ÄžV880X*20250130164956y366y10400832yyy1*20250130163305y366y10400832yyy1*20250120095610y333y10399852yyy1*08Ã‡E5UV1V30A09410878E194489Ã‡Ã‡717010V5390E9ÄžÃ‡V71E272265683E45346V","1.3.0~2502170003467091~2;~1=20161001004719|2=1|3=0|4=#ABCDEFGHIJKLMNOPQRSTUWVX|5=0|6=20250217|7=|8=0|9=#TB4CDBAFA5599CE84194EF70822FAF5D4|10=0|11=0|12=0|13=20170627130633|14=1|15=20250217081252|16=616|17=0|18=#TBD5D8A6BE435EF7B|19=#T143D5FED3B4EC880|~20250130164956;366;10400832;;;1~20250130163305;366;10400832;;;1~20250120095610;333;10399852;;;1~08BD5AC1C30F09410878D194489BB717010C5390D9EBC71D272265683D45346C") (0ms)
    Assert.That(input, Is.EqualTo(originalInput))
  Expected string length 411 but was 407. Strings differ at index 5.
  Expected: "1.3.0~2502170003467091~2;~1=20161001004719|2=1|3=0|4=#ABCDEFG..."
  But was:  "1.3.0*2502170003467091*2y*1-20161001004719X2-1X3-0X4-Ã›Ã‡VEÄžAÃœT..."
  ----------------^

  from C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\bin\Debug\net10.0\NunitTests.dll (net10.0|x64)
    Assert.That(input, Is.EqualTo(originalInput))
    Expected string length 411 but was 407. Strings differ at index 5.
    Expected: "1.3.0~2502170003467091~2;~1=20161001004719|2=1|3=0|4=#ABCDEFG..."
    But was:  "1.3.0*2502170003467091*2y*1-20161001004719X2-1X3-0X4-Ã›Ã‡VEÄžAÃœT..."
    ----------------^
  
    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    1)    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:line 229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    
failed ConvertBsQRCodeToOriginal_Valid("1.3.0*2502170003467091*2y*1-20161001004719X2-1X3-0X4-^VÃŠÃ›^Ã‡^ÄžÃ‚^Ãœ^T^N^K^M^L^S^Z^H^P^FÃŽÃ”^R^G^C^Ã–X5-0X6-20250217X7-X8-0X9-Ã”Ã‡4VEÃ‡UAU5599VÄž84194ÄžA70822AUA5E4X10-0X11-0X12-0X13-20170627130633X14-1X15-20250217081252X16-616X17-0X18-Ã”Ã‡E5E8U6Ã‡Äž435ÄžA7Ã‡X19-Ã”143E5AÄžE3Ã‡4ÄžV880X*20250130164956y366y10400832yyy1*20250130163305y366y10400832yyy1*20250120095610y333y10399852yyy1*08Ã‡E5UV1V30A09410878E194489Ã‡Ã‡717010V5390E9ÄžÃ‡V71E272265683E45346V","1.3.0~2502170003467091~2;~1=20161001004719|2=1|3=0|4=#C#D#A#B#E#F#G#H#I#J#K#L#M#N#O#P#Q#R#T#U#W#V#X|5=0|6=20250217|7=|8=0|9=#TB4CDBAFA5599CE84194EF70822FAF5D4|10=0|11=0|12=0|13=20170627130633|14=1|15=20250217081252|16=616|17=0|18=#TBD5D8A6BE435EF7B|19=#T143D5FED3B4EC880|~20250130164956;366;10400832;;;1~20250130163305;366;10400832;;;1~20250120095610;333;10399852;;;1~08BD5AC1C30F09410878D194489BB717010C5390D9EBC71D272265683D45346C") (0ms)
    Assert.That(input, Is.EqualTo(originalInput))
  Expected string length 432 but was 424. Strings differ at index 5.
  Expected: "1.3.0~2502170003467091~2;~1=20161001004719|2=1|3=0|4=#C#D#A#B..."
  But was:  "1.3.0*2502170003467091*2y*1-20161001004719X2-1X3-0X4-^VÃŠÃ›^Ã‡^Äž..."
  ----------------^

  from C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\bin\Debug\net10.0\NunitTests.dll (net10.0|x64)
    Assert.That(input, Is.EqualTo(originalInput))
    Expected string length 432 but was 424. Strings differ at index 5.
    Expected: "1.3.0~2502170003467091~2;~1=20161001004719|2=1|3=0|4=#C#D#A#B..."
    But was:  "1.3.0*2502170003467091*2y*1-20161001004719X2-1X3-0X4-^VÃŠÃ›^Ã‡^Äž..."
    ----------------^
  
    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    1)    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:line 229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    
failed ConvertBsQRCodeToOriginal_Valid("1Ð»3Ð»0~2502170003467091~2Ð¼~1.20161001004719)2.1)3.0)4.+Ð¬Ð¤ÐªÐÐ•ÐžÐ–Ð“Ð¡Ð¢ÐÐ’ÐŸÐ¥Ð”Ð—Ñ‹Ð˜Ð¯Ð¨ÐšÐ£Ð­Ð™)5.0)6.20250217)7.)8.0)9.+Ð¨Ð¤4ÐªÐÐ¤Ð¬ÐžÐ¬5599ÐªÐ•84194Ð•Ðž70822ÐžÐ¬Ðž5Ð4)10.0)11.0)12.0)13.20170627130633)14.1)15.20250217081252)16.616)17.0)18.+Ð¨Ð¤Ð5Ð8Ð¬6Ð¤Ð•435Ð•Ðž7Ð¤)19.+Ð¨143Ð5ÐžÐ•Ð3Ð¤4Ð•Ðª880)~20250130164956Ð¼366Ð¼10400832Ð¼Ð¼Ð¼1~20250130163305Ð¼366Ð¼10400832Ð¼Ð¼Ð¼1~20250120095610Ð¼333Ð¼10399852Ð¼Ð¼Ð¼1~08Ð¤Ð5Ð¬Ðª1Ðª30Ðž09410878Ð194489Ð¤Ð¤717010Ðª5390Ð9Ð•Ð¤Ðª71Ð272265683Ð45346Ðª","1.3.0~2502170003467091~2;~1=20161001004719|2=1|3=0|4=#ABCDEFGHIJKLMNOPQRSTUWVX|5=0|6=20250217|7=|8=0|9=#TB4CDBAFA5599CE84194EF70822FAF5D4|10=0|11=0|12=0|13=20170627130633|14=1|15=20250217081252|16=616|17=0|18=#TBD5D8A6BE435EF7B|19=#T143D5FED3B4EC880|~20250130164956;366;10400832;;;1~20250130163305;366;10400832;;;1~20250120095610;333;10399852;;;1~08BD5AC1C30F09410878D194489BB717010C5390D9EBC71D272265683D45346C") (0ms)
    Assert.That(input, Is.EqualTo(originalInput))
  String lengths are both 411. Strings differ at index 1.
  Expected: "1.3.0~2502170003467091~2;~1=20161001004719|2=1|3=0|4=#ABCDEFG..."
  But was:  "1Ð»3Ð»0~2502170003467091~2Ð¼~1.20161001004719)2.1)3.0)4.+Ð¬Ð¤ÐªÐÐ•ÐžÐ–..."
  ------------^

  from C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\bin\Debug\net10.0\NunitTests.dll (net10.0|x64)
    Assert.That(input, Is.EqualTo(originalInput))
    String lengths are both 411. Strings differ at index 1.
    Expected: "1.3.0~2502170003467091~2;~1=20161001004719|2=1|3=0|4=#ABCDEFG..."
    But was:  "1Ð»3Ð»0~2502170003467091~2Ð¼~1.20161001004719)2.1)3.0)4.+Ð¬Ð¤ÐªÐÐ•ÐžÐ–..."
    ------------^
  
    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    1)    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:line 229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    
failed ConvertBsQRCodeToOriginal_Valid("1=3=0Ð®2502170003467091Ð®2Ñ‰Ð®1â‚¬20161001004719Ð2â‚¬1Ð3â‚¬0Ð4â‚¬#ABCDEFGHIJKLMNOPQRSTUW+XÐ5â‚¬0Ð6â‚¬20250217Ð7â‚¬Ð8â‚¬0Ð9â‚¬#TB4CDBAFA5599CE84194EF70822FAF5D4Ð10â‚¬0Ð11â‚¬0Ð12â‚¬0Ð13â‚¬20170627130633Ð14â‚¬1Ð15â‚¬20250217081252Ð16â‚¬616Ð17â‚¬0Ð18â‚¬#TBD5D8A6BE435EF7BÐ19â‚¬#T143D5FED3B4EC880ÐÐ®20250130164956Ñ‰366Ñ‰10400832Ñ‰Ñ‰Ñ‰1Ð®20250130163305Ñ‰366Ñ‰10400832Ñ‰Ñ‰Ñ‰1Ð®20250120095610Ñ‰333Ñ‰10399852Ñ‰Ñ‰Ñ‰1Ð®08BD5AC1C30F09410878D194489BB717010C5390D9EBC71D272265683D45346C","1.3.0~2502170003467091~2;~1=20161001004719|2=1|3=0|4=#ABCDEFGHIJKLMNOPQRSTUWVX|5=0|6=20250217|7=|8=0|9=#TB4CDBAFA5599CE84194EF70822FAF5D4|10=0|11=0|12=0|13=20170627130633|14=1|15=20250217081252|16=616|17=0|18=#TBD5D8A6BE435EF7B|19=#T143D5FED3B4EC880|~20250130164956;366;10400832;;;1~20250130163305;366;10400832;;;1~20250120095610;333;10399852;;;1~08BD5AC1C30F09410878D194489BB717010C5390D9EBC71D272265683D45346C") (0ms)
    Assert.That(input, Is.EqualTo(originalInput))
  String lengths are both 411. Strings differ at index 1.
  Expected: "1.3.0~2502170003467091~2;~1=20161001004719|2=1|3=0|4=#ABCDEFG..."
  But was:  "1=3=0Ð®2502170003467091Ð®2Ñ‰Ð®1â‚¬20161001004719Ð2â‚¬1Ð3â‚¬0Ð4â‚¬#ABCDEFG..."
  ------------^

  from C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\bin\Debug\net10.0\NunitTests.dll (net10.0|x64)
    Assert.That(input, Is.EqualTo(originalInput))
    String lengths are both 411. Strings differ at index 1.
    Expected: "1.3.0~2502170003467091~2;~1=20161001004719|2=1|3=0|4=#ABCDEFG..."
    But was:  "1=3=0Ð®2502170003467091Ð®2Ñ‰Ð®1â‚¬20161001004719Ð2â‚¬1Ð3â‚¬0Ð4â‚¬#ABCDEFG..."
    ------------^
  
    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    1)    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:line 229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    
failed ConvertBsQRCodeToOriginal_Valid("1.3.0SL~2509260003527667SL~2:SL~1=20210327004210SL|2=1SL|3=0SL|4=SL3SLT7SLB9033SLBSLD99SLF6868SLESLDSLF8394SLCSLB54SLFSLB573SLFSL|5=0SL|6=20250926SL|8=0SL|9=316733998350SL|10=0SL|11=0SL|12=0SL|13=20210327004210SL|14=1SL|15=20250926083637SL|16=616SL|17=0SL|20=268SL|SL~20250925165346:10002861:10463687:::1SL~20250925143142:10002861:10463690:::1SL~547944SLD6SLCSLCSLFSLCSLA772722SLC41023SLF6SLD2SLCSLF7SLD1SLB7SLBSLCSLF609SLBSLB9996SLF8SLA3SLA0SLC85SLBSLFSLC9SLASLCSLC","1.3.0~2509260003527667~2;~1=20210327004210|2=1|3=0|4=#T7B9033BD99F6868EDF8394CB54FB573F|5=0|6=20250926|8=0|9=316733998350|10=0|11=0|12=0|13=20210327004210|14=1|15=20250926083637|16=616|17=0|20=268|~20250925165346;10002861;10463687;;;1~20250925143142;10002861;10463690;;;1~547944D6CCFCA772722C41023F6D2CF7D1B7BCF609BB9996F8A3A0C85BFC9ACC") (0ms)
    Assert.That(input, Is.EqualTo(originalInput))
  Expected string length 336 but was 466. Strings differ at index 5.
  Expected: "1.3.0~2509260003527667~2;~1=20210327004210|2=1|3=0|4=#T7B9033..."
  But was:  "1.3.0SL~2509260003527667SL~2:SL~1=20210327004210SL|2=1SL|3=0S..."
  ----------------^

  from C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\bin\Debug\net10.0\NunitTests.dll (net10.0|x64)
    Assert.That(input, Is.EqualTo(originalInput))
    Expected string length 336 but was 466. Strings differ at index 5.
    Expected: "1.3.0~2509260003527667~2;~1=20210327004210|2=1|3=0|4=#T7B9033..."
    But was:  "1.3.0SL~2509260003527667SL~2:SL~1=20210327004210SL|2=1SL|3=0S..."
    ----------------^
  
    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    1)    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:line 229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    
failed ConvertBsQRCodeToOriginal_Valid("SL1.SL3.SL0CLAR1SL2SL5SL0SL2SL1SL7SL0SL0SL0SL3SL4SL6SL7SL0SL9SL1CLAR1SL2~CLAR1SL1=SL2SL0SL1SL6SL1SL0SL0SL1SL0SL0SL4SL7SL1SL9CLARWSL2=SL1CLARWSL3=SL0CLARWSL4=CLARXSLASLBSLCSLDSLESLFSLGSLHSLISLJSLKSLLSLMSLNSLOSLPSLQSLRSLSSLTSLUSLWSLVSLXSLYSLZCLARWSL5=SL0CLARWSL6=SL2SL0SL2SL5SL0SL2SL1SL7CLARWSL7=CLARWSL8=SL0CLARWSL9=CLARXSLTSLBSL4SLCSLDSLBSLASLFSLASL5SL5SL9SL9SLCSLESL8SL4SL1SL9SL4SLESLFSL7SL0SL8SL2SL2SLFSLASLFSL5SLDSL4CLARWSL1SL0=SL0CLARWSL1SL1=SL0CLARWSL1SL2=SL0CLARWSL1SL3=SL2SL0SL1SL7SL0SL6SL2SL7SL1SL3SL0SL6SL3SL3CLARWSL1SL4=SL1CLARWSL1SL5=SL2SL0SL2SL5SL0SL2SL1SL7SL0SL8SL1SL2SL5SL2CLARWSL1SL6=SL6SL1SL6CLARWSL1SL7=SL0CLARWSL1SL8=CLARXSLTSLBSLDSL5SLDSL8SLASL6SLBSLESL4SL3SL5SLESLFSL7SLBCLARWSL1SL9=CLARXSLTSL1SL4SL3SLDSL5SLFSLESLDSL3SLBSL4SLESLCSL8SL8SL0CLARWCLAR1SL2SL0SL2SL5SL0SL1SL3SL0SL1SL6SL4SL9SL5SL6~SL3SL6SL6~SL1SL0SL4SL0SL0SL8SL3SL2~~~SL1CLAR1SL2SL0SL2SL5SL0SL1SL3SL0SL1SL6SL3SL3SL0SL5~SL3SL6SL6~SL1SL0SL4SL0SL0SL8SL3SL2~~~SL1CLAR1SL2SL0SL2SL5SL0SL1SL2SL0SL0SL9SL5SL6SL1SL0~SL3SL3SL3~SL1SL0SL3SL9SL9SL8SL5SL2~~~SL1CLAR1SL0SL8SLBSLDSL5SLASLCSL1SLCSL3SL0SLFSL0SL9SL4SL1SL0SL8SL7SL8SLDSL1SL9SL4SL4SL8SL9SLBSLBSL7SL1SL7SL0SL1SL0SLCSL5SL3SL9SL0SLDSL9SLESLBSLCSL7SL1SLDSL2SL7SL2SL2SL6SL5SL6SL8SL3SLDSL4SL5SL3SL4SL6SLC","1.3.0~2502170003467091~2;~1=20161001004719|2=1|3=0|4=#ABCDEFGHIJKLMNOPQRSTUWVXYZ|5=0|6=20250217|7=|8=0|9=#TB4CDBAFA5599CE84194EF70822FAF5D4|10=0|11=0|12=0|13=20170627130633|14=1|15=20250217081252|16=616|17=0|18=#TBD5D8A6BE435EF7B|19=#T143D5FED3B4EC880|~20250130164956;366;10400832;;;1~20250130163305;366;10400832;;;1~20250120095610;333;10399852;;;1~08BD5AC1C30F09410878D194489BB717010C5390D9EBC71D272265683D45346C") (0ms)
    Assert.That(input, Is.EqualTo(originalInput))
  Expected string length 413 but was 1225. Strings differ at index 0.
  Expected: "1.3.0~2502170003467091~2;~1=20161001004719|2=1|3=0|4=#ABCDEFG..."
  But was:  "SL1.SL3.SL0CLAR1SL2SL5SL0SL2SL1SL7SL0SL0SL0SL3SL4SL6SL7SL0SL9..."
  -----------^

  from C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\bin\Debug\net10.0\NunitTests.dll (net10.0|x64)
    Assert.That(input, Is.EqualTo(originalInput))
    Expected string length 413 but was 1225. Strings differ at index 0.
    Expected: "1.3.0~2502170003467091~2;~1=20161001004719|2=1|3=0|4=#ABCDEFG..."
    But was:  "SL1.SL3.SL0CLAR1SL2SL5SL0SL2SL1SL7SL0SL0SL0SL3SL4SL6SL7SL0SL9..."
    -----------^
  
    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    1)    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:line 229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    
failed ConvertBsQRCodeToOriginal_Valid("SL1.SL3.SL0CLAR1SL2SL5SL1SL0SL0SL1SL0SL0SL0SL3SL5SL2SL9SL4SL2SL6CLAR1SL2~CLAR1SL1=SL2SL0SL2SL5SL0SL9SL0SL2SL1SL7SL3SL6SL0SL1CLARWSL2=SL1CLARWSL3=SL0CLARWSL4=CLARXSLTSL3SL0SL2SLESL5SLASL0SL7SLFSLCSLESL2SL3SL2SL7SL8SL6SL2SL9SL0SL8SL9SLESLBSL2SLBSLFSLESLESL7SL0SL5CLARWSL5=SL0CLARWSL6=SL2SL0SL2SL5SL1SL0SL0SL1CLARWSL8=SL0CLARWSL9=SL3SL2SL4SL4SL4SL4SL3SL6SL5SL9SL3SL9CLARWSL1SL0=SL0CLARWSL1SL1=SL0CLARWSL1SL2=SL0CLARWSL1SL3=SL2SL0SL2SL5SL0SL9SL0SL2SL1SL7SL3SL6SL0SL1CLARWSL1SL4=SL1CLARWSL1SL5=SL2SL0SL2SL5SL1SL0SL0SL1SL1SL5SL0SL7SL1SL0CLARWSL1SL6=SL6SL1SL6CLARWSL1SL7=SL0CLARWSL2SL0=SL2SL6SL9CLARWCLAR1SL2SL0SL2SL5SL1SL0SL0SL1SL1SL4SL4SL6SL3SL5~SL1SL0SL0SL0SL2SL8SL6SL1~SL1SL0SL4SL6SL3SL6SL9SL0~~~SL1CLAR1SL2SL0SL2SL5SL1SL0SL0SL1SL1SL2SL3SL1SL5SL8~SL1SL0SL0SL0SL2SL8SL6SL1~SL1SL0SL4SL6SL3SL6SL9SL0~~~SL1CLAR1SLASLFSL0SL6SLASLASLESL8SL7SL7SL8SL8SLESLFSLFSLFSLASL2SLASL4SLCSL4SLESL0SL6SL2SL9SL4SL5SL3SL9SLBSL1SLFSL8SL6SL1SLCSLBSL4SLBSL4SL1SL6SL4SLESLCSLDSLFSL5SL4SLASL7SL3SL9SL9SL0SL6SL6SL9SL2SL9SL4SL2","1.3.0~2510010003529426~2;~1=20250902173601|2=1|3=0|4=#T302E5A07FCE23278629089EB2BFEE705|5=0|6=20251001|8=0|9=324444365939|10=0|11=0|12=0|13=20250902173601|14=1|15=20251001150710|16=616|17=0|20=269|~20251001144635;10002861;10463690;;;1~20251001123158;10002861;10463690;;;1~AF06AAE87788EFFFA2A4C4E06294539B1F861CB4B4164ECDF54A739906692942") (0ms)
    Assert.That(input, Is.EqualTo(originalInput))
  Expected string length 336 but was 996. Strings differ at index 0.
  Expected: "1.3.0~2510010003529426~2;~1=20250902173601|2=1|3=0|4=#T302E5A..."
  But was:  "SL1.SL3.SL0CLAR1SL2SL5SL1SL0SL0SL1SL0SL0SL0SL3SL5SL2SL9SL4SL2..."
  -----------^

  from C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\bin\Debug\net10.0\NunitTests.dll (net10.0|x64)
    Assert.That(input, Is.EqualTo(originalInput))
    Expected string length 336 but was 996. Strings differ at index 0.
    Expected: "1.3.0~2510010003529426~2;~1=20250902173601|2=1|3=0|4=#T302E5A..."
    But was:  "SL1.SL3.SL0CLAR1SL2SL5SL1SL0SL0SL1SL0SL0SL0SL3SL5SL2SL9SL4SL2..."
    -----------^
  
    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    1)    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:line 229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    
failed ConvertBsQRCodeToOriginal_Valid("1.3.0SL~2502170003467091SL~2:SL~1=20161001004719SL|2=1SL|3=0SL|4=SL3SLASLBSLCSLDSLESLFSLGSLHSLISLJSLKSLLSLMSLNSLOSLPSLQSLRSLSSLTSLUSLWSLVSLXSLYSLZSL|5=0SL|6=20250217SL|7=SL|8=0SL|9=SL3SLTSLB4SLCSLDSLBSLASLFSLA5599SLCSLE84194SLESLF70822SLFSLASLF5SLD4SL|10=0SL|11=0SL|12=0SL|13=20170627130633SL|14=1SL|15=20250217081252SL|16=616SL|17=0SL|18=SL3SLTSLBSLD5SLD8SLA6SLBSLE435SLESLF7SLBSL|19=SL3SLT143SLD5SLFSLESLD3SLB4SLESLC880SL|SL~20250130164956:366:10400832:::1SL~20250130163305:366:10400832:::1SL~20250120095610:333:10399852:::1SL~08SLBSLD5SLASLC1SLC30SLF09410878SLD194489SLBSLB717010SLC5390SLD9SLESLBSLC71SLD272265683SLD45346SLC","1.3.0~2502170003467091~2;~1=20161001004719|2=1|3=0|4=#ABCDEFGHIJKLMNOPQRSTUWVXYZ|5=0|6=20250217|7=|8=0|9=#TB4CDBAFA5599CE84194EF70822FAF5D4|10=0|11=0|12=0|13=20170627130633|14=1|15=20250217081252|16=616|17=0|18=#TBD5D8A6BE435EF7B|19=#T143D5FED3B4EC880|~20250130164956;366;10400832;;;1~20250130163305;366;10400832;;;1~20250120095610;333;10399852;;;1~08BD5AC1C30F09410878D194489BB717010C5390D9EBC71D272265683D45346C") (0ms)
    Assert.That(input, Is.EqualTo(originalInput))
  Expected string length 413 but was 627. Strings differ at index 5.
  Expected: "1.3.0~2502170003467091~2;~1=20161001004719|2=1|3=0|4=#ABCDEFG..."
  But was:  "1.3.0SL~2502170003467091SL~2:SL~1=20161001004719SL|2=1SL|3=0S..."
  ----------------^

  from C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\bin\Debug\net10.0\NunitTests.dll (net10.0|x64)
    Assert.That(input, Is.EqualTo(originalInput))
    Expected string length 413 but was 627. Strings differ at index 5.
    Expected: "1.3.0~2502170003467091~2;~1=20161001004719|2=1|3=0|4=#ABCDEFG..."
    But was:  "1.3.0SL~2502170003467091SL~2:SL~1=20161001004719SL|2=1SL|3=0S..."
    ----------------^
  
    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    1)    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:line 229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    
failed ConvertBsQRCodeToOriginal_Valid("SL1.SL3.SL0SLCLAR~SL2SL5SL0SL2SL1SL7SL0SL0SL0SL3SL4SL6SL7SL0SL9SL1SLCLAR~SL2~SLCLAR~SL1CLAR=SL2SL0SL1SL6SL1SL0SL0SL1SL0SL0SL4SL7SL1SL9SL|SL2CLAR=SL1SL|SL3CLAR=SL0SL|SL4CLAR=CLAR3SLASLBSLCSLDSLESLFSLGSLHSLISLJSLKSLLSLMSLNSLOSLPSLQSLRSLSSLTSLUSLWSLVSLXSLYSLZSL|SL5CLAR=SL0SL|SL6CLAR=SL2SL0SL2SL5SL0SL2SL1SL7SL|SL7CLAR=SL|SL8CLAR=SL0SL|SL9CLAR=CLAR3SLTSLBSL4SLCSLDSLBSLASLFSLASL5SL5SL9SL9SLCSLESL8SL4SL1SL9SL4SLESLFSL7SL0SL8SL2SL2SLFSLASLFSL5SLDSL4SL|SL1SL0CLAR=SL0SL|SL1SL1CLAR=SL0SL|SL1SL2CLAR=SL0SL|SL1SL3CLAR=SL2SL0SL1SL7SL0SL6SL2SL7SL1SL3SL0SL6SL3SL3SL|SL1SL4CLAR=SL1SL|SL1SL5CLAR=SL2SL0SL2SL5SL0SL2SL1SL7SL0SL8SL1SL2SL5SL2SL|SL1SL6CLAR=SL6SL1SL6SL|SL1SL7CLAR=SL0SL|SL1SL8CLAR=CLAR3SLTSLBSLDSL5SLDSL8SLASL6SLBSLESL4SL3SL5SLESLFSL7SLBSL|SL1SL9CLAR=CLAR3SLTSL1SL4SL3SLDSL5SLFSLESLDSL3SLBSL4SLESLCSL8SL8SL0SL|SLCLAR~SL2SL0SL2SL5SL0SL1SL3SL0SL1SL6SL4SL9SL5SL6~SL3SL6SL6~SL1SL0SL4SL0SL0SL8SL3SL2~~~SL1SLCLAR~SL2SL0SL2SL5SL0SL1SL3SL0SL1SL6SL3SL3SL0SL5~SL3SL6SL6~SL1SL0SL4SL0SL0SL8SL3SL2~~~SL1SLCLAR~SL2SL0SL2SL5SL0SL1SL2SL0SL0SL9SL5SL6SL1SL0~SL3SL3SL3~SL1SL0SL3SL9SL9SL8SL5SL2~~~SL1SLCLAR~SL0SL8SLBSLDSL5SLASLCSL1SLCSL3SL0SLFSL0SL9SL4SL1SL0SL8SL7SL8SLDSL1SL9SL4SL4SL8SL9SLBSLBSL7SL1SL7SL0SL1SL0SLCSL5SL3SL9SL0SLDSL9SLESLBSLCSL7SL1SLDSL2SL7SL2SL2SL6SL5SL6SL8SL3SLDSL4SL5SL3SL4SL6SLC","1.3.0~2502170003467091~2;~1=20161001004719|2=1|3=0|4=#ABCDEFGHIJKLMNOPQRSTUWVXYZ|5=0|6=20250217|7=|8=0|9=#TB4CDBAFA5599CE84194EF70822FAF5D4|10=0|11=0|12=0|13=20170627130633|14=1|15=20250217081252|16=616|17=0|18=#TBD5D8A6BE435EF7B|19=#T143D5FED3B4EC880|~20250130164956;366;10400832;;;1~20250130163305;366;10400832;;;1~20250120095610;333;10399852;;;1~08BD5AC1C30F09410878D194489BB717010C5390D9EBC71D272265683D45346C") (4ms)
    Assert.That(input, Is.EqualTo(originalInput))
  Expected string length 413 but was 1277. Strings differ at index 0.
  Expected: "1.3.0~2502170003467091~2;~1=20161001004719|2=1|3=0|4=#ABCDEFG..."
  But was:  "SL1.SL3.SL0SLCLAR~SL2SL5SL0SL2SL1SL7SL0SL0SL0SL3SL4SL6SL7SL0S..."
  -----------^

  from C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\bin\Debug\net10.0\NunitTests.dll (net10.0|x64)
    Assert.That(input, Is.EqualTo(originalInput))
    Expected string length 413 but was 1277. Strings differ at index 0.
    Expected: "1.3.0~2502170003467091~2;~1=20161001004719|2=1|3=0|4=#ABCDEFG..."
    But was:  "SL1.SL3.SL0SLCLAR~SL2SL5SL0SL2SL1SL7SL0SL0SL0SL3SL4SL6SL7SL0S..."
    -----------^
  
    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    1)    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:line 229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    
failed ConvertBsQRCodeToOriginal_Valid("SL1.SL3.SL0CLAR1SL2SL5SL1SL0SL0SL1SL0SL0SL0SL3SL5SL2SL9SL4SL2SL6CLAR1SL2~CLAR1SL1=SL2SL0SL2SL5SL0SL9SL0SL2SL1SL7SL3SL6SL0SL1SL|SL2=SL1SL|SL3=SL0SL|SL4=CLARXSLTSL3SL0SL2SLESL5SLASL0SL7SLFSLCSLESL2SL3SL2SL7SL8SL6SL2SL9SL0SL8SL9SLESLBSL2SLBSLFSLESLESL7SL0SL5SL|SL5=SL0SL|SL6=SL2SL0SL2SL5SL1SL0SL0SL1SL|SL8=SL0SL|SL9=SL3SL2SL4SL4SL4SL4SL3SL6SL5SL9SL3SL9SL|SL1SL0=SL0SL|SL1SL1=SL0SL|SL1SL2=SL0SL|SL1SL3=SL2SL0SL2SL5SL0SL9SL0SL2SL1SL7SL3SL6SL0SL1SL|SL1SL4=SL1SL|SL1SL5=SL2SL0SL2SL5SL1SL0SL0SL1SL1SL5SL0SL7SL1SL0SL|SL1SL6=SL6SL1SL6SL|SL1SL7=SL0SL|SL2SL0=SL2SL6SL9SL|CLAR1SL2SL0SL2SL5SL1SL0SL0SL1SL1SL4SL4SL6SL3SL5~SL1SL0SL0SL0SL2SL8SL6SL1~SL1SL0SL4SL6SL3SL6SL9SL0~~~SL1CLAR1SL2SL0SL2SL5SL1SL0SL0SL1SL1SL2SL3SL1SL5SL8~SL1SL0SL0SL0SL2SL8SL6SL1~SL1SL0SL4SL6SL3SL6SL9SL0~~~SL1CLAR1SLASLFSL0SL6SLASLASLESL8SL7SL7SL8SL8SLESLFSLFSLFSLASL2SLASL4SLCSL4SLESL0SL6SL2SL9SL4SL5SL3SL9SLBSL1SLFSL8SL6SL1SLCSLBSL4SLBSL4SL1SL6SL4SLESLCSLDSLFSL5SL4SLASL7SL3SL9SL9SL0SL6SL6SL9SL2SL9SL4SL2","1.3.0~2510010003529426~2;~1=20250902173601|2=1|3=0|4=#T302E5A07FCE23278629089EB2BFEE705|5=0|6=20251001|8=0|9=324444365939|10=0|11=0|12=0|13=20250902173601|14=1|15=20251001150710|16=616|17=0|20=269|~20251001144635;10002861;10463690;;;1~20251001123158;10002861;10463690;;;1~AF06AAE87788EFFFA2A4C4E06294539B1F861CB4B4164ECDF54A739906692942") (0ms)
    Assert.That(input, Is.EqualTo(originalInput))
  Expected string length 336 but was 962. Strings differ at index 0.
  Expected: "1.3.0~2510010003529426~2;~1=20250902173601|2=1|3=0|4=#T302E5A..."
  But was:  "SL1.SL3.SL0CLAR1SL2SL5SL1SL0SL0SL1SL0SL0SL0SL3SL5SL2SL9SL4SL2..."
  -----------^

  from C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\bin\Debug\net10.0\NunitTests.dll (net10.0|x64)
    Assert.That(input, Is.EqualTo(originalInput))
    Expected string length 336 but was 962. Strings differ at index 0.
    Expected: "1.3.0~2510010003529426~2;~1=20250902173601|2=1|3=0|4=#T302E5A..."
    But was:  "SL1.SL3.SL0CLAR1SL2SL5SL1SL0SL0SL1SL0SL0SL0SL3SL5SL2SL9SL4SL2..."
    -----------^
  
    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    1)    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:line 229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    
failed ConvertBsQRCodeToOriginal_Valid("1.3.0CLAR12510010003529426CLAR12SL;CLAR11SL020250902173601CLARW2SL01CLARW3SL00CLARW4SL0SL3SLT302SLE5SLA07SLFSLCSLE23278629089SLESLB2SLBSLFSLESLE705CLARW5SL00CLARW6SL020251001CLARW8SL00CLARW9SL0324444365939CLARW10SL00CLARW11SL00CLARW12SL00CLARW13SL020250902173601CLARW14SL01CLARW15SL020251001150710CLARW16SL0616CLARW17SL00CLARW20SL0269CLARWCLAR120251001144635SL;10002861SL;10463690SL;SL;SL;1CLAR120251001123158SL;10002861SL;10463690SL;SL;SL;1CLAR1SLASLF06SLASLASLE87788SLESLFSLFSLFSLA2SLA4SLC4SLE06294539SLB1SLF861SLCSLB4SLB4164SLESLCSLDSLF54SLA739906692942","1.3.0~2510010003529426~2;~1=20250902173601|2=1|3=0|4=#T302E5A07FCE23278629089EB2BFEE705|5=0|6=20251001|8=0|9=324444365939|10=0|11=0|12=0|13=20250902173601|14=1|15=20251001150710|16=616|17=0|20=269|~20251001144635;10002861;10463690;;;1~20251001123158;10002861;10463690;;;1~AF06AAE87788EFFFA2A4C4E06294539B1F861CB4B4164ECDF54A739906692942") (0ms)
    Assert.That(input, Is.EqualTo(originalInput))
  Expected string length 336 but was 556. Strings differ at index 5.
  Expected: "1.3.0~2510010003529426~2;~1=20250902173601|2=1|3=0|4=#T302E5A..."
  But was:  "1.3.0CLAR12510010003529426CLAR12SL;CLAR11SL020250902173601CLA..."
  ----------------^

  from C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\bin\Debug\net10.0\NunitTests.dll (net10.0|x64)
    Assert.That(input, Is.EqualTo(originalInput))
    Expected string length 336 but was 556. Strings differ at index 5.
    Expected: "1.3.0~2510010003529426~2;~1=20250902173601|2=1|3=0|4=#T302E5A..."
    But was:  "1.3.0CLAR12510010003529426CLAR12SL;CLAR11SL020250902173601CLA..."
    ----------------^
  
    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    1)    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:line 229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    
failed ConvertBsQRCodeToOriginal_Valid("1.3.0SL~2510010003529426SL~2:SL~1=20250902173601SL|2=1SL|3=0SL|4=SL3SLT302SLE5SLA07SLFSLCSLE23278629089SLESLB2SLBSLFSLESLE705SL|5=0SL|6=20251001SL|8=0SL|9=324444365939SL|10=0SL|11=0SL|12=0SL|13=20250902173601SL|14=1SL|15=20251001150710SL|16=616SL|17=0SL|20=269SL|SL~20251001144635:10002861:10463690:::1SL~20251001123158:10002861:10463690:::1SL~SLASLF06SLASLASLE87788SLESLFSLFSLFSLA2SLA4SLC4SLE06294539SLB1SLF861SLCSLB4SLB4164SLESLCSLDSLF54SLA739906692942","1.3.0~2510010003529426~2;~1=20250902173601|2=1|3=0|4=#T302E5A07FCE23278629089EB2BFEE705|5=0|6=20251001|8=0|9=324444365939|10=0|11=0|12=0|13=20250902173601|14=1|15=20251001150710|16=616|17=0|20=269|~20251001144635;10002861;10463690;;;1~20251001123158;10002861;10463690;;;1~AF06AAE87788EFFFA2A4C4E06294539B1F861CB4B4164ECDF54A739906692942") (0ms)
    Assert.That(input, Is.EqualTo(originalInput))
  Expected string length 336 but was 454. Strings differ at index 5.
  Expected: "1.3.0~2510010003529426~2;~1=20250902173601|2=1|3=0|4=#T302E5A..."
  But was:  "1.3.0SL~2510010003529426SL~2:SL~1=20250902173601SL|2=1SL|3=0S..."
  ----------------^

  from C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\bin\Debug\net10.0\NunitTests.dll (net10.0|x64)
    Assert.That(input, Is.EqualTo(originalInput))
    Expected string length 336 but was 454. Strings differ at index 5.
    Expected: "1.3.0~2510010003529426~2;~1=20250902173601|2=1|3=0|4=#T302E5A..."
    But was:  "1.3.0SL~2510010003529426SL~2:SL~1=20250902173601SL|2=1SL|3=0S..."
    ----------------^
  
    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    1)    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:line 229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    
failed ConvertBsQRCodeToOriginal_Valid("1.3.0CLAR) 2510010003529426CLAR) 2SL/CLAR) 1SL020250902173601CLAR|2SL01CLAR|3SL00CLAR|4SL0CLAR3SLH302SLD5SLF07SLQSLVSLD23278629089SLDSL;2SL;SLQSLDSLD705CLAR|5SL00CLAR|6SL020251001CLAR|8SL00CLAR|9SL0324444365939CLAR|10SL00CLAR|11SL00CLAR|12SL00CLAR|13SL020250902173601CLAR|14SL01CLAR|15SL020251001150710CLAR|16SL0616CLAR|17SL00CLAR|20SL0269CLAR|CLAR) 20251001144635SL/10002861SL/10463690SL/SL/SL/1CLAR) 20251001123158SL/10002861SL/10463690SL/SL/SL/1CLAR) SLFSLQ06SLFSLFSLD87788SLDSLQSLQSLQSLF2SLF4SLV4SLD06294539SL;1SLQ861SLVSL;4SL;4164SLDSLVSLYSLQ54SLF739906692942","1.3.0~2510010003529426~2;~1=20250902173601|2=1|3=0|4=#T302E5A07FCE23278629089EB2BFEE705|5=0|6=20251001|8=0|9=324444365939|10=0|11=0|12=0|13=20250902173601|14=1|15=20251001150710|16=616|17=0|20=269|~20251001144635;10002861;10463690;;;1~20251001123158;10002861;10463690;;;1~AF06AAE87788EFFFA2A4C4E06294539B1F861CB4B4164ECDF54A739906692942") (0ms)
    Assert.That(input, Is.EqualTo(originalInput))
  Expected string length 336 but was 564. Strings differ at index 5.
  Expected: "1.3.0~2510010003529426~2;~1=20250902173601|2=1|3=0|4=#T302E5A..."
  But was:  "1.3.0CLAR) 2510010003529426CLAR) 2SL/CLAR) 1SL020250902173601..."
  ----------------^

  from C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\bin\Debug\net10.0\NunitTests.dll (net10.0|x64)
    Assert.That(input, Is.EqualTo(originalInput))
    Expected string length 336 but was 564. Strings differ at index 5.
    Expected: "1.3.0~2510010003529426~2;~1=20250902173601|2=1|3=0|4=#T302E5A..."
    But was:  "1.3.0CLAR) 2510010003529426CLAR) 2SL/CLAR) 1SL020250902173601..."
    ----------------^
  
    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    1)    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:line 229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    
failed ConvertBsQRCodeToOriginal_Valid("1.3.0CLAR) 2502170003467091CLAR) 2SL/CLAR) 1SL020161001004719CLAR|2SL01CLAR|3SL00CLAR|4SL0CLAR3SLFSL;SLVSLYSLDSLQSLWSLOSLRSLZSLJSLLSLKSLISLTSLPSL(SLUSLMSLHSLASL)SLCSL|SL:SLNCLAR|5SL00CLAR|6SL020250217CLAR|7SL0CLAR|8SL00CLAR|9SL0CLAR3SLHSL;4SLVSLYSL;SLFSLQSLF5599SLVSLD84194SLDSLQ70822SLQSLFSLQ5SLY4CLAR|10SL00CLAR|11SL00CLAR|12SL00CLAR|13SL020170627130633CLAR|14SL01CLAR|15SL020250217081252CLAR|16SL0616CLAR|17SL00CLAR|18SL0CLAR3SLHSL;SLY5SLY8SLF6SL;SLD435SLDSLQ7SL;CLAR|19SL0CLAR3SLH143SLY5SLQSLDSLY3SL;4SLDSLV880CLAR|CLAR) 20250130164956SL/366SL/10400832SL/SL/SL/1CLAR) 20250130163305SL/366SL/10400832SL/SL/SL/1CLAR) 20250120095610SL/333SL/10399852SL/SL/SL/1CLAR) 08SL;SLY5SLFSLV1SLV30SLQ09410878SLY194489SL;SL;717010SLV5390SLY9SLDSL;SLV71SLY272265683SLY45346SLV","1.3.0~2502170003467091~2;~1=20161001004719|2=1|3=0|4=#ABCDEFGHIJKLMNOPQRSTUWVXYZ|5=0|6=20250217|7=|8=0|9=#TB4CDBAFA5599CE84194EF70822FAF5D4|10=0|11=0|12=0|13=20170627130633|14=1|15=20250217081252|16=616|17=0|18=#TBD5D8A6BE435EF7B|19=#T143D5FED3B4EC880|~20250130164956;366;10400832;;;1~20250130163305;366;10400832;;;1~20250120095610;333;10399852;;;1~08BD5AC1C30F09410878D194489BB717010C5390D9EBC71D272265683D45346C") (0ms)
    Assert.That(input, Is.EqualTo(originalInput))
  Expected string length 413 but was 764. Strings differ at index 5.
  Expected: "1.3.0~2502170003467091~2;~1=20161001004719|2=1|3=0|4=#ABCDEFG..."
  But was:  "1.3.0CLAR) 2502170003467091CLAR) 2SL/CLAR) 1SL020161001004719..."
  ----------------^

  from C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\bin\Debug\net10.0\NunitTests.dll (net10.0|x64)
    Assert.That(input, Is.EqualTo(originalInput))
    Expected string length 413 but was 764. Strings differ at index 5.
    Expected: "1.3.0~2502170003467091~2;~1=20161001004719|2=1|3=0|4=#ABCDEFG..."
    But was:  "1.3.0CLAR) 2502170003467091CLAR) 2SL/CLAR) 1SL020161001004719..."
    ----------------^
  
    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    1)    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:line 229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    
failed ConvertBsQRCodeToOriginal_Valid("1/3/0CLAR) 2510010003529426CLAR) 2SL|CLAR) 1SL020250902173601CLAR|2SL01CLAR|3SL00CLAR|4SL0CLAR3SLT302SLE5SLA07SLFSLCSLE23278629089SLESLB2SLBSLFSLESLE705CLAR|5SL00CLAR|6SL020251001CLAR|8SL00CLAR|9SL0324444365939CLAR|10SL00CLAR|11SL00CLAR|12SL00CLAR|13SL020250902173601CLAR|14SL01CLAR|15SL020251001150710CLAR|16SL0616CLAR|17SL00CLAR|20SL0269CLAR|CLAR) 20251001144635SL|10002861SL|10463690SL|SL|SL|1CLAR) 20251001123158SL|10002861SL|10463690SL|SL|SL|1CLAR) SLASLF06SLASLASLE87788SLESLFSLFSLFSLA2SLA4SLC4SLE06294539SLB1SLF861SLCSLB4SLB4164SLESLCSLDSLF54SLA739906692942","1.3.0~2510010003529426~2;~1=20250902173601|2=1|3=0|4=#T302E5A07FCE23278629089EB2BFEE705|5=0|6=20251001|8=0|9=324444365939|10=0|11=0|12=0|13=20250902173601|14=1|15=20251001150710|16=616|17=0|20=269|~20251001144635;10002861;10463690;;;1~20251001123158;10002861;10463690;;;1~AF06AAE87788EFFFA2A4C4E06294539B1F861CB4B4164ECDF54A739906692942") (0ms)
    Assert.That(input, Is.EqualTo(originalInput))
  Expected string length 336 but was 564. Strings differ at index 1.
  Expected: "1.3.0~2510010003529426~2;~1=20250902173601|2=1|3=0|4=#T302E5A..."
  But was:  "1/3/0CLAR) 2510010003529426CLAR) 2SL|CLAR) 1SL020250902173601..."
  ------------^

  from C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\bin\Debug\net10.0\NunitTests.dll (net10.0|x64)
    Assert.That(input, Is.EqualTo(originalInput))
    Expected string length 336 but was 564. Strings differ at index 1.
    Expected: "1.3.0~2510010003529426~2;~1=20250902173601|2=1|3=0|4=#T302E5A..."
    But was:  "1/3/0CLAR) 2510010003529426CLAR) 2SL|CLAR) 1SL020250902173601..."
    ------------^
  
    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    1)    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:line 229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    
failed ConvertBsQRCodeToOriginal_Valid("1/3/0CLAR) 2502170003467091CLAR) 2SL|CLAR) 1SL020161001004719CLAR|2SL01CLAR|3SL00CLAR|4SL0CLAR3SLASLBSLCSLDSLESLFSLGSLHSLISLJSLKSLLSLMSLNSLOSLPSLQSLRSLSSLTSLUSLWSLVSLXSLYSLZCLAR|5SL00CLAR|6SL020250217CLAR|7SL0CLAR|8SL00CLAR|9SL0CLAR3SLTSLB4SLCSLDSLBSLASLFSLA5599SLCSLE84194SLESLF70822SLFSLASLF5SLD4CLAR|10SL00CLAR|11SL00CLAR|12SL00CLAR|13SL020170627130633CLAR|14SL01CLAR|15SL020250217081252CLAR|16SL0616CLAR|17SL00CLAR|18SL0CLAR3SLTSLBSLD5SLD8SLA6SLBSLE435SLESLF7SLBCLAR|19SL0CLAR3SLT143SLD5SLFSLESLD3SLB4SLESLC880CLAR|CLAR) 20250130164956SL|366SL|10400832SL|SL|SL|1CLAR) 20250130163305SL|366SL|10400832SL|SL|SL|1CLAR) 20250120095610SL|333SL|10399852SL|SL|SL|1CLAR) 08SLBSLD5SLASLC1SLC30SLF09410878SLD194489SLBSLB717010SLC5390SLD9SLESLBSLC71SLD272265683SLD45346SLC","1.3.0~2502170003467091~2;~1=20161001004719|2=1|3=0|4=#ABCDEFGHIJKLMNOPQRSTUWVXYZ|5=0|6=20250217|7=|8=0|9=#TB4CDBAFA5599CE84194EF70822FAF5D4|10=0|11=0|12=0|13=20170627130633|14=1|15=20250217081252|16=616|17=0|18=#TBD5D8A6BE435EF7B|19=#T143D5FED3B4EC880|~20250130164956;366;10400832;;;1~20250130163305;366;10400832;;;1~20250120095610;333;10399852;;;1~08BD5AC1C30F09410878D194489BB717010C5390D9EBC71D272265683D45346C") (0ms)
    Assert.That(input, Is.EqualTo(originalInput))
  Expected string length 413 but was 764. Strings differ at index 1.
  Expected: "1.3.0~2502170003467091~2;~1=20161001004719|2=1|3=0|4=#ABCDEFG..."
  But was:  "1/3/0CLAR) 2502170003467091CLAR) 2SL|CLAR) 1SL020161001004719..."
  ------------^

  from C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\bin\Debug\net10.0\NunitTests.dll (net10.0|x64)
    Assert.That(input, Is.EqualTo(originalInput))
    Expected string length 413 but was 764. Strings differ at index 1.
    Expected: "1.3.0~2502170003467091~2;~1=20161001004719|2=1|3=0|4=#ABCDEFG..."
    But was:  "1/3/0CLAR) 2502170003467091CLAR) 2SL|CLAR) 1SL020161001004719..."
    ------------^
  
    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    1)    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:line 229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    
failed ConvertBsQRCodeToOriginal_Valid("1=3=0SL~2510010003529426SL~2)SL~1SL620250902173601SL|2SL61SL|3SL60SL|4SL6NckNp0Np0Np3AftNp5NckNckNp0Np0Np8AftNp4Nck302NckNp0Np0Np6AftNp9Nck5NckNp0Np0Np6AftNp5Nck07NckNp0Np0Np7AftNp0NckNckNp0Np0Np6AftNp7NckNckNp0Np0Np6AftNp9Nck23278629089NckNp0Np0Np6AftNp9NckNckNp0Np0Np6AftNp6Nck2NckNp0Np0Np6AftNp6NckNckNp0Np0Np7AftNp0NckNckNp0Np0Np6AftNp9NckNckNp0Np0Np6AftNp9Nck705SL|5SL60SL|6SL620251001SL|8SL60SL|9SL6324444365939SL|10SL60SL|11SL60SL|12SL60SL|13SL620250902173601SL|14SL61SL|15SL620251001150710SL|16SL6616SL|17SL60SL|20SL6269SL|SL~20251001144635)10002861)10463690)))1SL~20251001123158)10002861)10463690)))1SL~NckNp0Np0Np6AftNp5NckNckNp0Np0Np7AftNp0Nck06NckNp0Np0Np6AftNp5NckNckNp0Np0Np6AftNp5NckNckNp0Np0Np6AftNp9Nck87788NckNp0Np0Np6AftNp9NckNckNp0Np0Np7AftNp0NckNckNp0Np0Np7AftNp0NckNckNp0Np0Np7AftNp0NckNckNp0Np0Np6AftNp5Nck2NckNp0Np0Np6AftNp5Nck4NckNp0Np0Np6AftNp7Nck4NckNp0Np0Np6AftNp9Nck06294539NckNp0Np0Np6AftNp6Nck1NckNp0Np0Np7AftNp0Nck861NckNp0Np0Np6AftNp7NckNckNp0Np0Np6AftNp6Nck4NckNp0Np0Np6AftNp6Nck4164NckNp0Np0Np6AftNp9NckNckNp0Np0Np6AftNp7NckNckNp0Np0Np6AftNp8NckNckNp0Np0Np7AftNp0Nck54NckNp0Np0Np6AftNp5Nck739906692942","1.3.0~2510010003529426~2;~1=20250902173601|2=1|3=0|4=#T302E5A07FCE23278629089EB2BFEE705|5=0|6=20251001|8=0|9=324444365939|10=0|11=0|12=0|13=20250902173601|14=1|15=20251001150710|16=616|17=0|20=269|~20251001144635;10002861;10463690;;;1~20251001123158;10002861;10463690;;;1~AF06AAE87788EFFFA2A4C4E06294539B1F861CB4B4164ECDF54A739906692942") (0ms)
    Assert.That(input, Is.EqualTo(originalInput))
  Expected string length 336 but was 1136. Strings differ at index 1.
  Expected: "1.3.0~2510010003529426~2;~1=20250902173601|2=1|3=0|4=#T302E5A..."
  But was:  "1=3=0SL~2510010003529426SL~2)SL~1SL620250902173601SL|2SL61SL|..."
  ------------^

  from C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\bin\Debug\net10.0\NunitTests.dll (net10.0|x64)
    Assert.That(input, Is.EqualTo(originalInput))
    Expected string length 336 but was 1136. Strings differ at index 1.
    Expected: "1.3.0~2510010003529426~2;~1=20250902173601|2=1|3=0|4=#T302E5A..."
    But was:  "1=3=0SL~2510010003529426SL~2)SL~1SL620250902173601SL|2SL61SL|..."
    ------------^
  
    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    1)    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:line 229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    
failed ConvertBsQRCodeToOriginal_Valid("1=3=0SL~2502170003467091SL~2)SL~1SL620161001004719SL|2SL61SL|3SL60SL|4SL6NckNp0Np0Np3AftNp5NckNckNp0Np0Np6AftNp5NckNckNp0Np0Np6AftNp6NckNckNp0Np0Np6AftNp7NckNckNp0Np0Np6AftNp8NckNckNp0Np0Np6AftNp9NckNckNp0Np0Np7AftNp0NckNckNp0Np0Np7AftNp1NckNckNp0Np0Np7AftNp2NckNckNp0Np0Np7AftNp3NckNckNp0Np0Np7AftNp4NckNckNp0Np0Np7AftNp5NckNckNp0Np0Np7AftNp6NckNckNp0Np0Np7AftNp7NckNckNp0Np0Np7AftNp8NckNckNp0Np0Np7AftNp9NckNckNp0Np0Np8AftNp0NckNckNp0Np0Np8AftNp1NckNckNp0Np0Np8AftNp2NckNckNp0Np0Np8AftNp3NckNckNp0Np0Np8AftNp4NckNckNp0Np0Np8AftNp5NckNckNp0Np0Np8AftNp7NckSL=NckNp0Np0Np8AftNp8NckNckNp0Np0Np8AftNp9NckNckNp0Np0Np9AftNp0NckSL|5SL60SL|6SL620250217SL|7SL6SL|8SL60SL|9SL6NckNp0Np0Np3AftNp5NckNckNp0Np0Np8AftNp4NckNckNp0Np0Np6AftNp6Nck4NckNp0Np0Np6AftNp7NckNckNp0Np0Np6AftNp8NckNckNp0Np0Np6AftNp6NckNckNp0Np0Np6AftNp5NckNckNp0Np0Np7AftNp0NckNckNp0Np0Np6AftNp5Nck5599NckNp0Np0Np6AftNp7NckNckNp0Np0Np6AftNp9Nck84194NckNp0Np0Np6AftNp9NckNckNp0Np0Np7AftNp0Nck70822NckNp0Np0Np7AftNp0NckNckNp0Np0Np6AftNp5NckNckNp0Np0Np7AftNp0Nck5NckNp0Np0Np6AftNp8Nck4SL|10SL60SL|11SL60SL|12SL60SL|13SL620170627130633SL|14SL61SL|15SL620250217081252SL|16SL6616SL|17SL60SL|18SL6NckNp0Np0Np3AftNp5NckNckNp0Np0Np8AftNp4NckNckNp0Np0Np6AftNp6NckNckNp0Np0Np6AftNp8Nck5NckNp0Np0Np6AftNp8Nck8NckNp0Np0Np6AftNp5Nck6NckNp0Np0Np6AftNp6NckNckNp0Np0Np6AftNp9Nck435NckNp0Np0Np6AftNp9NckNckNp0Np0Np7AftNp0Nck7NckNp0Np0Np6AftNp6NckSL|19SL6NckNp0Np0Np3AftNp5NckNckNp0Np0Np8AftNp4Nck143NckNp0Np0Np6AftNp8Nck5NckNp0Np0Np7AftNp0NckNckNp0Np0Np6AftNp9NckNckNp0Np0Np6AftNp8Nck3NckNp0Np0Np6AftNp6Nck4NckNp0Np0Np6AftNp9NckNckNp0Np0Np6AftNp7Nck880SL|SL~20250130164956)366)10400832)))1SL~20250130163305)366)10400832)))1SL~20250120095610)333)10399852)))1SL~08NckNp0Np0Np6AftNp6NckNckNp0Np0Np6AftNp8Nck5NckNp0Np0Np6AftNp5NckNckNp0Np0Np6AftNp7Nck1NckNp0Np0Np6AftNp7Nck30NckNp0Np0Np7AftNp0Nck09410878NckNp0Np0Np6AftNp8Nck194489NckNp0Np0Np6AftNp6NckNckNp0Np0Np6AftNp6Nck717010NckNp0Np0Np6AftNp7Nck5390NckNp0Np0Np6AftNp8Nck9NckNp0Np0Np6AftNp9NckNckNp0Np0Np6AftNp6NckNckNp0Np0Np6AftNp7Nck71NckNp0Np0Np6AftNp8Nck272265683NckNp0Np0Np6AftNp8Nck45346NckNp0Np0Np6AftNp7Nck","1.3.0~2502170003467091~2;~1=20161001004719|2=1|3=0|4=#ABCDEFGHIJKLMNOPQRSTUWVXYZ|5=0|6=20250217|7=|8=0|9=#TB4CDBAFA5599CE84194EF70822FAF5D4|10=0|11=0|12=0|13=20170627130633|14=1|15=20250217081252|16=616|17=0|18=#TBD5D8A6BE435EF7B|19=#T143D5FED3B4EC880|~20250130164956;366;10400832;;;1~20250130163305;366;10400832;;;1~20250120095610;333;10399852;;;1~08BD5AC1C30F09410878D194489BB717010C5390D9EBC71D272265683D45346C") (0ms)
    Assert.That(input, Is.EqualTo(originalInput))
  Expected string length 413 but was 2105. Strings differ at index 1.
  Expected: "1.3.0~2502170003467091~2;~1=20161001004719|2=1|3=0|4=#ABCDEFG..."
  But was:  "1=3=0SL~2502170003467091SL~2)SL~1SL620161001004719SL|2SL61SL|..."
  ------------^

  from C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\bin\Debug\net10.0\NunitTests.dll (net10.0|x64)
    Assert.That(input, Is.EqualTo(originalInput))
    Expected string length 413 but was 2105. Strings differ at index 1.
    Expected: "1.3.0~2502170003467091~2;~1=20161001004719|2=1|3=0|4=#ABCDEFG..."
    But was:  "1=3=0SL~2502170003467091SL~2)SL~1SL620161001004719SL|2SL61SL|..."
    ------------^
  
    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    1)    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:line 229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    
failed ConvertBsQRCodeToOriginal_Valid("1=3=0SL~2510010003529426SL~2)SL~1SL620250902173601SL|2SL61SL|3SL60SL|4SL6NckNp0Np0Np3AftNp5NckNckNp0Np0Np8AftNp4Nck302NckNp0Np0Np6AftNp9Nck5NckNp0Np0Np6AftNp5Nck07NckNp0Np0Np7AftNp0NckNckNp0Np0Np6AftNp7NckNckNp0Np0Np6AftNp9Nck23278629089NckNp0Np0Np6AftNp9NckNckNp0Np0Np6AftNp6Nck2NckNp0Np0Np6AftNp6NckNckNp0Np0Np7AftNp0NckNckNp0Np0Np6AftNp9NckNckNp0Np0Np6AftNp9Nck705SL|5SL60SL|6SL620251001SL|8SL60SL|9SL6324444365939SL|10SL60SL|11SL60SL|12SL60SL|13SL620250902173601SL|14SL61SL|15SL620251001150710SL|16SL6616SL|17SL60SL|20SL6269SL|SL~20251001144635)10002861)10463690)))1SL~20251001123158)10002861)10463690)))1SL~NckNp0Np0Np6AftNp5NckNckNp0Np0Np7AftNp0Nck06NckNp0Np0Np6AftNp5NckNckNp0Np0Np6AftNp5NckNckNp0Np0Np6AftNp9Nck87788NckNp0Np0Np6AftNp9NckNckNp0Np0Np7AftNp0NckNckNp0Np0Np7AftNp0NckNckNp0Np0Np7AftNp0NckNckNp0Np0Np6AftNp5Nck2NckNp0Np0Np6AftNp5Nck4NckNp0Np0Np6AftNp7Nck4NckNp0Np0Np6AftNp9Nck06294539NckNp0Np0Np6AftNp6Nck1NckNp0Np0Np7AftNp0Nck861NckNp0Np0Np6AftNp7NckNckNp0Np0Np6AftNp6Nck4NckNp0Np0Np6AftNp6Nck4164NckNp0Np0Np6AftNp9NckNckNp0Np0Np6AftNp7NckNckNp0Np0Np6AftNp8NckNckNp0Np0Np7AftNp0Nck54NckNp0Np0Np6AftNp5Nck739906692942","1.3.0~2510010003529426~2;~1=20250902173601|2=1|3=0|4=#T302E5A07FCE23278629089EB2BFEE705|5=0|6=20251001|8=0|9=324444365939|10=0|11=0|12=0|13=20250902173601|14=1|15=20251001150710|16=616|17=0|20=269|~20251001144635;10002861;10463690;;;1~20251001123158;10002861;10463690;;;1~AF06AAE87788EFFFA2A4C4E06294539B1F861CB4B4164ECDF54A739906692942") (0ms)
    Assert.That(input, Is.EqualTo(originalInput))
  Expected string length 336 but was 1136. Strings differ at index 1.
  Expected: "1.3.0~2510010003529426~2;~1=20250902173601|2=1|3=0|4=#T302E5A..."
  But was:  "1=3=0SL~2510010003529426SL~2)SL~1SL620250902173601SL|2SL61SL|..."
  ------------^

  from C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\bin\Debug\net10.0\NunitTests.dll (net10.0|x64)
    Assert.That(input, Is.EqualTo(originalInput))
    Expected string length 336 but was 1136. Strings differ at index 1.
    Expected: "1.3.0~2510010003529426~2;~1=20250902173601|2=1|3=0|4=#T302E5A..."
    But was:  "1=3=0SL~2510010003529426SL~2)SL~1SL620250902173601SL|2SL61SL|..."
    ------------^
  
    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    1)    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:line 229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    
failed ConvertBsQRCodeToOriginal_Valid("1=3=0SL~2502170003467091SL~2)SL~1SL620161001004719SL|2SL61SL|3SL60SL|4SL6NckNp0Np0Np3AftNp5NckNckNp0Np0Np6AftNp5NckNckNp0Np0Np6AftNp6NckNckNp0Np0Np6AftNp7NckNckNp0Np0Np6AftNp8NckNckNp0Np0Np6AftNp9NckNckNp0Np0Np7AftNp0NckNckNp0Np0Np7AftNp1NckNckNp0Np0Np7AftNp2NckNckNp0Np0Np7AftNp3NckNckNp0Np0Np7AftNp4NckNckNp0Np0Np7AftNp5NckNckNp0Np0Np7AftNp6NckNckNp0Np0Np7AftNp7NckNckNp0Np0Np7AftNp8NckNckNp0Np0Np7AftNp9NckNckNp0Np0Np8AftNp0NckNckNp0Np0Np8AftNp1NckNckNp0Np0Np8AftNp2NckNckNp0Np0Np8AftNp3NckNckNp0Np0Np8AftNp4NckNckNp0Np0Np8AftNp5NckNckNp0Np0Np8AftNp7NckSL=NckNp0Np0Np8AftNp8NckNckNp0Np0Np8AftNp9NckNckNp0Np0Np9AftNp0NckSL|5SL60SL|6SL620250217SL|7SL6SL|8SL60SL|9SL6NckNp0Np0Np3AftNp5NckNckNp0Np0Np8AftNp4NckNckNp0Np0Np6AftNp6Nck4NckNp0Np0Np6AftNp7NckNckNp0Np0Np6AftNp8NckNckNp0Np0Np6AftNp6NckNckNp0Np0Np6AftNp5NckNckNp0Np0Np7AftNp0NckNckNp0Np0Np6AftNp5Nck5599NckNp0Np0Np6AftNp7NckNckNp0Np0Np6AftNp9Nck84194NckNp0Np0Np6AftNp9NckNckNp0Np0Np7AftNp0Nck70822NckNp0Np0Np7AftNp0NckNckNp0Np0Np6AftNp5NckNckNp0Np0Np7AftNp0Nck5NckNp0Np0Np6AftNp8Nck4SL|10SL60SL|11SL60SL|12SL60SL|13SL620170627130633SL|14SL61SL|15SL620250217081252SL|16SL6616SL|17SL60SL|18SL6NckNp0Np0Np3AftNp5NckNckNp0Np0Np8AftNp4NckNckNp0Np0Np6AftNp6NckNckNp0Np0Np6AftNp8Nck5NckNp0Np0Np6AftNp8Nck8NckNp0Np0Np6AftNp5Nck6NckNp0Np0Np6AftNp6NckNckNp0Np0Np6AftNp9Nck435NckNp0Np0Np6AftNp9NckNckNp0Np0Np7AftNp0Nck7NckNp0Np0Np6AftNp6NckSL|19SL6NckNp0Np0Np3AftNp5NckNckNp0Np0Np8AftNp4Nck143NckNp0Np0Np6AftNp8Nck5NckNp0Np0Np7AftNp0NckNckNp0Np0Np6AftNp9NckNckNp0Np0Np6AftNp8Nck3NckNp0Np0Np6AftNp6Nck4NckNp0Np0Np6AftNp9NckNckNp0Np0Np6AftNp7Nck880SL|SL~20250130164956)366)10400832)))1SL~20250130163305)366)10400832)))1SL~20250120095610)333)10399852)))1SL~08NckNp0Np0Np6AftNp6NckNckNp0Np0Np6AftNp8Nck5NckNp0Np0Np6AftNp5NckNckNp0Np0Np6AftNp7Nck1NckNp0Np0Np6AftNp7Nck30NckNp0Np0Np7AftNp0Nck09410878NckNp0Np0Np6AftNp8Nck194489NckNp0Np0Np6AftNp6NckNckNp0Np0Np6AftNp6Nck717010NckNp0Np0Np6AftNp7Nck5390NckNp0Np0Np6AftNp8Nck9NckNp0Np0Np6AftNp9NckNckNp0Np0Np6AftNp6NckNckNp0Np0Np6AftNp7Nck71NckNp0Np0Np6AftNp8Nck272265683NckNp0Np0Np6AftNp8Nck45346NckNp0Np0Np6AftNp7Nck","1.3.0~2502170003467091~2;~1=20161001004719|2=1|3=0|4=#ABCDEFGHIJKLMNOPQRSTUWVXYZ|5=0|6=20250217|7=|8=0|9=#TB4CDBAFA5599CE84194EF70822FAF5D4|10=0|11=0|12=0|13=20170627130633|14=1|15=20250217081252|16=616|17=0|18=#TBD5D8A6BE435EF7B|19=#T143D5FED3B4EC880|~20250130164956;366;10400832;;;1~20250130163305;366;10400832;;;1~20250120095610;333;10399852;;;1~08BD5AC1C30F09410878D194489BB717010C5390D9EBC71D272265683D45346C") (0ms)
    Assert.That(input, Is.EqualTo(originalInput))
  Expected string length 413 but was 2105. Strings differ at index 1.
  Expected: "1.3.0~2502170003467091~2;~1=20161001004719|2=1|3=0|4=#ABCDEFG..."
  But was:  "1=3=0SL~2502170003467091SL~2)SL~1SL620161001004719SL|2SL61SL|..."
  ------------^

  from C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\bin\Debug\net10.0\NunitTests.dll (net10.0|x64)
    Assert.That(input, Is.EqualTo(originalInput))
    Expected string length 413 but was 2105. Strings differ at index 1.
    Expected: "1.3.0~2502170003467091~2;~1=20161001004719|2=1|3=0|4=#ABCDEFG..."
    But was:  "1=3=0SL~2502170003467091SL~2)SL~1SL620161001004719SL|2SL61SL|..."
    ------------^
  
    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    1)    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:line 229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    
failed ConvertBsQRCodeToOriginal_Valid("1.3.0SL~2502170003467091SL~2:SL~1=20161001004719SL|2=1SL|3=0SL|4=SL3SLASLBSLCSLDSLESLFSLGSLHSLISLJSLKSLLSLMSLNSLOSLPSLQSLRSLSSLTSLUSLWSLVSLXSLYSLZSL|5=0SL|6=20250217SL|7=SL|8=0SL|9=SL3SLTSLB4SLCSLDSLBSLASLFSLA5599SLCSLE84194SLESLF70822SLFSLASLF5SLD4SL|10=0SL|11=0SL|12=0SL|13=20170627130633SL|14=1SL|15=20250217081252SL|16=616SL|17=0SL|18=SL3SLTSLBSLD5SLD8SLA6SLBSLE435SLESLF7SLBSL|19=SL3SLT143SLD5SLFSLESLD3SLB4SLESLC880SL|SL~20250130164956:366:10400832:::1SL~20250130163305:366:10400832:::1SL~20250120095610:333:10399852:::1SL~08SLBSLD5SLASLC1SLC30SLF09410878SLD194489SLBSLB717010SLC5390SLD9SLESLBSLC71SLD272265683SLD45346SLC","1.3.0~2502170003467091~2;~1=20161001004719|2=1|3=0|4=#ABCDEFGHIJKLMNOPQRSTUWVXYZ|5=0|6=20250217|7=|8=0|9=#TB4CDBAFA5599CE84194EF70822FAF5D4|10=0|11=0|12=0|13=20170627130633|14=1|15=20250217081252|16=616|17=0|18=#TBD5D8A6BE435EF7B|19=#T143D5FED3B4EC880|~20250130164956;366;10400832;;;1~20250130163305;366;10400832;;;1~20250120095610;333;10399852;;;1~08BD5AC1C30F09410878D194489BB717010C5390D9EBC71D272265683D45346C") (0ms)
    Assert.That(input, Is.EqualTo(originalInput))
  Expected string length 413 but was 627. Strings differ at index 5.
  Expected: "1.3.0~2502170003467091~2;~1=20161001004719|2=1|3=0|4=#ABCDEFG..."
  But was:  "1.3.0SL~2502170003467091SL~2:SL~1=20161001004719SL|2=1SL|3=0S..."
  ----------------^

  from C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\bin\Debug\net10.0\NunitTests.dll (net10.0|x64)
    Assert.That(input, Is.EqualTo(originalInput))
    Expected string length 413 but was 627. Strings differ at index 5.
    Expected: "1.3.0~2502170003467091~2;~1=20161001004719|2=1|3=0|4=#ABCDEFG..."
    But was:  "1.3.0SL~2502170003467091SL~2:SL~1=20161001004719SL|2=1SL|3=0S..."
    ----------------^
  
    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    1)    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:line 229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    
failed ConvertBsQRCodeToOriginal_Valid("1Ð»3Ð»0)2510010003529426)2Ð¼)1.20250902173601â€œ2.1â€œ3.0â€œ4.+Ð¨302Ð•5Ñ07ÐžÐªÐ•23278629089Ð•Ð¤2Ð¤ÐžÐ•Ð•705â€œ5.0â€œ6.20251001â€œ8.0â€œ9.324444365939â€œ10.0â€œ11.0â€œ12.0â€œ13.20250902173601â€œ14.1â€œ15.20251001150710â€œ16.616â€œ17.0â€œ20.269â€œ)20251001144635Ð¼10002861Ð¼10463690Ð¼Ð¼Ð¼1)20251001123158Ð¼10002861Ð¼10463690Ð¼Ð¼Ð¼1)ÑÐž06ÑÑÐ•87788Ð•ÐžÐžÐžÑ2Ñ4Ðª4Ð•06294539Ð¤1Ðž861ÐªÐ¤4Ð¤4164Ð•ÐªÐÐž54Ñ739906692942","1.3.0~2510010003529426~2;~1=20250902173601|2=1|3=0|4=#T302E5A07FCE23278629089EB2BFEE705|5=0|6=20251001|8=0|9=324444365939|10=0|11=0|12=0|13=20250902173601|14=1|15=20251001150710|16=616|17=0|20=269|~20251001144635;10002861;10463690;;;1~20251001123158;10002861;10463690;;;1~AF06AAE87788EFFFA2A4C4E06294539B1F861CB4B4164ECDF54A739906692942") (0ms)
    Assert.That(input, Is.EqualTo(originalInput))
  String lengths are both 336. Strings differ at index 1.
  Expected: "1.3.0~2510010003529426~2;~1=20250902173601|2=1|3=0|4=#T302E5A..."
  But was:  "1Ð»3Ð»0)2510010003529426)2Ð¼)1.20250902173601â€œ2.1â€œ3.0â€œ4.+Ð¨302Ð•5Ñ..."
  ------------^

  from C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\bin\Debug\net10.0\NunitTests.dll (net10.0|x64)
    Assert.That(input, Is.EqualTo(originalInput))
    String lengths are both 336. Strings differ at index 1.
    Expected: "1.3.0~2510010003529426~2;~1=20250902173601|2=1|3=0|4=#T302E5A..."
    But was:  "1Ð»3Ð»0)2510010003529426)2Ð¼)1.20250902173601â€œ2.1â€œ3.0â€œ4.+Ð¨302Ð•5Ñ..."
    ------------^
  
    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    1)    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:line 229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    
failed ConvertBsQRCodeToOriginal_Valid("1Ð»3Ð»0)2502170003467091)2Ð¼)1.20161001004719â€œ2.1â€œ3.0â€œ4.+ÑÐ¤ÐªÐÐ•ÐžÐ–Ð“Ð¡Ð¢ÐÐ’ÐŸÐ¥Ð”Ð—Ñ‹Ð˜Ð¯Ð¨ÐšÐ£Ð­Ð™Ð©Ð®â€œ5.0â€œ6.20250217â€œ7.â€œ8.0â€œ9.+Ð¨Ð¤4ÐªÐÐ¤ÑÐžÑ5599ÐªÐ•84194Ð•Ðž70822ÐžÑÐž5Ð4â€œ10.0â€œ11.0â€œ12.0â€œ13.20170627130633â€œ14.1â€œ15.20250217081252â€œ16.616â€œ17.0â€œ18.+Ð¨Ð¤Ð5Ð8Ñ6Ð¤Ð•435Ð•Ðž7Ð¤â€œ19.+Ð¨143Ð5ÐžÐ•Ð3Ð¤4Ð•Ðª880â€œ)20250130164956Ð¼366Ð¼10400832Ð¼Ð¼Ð¼1)20250130163305Ð¼366Ð¼10400832Ð¼Ð¼Ð¼1)20250120095610Ð¼333Ð¼10399852Ð¼Ð¼Ð¼1)08Ð¤Ð5ÑÐª1Ðª30Ðž09410878Ð194489Ð¤Ð¤717010Ðª5390Ð9Ð•Ð¤Ðª71Ð272265683Ð45346Ðª","1.3.0~2502170003467091~2;~1=20161001004719|2=1|3=0|4=#ABCDEFGHIJKLMNOPQRSTUWVXYZ|5=0|6=20250217|7=|8=0|9=#TB4CDBAFA5599CE84194EF70822FAF5D4|10=0|11=0|12=0|13=20170627130633|14=1|15=20250217081252|16=616|17=0|18=#TBD5D8A6BE435EF7B|19=#T143D5FED3B4EC880|~20250130164956;366;10400832;;;1~20250130163305;366;10400832;;;1~20250120095610;333;10399852;;;1~08BD5AC1C30F09410878D194489BB717010C5390D9EBC71D272265683D45346C") (0ms)
    Assert.That(input, Is.EqualTo(originalInput))
  String lengths are both 413. Strings differ at index 1.
  Expected: "1.3.0~2502170003467091~2;~1=20161001004719|2=1|3=0|4=#ABCDEFG..."
  But was:  "1Ð»3Ð»0)2502170003467091)2Ð¼)1.20161001004719â€œ2.1â€œ3.0â€œ4.+ÑÐ¤ÐªÐÐ•ÐžÐ–..."
  ------------^

  from C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\bin\Debug\net10.0\NunitTests.dll (net10.0|x64)
    Assert.That(input, Is.EqualTo(originalInput))
    String lengths are both 413. Strings differ at index 1.
    Expected: "1.3.0~2502170003467091~2;~1=20161001004719|2=1|3=0|4=#ABCDEFG..."
    But was:  "1Ð»3Ð»0)2502170003467091)2Ð¼)1.20161001004719â€œ2.1â€œ3.0â€œ4.+ÑÐ¤ÐªÐÐ•ÐžÐ–..."
    ------------^
  
    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    1)    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:line 229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    
failed ConvertBsQRCodeToOriginal_Valid("1Ð»3Ð»0)2502170003467091)2Ð¼)1.20161001004719â€œ2.1â€œ3.0â€œ4.+ÑÐ¤ÐªÐÐ•ÐžÐ–Ð“Ð¡Ð¢ÐÐ’ÐŸÐ¥Ð”Ð—Ñ‹Ð˜Ð¯Ð¨ÐšÐ£Ð­Ð™Ð©Ð®â€œ5.0â€œ6.20250217â€œ7.â€œ8.0â€œ9.+Ð¨Ð¤4ÐªÐÐ¤ÑÐžÑ5599ÐªÐ•84194Ð•Ðž70822ÐžÑÐž5Ð4â€œ10.0â€œ11.0â€œ12.0â€œ13.20170627130633â€œ14.1â€œ15.20250217081252â€œ16.616â€œ17.0â€œ18.+Ð¨Ð¤Ð5Ð8Ñ6Ð¤Ð•435Ð•Ðž7Ð¤â€œ19.+Ð¨143Ð5ÐžÐ•Ð3Ð¤4Ð•Ðª880â€œ)20250130164956Ð¼366Ð¼10400832Ð¼Ð¼Ð¼1)20250130163305Ð¼366Ð¼10400832Ð¼Ð¼Ð¼1)20250120095610Ð¼333Ð¼10399852Ð¼Ð¼Ð¼1)08Ð¤Ð5ÑÐª1Ðª30Ðž09410878Ð194489Ð¤Ð¤717010Ðª5390Ð9Ð•Ð¤Ðª71Ð272265683Ð45346Ðª1Ð»3Ð»0)2502170003467091)2Ð¼)1.20161001004719â€œ2.1â€œ3.0â€œ4.+ÑÐ¤ÐªÐÐ•ÐžÐ–Ð“Ð¡Ð¢ÐÐ’ÐŸÐ¥Ð”Ð—Ñ‹Ð˜Ð¯Ð¨ÐšÐ£Ð­Ð™Ð©Ð®â€œ5.0â€œ6.20250217â€œ7.â€œ8.0â€œ9.+Ð¨Ð¤4ÐªÐÐ¤ÑÐžÑ5599ÐªÐ•84194Ð•Ðž70822ÐžÑÐž5Ð4â€œ10.0â€œ11.0â€œ12.0â€œ13.20170627130633â€œ14.1â€œ15.20250217081252â€œ16.616â€œ17.0","1.3.0~2502170003467091~2;~1=20161001004719|2=1|3=0|4=#ABCDEFGHIJKLMNOPQRSTUWVXYZ|5=0|6=20250217|7=|8=0|9=#TB4CDBAFA5599CE84194EF70822FAF5D4|10=0|11=0|12=0|13=20170627130633|14=1|15=20250217081252|16=616|17=0|18=#TBD5D8A6BE435EF7B|19=#T143D5FED3B4EC880|~20250130164956;366;10400832;;;1~20250130163305;366;10400832;;;1~20250120095610;333;10399852;;;1~08BD5AC1C30F09410878D194489BB717010C5390D9EBC71D272265683D45346C") (0ms)
    Assert.That(input, Is.EqualTo(originalInput))
  Expected string length 413 but was 620. Strings differ at index 1.
  Expected: "1.3.0~2502170003467091~2;~1=20161001004719|2=1|3=0|4=#ABCDEFG..."
  But was:  "1Ð»3Ð»0)2502170003467091)2Ð¼)1.20161001004719â€œ2.1â€œ3.0â€œ4.+ÑÐ¤ÐªÐÐ•ÐžÐ–..."
  ------------^

  from C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\bin\Debug\net10.0\NunitTests.dll (net10.0|x64)
    Assert.That(input, Is.EqualTo(originalInput))
    Expected string length 413 but was 620. Strings differ at index 1.
    Expected: "1.3.0~2502170003467091~2;~1=20161001004719|2=1|3=0|4=#ABCDEFG..."
    But was:  "1Ð»3Ð»0)2502170003467091)2Ð¼)1.20161001004719â€œ2.1â€œ3.0â€œ4.+ÑÐ¤ÐªÐÐ•ÐžÐ–..."
    ------------^
  
    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    1)    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:line 229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    
failed ConvertBsQRCodeToOriginal_Valid("1.3.0~2510140003538073~2;~1=20240717143601|2=1|3=0|4=#T4E973DFA373F613C1CBED367A128A637|5=0|6=20251014|8=1|9=319374223583|10=0|11=0|12=0|13=20240717143601|14=1|15=20251014090407|16=616|17=0|20=10000109|~20251013152024;10002861;10463687;;;1~20251013150914;10002861;10463687;;;1~55B700A1B6B05952664A2F986A727DF3A7D2DE5FA9762E51B127E2E4F68F1F671.3.0~2510140003538073~2;~1=20240717143601|2=1|3=0|4=#T4E973DFA373F613C1CBED367A128A637|5=0|6=20251014|8=1|9=319374223583|10=0|11=0|12=0|13=20240717143601|14=1|15=20251014090407|16=616|17=0|20=10000109|~20251013152024;10002861;10463687;;;1~20251013150914;10002861;10463687;;;1~55B700A1B6B05952664A2F986A727DF3A7D2DE5FA9762E51B127E2E4F68F1F67","1.3.0~2510140003538073~2;~1=20240717143601|2=1|3=0|4=#T4E973DFA373F613C1CBED367A128A637|5=0|6=20251014|8=1|9=319374223583|10=0|11=0|12=0|13=20240717143601|14=1|15=20251014090407|16=616|17=0|20=10000109|~20251013152024;10002861;10463687;;;1~20251013150914;10002861;10463687;;;1~55B700A1B6B05952664A2F986A727DF3A7D2DE5FA9762E51B127E2E4F68F1F67") (0ms)
    Assert.That(input, Is.EqualTo(originalInput))
  Expected string length 341 but was 682. Strings differ at index 341.
  Expected: "...2DE5FA9762E51B127E2E4F68F1F67"
  But was:  "...2DE5FA9762E51B127E2E4F68F1F671.3.0~2510140003538073~2;~1=2..."
  -------------------------------------------^

  from C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\bin\Debug\net10.0\NunitTests.dll (net10.0|x64)
    Assert.That(input, Is.EqualTo(originalInput))
    Expected string length 341 but was 682. Strings differ at index 341.
    Expected: "...2DE5FA9762E51B127E2E4F68F1F67"
    But was:  "...2DE5FA9762E51B127E2E4F68F1F671.3.0~2510140003538073~2;~1=2..."
    -------------------------------------------^
  
    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    1)    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:line 229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    
failed ConvertBsQRCodeToOriginal_Valid("1Ð»3Ð»0~2510100003536386~2Ð¼~1.20240717143601)2.1)3.0)4.+Ð¨Ð¤9469Ðª453879ÐÐ¬4Ðª1Ð•743303ÐžÐ¤Ð¤8Ð¤73Ðª)5.0)6.20251010)8.1)9.319374223583)10.0)11.0)12.0)13.20240717143601)14.1)15.20251010120108)16.616)17.0)20.10000109)~20251010120023Ð¼10002861Ð¼10463690Ð¼Ð¼Ð¼1~20251010120016Ð¼10002861Ð¼10463690Ð¼Ð¼Ð¼1~4ÐªÐª85ÐÐ59Ð26Ð¬Ð¤Ð¤06492ÐÐ6Ð238Ð•Ðž79Ð¤9Ð3ÐªÐ¤Ð•0872Ð8Ð9047354Ðž1016Ð¤3Ð¬7027","1.3.0~2510100003536386~2;~1=20240717143601|2=1|3=0|4=#TB9469C453879DA4C1E743303FBB8B73C|5=0|6=20251010|8=1|9=319374223583|10=0|11=0|12=0|13=20240717143601|14=1|15=20251010120108|16=616|17=0|20=10000109|~20251010120023;10002861;10463690;;;1~20251010120016;10002861;10463690;;;1~4CC85DD59D26ABB06492DD6D238EF79B9D3CBE0872D8D9047354F1016B3A7027") (0ms)
    Assert.That(input, Is.EqualTo(originalInput))
  String lengths are both 341. Strings differ at index 1.
  Expected: "1.3.0~2510100003536386~2;~1=20240717143601|2=1|3=0|4=#TB9469C..."
  But was:  "1Ð»3Ð»0~2510100003536386~2Ð¼~1.20240717143601)2.1)3.0)4.+Ð¨Ð¤9469Ðª..."
  ------------^

  from C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\bin\Debug\net10.0\NunitTests.dll (net10.0|x64)
    Assert.That(input, Is.EqualTo(originalInput))
    String lengths are both 341. Strings differ at index 1.
    Expected: "1.3.0~2510100003536386~2;~1=20240717143601|2=1|3=0|4=#TB9469C..."
    But was:  "1Ð»3Ð»0~2510100003536386~2Ð¼~1.20240717143601)2.1)3.0)4.+Ð¨Ð¤9469Ðª..."
    ------------^
  
    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    1)    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:line 229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    
failed ConvertBsQRCodeToOriginal_Valid("1Ð»3Ð»0)2510100003536386)2Ð¼)1.20240717143601â€œ2.1â€œ3.0â€œ4.+Ð¨Ð¤9469Ðª453879ÐÑ4Ðª1Ð•743303ÐžÐ¤Ð¤8Ð¤73Ðªâ€œ5.0â€œ6.20251010â€œ8.1â€œ9.319374223583â€œ10.0â€œ11.0â€œ12.0â€œ13.20240717143601â€œ14.1â€œ15.20251010120108â€œ16.616â€œ17.0â€œ20.10000109â€œ)20251010120023Ð¼10002861Ð¼10463690Ð¼Ð¼Ð¼1)20251010120016Ð¼10002861Ð¼10463690Ð¼Ð¼Ð¼1)4ÐªÐª85ÐÐ59Ð26ÑÐ¤Ð¤06492ÐÐ6Ð238Ð•Ðž79Ð¤9Ð3ÐªÐ¤Ð•0872Ð8Ð9047354Ðž1016Ð¤3Ñ7027","1.3.0~2510100003536386~2;~1=20240717143601|2=1|3=0|4=#TB9469C453879DA4C1E743303FBB8B73C|5=0|6=20251010|8=1|9=319374223583|10=0|11=0|12=0|13=20240717143601|14=1|15=20251010120108|16=616|17=0|20=10000109|~20251010120023;10002861;10463690;;;1~20251010120016;10002861;10463690;;;1~4CC85DD59D26ABB06492DD6D238EF79B9D3CBE0872D8D9047354F1016B3A7027") (0ms)
    Assert.That(input, Is.EqualTo(originalInput))
  String lengths are both 341. Strings differ at index 1.
  Expected: "1.3.0~2510100003536386~2;~1=20240717143601|2=1|3=0|4=#TB9469C..."
  But was:  "1Ð»3Ð»0)2510100003536386)2Ð¼)1.20240717143601â€œ2.1â€œ3.0â€œ4.+Ð¨Ð¤9469Ðª..."
  ------------^

  from C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\bin\Debug\net10.0\NunitTests.dll (net10.0|x64)
    Assert.That(input, Is.EqualTo(originalInput))
    String lengths are both 341. Strings differ at index 1.
    Expected: "1.3.0~2510100003536386~2;~1=20240717143601|2=1|3=0|4=#TB9469C..."
    But was:  "1Ð»3Ð»0)2510100003536386)2Ð¼)1.20240717143601â€œ2.1â€œ3.0â€œ4.+Ð¨Ð¤9469Ðª..."
    ------------^
  
    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    1)    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:line 229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    
failed ConvertBsQRCodeToOriginal_Valid("1Ã§3Ã§0Ã©2510100003536386Ã©2ÅŸÃ©1-20240717143601;2-1;3-0;4-^TB9469C453879DA4C1E743303FBB8B73C;5-0;6-20251010;8-1;9-319374223583;10-0;11-0;12-0;13-20240717143601;14-1;15-20251010120108;16-616;17-0;20-10000109;Ã©20251010120023ÅŸ10002861ÅŸ10463690ÅŸÅŸÅŸ1Ã©20251010120016ÅŸ10002861ÅŸ10463690ÅŸÅŸÅŸ1Ã©4CC85DD59D26ABB06492DD6D238EF79B9D3CBE0872D8D9047354F1016B3A7027","1.3.0~2510100003536386~2;~1=20240717143601|2=1|3=0|4=#TB9469C453879DA4C1E743303FBB8B73C|5=0|6=20251010|8=1|9=319374223583|10=0|11=0|12=0|13=20240717143601|14=1|15=20251010120108|16=616|17=0|20=10000109|~20251010120023;10002861;10463690;;;1~20251010120016;10002861;10463690;;;1~4CC85DD59D26ABB06492DD6D238EF79B9D3CBE0872D8D9047354F1016B3A7027") (0ms)
    Assert.That(input, Is.EqualTo(originalInput))
  String lengths are both 341. Strings differ at index 1.
  Expected: "1.3.0~2510100003536386~2;~1=20240717143601|2=1|3=0|4=#TB9469C..."
  But was:  "1Ã§3Ã§0Ã©2510100003536386Ã©2ÅŸÃ©1-20240717143601;2-1;3-0;4-^TB9469C..."
  ------------^

  from C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\bin\Debug\net10.0\NunitTests.dll (net10.0|x64)
    Assert.That(input, Is.EqualTo(originalInput))
    String lengths are both 341. Strings differ at index 1.
    Expected: "1.3.0~2510100003536386~2;~1=20240717143601|2=1|3=0|4=#TB9469C..."
    But was:  "1Ã§3Ã§0Ã©2510100003536386Ã©2ÅŸÃ©1-20240717143601;2-1;3-0;4-^TB9469C..."
    ------------^
  
    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    1)    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:line 229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    
failed ConvertBsQRCodeToOriginal_Valid("1.3.0>2510280003552482>2Ä>1+20250902173601Å½2+1Å½3+0Å½4+#T00DBEB78C4C70CF1A6EB970A30C2B80EÅ½5+0Å½6+20251028Å½8+0Å½9+324444352748Å½10+0Å½11+0Å½12+0Å½13+20250902173601Å½14+1Å½15+20251028102356Å½16+616Å½17+0Å½20+268Å½>20251028092216Ä10002861Ä10463687ÄÄÄ1>20251023143752Ä10002861Ä10463689ÄÄÄ1>D1701073EF7A70ACD897BE74951CA8A59B2AEBE9678BC0C5D7B735568BA1FC9C","1.3.0~2510280003552482~2;~1=20250902173601|2=1|3=0|4=#T00DBEB78C4C70CF1A6EB970A30C2B80E|5=0|6=20251028|8=0|9=324444352748|10=0|11=0|12=0|13=20250902173601|14=1|15=20251028102356|16=616|17=0|20=268|~20251028092216;10002861;10463687;;;1~20251023143752;10002861;10463689;;;1~D1701073EF7A70ACD897BE74951CA8A59B2AEBE9678BC0C5D7B735568BA1FC9C") (0ms)
    Assert.That(input, Is.EqualTo(originalInput))
  String lengths are both 336. Strings differ at index 5.
  Expected: "1.3.0~2510280003552482~2;~1=20250902173601|2=1|3=0|4=#T00DBEB..."
  But was:  "1.3.0>2510280003552482>2Ä>1+20250902173601Å½2+1Å½3+0Å½4+#T00DBEB..."
  ----------------^

  from C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\bin\Debug\net10.0\NunitTests.dll (net10.0|x64)
    Assert.That(input, Is.EqualTo(originalInput))
    String lengths are both 336. Strings differ at index 5.
    Expected: "1.3.0~2510280003552482~2;~1=20250902173601|2=1|3=0|4=#T00DBEB..."
    But was:  "1.3.0>2510280003552482>2Ä>1+20250902173601Å½2+1Å½3+0Å½4+#T00DBEB..."
    ----------------^
  
    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    1)    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:line 229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    
failed ConvertBsQRCodeToOriginal_Valid("1.3.0>2510280003552482>2Ä>1+20250902173601Å½2+1Å½3+0Å½4+#T00DBEB78C4C70CF1A6EB970A30C2B80EÅ½5+0Å½6+20251028Å½8+0Å½9+324444352748Å½10+0Å½11+0Å½12+0Å½13+20250902173601Å½14+1Å½15+20251028102356Å½16+616Å½17+0Å½20+268Å½>20251028092216Ä10002861Ä10463687ÄÄÄ1>20251023143752Ä10002861Ä10463689ÄÄÄ1>D1701073EF7A70ACD897BE74951CA8A59B2AEBE9678BC0C5D7B735568BA1FC9C","1.3.0~2510280003552482~2;~1=20250902173601|2=1|3=0|4=#T00DBEB78C4C70CF1A6EB970A30C2B80E|5=0|6=20251028|8=0|9=324444352748|10=0|11=0|12=0|13=20250902173601|14=1|15=20251028102356|16=616|17=0|20=268|~20251028092216;10002861;10463687;;;1~20251023143752;10002861;10463689;;;1~D1701073EF7A70ACD897BE74951CA8A59B2AEBE9678BC0C5D7B735568BA1FC9C") (0ms)
    Assert.That(input, Is.EqualTo(originalInput))
  String lengths are both 336. Strings differ at index 5.
  Expected: "1.3.0~2510280003552482~2;~1=20250902173601|2=1|3=0|4=#T00DBEB..."
  But was:  "1.3.0>2510280003552482>2Ä>1+20250902173601Å½2+1Å½3+0Å½4+#T00DBEB..."
  ----------------^

  from C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\bin\Debug\net10.0\NunitTests.dll (net10.0|x64)
    Assert.That(input, Is.EqualTo(originalInput))
    String lengths are both 336. Strings differ at index 5.
    Expected: "1.3.0~2510280003552482~2;~1=20250902173601|2=1|3=0|4=#T00DBEB..."
    But was:  "1.3.0>2510280003552482>2Ä>1+20250902173601Å½2+1Å½3+0Å½4+#T00DBEB..."
    ----------------^
  
    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    1)    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:line 229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    
failed ConvertBsQRCodeToOriginal_Valid("+.Å¡.Ã©|Ä›Å™+Ã©Ä›Ã¡Ã©Ã©Ã©Å¡Å™Å™Ä›ÄÃ¡Ä›|Ä›Å¯|+'Ä›Ã©Ä›Å™Ã©Ã­Ã©Ä›+Ã½Å¡Å¾Ã©+`Ä›'+`Å¡'Ã©`Ä'3TÃ©Ã©DBEBÃ½Ã¡CÄCÃ½Ã©CF+AÅ¾EBÃ­Ã½Ã©AÅ¡Ã©CÄ›BÃ¡Ã©E`Å™'Ã©`Å¾'Ä›Ã©Ä›Å™+Ã©Ä›Ã¡`Ã¡'Ã©`Ã­'Å¡Ä›ÄÄÄÄÅ¡Å™Ä›Ã½ÄÃ¡`+Ã©'Ã©`++'Ã©`+Ä›'Ã©`+Å¡'Ä›Ã©Ä›Å™Ã©Ã­Ã©Ä›+Ã½Å¡Å¾Ã©+`+Ä'+`+Å™'Ä›Ã©Ä›Å™+Ã©Ä›Ã¡+Ã©Ä›Å¡Å™Å¾`+Å¾'Å¾+Å¾`+Ã½'Ã©`Ä›Ã©'Ä›Å¾Ã¡`|Ä›Ã©Ä›Å™+Ã©Ä›Ã¡Ã©Ã­Ä›Ä›+Å¾Å¯+Ã©Ã©Ã©Ä›Ã¡Å¾+Å¯+Ã©ÄÅ¾Å¡Å¾Ã¡Ã½Å¯Å¯Å¯+|Ä›Ã©Ä›Å™+Ã©Ä›Å¡+ÄÅ¡Ã½Å™Ä›Å¯+Ã©Ã©Ã©Ä›Ã¡Å¾+Å¯+Ã©ÄÅ¾Å¡Å¾Ã¡Ã­Å¯Å¯Å¯+|D+Ã½Ã©+Ã©Ã½Å¡EFÃ½AÃ½Ã©ACDÃ¡Ã­Ã½BEÃ½ÄÃ­Å™+CAÃ¡AÅ™Ã­BÄ›AEBEÃ­Å¾Ã½Ã¡BCÃ©CÅ™DÃ½BÃ½Å¡Å™Å™Å¾Ã¡BA+FCÃ­C","1.3.0~2510280003552482~2;~1=20250902173601|2=1|3=0|4=#T00DBEB78C4C70CF1A6EB970A30C2B80E|5=0|6=20251028|8=0|9=324444352748|10=0|11=0|12=0|13=20250902173601|14=1|15=20251028102356|16=616|17=0|20=268|~20251028092216;10002861;10463687;;;1~20251023143752;10002861;10463689;;;1~D1701073EF7A70ACD897BE74951CA8A59B2AEBE9678BC0C5D7B735568BA1FC9C") (0ms)
    Assert.That(input, Is.EqualTo(originalInput))
  String lengths are both 336. Strings differ at index 0.
  Expected: "1.3.0~2510280003552482~2;~1=20250902173601|2=1|3=0|4=#T00DBEB..."
  But was:  "+.Å¡.Ã©|Ä›Å™+Ã©Ä›Ã¡Ã©Ã©Ã©Å¡Å™Å™Ä›ÄÃ¡Ä›|Ä›Å¯|+'Ä›Ã©Ä›Å™Ã©Ã­Ã©Ä›+Ã½Å¡Å¾Ã©+`Ä›'+`Å¡'Ã©`Ä'3TÃ©Ã©DBEB..."
  -----------^

  from C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\bin\Debug\net10.0\NunitTests.dll (net10.0|x64)
    Assert.That(input, Is.EqualTo(originalInput))
    String lengths are both 336. Strings differ at index 0.
    Expected: "1.3.0~2510280003552482~2;~1=20250902173601|2=1|3=0|4=#T00DBEB..."
    But was:  "+.Å¡.Ã©|Ä›Å™+Ã©Ä›Ã¡Ã©Ã©Ã©Å¡Å™Å™Ä›ÄÃ¡Ä›|Ä›Å¯|+'Ä›Ã©Ä›Å™Ã©Ã­Ã©Ä›+Ã½Å¡Å¾Ã©+`Ä›'+`Å¡'Ã©`Ä'3TÃ©Ã©DBEB..."
    -----------^
  
    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    1)    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:line 229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    
failed ConvertBsQRCodeToOriginal_Valid("+.Å¡.Ã©|Ä›Å™+Ã©Ä›Ã¡Ã©Ã©Ã©Å¡Å™Å™Ä›ÄÃ¡Ä›|Ä›Å¯|+'Ä›Ã©Ä›Å™Ã©Ã­Ã©Ä›+Ã½Å¡Å¾Ã©+`Ä›'+`Å¡'Ã©`Ä'3TÃ©Ã©DBEBÃ½Ã¡CÄCÃ½Ã©CF+AÅ¾EBÃ­Ã½Ã©AÅ¡Ã©CÄ›BÃ¡Ã©E`Å™'Ã©`Å¾'Ä›Ã©Ä›Å™+Ã©Ä›Ã¡`Ã¡'Ã©`Ã­'Å¡Ä›ÄÄÄÄÅ¡Å™Ä›Ã½ÄÃ¡`+Ã©'Ã©`++'Ã©`+Ä›'Ã©`+Å¡'Ä›Ã©Ä›Å™Ã©Ã­Ã©Ä›+Ã½Å¡Å¾Ã©+`+Ä'+`+Å™'Ä›Ã©Ä›Å™+Ã©Ä›Ã¡+Ã©Ä›Å¡Å™Å¾`+Å¾'Å¾+Å¾`+Ã½'Ã©`Ä›Ã©'Ä›Å¾Ã¡`|Ä›Ã©Ä›Å™+Ã©Ä›Ã¡Ã©Ã­Ä›Ä›+Å¾Å¯+Ã©Ã©Ã©Ä›Ã¡Å¾+Å¯+Ã©ÄÅ¾Å¡Å¾Ã¡Ã½Å¯Å¯Å¯+|Ä›Ã©Ä›Å™+Ã©Ä›Å¡+ÄÅ¡Ã½Å™Ä›Å¯+Ã©Ã©Ã©Ä›Ã¡Å¾+Å¯+Ã©ÄÅ¾Å¡Å¾Ã¡Ã­Å¯Å¯Å¯+|D+Ã½Ã©+Ã©Ã½Å¡EFÃ½AÃ½Ã©ACDÃ¡Ã­Ã½BEÃ½ÄÃ­Å™+CAÃ¡AÅ™Ã­BÄ›AEBEÃ­Å¾Ã½Ã¡BCÃ©CÅ™DÃ½BÃ½Å¡Å™Å™Å¾Ã¡BA+FCÃ­C","1.3.0~2510280003552482~2;~1=20250902173601|2=1|3=0|4=#T00DBEB78C4C70CF1A6EB970A30C2B80E|5=0|6=20251028|8=0|9=324444352748|10=0|11=0|12=0|13=20250902173601|14=1|15=20251028102356|16=616|17=0|20=268|~20251028092216;10002861;10463687;;;1~20251023143752;10002861;10463689;;;1~D1701073EF7A70ACD897BE74951CA8A59B2AEBE9678BC0C5D7B735568BA1FC9C") (0ms)
    Assert.That(input, Is.EqualTo(originalInput))
  String lengths are both 336. Strings differ at index 0.
  Expected: "1.3.0~2510280003552482~2;~1=20250902173601|2=1|3=0|4=#T00DBEB..."
  But was:  "+.Å¡.Ã©|Ä›Å™+Ã©Ä›Ã¡Ã©Ã©Ã©Å¡Å™Å™Ä›ÄÃ¡Ä›|Ä›Å¯|+'Ä›Ã©Ä›Å™Ã©Ã­Ã©Ä›+Ã½Å¡Å¾Ã©+`Ä›'+`Å¡'Ã©`Ä'3TÃ©Ã©DBEB..."
  -----------^

  from C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\bin\Debug\net10.0\NunitTests.dll (net10.0|x64)
    Assert.That(input, Is.EqualTo(originalInput))
    String lengths are both 336. Strings differ at index 0.
    Expected: "1.3.0~2510280003552482~2;~1=20250902173601|2=1|3=0|4=#T00DBEB..."
    But was:  "+.Å¡.Ã©|Ä›Å™+Ã©Ä›Ã¡Ã©Ã©Ã©Å¡Å™Å™Ä›ÄÃ¡Ä›|Ä›Å¯|+'Ä›Ã©Ä›Å™Ã©Ã­Ã©Ä›+Ã½Å¡Å¾Ã©+`Ä›'+`Å¡'Ã©`Ä'3TÃ©Ã©DBEB..."
    -----------^
  
    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    1)    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:line 229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    
failed ConvertBsQRCodeToOriginal_Valid("+.Å¡.Ã©|Ä¾Å¥+Ã©Ä¾Ã¡Ã©Ã©Ã©Å¡Å¥Å¥Ä¾ÄÃ¡Ä¾|Ä¾Ã´|+'Ä¾Ã©Ä¾Å¥Ã©Ã­Ã©Ä¾+Ã½Å¡Å¾Ã©+)Ä¾'+)Å¡'Ã©)Ä'3TÃ©Ã©DBEBÃ½Ã¡CÄCÃ½Ã©CF+AÅ¾EBÃ­Ã½Ã©AÅ¡Ã©CÄ¾BÃ¡Ã©E)Å¥'Ã©)Å¾'Ä¾Ã©Ä¾Å¥+Ã©Ä¾Ã¡)Ã¡'Ã©)Ã­'Å¡Ä¾ÄÄÄÄÅ¡Å¥Ä¾Ã½ÄÃ¡)+Ã©'Ã©)++'Ã©)+Ä¾'Ã©)+Å¡'Ä¾Ã©Ä¾Å¥Ã©Ã­Ã©Ä¾+Ã½Å¡Å¾Ã©+)+Ä'+)+Å¥'Ä¾Ã©Ä¾Å¥+Ã©Ä¾Ã¡+Ã©Ä¾Å¡Å¥Å¾)+Å¾'Å¾+Å¾)+Ã½'Ã©)Ä¾Ã©'Ä¾Å¾Ã¡)|Ä¾Ã©Ä¾Å¥+Ã©Ä¾Ã¡Ã©Ã­Ä¾Ä¾+Å¾Ã´+Ã©Ã©Ã©Ä¾Ã¡Å¾+Ã´+Ã©ÄÅ¾Å¡Å¾Ã¡Ã½Ã´Ã´Ã´+|Ä¾Ã©Ä¾Å¥+Ã©Ä¾Å¡+ÄÅ¡Ã½Å¥Ä¾Ã´+Ã©Ã©Ã©Ä¾Ã¡Å¾+Ã´+Ã©ÄÅ¾Å¡Å¾Ã¡Ã­Ã´Ã´Ã´+|D+Ã½Ã©+Ã©Ã½Å¡EFÃ½AÃ½Ã©ACDÃ¡Ã­Ã½BEÃ½ÄÃ­Å¥+CAÃ¡AÅ¥Ã­BÄ¾AEBEÃ­Å¾Ã½Ã¡BCÃ©CÅ¥DÃ½BÃ½Å¡Å¥Å¥Å¾Ã¡BA+FCÃ­C","1.3.0~2510280003552482~2;~1=20250902173601|2=1|3=0|4=#T00DBEB78C4C70CF1A6EB970A30C2B80E|5=0|6=20251028|8=0|9=324444352748|10=0|11=0|12=0|13=20250902173601|14=1|15=20251028102356|16=616|17=0|20=268|~20251028092216;10002861;10463687;;;1~20251023143752;10002861;10463689;;;1~D1701073EF7A70ACD897BE74951CA8A59B2AEBE9678BC0C5D7B735568BA1FC9C") (0ms)
    Assert.That(input, Is.EqualTo(originalInput))
  String lengths are both 336. Strings differ at index 0.
  Expected: "1.3.0~2510280003552482~2;~1=20250902173601|2=1|3=0|4=#T00DBEB..."
  But was:  "+.Å¡.Ã©|Ä¾Å¥+Ã©Ä¾Ã¡Ã©Ã©Ã©Å¡Å¥Å¥Ä¾ÄÃ¡Ä¾|Ä¾Ã´|+'Ä¾Ã©Ä¾Å¥Ã©Ã­Ã©Ä¾+Ã½Å¡Å¾Ã©+)Ä¾'+)Å¡'Ã©)Ä'3TÃ©Ã©DBEB..."
  -----------^

  from C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\bin\Debug\net10.0\NunitTests.dll (net10.0|x64)
    Assert.That(input, Is.EqualTo(originalInput))
    String lengths are both 336. Strings differ at index 0.
    Expected: "1.3.0~2510280003552482~2;~1=20250902173601|2=1|3=0|4=#T00DBEB..."
    But was:  "+.Å¡.Ã©|Ä¾Å¥+Ã©Ä¾Ã¡Ã©Ã©Ã©Å¡Å¥Å¥Ä¾ÄÃ¡Ä¾|Ä¾Ã´|+'Ä¾Ã©Ä¾Å¥Ã©Ã­Ã©Ä¾+Ã½Å¡Å¾Ã©+)Ä¾'+)Å¡'Ã©)Ä'3TÃ©Ã©DBEB..."
    -----------^
  
    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    1)    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:line 229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    
failed ConvertBsQRCodeToOriginal_Valid("+.Å¡.Ã©|Ä¾Å¥+Ã©Ä¾Ã¡Ã©Ã©Ã©Å¡Å¥Å¥Ä¾ÄÃ¡Ä¾|Ä¾Ã´|+'Ä¾Ã©Ä¾Å¥Ã©Ã­Ã©Ä¾+Ã½Å¡Å¾Ã©+)Ä¾'+)Å¡'Ã©)Ä'3TÃ©Ã©DBEBÃ½Ã¡CÄCÃ½Ã©CF+AÅ¾EBÃ­Ã½Ã©AÅ¡Ã©CÄ¾BÃ¡Ã©E)Å¥'Ã©)Å¾'Ä¾Ã©Ä¾Å¥+Ã©Ä¾Ã¡)Ã¡'Ã©)Ã­'Å¡Ä¾ÄÄÄÄÅ¡Å¥Ä¾Ã½ÄÃ¡)+Ã©'Ã©)++'Ã©)+Ä¾'Ã©)+Å¡'Ä¾Ã©Ä¾Å¥Ã©Ã­Ã©Ä¾+Ã½Å¡Å¾Ã©+)+Ä'+)+Å¥'Ä¾Ã©Ä¾Å¥+Ã©Ä¾Ã¡+Ã©Ä¾Å¡Å¥Å¾)+Å¾'Å¾+Å¾)+Ã½'Ã©)Ä¾Ã©'Ä¾Å¾Ã¡)|Ä¾Ã©Ä¾Å¥+Ã©Ä¾Ã¡Ã©Ã­Ä¾Ä¾+Å¾Ã´+Ã©Ã©Ã©Ä¾Ã¡Å¾+Ã´+Ã©ÄÅ¾Å¡Å¾Ã¡Ã½Ã´Ã´Ã´+|Ä¾Ã©Ä¾Å¥+Ã©Ä¾Å¡+ÄÅ¡Ã½Å¥Ä¾Ã´+Ã©Ã©Ã©Ä¾Ã¡Å¾+Ã´+Ã©ÄÅ¾Å¡Å¾Ã¡Ã­Ã´Ã´Ã´+|D+Ã½Ã©+Ã©Ã½Å¡EFÃ½AÃ½Ã©ACDÃ¡Ã­Ã½BEÃ½ÄÃ­Å¥+CAÃ¡AÅ¥Ã­BÄ¾AEBEÃ­Å¾Ã½Ã¡BCÃ©CÅ¥DÃ½BÃ½Å¡Å¥Å¥Å¾Ã¡BA+FCÃ­C","1.3.0~2510280003552482~2;~1=20250902173601|2=1|3=0|4=#T00DBEB78C4C70CF1A6EB970A30C2B80E|5=0|6=20251028|8=0|9=324444352748|10=0|11=0|12=0|13=20250902173601|14=1|15=20251028102356|16=616|17=0|20=268|~20251028092216;10002861;10463687;;;1~20251023143752;10002861;10463689;;;1~D1701073EF7A70ACD897BE74951CA8A59B2AEBE9678BC0C5D7B735568BA1FC9C") (0ms)
    Assert.That(input, Is.EqualTo(originalInput))
  String lengths are both 336. Strings differ at index 0.
  Expected: "1.3.0~2510280003552482~2;~1=20250902173601|2=1|3=0|4=#T00DBEB..."
  But was:  "+.Å¡.Ã©|Ä¾Å¥+Ã©Ä¾Ã¡Ã©Ã©Ã©Å¡Å¥Å¥Ä¾ÄÃ¡Ä¾|Ä¾Ã´|+'Ä¾Ã©Ä¾Å¥Ã©Ã­Ã©Ä¾+Ã½Å¡Å¾Ã©+)Ä¾'+)Å¡'Ã©)Ä'3TÃ©Ã©DBEB..."
  -----------^

  from C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\bin\Debug\net10.0\NunitTests.dll (net10.0|x64)
    Assert.That(input, Is.EqualTo(originalInput))
    String lengths are both 336. Strings differ at index 0.
    Expected: "1.3.0~2510280003552482~2;~1=20250902173601|2=1|3=0|4=#T00DBEB..."
    But was:  "+.Å¡.Ã©|Ä¾Å¥+Ã©Ä¾Ã¡Ã©Ã©Ã©Å¡Å¥Å¥Ä¾ÄÃ¡Ä¾|Ä¾Ã´|+'Ä¾Ã©Ä¾Å¥Ã©Ã­Ã©Ä¾+Ã½Å¡Å¾Ã©+)Ä¾'+)Å¡'Ã©)Ä'3TÃ©Ã©DBEB..."
    -----------^
  
    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    1)    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:line 229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    
failed ConvertBsQRCodeToOriginal_Valid("1Ã§3Ã§0>2510280003552482>2ÅŸ>1-20250902173601;2-1;3-0;4-^T00DBEB78C4C70CF1A6EB970A30C2B80E;5-0;6-20251028;8-0;9-324444352748;10-0;11-0;12-0;13-20250902173601;14-1;15-20251028102356;16-616;17-0;20-268;>20251028092216ÅŸ10002861ÅŸ10463687ÅŸÅŸÅŸ1>20251023143752ÅŸ10002861ÅŸ10463689ÅŸÅŸÅŸ1>D1701073EF7A70ACD897BE74951CA8A59B2AEBE9678BC0C5D7B735568BA1FC9C","1.3.0~2510280003552482~2;~1=20250902173601|2=1|3=0|4=#T00DBEB78C4C70CF1A6EB970A30C2B80E|5=0|6=20251028|8=0|9=324444352748|10=0|11=0|12=0|13=20250902173601|14=1|15=20251028102356|16=616|17=0|20=268|~20251028092216;10002861;10463687;;;1~20251023143752;10002861;10463689;;;1~D1701073EF7A70ACD897BE74951CA8A59B2AEBE9678BC0C5D7B735568BA1FC9C") (0ms)
    Assert.That(input, Is.EqualTo(originalInput))
  String lengths are both 336. Strings differ at index 1.
  Expected: "1.3.0~2510280003552482~2;~1=20250902173601|2=1|3=0|4=#T00DBEB..."
  But was:  "1Ã§3Ã§0>2510280003552482>2ÅŸ>1-20250902173601;2-1;3-0;4-^T00DBEB..."
  ------------^

  from C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\bin\Debug\net10.0\NunitTests.dll (net10.0|x64)
    Assert.That(input, Is.EqualTo(originalInput))
    String lengths are both 336. Strings differ at index 1.
    Expected: "1.3.0~2510280003552482~2;~1=20250902173601|2=1|3=0|4=#T00DBEB..."
    But was:  "1Ã§3Ã§0>2510280003552482>2ÅŸ>1-20250902173601;2-1;3-0;4-^T00DBEB..."
    ------------^
  
    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    1)    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:line 229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    
failed ConvertBsQRCodeToOriginal_Valid("1.3.0AR12510280003552482AR12SL;AR11SL020250902173601ARW2SL01ARW3SL00ARW4SL0SL3SLT00SLDSLBSLESLB78SLC4SLC70SLCSLF1SLA6SLESLB970SLA30SLC2SLB80SLEARW5SL00ARW6SL020251028ARW8SL00ARW9SL0324444352748ARW10SL00ARW11SL00ARW12SL00ARW13SL020250902173601ARW14SL01ARW15SL020251028102356ARW16SL0616ARW17SL00ARW20SL0268ARWAR120251028092216SL;10002861SL;10463687SL;SL;SL;1AR120251023143752SL;10002861SL;10463689SL;SL;SL;1AR1SLD1701073SLESLF7SLA70SLASLCSLD897SLBSLE74951SLCSLA8SLA59SLB2SLASLESLBSLE9678SLBSLC0SLC5SLD7SLB735568SLBSLA1SLFSLC9SLC","1.3.0~2510280003552482~2;~1=20250902173601|2=1|3=0|4=#T00DBEB78C4C70CF1A6EB970A30C2B80E|5=0|6=20251028|8=0|9=324444352748|10=0|11=0|12=0|13=20250902173601|14=1|15=20251028102356|16=616|17=0|20=268|~20251028092216;10002861;10463687;;;1~20251023143752;10002861;10463689;;;1~D1701073EF7A70ACD897BE74951CA8A59B2AEBE9678BC0C5D7B735568BA1FC9C") (0ms)
    Assert.That(input, Is.EqualTo(originalInput))
  Expected string length 336 but was 526. Strings differ at index 5.
  Expected: "1.3.0~2510280003552482~2;~1=20250902173601|2=1|3=0|4=#T00DBEB..."
  But was:  "1.3.0AR12510280003552482AR12SL;AR11SL020250902173601ARW2SL01A..."
  ----------------^

  from C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\bin\Debug\net10.0\NunitTests.dll (net10.0|x64)
    Assert.That(input, Is.EqualTo(originalInput))
    Expected string length 336 but was 526. Strings differ at index 5.
    Expected: "1.3.0~2510280003552482~2;~1=20250902173601|2=1|3=0|4=#T00DBEB..."
    But was:  "1.3.0AR12510280003552482AR12SL;AR11SL020250902173601ARW2SL01A..."
    ----------------^
  
    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    1)    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:line 229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    
failed ConvertBsQRCodeToOriginal_Valid("1.3.0Â¡2510280003552482Â¡2;Â¡1=20250902173601âˆ‘2=1âˆ‘3=0âˆ‘4=#T00DBEB78C4C70CF1A6EB970A30C2B80Eâˆ‘5=0âˆ‘6=20251028âˆ‘8=0âˆ‘9=324444352748âˆ‘10=0âˆ‘11=0âˆ‘12=0âˆ‘13=20250902173601âˆ‘14=1âˆ‘15=20251028102356âˆ‘16=616âˆ‘17=0âˆ‘20=268âˆ‘Â¡20251028092216;10002861;10463687;;;1Â¡20251023143752;10002861;10463689;;;1Â¡D1701073EF7A70ACD897BE74951CA8A59B2AEBE9678BC0C5D7B735568BA1FC9C","1.3.0~2510280003552482~2;~1=20250902173601|2=1|3=0|4=#T00DBEB78C4C70CF1A6EB970A30C2B80E|5=0|6=20251028|8=0|9=324444352748|10=0|11=0|12=0|13=20250902173601|14=1|15=20251028102356|16=616|17=0|20=268|~20251028092216;10002861;10463687;;;1~20251023143752;10002861;10463689;;;1~D1701073EF7A70ACD897BE74951CA8A59B2AEBE9678BC0C5D7B735568BA1FC9C") (0ms)
    Assert.That(input, Is.EqualTo(originalInput))
  String lengths are both 336. Strings differ at index 5.
  Expected: "1.3.0~2510280003552482~2;~1=20250902173601|2=1|3=0|4=#T00DBEB..."
  But was:  "1.3.0Â¡2510280003552482Â¡2;Â¡1=20250902173601âˆ‘2=1âˆ‘3=0âˆ‘4=#T00DBEB..."
  ----------------^

  from C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\bin\Debug\net10.0\NunitTests.dll (net10.0|x64)
    Assert.That(input, Is.EqualTo(originalInput))
    String lengths are both 336. Strings differ at index 5.
    Expected: "1.3.0~2510280003552482~2;~1=20250902173601|2=1|3=0|4=#T00DBEB..."
    But was:  "1.3.0Â¡2510280003552482Â¡2;Â¡1=20250902173601âˆ‘2=1âˆ‘3=0âˆ‘4=#T00DBEB..."
    ----------------^
  
    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    1)    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:line 229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    
failed ConvertBsQRCodeToOriginal_Valid("1.3.0AR12510280003552482AR12SL;AR11SL020250902173601ARW2SL01ARW3SL00ARW4SL0SL3SLT00SLDSLBSLESLB78SLC4SLC70SLCSLF1SLA6SLESLB970SLA30SLC2SLB80SLEARW5SL00ARW6SL020251028ARW8SL00ARW9SL0324444352748ARW10SL00ARW11SL00ARW12SL00ARW13SL020250902173601ARW14SL01ARW15SL020251028102356ARW16SL0616ARW17SL00ARW20SL0268ARWAR120251028092216SL;10002861SL;10463687SL;SL;SL;1AR120251023143752SL;10002861SL;10463689SL;SL;SL;1AR1SLD1701073SLESLF7SLA70SLASLCSLD897SLBSLE74951SLCSLA8SLA59SLB2SLASLESLBSLE9678SLBSLC0SLC5SLD7SLB735568SLBSLA1SLFSLC9SLC","1.3.0~2510280003552482~2;~1=20250902173601|2=1|3=0|4=#T00DBEB78C4C70CF1A6EB970A30C2B80E|5=0|6=20251028|8=0|9=324444352748|10=0|11=0|12=0|13=20250902173601|14=1|15=20251028102356|16=616|17=0|20=268|~20251028092216;10002861;10463687;;;1~20251023143752;10002861;10463689;;;1~D1701073EF7A70ACD897BE74951CA8A59B2AEBE9678BC0C5D7B735568BA1FC9C") (0ms)
    Assert.That(input, Is.EqualTo(originalInput))
  Expected string length 336 but was 526. Strings differ at index 5.
  Expected: "1.3.0~2510280003552482~2;~1=20250902173601|2=1|3=0|4=#T00DBEB..."
  But was:  "1.3.0AR12510280003552482AR12SL;AR11SL020250902173601ARW2SL01A..."
  ----------------^

  from C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\bin\Debug\net10.0\NunitTests.dll (net10.0|x64)
    Assert.That(input, Is.EqualTo(originalInput))
    Expected string length 336 but was 526. Strings differ at index 5.
    Expected: "1.3.0~2510280003552482~2;~1=20250902173601|2=1|3=0|4=#T00DBEB..."
    But was:  "1.3.0AR12510280003552482AR12SL;AR11SL020250902173601ARW2SL01A..."
    ----------------^
  
    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    1)    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:line 229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    
failed ConvertBsQRCodeToOriginal_Valid("SL1.SL3.SL0AR1SL2SL5SL1SL0SL2SL8SL0SL0SL0SL3SL5SL5SL2SL4SL8SL2AR1SL2~AR1SL1=SL2SL0SL2SL5SL0SL9SL0SL2SL1SL7SL3SL6SL0SL1SL|SL2=SL1SL|SL3=SL0SL|SL4=ARXSLTSL0SL0SLDSLBSLESLBSL7SL8SLCSL4SLCSL7SL0SLCSLFSL1SLASL6SLESLBSL9SL7SL0SLASL3SL0SLCSL2SLBSL8SL0SLESL|SL5=SL0SL|SL6=SL2SL0SL2SL5SL1SL0SL2SL8SL|SL8=SL0SL|SL9=SL3SL2SL4SL4SL4SL4SL3SL5SL2SL7SL4SL8SL|SL1SL0=SL0SL|SL1SL1=SL0SL|SL1SL2=SL0SL|SL1SL3=SL2SL0SL2SL5SL0SL9SL0SL2SL1SL7SL3SL6SL0SL1SL|SL1SL4=SL1SL|SL1SL5=SL2SL0SL2SL5SL1SL0SL2SL8SL1SL0SL2SL3SL5SL6SL|SL1SL6=SL6SL1SL6SL|SL1SL7=SL0SL|SL2SL0=SL2SL6SL8SL|AR1SL2SL0SL2SL5SL1SL0SL2SL8SL0SL9SL2SL2SL1SL6~SL1SL0SL0SL0SL2SL8SL6SL1~SL1SL0SL4SL6SL3SL6SL8SL7~~~SL1AR1SL2SL0SL2SL5SL1SL0SL2SL3SL1SL4SL3SL7SL5SL2~SL1SL0SL0SL0SL2SL8SL6SL1~SL1SL0SL4SL6SL3SL6SL8SL9~~~SL1AR1SLDSL1SL7SL0SL1SL0SL7SL3SLESLFSL7SLASL7SL0SLASLCSLDSL8SL9SL7SLBSLESL7SL4SL9SL5SL1SLCSLASL8SLASL5SL9SLBSL2SLASLESLBSLESL9SL6SL7SL8SLBSLCSL0SLCSL5SLDSL7SLBSL7SL3SL5SL5SL6SL8SLBSLASL1SLFSLCSL9SLC","1.3.0~2510280003552482~2;~1=20250902173601|2=1|3=0|4=#T00DBEB78C4C70CF1A6EB970A30C2B80E|5=0|6=20251028|8=0|9=324444352748|10=0|11=0|12=0|13=20250902173601|14=1|15=20251028102356|16=616|17=0|20=268|~20251028092216;10002861;10463687;;;1~20251023143752;10002861;10463689;;;1~D1701073EF7A70ACD897BE74951CA8A59B2AEBE9678BC0C5D7B735568BA1FC9C") (0ms)
    Assert.That(input, Is.EqualTo(originalInput))
  Expected string length 336 but was 948. Strings differ at index 0.
  Expected: "1.3.0~2510280003552482~2;~1=20250902173601|2=1|3=0|4=#T00DBEB..."
  But was:  "SL1.SL3.SL0AR1SL2SL5SL1SL0SL2SL8SL0SL0SL0SL3SL5SL5SL2SL4SL8SL..."
  -----------^

  from C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\bin\Debug\net10.0\NunitTests.dll (net10.0|x64)
    Assert.That(input, Is.EqualTo(originalInput))
    Expected string length 336 but was 948. Strings differ at index 0.
    Expected: "1.3.0~2510280003552482~2;~1=20250902173601|2=1|3=0|4=#T00DBEB..."
    But was:  "SL1.SL3.SL0AR1SL2SL5SL1SL0SL2SL8SL0SL0SL0SL3SL5SL5SL2SL4SL8SL..."
    -----------^
  
    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    1)    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:line 229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    
failed ConvertBsQRCodeToOriginal_Valid("1.3.0`2510280003552482`2\\`1=20250902173601>2=1>3=0>4=T00DBEB78C4C70CF1A6EB970A30C2B80E>5=0>6=20251028>8=0>9=324444352748>10=0>11=0>12=0>13=20250902173601>14=1>15=20251028102356>16=616>17=0>20=268>`20251028092216\\10002861\\10463687\\\\\\1`20251023143752\\10002861\\10463689\\\\\\1`D1701073EF7A70ACD897BE74951CA8A59B2AEBE9678BC0C5D7B735568BA1FC9C","1.3.0~2510280003552482~2;~1=20250902173601|2=1|3=0|4=#T00DBEB78C4C70CF1A6EB970A30C2B80E|5=0|6=20251028|8=0|9=324444352748|10=0|11=0|12=0|13=20250902173601|14=1|15=20251028102356|16=616|17=0|20=268|~20251028092216;10002861;10463687;;;1~20251023143752;10002861;10463689;;;1~D1701073EF7A70ACD897BE74951CA8A59B2AEBE9678BC0C5D7B735568BA1FC9C") (0ms)
    Assert.That(input, Is.EqualTo(originalInput))
  Expected string length 336 but was 335. Strings differ at index 5.
  Expected: "1.3.0~2510280003552482~2;~1=20250902173601|2=1|3=0|4=#T00DBEB..."
  But was:  "1.3.0`2510280003552482`2\\`1=20250902173601>2=1>3=0>4=T00DBEB7..."
  ----------------^

  from C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\bin\Debug\net10.0\NunitTests.dll (net10.0|x64)
    Assert.That(input, Is.EqualTo(originalInput))
    Expected string length 336 but was 335. Strings differ at index 5.
    Expected: "1.3.0~2510280003552482~2;~1=20250902173601|2=1|3=0|4=#T00DBEB..."
    But was:  "1.3.0`2510280003552482`2\\`1=20250902173601>2=1>3=0>4=T00DBEB7..."
    ----------------^
  
    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    1)    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:line 229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    
failed ConvertBsQRCodeToOriginal_Valid("1.3.0`2511040058871530`2\\`1=20250923164601>2=1>3=0>4=T12A1DD7892BF46F79E921923B49C7BD2>5=0>6=20251104>8=0>9=344796270974>10=0>11=0>12=0>13=20250923164601>14=1>15=20251104090223>16=616>17=0>20=268>`20251103162949\\10001592\\43584\\\\\\1`20251030174336\\10001592\\43582\\\\\\1`22DE294BD460FFC3F3EDB82F1ED469E315632550BE2AB8F5DAA6071B0C081333","1.3.0~2511040058871530~2;~1=20250923164601|2=1|3=0|4=#T12A1DD7892BF46F79E921923B49C7BD2|5=0|6=20251104|8=0|9=344796270974|10=0|11=0|12=0|13=20250923164601|14=1|15=20251104090223|16=616|17=0|20=268|~20251103162949;10001592;43584;;;1~20251030174336;10001592;43582;;;1~22DE294BD460FFC3F3EDB82F1ED469E315632550BE2AB8F5DAA6071B0C081333") (0ms)
    Assert.That(input, Is.EqualTo(originalInput))
  Expected string length 330 but was 329. Strings differ at index 5.
  Expected: "1.3.0~2511040058871530~2;~1=20250923164601|2=1|3=0|4=#T12A1DD..."
  But was:  "1.3.0`2511040058871530`2\\`1=20250923164601>2=1>3=0>4=T12A1DD7..."
  ----------------^

  from C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\bin\Debug\net10.0\NunitTests.dll (net10.0|x64)
    Assert.That(input, Is.EqualTo(originalInput))
    Expected string length 330 but was 329. Strings differ at index 5.
    Expected: "1.3.0~2511040058871530~2;~1=20250923164601|2=1|3=0|4=#T12A1DD..."
    But was:  "1.3.0`2511040058871530`2\\`1=20250923164601>2=1>3=0>4=T12A1DD7..."
    ----------------^
  
    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    1)    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:line 229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    
failed ConvertBsQRCodeToOriginal_Valid("1.3.0`2511040058871532`2\\`1=20250923164601>2=1>3=0>4=T709A17DAB63BB2CD1D3684927825F98D>5=0>6=20251104>8=0>9=344796270974>10=0>11=0>12=0>13=20250923164601>14=1>15=20251104090432>16=616>17=0>20=268>`20251103162949\\10001592\\43584\\\\\\1`20251030174336\\10001592\\43582\\\\\\1`F433A2F8CF476FF3187FD092A8DA0B0472309611F66B7257DC35D89CC24FF783","1.3.0~2511040058871532~2;~1=20250923164601|2=1|3=0|4=#T709A17DAB63BB2CD1D3684927825F98D|5=0|6=20251104|8=0|9=344796270974|10=0|11=0|12=0|13=20250923164601|14=1|15=20251104090432|16=616|17=0|20=268|~20251103162949;10001592;43584;;;1~20251030174336;10001592;43582;;;1~F433A2F8CF476FF3187FD092A8DA0B0472309611F66B7257DC35D89CC24FF783") (0ms)
    Assert.That(input, Is.EqualTo(originalInput))
  Expected string length 330 but was 329. Strings differ at index 5.
  Expected: "1.3.0~2511040058871532~2;~1=20250923164601|2=1|3=0|4=#T709A17..."
  But was:  "1.3.0`2511040058871532`2\\`1=20250923164601>2=1>3=0>4=T709A17D..."
  ----------------^

  from C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\bin\Debug\net10.0\NunitTests.dll (net10.0|x64)
    Assert.That(input, Is.EqualTo(originalInput))
    Expected string length 330 but was 329. Strings differ at index 5.
    Expected: "1.3.0~2511040058871532~2;~1=20250923164601|2=1|3=0|4=#T709A17..."
    But was:  "1.3.0`2511040058871532`2\\`1=20250923164601>2=1>3=0>4=T709A17D..."
    ----------------^
  
    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    1)    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:line 229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    
failed ConvertBsQRCodeToOriginal_Valid("SL1.SL3.SL0SLAR~SL2SL5SL1SL1SL0SL4SL0SL0SL5SL8SL8SL7SL3SL4SL0SL2SLAR~SL2~SLAR~SL1AR=SL2SL0SL2SL5SL0SL9SL2SL3SL1SL6SL4SL6SL0SL1SL|SL2AR=SL1SL|SL3AR=SL0SL|SL4AR=AR3SLTSL1SL3SL9SL2SL5SL2SLESLFSL9SLESL4SL6SL6SL8SLESL8SL2SLCSL4SL8SLCSL5SL3SL2SL6SL1SL6SL3SL0SL3SL5SLCSL|SL5AR=SL0SL|SL6AR=SL2SL0SL2SL5SL1SL1SL0SL4SL|SL8AR=SL0SL|SL9AR=SL3SL4SL4SL7SL9SL6SL2SL0SL7SL9SL7SL9SL|SL1SL0AR=SL0SL|SL1SL1AR=SL0SL|SL1SL2AR=SL0SL|SL1SL3AR=SL2SL0SL2SL5SL0SL9SL2SL3SL1SL6SL4SL6SL0SL1SL|SL1SL4AR=SL1SL|SL1SL5AR=SL2SL0SL2SL5SL1SL1SL0SL4SL1SL5SL4SL5SL1SL3SL|SL1SL6AR=SL6SL1SL6SL|SL1SL7AR=SL0SL|SL2SL0AR=SL2SL8SL9SL|SLAR~SL2SL0SL2SL5SL1SL1SL0SL4SL1SL3SL1SL2SL1SL7~SL1SL0SL0SL0SL1SL5SL9SL2~SL4SL3SL5SL8SL3~~~SL1SLAR~SL3SL2SLESL0SL4SL1SL6SL2SL0SL9SLASL5SLESLFSL8SL8SL5SL0SLDSLESL7SL0SL3SLASLASL6SLCSL0SLDSL0SL9SL7SLCSL7SL3SL2SL6SL9SL8SL8SL7SL5SL1SL3SL8SLBSL1SL3SL3SLBSL6SL7SL5SL6SL8SLCSLFSLCSL4SL9SL2SLBSL6SL3","1.3.0~2511040058873402~2;~1=20250923164601|2=1|3=0|4=#T139252EF9E4668E82C48C5326163035C|5=0|6=20251104|8=0|9=344796207979|10=0|11=0|12=0|13=20250923164601|14=1|15=20251104154513|16=616|17=0|20=289|~20251104131217;10001592;43583;;;1~32E0416209A5EF8850DE703AA6C0D097C732698875138B133B67568CFC492B63") (0ms)
    Assert.That(input, Is.EqualTo(originalInput))
  Expected string length 296 but was 882. Strings differ at index 0.
  Expected: "1.3.0~2511040058873402~2;~1=20250923164601|2=1|3=0|4=#T139252..."
  But was:  "SL1.SL3.SL0SLAR~SL2SL5SL1SL1SL0SL4SL0SL0SL5SL8SL8SL7SL3SL4SL0..."
  -----------^

  from C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\bin\Debug\net10.0\NunitTests.dll (net10.0|x64)
    Assert.That(input, Is.EqualTo(originalInput))
    Expected string length 296 but was 882. Strings differ at index 0.
    Expected: "1.3.0~2511040058873402~2;~1=20250923164601|2=1|3=0|4=#T139252..."
    But was:  "SL1.SL3.SL0SLAR~SL2SL5SL1SL1SL0SL4SL0SL0SL5SL8SL8SL7SL3SL4SL0..."
    -----------^
  
    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    1)    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:line 229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    
failed ConvertBsQRCodeToOriginal_Valid("1Ð»3Ð»0~2511060058874906~2Ð¼~1.20250923164601)2.1)3.0)4.+Ð¨45Ð¤Ð¤462Ñ460ÐªÐ•ÐžÐª2Ð2ÐžÑ887Ð•51ÑÐ•ÑÐž46)5.0)6.20251106)8.0)9.344796270974)10.0)11.0)12.0)13.20250923164601)14.1)15.20251106093014)16.616)17.0)20.268)~20251105160108Ð¼10001592Ð¼43583Ð¼Ð¼Ð¼1~20251105101935Ð¼10001592Ð¼43580Ð¼Ð¼Ð¼1~33ÐÐ¤3ÑÐ•1Ðª77785Ð¤25775Ð¤7Ðª71Ð•1Ð¤Ñ5Ðª9Ðª088Ðª56Ð•963091476974Ð•Ñ90Ð¤Ð•ÐÑ2625","1.3.0~2511060058874906~2;~1=20250923164601|2=1|3=0|4=#T45BB462A460CEFC2D2FA887E51AEAF46|5=0|6=20251106|8=0|9=344796270974|10=0|11=0|12=0|13=20250923164601|14=1|15=20251106093014|16=616|17=0|20=268|~20251105160108;10001592;43583;;;1~20251105101935;10001592;43580;;;1~33DB3AE1C77785B25775B7C71E1BA5C9C088C56E963091476974EA90BEDA2625") (0ms)
    Assert.That(input, Is.EqualTo(originalInput))
  String lengths are both 330. Strings differ at index 1.
  Expected: "1.3.0~2511060058874906~2;~1=20250923164601|2=1|3=0|4=#T45BB46..."
  But was:  "1Ð»3Ð»0~2511060058874906~2Ð¼~1.20250923164601)2.1)3.0)4.+Ð¨45Ð¤Ð¤46..."
  ------------^

  from C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\bin\Debug\net10.0\NunitTests.dll (net10.0|x64)
    Assert.That(input, Is.EqualTo(originalInput))
    String lengths are both 330. Strings differ at index 1.
    Expected: "1.3.0~2511060058874906~2;~1=20250923164601|2=1|3=0|4=#T45BB46..."
    But was:  "1Ð»3Ð»0~2511060058874906~2Ð¼~1.20250923164601)2.1)3.0)4.+Ð¨45Ð¤Ð¤46..."
    ------------^
  
    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    1)    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:line 229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    
failed ConvertBsQRCodeToOriginal_Valid("1.3.025102800035524822\\1^20250902173601>2^1>3^0>4^#T00DBEB78C4C70CF1A6EB970A30C2B80E>5^0>6^20251028>8^0>9^324444352748>10^0>11^0>12^0>13^20250902173601>14^1>15^20251028102356>16^616>17^0>20^268>20251028092216\\10002861\\10463687\\\\\\120251023143752\\10002861\\10463689\\\\\\1D1701073EF7A70ACD897BE74951CA8A59B2AEBE9678BC0C5D7B735568BA1FC9C","1.3.0~2510280003552482~2;~1=20250902173601|2=1|3=0|4=#T00DBEB78C4C70CF1A6EB970A30C2B80E|5=0|6=20251028|8=0|9=324444352748|10=0|11=0|12=0|13=20250902173601|14=1|15=20251028102356|16=616|17=0|20=268|~20251028092216;10002861;10463687;;;1~20251023143752;10002861;10463689;;;1~D1701073EF7A70ACD897BE74951CA8A59B2AEBE9678BC0C5D7B735568BA1FC9C") (0ms)
    Assert.That(input, Is.EqualTo(originalInput))
  Expected string length 336 but was 330. Strings differ at index 5.
  Expected: "1.3.0~2510280003552482~2;~1=20250902173601|2=1|3=0|4=#T00DBEB..."
  But was:  "1.3.025102800035524822\\1^20250902173601>2^1>3^0>4^#T00DBEB78C..."
  ----------------^

  from C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\bin\Debug\net10.0\NunitTests.dll (net10.0|x64)
    Assert.That(input, Is.EqualTo(originalInput))
    Expected string length 336 but was 330. Strings differ at index 5.
    Expected: "1.3.0~2510280003552482~2;~1=20250902173601|2=1|3=0|4=#T00DBEB..."
    But was:  "1.3.025102800035524822\\1^20250902173601>2^1>3^0>4^#T00DBEB78C..."
    ----------------^
  
    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    1)    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:line 229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    
failed ConvertBsQRCodeToOriginal_Valid("1.3.025110400588715262\\1^20250923164601>2^1>3^0>4^#TC4D6BE62BCAA3E60D1A017A7741F4C8C>5^0>6^20251104>8^0>9^344796270974>10^0>11^0>12^0>13^20250923164601>14^1>15^20251104085721>16^616>17^0>20^268>20251103162949\\10001592\\43584\\\\\\120251030174336\\10001592\\43582\\\\\\18232AB34D93DF5301C407FB659378DD029C3DBFF06BDF332828AAC64FD70448D","1.3.0~2511040058871526~2;~1=20250923164601|2=1|3=0|4=#TC4D6BE62BCAA3E60D1A017A7741F4C8C|5=0|6=20251104|8=0|9=344796270974|10=0|11=0|12=0|13=20250923164601|14=1|15=20251104085721|16=616|17=0|20=268|~20251103162949;10001592;43584;;;1~20251030174336;10001592;43582;;;1~8232AB34D93DF5301C407FB659378DD029C3DBFF06BDF332828AAC64FD70448D") (0ms)
    Assert.That(input, Is.EqualTo(originalInput))
  Expected string length 330 but was 324. Strings differ at index 5.
  Expected: "1.3.0~2511040058871526~2;~1=20250923164601|2=1|3=0|4=#TC4D6BE..."
  But was:  "1.3.025110400588715262\\1^20250923164601>2^1>3^0>4^#TC4D6BE62B..."
  ----------------^

  from C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\bin\Debug\net10.0\NunitTests.dll (net10.0|x64)
    Assert.That(input, Is.EqualTo(originalInput))
    Expected string length 330 but was 324. Strings differ at index 5.
    Expected: "1.3.0~2511040058871526~2;~1=20250923164601|2=1|3=0|4=#TC4D6BE..."
    But was:  "1.3.025110400588715262\\1^20250923164601>2^1>3^0>4^#TC4D6BE62B..."
    ----------------^
  
    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    1)    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:line 229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    
failed ConvertBsQRCodeToOriginal_Valid("1.3.025110400588715282\\1^20250923164601>2^1>3^0>4^#T5CC714AFF94023526E9BDFE05975B162>5^0>6^20251104>8^0>9^344796270974>10^0>11^0>12^0>13^20250923164601>14^1>15^20251104090021>16^616>17^0>20^268>20251103162949\\10001592\\43584\\\\\\120251030174336\\10001592\\43582\\\\\\19BA60C3CF6E1FB1C28FCC263F04DBEBE3FDF5594946F04B6719E87C4A63CEB2C","1.3.0~2511040058871528~2;~1=20250923164601|2=1|3=0|4=#T5CC714AFF94023526E9BDFE05975B162|5=0|6=20251104|8=0|9=344796270974|10=0|11=0|12=0|13=20250923164601|14=1|15=20251104090021|16=616|17=0|20=268|~20251103162949;10001592;43584;;;1~20251030174336;10001592;43582;;;1~9BA60C3CF6E1FB1C28FCC263F04DBEBE3FDF5594946F04B6719E87C4A63CEB2C") (0ms)
    Assert.That(input, Is.EqualTo(originalInput))
  Expected string length 330 but was 324. Strings differ at index 5.
  Expected: "1.3.0~2511040058871528~2;~1=20250923164601|2=1|3=0|4=#T5CC714..."
  But was:  "1.3.025110400588715282\\1^20250923164601>2^1>3^0>4^#T5CC714AFF..."
  ----------------^

  from C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\bin\Debug\net10.0\NunitTests.dll (net10.0|x64)
    Assert.That(input, Is.EqualTo(originalInput))
    Expected string length 330 but was 324. Strings differ at index 5.
    Expected: "1.3.0~2511040058871528~2;~1=20250923164601|2=1|3=0|4=#T5CC714..."
    But was:  "1.3.025110400588715282\\1^20250923164601>2^1>3^0>4^#T5CC714AFF..."
    ----------------^
  
    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    1)    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:line 229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    
failed ConvertBsQRCodeToOriginal_Valid("1.3.025102800035524822\\1^20250902173601>2^1>3^0>4^#T00DBEB78C4C70CF1A6EB970A30C2B80E>5^0>6^20251028>8^0>9^324444352748>10^0>11^0>12^0>13^20250902173601>14^1>15^20251028102356>16^616>17^0>20^268>20251028092216\\10002861\\10463687\\\\\\120251023143752\\10002861\\10463689\\\\\\1D1701073EF7A70ACD897BE74951CA8A59B2AEBE9678BC0C5D7B735568BA1FC9C","1.3.0~2510280003552482~2;~1=20250902173601|2=1|3=0|4=#T00DBEB78C4C70CF1A6EB970A30C2B80E|5=0|6=20251028|8=0|9=324444352748|10=0|11=0|12=0|13=20250902173601|14=1|15=20251028102356|16=616|17=0|20=268|~20251028092216;10002861;10463687;;;1~20251023143752;10002861;10463689;;;1~D1701073EF7A70ACD897BE74951CA8A59B2AEBE9678BC0C5D7B735568BA1FC9C") (0ms)
    Assert.That(input, Is.EqualTo(originalInput))
  Expected string length 336 but was 330. Strings differ at index 5.
  Expected: "1.3.0~2510280003552482~2;~1=20250902173601|2=1|3=0|4=#T00DBEB..."
  But was:  "1.3.025102800035524822\\1^20250902173601>2^1>3^0>4^#T00DBEB78C..."
  ----------------^

  from C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\bin\Debug\net10.0\NunitTests.dll (net10.0|x64)
    Assert.That(input, Is.EqualTo(originalInput))
    Expected string length 336 but was 330. Strings differ at index 5.
    Expected: "1.3.0~2510280003552482~2;~1=20250902173601|2=1|3=0|4=#T00DBEB..."
    But was:  "1.3.025102800035524822\\1^20250902173601>2^1>3^0>4^#T00DBEB78C..."
    ----------------^
  
    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    1)    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:line 229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    
failed ConvertBsQRCodeToOriginal_Valid("1.3.0`2511060058875078`2\\`1=20250923164601Ä—2=1Ä—3=0Ä—4=T35E2E99EE9C7E26C233FAEAEF2CD2AACÄ—5=0Ä—6=20251106Ä—8=0Ä—9=344796270974Ä—10=0Ä—11=0Ä—12=0Ä—13=20250923164601Ä—14=1Ä—15=20251106162756Ä—16=616Ä—17=0Ä—20=268Ä—`20251106161533\\10001592\\43583\\\\\\1`20251106120125\\10001592\\43583\\\\\\1`7C8D76BB7CC928E2A97F0930BE0059DF617521F2A94B9C39FF033ED091FCA550","1.3.0~2511060058875078~2;~1=20250923164601|2=1|3=0|4=#T35E2E99EE9C7E26C233FAEAEF2CD2AAC|5=0|6=20251106|8=0|9=344796270974|10=0|11=0|12=0|13=20250923164601|14=1|15=20251106162756|16=616|17=0|20=268|~20251106161533;10001592;43583;;;1~20251106120125;10001592;43583;;;1~7C8D76BB7CC928E2A97F0930BE0059DF617521F2A94B9C39FF033ED091FCA550") (0ms)
    Assert.That(input, Is.EqualTo(originalInput))
  Expected string length 330 but was 329. Strings differ at index 5.
  Expected: "1.3.0~2511060058875078~2;~1=20250923164601|2=1|3=0|4=#T35E2E9..."
  But was:  "1.3.0`2511060058875078`2\\`1=20250923164601Ä—2=1Ä—3=0Ä—4=T35E2E99..."
  ----------------^

  from C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\bin\Debug\net10.0\NunitTests.dll (net10.0|x64)
    Assert.That(input, Is.EqualTo(originalInput))
    Expected string length 330 but was 329. Strings differ at index 5.
    Expected: "1.3.0~2511060058875078~2;~1=20250923164601|2=1|3=0|4=#T35E2E9..."
    But was:  "1.3.0`2511060058875078`2\\`1=20250923164601Ä—2=1Ä—3=0Ä—4=T35E2E99..."
    ----------------^
  
    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    1)    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:line 229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    
failed ConvertBsQRCodeToOriginal_Valid("1.3.0`2511060058875090`2\\`1=20250923164601Ä—2=1Ä—3=0Ä—4=T2A557C38AFB487FB76C1C709B7D7520DÄ—5=0Ä—6=20251106Ä—8=0Ä—9=344796270974Ä—10=0Ä—11=0Ä—12=0Ä—13=20250923164601Ä—14=1Ä—15=20251106163415Ä—16=616Ä—17=0Ä—20=268Ä—`20251106161533\\10001592\\43583\\\\\\1`20251106120125\\10001592\\43583\\\\\\1`76A5A38BF32E65D139A034F4D20198BEE84CE47D463C83F1A643FD5D396FC0B0","1.3.0~2511060058875090~2;~1=20250923164601|2=1|3=0|4=#T2A557C38AFB487FB76C1C709B7D7520D|5=0|6=20251106|8=0|9=344796270974|10=0|11=0|12=0|13=20250923164601|14=1|15=20251106163415|16=616|17=0|20=268|~20251106161533;10001592;43583;;;1~20251106120125;10001592;43583;;;1~76A5A38BF32E65D139A034F4D20198BEE84CE47D463C83F1A643FD5D396FC0B0") (0ms)
    Assert.That(input, Is.EqualTo(originalInput))
  Expected string length 330 but was 329. Strings differ at index 5.
  Expected: "1.3.0~2511060058875090~2;~1=20250923164601|2=1|3=0|4=#T2A557C..."
  But was:  "1.3.0`2511060058875090`2\\`1=20250923164601Ä—2=1Ä—3=0Ä—4=T2A557C3..."
  ----------------^

  from C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\bin\Debug\net10.0\NunitTests.dll (net10.0|x64)
    Assert.That(input, Is.EqualTo(originalInput))
    Expected string length 330 but was 329. Strings differ at index 5.
    Expected: "1.3.0~2511060058875090~2;~1=20250923164601|2=1|3=0|4=#T2A557C..."
    But was:  "1.3.0`2511060058875090`2\\`1=20250923164601Ä—2=1Ä—3=0Ä—4=T2A557C3..."
    ----------------^
  
    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    1)    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:line 229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    
failed ConvertBsQRCodeToOriginal_Valid("SL1.SL3.SL0AR1SL2SL5SL1SL1SL0SL4SL0SL0SL5SL8SL8SL7SL3SL4SL0SL2AR1SL2~AR1SL1=SL2SL0SL2SL5SL0SL9SL2SL3SL1SL6SL4SL6SL0SL1ARWSL2=SL1ARWSL3=SL0ARWSL4=ARXSLTSL1SL3SL9SL2SL5SL2SLESLFSL9SLESL4SL6SL6SL8SLESL8SL2SLCSL4SL8SLCSL5SL3SL2SL6SL1SL6SL3SL0SL3SL5SLCARWSL5=SL0ARWSL6=SL2SL0SL2SL5SL1SL1SL0SL4ARWSL8=SL0ARWSL9=SL3SL4SL4SL7SL9SL6SL2SL0SL7SL9SL7SL9ARWSL1SL0=SL0ARWSL1SL1=SL0ARWSL1SL2=SL0ARWSL1SL3=SL2SL0SL2SL5SL0SL9SL2SL3SL1SL6SL4SL6SL0SL1ARWSL1SL4=SL1ARWSL1SL5=SL2SL0SL2SL5SL1SL1SL0SL4SL1SL5SL4SL5SL1SL3ARWSL1SL6=SL6SL1SL6ARWSL1SL7=SL0ARWSL2SL0=SL2SL8SL9ARWAR1SL2SL0SL2SL5SL1SL1SL0SL4SL1SL3SL1SL2SL1SL7~SL1SL0SL0SL0SL1SL5SL9SL2~SL4SL3SL5SL8SL3~~~SL1AR1SL3SL2SLESL0SL4SL1SL6SL2SL0SL9SLASL5SLESLFSL8SL8SL5SL0SLDSLESL7SL0SL3SLASLASL6SLCSL0SLDSL0SL9SL7SLCSL7SL3SL2SL6SL9SL8SL8SL7SL5SL1SL3SL8SLBSL1SL3SL3SLBSL6SL7SL5SL6SL8SLCSLFSLCSL4SL9SL2SLBSL6SL3","1.3.0~2511040058873402~2;~1=20250923164601|2=1|3=0|4=#T139252EF9E4668E82C48C5326163035C|5=0|6=20251104|8=0|9=344796207979|10=0|11=0|12=0|13=20250923164601|14=1|15=20251104154513|16=616|17=0|20=289|~20251104131217;10001592;43583;;;1~32E0416209A5EF8850DE703AA6C0D097C732698875138B133B67568CFC492B63") (0ms)
    Assert.That(input, Is.EqualTo(originalInput))
  Expected string length 296 but was 838. Strings differ at index 0.
  Expected: "1.3.0~2511040058873402~2;~1=20250923164601|2=1|3=0|4=#T139252..."
  But was:  "SL1.SL3.SL0AR1SL2SL5SL1SL1SL0SL4SL0SL0SL5SL8SL8SL7SL3SL4SL0SL..."
  -----------^

  from C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\bin\Debug\net10.0\NunitTests.dll (net10.0|x64)
    Assert.That(input, Is.EqualTo(originalInput))
    Expected string length 296 but was 838. Strings differ at index 0.
    Expected: "1.3.0~2511040058873402~2;~1=20250923164601|2=1|3=0|4=#T139252..."
    But was:  "SL1.SL3.SL0AR1SL2SL5SL1SL1SL0SL4SL0SL0SL5SL8SL8SL7SL3SL4SL0SL..."
    -----------^
  
    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    1)    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:line 229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    
failed ConvertBsQRCodeToOriginal_Valid("1.3.0~ 2510280003552482~ 2;~ 1=202509021736012=13=04=#T00DBEB78C4C70CF1A6EB970A30C2B80E5=06=202510288=09=32444435274810=011=012=013=2025090217360114=115=2025102810235616=61617=020=268~ 20251028092216;10002861;10463687;;;1~ 20251023143752;10002861;10463689;;;1~ D1701073EF7A70ACD897BE74951CA8A59B2AEBE9678BC0C5D7B735568BA1FC9C","1.3.0~2510280003552482~2;~1=20250902173601|2=1|3=0|4=#T00DBEB78C4C70CF1A6EB970A30C2B80E|5=0|6=20251028|8=0|9=324444352748|10=0|11=0|12=0|13=20250902173601|14=1|15=20251028102356|16=616|17=0|20=268|~20251028092216;10002861;10463687;;;1~20251023143752;10002861;10463689;;;1~D1701073EF7A70ACD897BE74951CA8A59B2AEBE9678BC0C5D7B735568BA1FC9C") (0ms)
    Assert.That(input, Is.EqualTo(originalInput))
  Expected string length 336 but was 325. Strings differ at index 6.
  Expected: "1.3.0~2510280003552482~2;~1=20250902173601|2=1|3=0|4=#T00DBEB..."
  But was:  "1.3.0~ 2510280003552482~ 2;~ 1=202509021736012=13=04=#T00DBEB..."
  -----------------^

  from C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\bin\Debug\net10.0\NunitTests.dll (net10.0|x64)
    Assert.That(input, Is.EqualTo(originalInput))
    Expected string length 336 but was 325. Strings differ at index 6.
    Expected: "1.3.0~2510280003552482~2;~1=20250902173601|2=1|3=0|4=#T00DBEB..."
    But was:  "1.3.0~ 2510280003552482~ 2;~ 1=202509021736012=13=04=#T00DBEB..."
    -----------------^
  
    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    1)    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:line 229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    
failed ConvertBsQRCodeToOriginal_Valid("1.3.0~ 2511040058871534~ 2;~ 1=202509231646012=13=04=#T881E5E6808B28115C0C1A6513BCBF8E25=06=202511048=09=34479627097410=011=012=013=2025092316460114=115=2025110409063816=61617=020=268~ 20251103162949;10001592;43584;;;1~ 20251030174336;10001592;43582;;;1~ 944E098390B09069DB9915FF56785896C192232877581EDF2B08828A0CA7DB33","1.3.0~2511040058871534~2;~1=20250923164601|2=1|3=0|4=#T881E5E6808B28115C0C1A6513BCBF8E2|5=0|6=20251104|8=0|9=344796270974|10=0|11=0|12=0|13=20250923164601|14=1|15=20251104090638|16=616|17=0|20=268|~20251103162949;10001592;43584;;;1~20251030174336;10001592;43582;;;1~944E098390B09069DB9915FF56785896C192232877581EDF2B08828A0CA7DB33") (0ms)
    Assert.That(input, Is.EqualTo(originalInput))
  Expected string length 330 but was 319. Strings differ at index 6.
  Expected: "1.3.0~2511040058871534~2;~1=20250923164601|2=1|3=0|4=#T881E5E..."
  But was:  "1.3.0~ 2511040058871534~ 2;~ 1=202509231646012=13=04=#T881E5E..."
  -----------------^

  from C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\bin\Debug\net10.0\NunitTests.dll (net10.0|x64)
    Assert.That(input, Is.EqualTo(originalInput))
    Expected string length 330 but was 319. Strings differ at index 6.
    Expected: "1.3.0~2511040058871534~2;~1=20250923164601|2=1|3=0|4=#T881E5E..."
    But was:  "1.3.0~ 2511040058871534~ 2;~ 1=202509231646012=13=04=#T881E5E..."
    -----------------^
  
    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    1)    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:line 229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    
failed ConvertBsQRCodeToOriginal_Valid("1.3.0~ 2511040058871539~ 2;~ 1=202509231646012=13=04=#T7FE1223D6502A3A4CFA434FFD2CA66B35=06=202511048=09=34479627097410=011=012=013=2025092316460114=115=2025110409084216=61617=020=268~ 20251103162949;10001592;43584;;;1~ 20251030174336;10001592;43582;;;1~ CC0BF03FB114D4A9612D4F50819B6DA196E0AF90E1E1EFE0537A524F1C3EA2DF","1.3.0~2511040058871539~2;~1=20250923164601|2=1|3=0|4=#T7FE1223D6502A3A4CFA434FFD2CA66B3|5=0|6=20251104|8=0|9=344796270974|10=0|11=0|12=0|13=20250923164601|14=1|15=20251104090842|16=616|17=0|20=268|~20251103162949;10001592;43584;;;1~20251030174336;10001592;43582;;;1~CC0BF03FB114D4A9612D4F50819B6DA196E0AF90E1E1EFE0537A524F1C3EA2DF") (0ms)
    Assert.That(input, Is.EqualTo(originalInput))
  Expected string length 330 but was 319. Strings differ at index 6.
  Expected: "1.3.0~2511040058871539~2;~1=20250923164601|2=1|3=0|4=#T7FE122..."
  But was:  "1.3.0~ 2511040058871539~ 2;~ 1=202509231646012=13=04=#T7FE122..."
  -----------------^

  from C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\bin\Debug\net10.0\NunitTests.dll (net10.0|x64)
    Assert.That(input, Is.EqualTo(originalInput))
    Expected string length 330 but was 319. Strings differ at index 6.
    Expected: "1.3.0~2511040058871539~2;~1=20250923164601|2=1|3=0|4=#T7FE122..."
    But was:  "1.3.0~ 2511040058871539~ 2;~ 1=202509231646012=13=04=#T7FE122..."
    -----------------^
  
    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    1)    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:line 229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    
failed ConvertBsQRCodeToOriginal_Valid("1.3.0AR) 2511040058871516AR) 2SL/AR) 1SL020250923164601AR|2SL01AR|3SL00AR|4SL0AR3SLH85SLQSLDSLD4SLV1SLQ5SL;06SLF34SL;1SLDSL;805SLFSLFSLQ98917SLYAR|5SL00AR|6SL020251104AR|8SL00AR|9SL0344796270974AR|10SL00AR|11SL00AR|12SL00AR|13SL020250923164601AR|14SL01AR|15SL020251104085228AR|16SL0616AR|17SL00AR|20SL0268AR|AR) 20251103162949SL/10001592SL/43584SL/SL/SL/1AR) 20251030174336SL/10001592SL/43582SL/SL/SL/1AR) 2SLQ014SL;SLQ884SLQ1SLD13SLF2180SLDSLV1SLD471321SLF002SL;47SLQ517SLYSL;95SL;260SLY8SLVSLD8SLF005SLFSLF6SLQ6SLQ","1.3.0~2511040058871516~2;~1=20250923164601|2=1|3=0|4=#T85FEE4C1F5B06A34B1EB805AAF98917D|5=0|6=20251104|8=0|9=344796270974|10=0|11=0|12=0|13=20250923164601|14=1|15=20251104085228|16=616|17=0|20=268|~20251103162949;10001592;43584;;;1~20251030174336;10001592;43582;;;1~2F014BF884F1E13A2180EC1E471321A002B47F517DB95B260D8CE8A005AA6F6F") (0ms)
    Assert.That(input, Is.EqualTo(originalInput))
  Expected string length 330 but was 516. Strings differ at index 5.
  Expected: "1.3.0~2511040058871516~2;~1=20250923164601|2=1|3=0|4=#T85FEE4..."
  But was:  "1.3.0AR) 2511040058871516AR) 2SL/AR) 1SL020250923164601AR|2SL..."
  ----------------^

  from C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\bin\Debug\net10.0\NunitTests.dll (net10.0|x64)
    Assert.That(input, Is.EqualTo(originalInput))
    Expected string length 330 but was 516. Strings differ at index 5.
    Expected: "1.3.0~2511040058871516~2;~1=20250923164601|2=1|3=0|4=#T85FEE4..."
    But was:  "1.3.0AR) 2511040058871516AR) 2SL/AR) 1SL020250923164601AR|2SL..."
    ----------------^
  
    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    1)    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:line 229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    
failed ConvertBsQRCodeToOriginal_Valid("1.3.0AR) 2511040058871520AR) 2SL/AR) 1SL020250923164601AR|2SL01AR|3SL00AR|4SL0AR3SLH58SLY30SL;2SLDSLQ10489SLQ8174109SLQ146SLFSLD78SLDSLDAR|5SL00AR|6SL020251104AR|8SL00AR|9SL0344796270974AR|10SL00AR|11SL00AR|12SL00AR|13SL020250923164601AR|14SL01AR|15SL020251104085407AR|16SL0616AR|17SL00AR|20SL0268AR|AR) 20251103162949SL/10001592SL/43584SL/SL/SL/1AR) 20251030174336SL/10001592SL/43582SL/SL/SL/1AR) 63SL;65SLQ2085SLQ412SL;9058SLV639SLV6SLV30SLDSLD2386SLFSLD60SLQSL;SLY53SLD26SLV524SLYSL;9SLVSLQ51SLDSL;SLY302SLY","1.3.0~2511040058871520~2;~1=20250923164601|2=1|3=0|4=#T58D30B2EF10489F8174109F146AE78EE|5=0|6=20251104|8=0|9=344796270974|10=0|11=0|12=0|13=20250923164601|14=1|15=20251104085407|16=616|17=0|20=268|~20251103162949;10001592;43584;;;1~20251030174336;10001592;43582;;;1~63B65F2085F412B9058C639C6C30EE2386AE60FBD53E26C524DB9CF51EBD302D") (0ms)
    Assert.That(input, Is.EqualTo(originalInput))
  Expected string length 330 but was 510. Strings differ at index 5.
  Expected: "1.3.0~2511040058871520~2;~1=20250923164601|2=1|3=0|4=#T58D30B..."
  But was:  "1.3.0AR) 2511040058871520AR) 2SL/AR) 1SL020250923164601AR|2SL..."
  ----------------^

  from C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\bin\Debug\net10.0\NunitTests.dll (net10.0|x64)
    Assert.That(input, Is.EqualTo(originalInput))
    Expected string length 330 but was 510. Strings differ at index 5.
    Expected: "1.3.0~2511040058871520~2;~1=20250923164601|2=1|3=0|4=#T58D30B..."
    But was:  "1.3.0AR) 2511040058871520AR) 2SL/AR) 1SL020250923164601AR|2SL..."
    ----------------^
  
    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    1)    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:line 229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    
failed ConvertBsQRCodeToOriginal_Valid("1.3.0AR) 2511040058871522AR) 2SL/AR) 1SL020250923164601AR|2SL01AR|3SL00AR|4SL0AR3SLH41SLFSLVSLF51SLD7SLV11SLD8932292SLFSLY735SLF00SL;917AR|5SL00AR|6SL020251104AR|8SL00AR|9SL0344796270974AR|10SL00AR|11SL00AR|12SL00AR|13SL020250923164601AR|14SL01AR|15SL020251104085516AR|16SL0616AR|17SL00AR|20SL0268AR|AR) 20251103162949SL/10001592SL/43584SL/SL/SL/1AR) 20251030174336SL/10001592SL/43582SL/SL/SL/1AR) 8SLDSLV85SLV0197SL;1546SLV114960SLYSLQ1SLQSLQSLF0SLF325SLD92SLQ2SL;SLY014SL;838671SLV4SL;SLYSL;SLF5SLFSLF6SL;SLVSLY6","1.3.0~2511040058871522~2;~1=20250923164601|2=1|3=0|4=#T41ACA51E7C11E8932292AD735A00B917|5=0|6=20251104|8=0|9=344796270974|10=0|11=0|12=0|13=20250923164601|14=1|15=20251104085516|16=616|17=0|20=268|~20251103162949;10001592;43584;;;1~20251030174336;10001592;43582;;;1~8EC85C0197B1546C114960DF1FFA0A325E92F2BD014B838671C4BDBA5AA6BCD6") (0ms)
    Assert.That(input, Is.EqualTo(originalInput))
  Expected string length 330 but was 514. Strings differ at index 5.
  Expected: "1.3.0~2511040058871522~2;~1=20250923164601|2=1|3=0|4=#T41ACA5..."
  But was:  "1.3.0AR) 2511040058871522AR) 2SL/AR) 1SL020250923164601AR|2SL..."
  ----------------^

  from C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\bin\Debug\net10.0\NunitTests.dll (net10.0|x64)
    Assert.That(input, Is.EqualTo(originalInput))
    Expected string length 330 but was 514. Strings differ at index 5.
    Expected: "1.3.0~2511040058871522~2;~1=20250923164601|2=1|3=0|4=#T41ACA5..."
    But was:  "1.3.0AR) 2511040058871522AR) 2SL/AR) 1SL020250923164601AR|2SL..."
    ----------------^
  
    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    1)    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:line 229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    
failed ConvertBsQRCodeToOriginal_Valid("1.3.0AR) 2510280003552482AR) 2SL/AR) 1SL020250902173601AR|2SL01AR|3SL00AR|4SL0AR3SLH00SLYSL;SLDSL;78SLV4SLV70SLVSLQ1SLF6SLDSL;970SLF30SLV2SL;80SLDAR|5SL00AR|6SL020251028AR|8SL00AR|9SL0324444352748AR|10SL00AR|11SL00AR|12SL00AR|13SL020250902173601AR|14SL01AR|15SL020251028102356AR|16SL0616AR|17SL00AR|20SL0268AR|AR) 20251028092216SL/10002861SL/10463687SL/SL/SL/1AR) 20251023143752SL/10002861SL/10463689SL/SL/SL/1AR) SLY1701073SLDSLQ7SLF70SLFSLVSLY897SL;SLD74951SLVSLF8SLF59SL;2SLFSLDSL;SLD9678SL;SLV0SLV5SLY7SL;735568SL;SLF1SLQSLV9SLV","1.3.0~2510280003552482~2;~1=20250902173601|2=1|3=0|4=#T00DBEB78C4C70CF1A6EB970A30C2B80E|5=0|6=20251028|8=0|9=324444352748|10=0|11=0|12=0|13=20250902173601|14=1|15=20251028102356|16=616|17=0|20=268|~20251028092216;10002861;10463687;;;1~20251023143752;10002861;10463689;;;1~D1701073EF7A70ACD897BE74951CA8A59B2AEBE9678BC0C5D7B735568BA1FC9C") (0ms)
    Assert.That(input, Is.EqualTo(originalInput))
  Expected string length 336 but was 532. Strings differ at index 5.
  Expected: "1.3.0~2510280003552482~2;~1=20250902173601|2=1|3=0|4=#T00DBEB..."
  But was:  "1.3.0AR) 2510280003552482AR) 2SL/AR) 1SL020250902173601AR|2SL..."
  ----------------^

  from C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\bin\Debug\net10.0\NunitTests.dll (net10.0|x64)
    Assert.That(input, Is.EqualTo(originalInput))
    Expected string length 336 but was 532. Strings differ at index 5.
    Expected: "1.3.0~2510280003552482~2;~1=20250902173601|2=1|3=0|4=#T00DBEB..."
    But was:  "1.3.0AR) 2510280003552482AR) 2SL/AR) 1SL020250902173601AR|2SL..."
    ----------------^
  
    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    1)    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:line 229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    
failed ConvertBsQRCodeToOriginal_Valid("1.3.0~2511050058874778~2;~1=20250923164601<2=1<3=0<4=#T7C0D4A8C556B8B954571CAA36BBED711<5=0<6=20251105<8=0<9=344796270974<10=0<11=0<12=0<13=20250923164601<14=1<15=20251105161216<16=616<17=0<20=268<~20251105160108;10001592;43583;;;1~20251105101935;10001592;43580;;;1~E7BDECA47049CE961E9571168BDB6804663BD4C4A550717CAE1DBEEC2EAEE659","1.3.0~2511050058874778~2;~1=20250923164601|2=1|3=0|4=#T7C0D4A8C556B8B954571CAA36BBED711|5=0|6=20251105|8=0|9=344796270974|10=0|11=0|12=0|13=20250923164601|14=1|15=20251105161216|16=616|17=0|20=268|~20251105160108;10001592;43583;;;1~20251105101935;10001592;43580;;;1~E7BDECA47049CE961E9571168BDB6804663BD4C4A550717CAE1DBEEC2EAEE659") (0ms)
    Assert.That(input, Is.EqualTo(originalInput))
  String lengths are both 330. Strings differ at index 42.
  Expected: "1.3.0~2511050058874778~2;~1=20250923164601|2=1|3=0|4=#T7C0D4A..."
  But was:  "1.3.0~2511050058874778~2;~1=20250923164601<2=1<3=0<4=#T7C0D4A..."
  -----------------------------------------------------^

  from C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\bin\Debug\net10.0\NunitTests.dll (net10.0|x64)
    Assert.That(input, Is.EqualTo(originalInput))
    String lengths are both 330. Strings differ at index 42.
    Expected: "1.3.0~2511050058874778~2;~1=20250923164601|2=1|3=0|4=#T7C0D4A..."
    But was:  "1.3.0~2511050058874778~2;~1=20250923164601<2=1<3=0<4=#T7C0D4A..."
    -----------------------------------------------------^
  
    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    1)    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:line 229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    
failed ConvertBsQRCodeToOriginal_Valid("1.3.0~2511060058874822~2;~1=20250923164601<2=1<3=0<4=#T500AE3C4AE615094BB73E346B560CA3C<5=0<6=20251106<8=0<9=344796270974<10=0<11=0<12=0<13=20250923164601<14=1<15=20251106090729<16=616<17=0<20=268<~20251105160108;10001592;43583;;;1~20251105101935;10001592;43580;;;1~20EEB083A8883128265E36DA44962FEE393B06EB6CF8B266B9DA167664D5488B","1.3.0~2511060058874822~2;~1=20250923164601|2=1|3=0|4=#T500AE3C4AE615094BB73E346B560CA3C|5=0|6=20251106|8=0|9=344796270974|10=0|11=0|12=0|13=20250923164601|14=1|15=20251106090729|16=616|17=0|20=268|~20251105160108;10001592;43583;;;1~20251105101935;10001592;43580;;;1~20EEB083A8883128265E36DA44962FEE393B06EB6CF8B266B9DA167664D5488B") (0ms)
    Assert.That(input, Is.EqualTo(originalInput))
  String lengths are both 330. Strings differ at index 42.
  Expected: "1.3.0~2511060058874822~2;~1=20250923164601|2=1|3=0|4=#T500AE3..."
  But was:  "1.3.0~2511060058874822~2;~1=20250923164601<2=1<3=0<4=#T500AE3..."
  -----------------------------------------------------^

  from C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\bin\Debug\net10.0\NunitTests.dll (net10.0|x64)
    Assert.That(input, Is.EqualTo(originalInput))
    String lengths are both 330. Strings differ at index 42.
    Expected: "1.3.0~2511060058874822~2;~1=20250923164601|2=1|3=0|4=#T500AE3..."
    But was:  "1.3.0~2511060058874822~2;~1=20250923164601<2=1<3=0<4=#T500AE3..."
    -----------------------------------------------------^
  
    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    1)    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:line 229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    
failed ConvertBsQRCodeToOriginal_Valid("1/3/0AR) 2511040058873402AR) 2SL|AR) 1SL020250923164601AR|2SL01AR|3SL00AR|4SL0AR3SLT139252SLESLF9SLE4668SLE82SLC48SLC5326163035SLCAR|5SL00AR|6SL020251104AR|8SL00AR|9SL0344796207979AR|10SL00AR|11SL00AR|12SL00AR|13SL020250923164601AR|14SL01AR|15SL020251104154513AR|16SL0616AR|17SL00AR|20SL0289AR|AR) 20251104131217SL|10001592SL|43583SL|SL|SL|1AR) 32SLE0416209SLA5SLESLF8850SLDSLE703SLASLA6SLC0SLD097SLC732698875138SLB133SLB67568SLCSLFSLC492SLB63","1.3.0~2511040058873402~2;~1=20250923164601|2=1|3=0|4=#T139252EF9E4668E82C48C5326163035C|5=0|6=20251104|8=0|9=344796207979|10=0|11=0|12=0|13=20250923164601|14=1|15=20251104154513|16=616|17=0|20=289|~20251104131217;10001592;43583;;;1~32E0416209A5EF8850DE703AA6C0D097C732698875138B133B67568CFC492B63") (0ms)
    Assert.That(input, Is.EqualTo(originalInput))
  Expected string length 296 but was 443. Strings differ at index 1.
  Expected: "1.3.0~2511040058873402~2;~1=20250923164601|2=1|3=0|4=#T139252..."
  But was:  "1/3/0AR) 2511040058873402AR) 2SL|AR) 1SL020250923164601AR|2SL..."
  ------------^

  from C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\bin\Debug\net10.0\NunitTests.dll (net10.0|x64)
    Assert.That(input, Is.EqualTo(originalInput))
    Expected string length 296 but was 443. Strings differ at index 1.
    Expected: "1.3.0~2511040058873402~2;~1=20250923164601|2=1|3=0|4=#T139252..."
    But was:  "1/3/0AR) 2511040058873402AR) 2SL|AR) 1SL020250923164601AR|2SL..."
    ------------^
  
    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    1)    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:line 229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    
failed ConvertBsQRCodeToOriginal_Valid("+.Å¡.Ã©Â°Ä¾Å¥++Ã©Ã½Ã©Ã©Å¥Ã¡Ã¡Ã½Å¥Å¥Ä¾ÄÂ°Ä¾Ã´Â°+Â´Ä¾Ã©Ä¾Å¥Ã©Ã­Ä¾Å¡+Å¾ÄÅ¾Ã©+)Ä¾Â´+)Å¡Â´Ã©)ÄÂ´3TAAACÃ©Ã¡Å¾DÅ¥+Ã©EDBDÅ¥Ã­AÃ½Å¥Å¾CAC+FB+CÄEÃ¡)Å¥Â´Ã©)Å¾Â´Ä¾Ã©Ä¾Å¥++Ã©Ã½)Ã¡Â´Ã©)Ã­Â´Å¡ÄÄÃ½Ã­Å¾Ä¾Ã½Ã©Ã­Ã½Ä)+Ã©Â´Ã©)++Â´Ã©)+Ä¾Â´Ã©)+Å¡Â´Ä¾Ã©Ä¾Å¥Ã©Ã­Ä¾Å¡+Å¾ÄÅ¾Ã©+)+ÄÂ´+)+Å¥Â´Ä¾Ã©Ä¾Å¥++Ã©Ã½+ÄÅ¥Ã­Å¡Ã¡)+Å¾Â´Å¾+Å¾)+Ã½Â´Ã©)Ä¾Ã©Â´Ä¾Å¾Ã¡)Â°Ä¾Ã©Ä¾Å¥++Ã©Ã½+ÄÄ¾Ã¡Ä¾ÄÃ´+Ã©Ã©Ã©+Å¥Ã­Ä¾Ã´ÄÅ¡Å¥Ã¡Å¡Ã´Ã´Ã´+Â°Ä¾Ã©Ä¾Å¥++Ã©Ã½+Ä¾+Å¥+Å¾Ã´+Ã©Ã©Ã©+Å¥Ã­Ä¾Ã´ÄÅ¡Å¥Ã¡Å¡Ã´Ã´Ã´+Â°Ã­Å¥Å¾+FBFÄ¾AÃ©Ã½DÅ¥ÄÃ½CÃ½+Ã©Å¥Å¥Ã©Ã½AÃ©FÃ½AÅ¥EÃ½+AÅ¾DÄÃ©FCÅ¥Å¾CÃ½CÃ¡EÃ¡Ã­Ã­Ä¾BDÃ­FÅ¥AFÃ©Ä¾Å¥Ã­Ã©ÄÄ","1.3.0~2511070058875524~2;~1=20250923164601|2=1|3=0|4=#TAAAC086D510EDBD59A756CAC1FB1C4E8|5=0|6=20251107|8=0|9=344796270974|10=0|11=0|12=0|13=20250923164601|14=1|15=20251107145938|16=616|17=0|20=268|~20251107142824;10001592;43583;;;1~20251107121516;10001592;43583;;;1~9561FBF2A07D547C7105507A0F7A5E71A6D40FC56C7C8E8992BD9F5AF0259044") (0ms)
    Assert.That(input, Is.EqualTo(originalInput))
  String lengths are both 330. Strings differ at index 0.
  Expected: "1.3.0~2511070058875524~2;~1=20250923164601|2=1|3=0|4=#TAAAC08..."
  But was:  "+.Å¡.Ã©Â°Ä¾Å¥++Ã©Ã½Ã©Ã©Å¥Ã¡Ã¡Ã½Å¥Å¥Ä¾ÄÂ°Ä¾Ã´Â°+Â´Ä¾Ã©Ä¾Å¥Ã©Ã­Ä¾Å¡+Å¾ÄÅ¾Ã©+)Ä¾Â´+)Å¡Â´Ã©)ÄÂ´3TAAACÃ©Ã¡..."
  -----------^

  from C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\bin\Debug\net10.0\NunitTests.dll (net10.0|x64)
    Assert.That(input, Is.EqualTo(originalInput))
    String lengths are both 330. Strings differ at index 0.
    Expected: "1.3.0~2511070058875524~2;~1=20250923164601|2=1|3=0|4=#TAAAC08..."
    But was:  "+.Å¡.Ã©Â°Ä¾Å¥++Ã©Ã½Ã©Ã©Å¥Ã¡Ã¡Ã½Å¥Å¥Ä¾ÄÂ°Ä¾Ã´Â°+Â´Ä¾Ã©Ä¾Å¥Ã©Ã­Ä¾Å¡+Å¾ÄÅ¾Ã©+)Ä¾Â´+)Å¡Â´Ã©)ÄÂ´3TAAACÃ©Ã¡..."
    -----------^
  
    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    1)    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:line 229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    
failed ConvertBsQRCodeToOriginal_Valid("+.Å¡.Ã©Â°Ä›Å™++Ã©Ã½Ã©Ã©Å™Ã¡Ã¡Ã½Å™Å¾+Ã©Â°Ä›Å¯Â°+Â´Ä›Ã©Ä›Å™Ã©Ã­Ä›Å¡+Å¾ÄÅ¾Ã©+'Ä›Â´+'Å¡Â´Ã©'ÄÂ´3TB+AEÄÅ¾+FÄÄÄ+Ã½ADÅ¾Å™AÄ›+EÃ©Å™Å¡Å™Ã½Ä+CÅ¡+E'Å™Â´Ã©'Å¾Â´Ä›Ã©Ä›Å™++Ã©Ã½'Ã¡Â´Ã©'Ã­Â´Å¡ÄÄÃ½Ã­Å¾Ä›Ã½Ã©Ã­Ã½Ä'+Ã©Â´Ã©'++Â´Ã©'+Ä›Â´Ã©'+Å¡Â´Ä›Ã©Ä›Å™Ã©Ã­Ä›Å¡+Å¾ÄÅ¾Ã©+'+ÄÂ´+'+Å™Â´Ä›Ã©Ä›Å™++Ã©Ã½+Å™+Ã­+Ã¡'+Å¾Â´Å¾+Å¾'+Ã½Â´Ã©'Ä›Ã©Â´Ä›Å¾Ã¡'Â°Ä›Ã©Ä›Å™++Ã©Ã½+ÄÄ›Ã¡Ä›ÄÅ¯+Ã©Ã©Ã©+Å™Ã­Ä›Å¯ÄÅ¡Å™Ã¡Å¡Å¯Å¯Å¯+Â°Ä›Ã©Ä›Å™++Ã©Ã½+Ä›+Å™+Å¾Å¯+Ã©Ã©Ã©+Å™Ã­Ä›Å¯ÄÅ¡Å™Ã¡Å¡Å¯Å¯Å¯+Ã…++Ã½Å™Ã­EÄ›Ä›+Ä›Å¡Å™Ã½FÃ½Å¡BCÅ¡Ã½EFÄ›Ã©AÃ½Ã­CAÃ¡AÃ­CÃ¡ECÅ™FÃ¡Ã©Å¡BÃ½Ã½FÃ©Ã­Ä›Ã¡Ã­DÃ½Ã¡+A+Å™Ã¡+FFÅ¡+","1.3.0~2511070058875610~2;~1=20250923164601|2=1|3=0|4=#TB1AE461F44417AD65A21E0535741C31E|5=0|6=20251107|8=0|9=344796270974|10=0|11=0|12=0|13=20250923164601|14=1|15=20251107151918|16=616|17=0|20=268|~20251107142824;10001592;43583;;;1~20251107121516;10001592;43583;;;1~A11759E2212357F73BC37EF20A79CA8A9C8EC5F803B77F09289D781A1581FF31") (0ms)
    Assert.That(input, Is.EqualTo(originalInput))
  Expected string length 330 but was 329. Strings differ at index 0.
  Expected: "1.3.0~2511070058875610~2;~1=20250923164601|2=1|3=0|4=#TB1AE46..."
  But was:  "+.Å¡.Ã©Â°Ä›Å™++Ã©Ã½Ã©Ã©Å™Ã¡Ã¡Ã½Å™Å¾+Ã©Â°Ä›Å¯Â°+Â´Ä›Ã©Ä›Å™Ã©Ã­Ä›Å¡+Å¾ÄÅ¾Ã©+'Ä›Â´+'Å¡Â´Ã©'ÄÂ´3TB+AEÄÅ¾..."
  -----------^

  from C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\bin\Debug\net10.0\NunitTests.dll (net10.0|x64)
    Assert.That(input, Is.EqualTo(originalInput))
    Expected string length 330 but was 329. Strings differ at index 0.
    Expected: "1.3.0~2511070058875610~2;~1=20250923164601|2=1|3=0|4=#TB1AE46..."
    But was:  "+.Å¡.Ã©Â°Ä›Å™++Ã©Ã½Ã©Ã©Å™Ã¡Ã¡Ã½Å™Å¾+Ã©Â°Ä›Å¯Â°+Â´Ä›Ã©Ä›Å™Ã©Ã­Ä›Å¡+Å¾ÄÅ¾Ã©+'Ä›Â´+'Å¡Â´Ã©'ÄÂ´3TB+AEÄÅ¾..."
    -----------^
  
    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    1)    at NunitTests.UnitTest1.ConvertBsQRCodeToOriginal_Valid(String input, String originalInput) in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\UnitTest1.cs:line 229
    at InvokeStub_UnitTest1.ConvertBsQRCodeToOriginal_Valid(Object, Span`1)
    
    
C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\bin\Debug\net10.0\NunitTests.dll (net10.0|x64) failed with 74 error(s) (1s 674ms)
Exit code: 2
  Standard output: Microsoft.Testing.Platform v2.0.2+88c9fc53a3 (UTC 11/11/2025) [win-x64 - net10.0]
  NUnit Adapter 6.1.0.0: Test execution started
  Running all tests in C:\repos\nunit\nunit3-vs-adapter.issues\Issue1377\bin\Debug\net10.0\NunitTests.dll
     NUnit3TestExecutor discovered 75 of 75 NUnit test cases using Current Discovery mode, Non-Explicit run
  NUnit Adapter 6.1.0.0: Test execution complete
  

Test run summary: Failed!
  total: 75
  failed: 74
  succeeded: 1
  skipped: 0
  duration: 2s 345ms
Test run completed with non-success exit code: 2 (see: https://aka.ms/testingplatform/exitcodes)
```

