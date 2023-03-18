# Issue [1044](https://github.com/nunit/nunit3-vs-adapter/issues/1044)

Run with :
```cmd
dotnet test TestTestCaseSource.dll --filter FullyQualifiedName~Issue1041.Tests --logger="Console;verbosity=detailed"
```


The filter that is created is based on the FQN, and results in this NUnit filter:

```
<filter><or><test>Issue1041.Tests.The test number 1.</test><test>Issue1041.Tests.The test number 2.</test></or></filter>
```

The XML returned from discovery is:
```xml
<test-run id='0' name='Issue1044.dll' fullname='d:\repos\NUnit\nunit3-vs-adapter.issues\Issue1044\bin\Debug\net7.0\Issue1044.dll' runstate='Runnable' testcasecount='4'>
   <test-suite type='Assembly' id='0-1009' name='Issue1044.dll' fullname='d:/repos/NUnit/nunit3-vs-adapter.issues/Issue1044/bin/Debug/net7.0/Issue1044.dll' runstate='Runnable' testcasecount='4'>
      <environment framework-version='3.13.3.0' clr-version='7.0.0' os-version='Microsoft Windows 10.0.19044' platform='Win32NT' cwd='d:\repos\NUnit\nunit3-vs-adapter.issues\Issue1044\bin\Debug\net7.0' machine-name='DESKTOP-SIATMVB' user='TerjeSandstrom' user-domain='AzureAD' culture='en-US' uiculture='en-US' os-architecture='x64' />
      <settings>
         <setting name='NumberOfTestWorkers' value='0' />
         <setting name='SynchronousEvents' value='False' />
         <setting name='InternalTraceLevel' value='Off' />
         <setting name='RandomSeed' value='1197244076' />
         <setting name='ProcessModel' value='InProcess' />
         <setting name='DomainUsage' value='Single' />
         <setting name='DefaultTestNamePattern' value='{m}{a}' />
         <setting name='WorkDirectory' value='d:\repos\NUnit\nunit3-vs-adapter.issues\Issue1044\bin\Debug\net7.0' />
      </settings>
      <properties>
         <property name='_PID' value='83292' />
         <property name='_APPDOMAIN' value='testhost' />
      </properties>
      <test-suite type='TestSuite' id='0-1010' name='Issue1041' fullname='Issue1041' runstate='Runnable' testcasecount='4'>
         <test-suite type='TestFixture' id='0-1011' name='Tests' fullname='Issue1041.Tests' runstate='Runnable' testcasecount='4'>
            <test-suite type='ParameterizedMethod' id='0-1012' name='ExplicitTest' fullname='Issue1041.Tests.ExplicitTest' classname='Issue1041.Tests' runstate='Explicit' testcasecount='2'>
               <test-case id='0-1004' name='The test number 1.' fullname='Issue1041.Tests.The test number 1.' methodname='ExplicitTest' classname='Issue1041.Tests' runstate='Explicit' seed='1882810222' />
               <test-case id='0-1005' name='The test number 2.' fullname='Issue1041.Tests.The test number 2.' methodname='ExplicitTest' classname='Issue1041.Tests' runstate='Explicit' seed='531831831' />
            </test-suite>
            <test-suite type='ParameterizedMethod' id='0-1013' name='RegularTest' fullname='Issue1041.Tests.RegularTest' classname='Issue1041.Tests' runstate='Runnable' testcasecount='2'>
               <test-case id='0-1001' name='The test number 1.' fullname='Issue1041.Tests.The test number 1.' methodname='RegularTest' classname='Issue1041.Tests' runstate='Runnable' seed='501371230' />
               <test-case id='0-1002' name='The test number 2.' fullname='Issue1041.Tests.The test number 2.' methodname='RegularTest' classname='Issue1041.Tests' runstate='Runnable' seed='1044448715' />
            </test-suite>
         </test-suite>
      </test-suite>
   </test-suite>
</test-run>
```

Note that the fullname is identical for both tests, even if they are running from different tests, because the test name was changed.