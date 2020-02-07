## Issue 676: Test cases are skipped with TestCaseSource under Visual Studio 2019

[Issue 676 on github](https://github.com/nunit/nunit3-vs-adapter/issues/676)

Latest version using VS 16.4 and adapter 3.16 still show error.

The project runs under .Net Core 2.2

Two exceptions of type " System.ArgumentException: An item with the same key has already been added." appears, no tests are shown.

The full stacktrace look like this:

```
[1/6/2020 12:49:52.065 AM] Trying to update view 48037535
[1/6/2020 12:49:52.065 AM] Updating view 48037535
[1/6/2020 12:49:52.066 AM] Updating view 48037535
[1/6/2020 12:49:52.067 AM] System.ArgumentException: An item with the same key has already been added.
   at System.ThrowHelper.ThrowArgumentException(ExceptionResource resource)
   at System.Collections.Generic.Dictionary`2.Insert(TKey key, TValue value, Boolean add)
   at System.Linq.Enumerable.ToDictionary[TSource,TKey,TElement](IEnumerable`1 source, Func`2 keySelector, Func`2 elementSelector, IEqualityComparer`1 comparer)
   at Microsoft.VisualStudio.TestWindow.Client.VirtualTestNodeHierarchy.<>c__DisplayClass42_1.<<RefreshTestsAsync>b__1>d.MoveNext()
--- End of stack trace from previous location where exception was thrown ---
   at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task)
   at Microsoft.VisualStudio.TestWindow.Utilities.EventPumpExtensions.<>c__DisplayClass3_0.<<EnqueueAsync>b__0>d.MoveNext()
--- End of stack trace from previous location where exception was thrown ---
   at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task)
   at Microsoft.VisualStudio.TestWindow.Extensibility.ILoggerExtensions.<CallWithCatchAsync>d__9.MoveNext()
--- End of stack trace from previous location where exception was thrown ---
   at Microsoft.VisualStudio.Telemetry.WindowsErrorReporting.WatsonReport.GetClrWatsonExceptionInfo(Exception exceptionObject)
[1/6/2020 12:49:52.080 AM] System.ArgumentException: An item with the same key has already been added.
   at System.ThrowHelper.ThrowArgumentException(ExceptionResource resource)
   at System.Collections.Generic.Dictionary`2.Insert(TKey key, TValue value, Boolean add)
   at System.Linq.Enumerable.ToDictionary[TSource,TKey,TElement](IEnumerable`1 source, Func`2 keySelector, Func`2 elementSelector, IEqualityComparer`1 comparer)
   at Microsoft.VisualStudio.TestWindow.Client.VirtualTestNodeHierarchy.<>c__DisplayClass42_1.<<RefreshTestsAsync>b__1>d.MoveNext()
--- End of stack trace from previous location where exception was thrown ---
   at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task)
   at Microsoft.VisualStudio.TestWindow.Utilities.EventPumpExtensions.<>c__DisplayClass3_0.<<EnqueueAsync>b__0>d.MoveNext()
--- End of stack trace from previous location where exception was thrown ---
   at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task)
   at Microsoft.VisualStudio.TestWindow.Extensibility.ILoggerExtensions.<CallWithCatchAsync>d__9.MoveNext()
--- End of stack trace from previous location where exception was thrown ---
   at Microsoft.VisualStudio.Telemetry.WindowsErrorReporting.WatsonReport.GetClrWatsonExceptionInfo(Exception exceptionObject)
[1/6/2020 12:51:42.315 AM] OnAfterOpenProject [1]
[1/6/2020 12:51:42.316 AM] Loading Project 1de8bd40-7661-4c0d-9d88-3d341deadd30 [2] => 
```