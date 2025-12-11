# NUnit3TestAdapter 6.0.0 Assembly Binding Issue - Repro

> **Note**: See [AppLoadCrash.readme.md](AppLoadCrash.readme.md) for a detailed analysis from the actual production codebase where this issue was originally reported. That document contains a similar analysis with the same conclusions, based on real-world code.

## Issue Description

NUnit3TestAdapter version 6.0.0 fails with a `FileLoadException` when running tests that use `WebApplicationFactory<T>` in projects that reference `Microsoft.ApplicationInsights.AspNetCore` version 2.23.0.

### Error Message
```
System.IO.FileLoadException : Could not load file or assembly 'Microsoft.AspNetCore.Http.Extensions, Version=9.0.0.0, Culture=neutral, PublicKeyToken=adb9793829ddae60'. 
The requested assembly version conflicts with what is already bound in the app domain or specified in the manifest. (0x80131053)
```

## Root Cause

1. **Microsoft.ApplicationInsights.AspNetCore v2.23.0** brings in old ASP.NET Core 2.1 dependencies:
   - `Microsoft.AspNetCore.Hosting` v2.1.1
   - `Microsoft.AspNetCore.Http` v2.1.22
   - `Microsoft.AspNetCore.Http.Extensions` v2.1.1

2. **NUnit3TestAdapter 6.0.0** differences:
   - Targets `net8.0` (previous version 5.2.0 targeted `netcoreapp3.1`)
   - Uses NUnit.Engine v3.21.0 (previous was v3.18.1)
   - Uses stricter assembly loading with .NET 8/9's `AssemblyLoadContext`
   - Enforces exact version matching for assemblies

3. **Conflict**: The old ASP.NET Core 2.1 assemblies get loaded during NUnit Engine's test discovery/assembly metadata scanning:
   - `Microsoft.AspNetCore.Http` v3.1.32 (or v2.1.x) gets loaded BEFORE the test even runs
   - When the test creates `WebApplicationFactory<Program>`, it needs v9.0.0.0 from the shared framework
   - The .NET 8/9 runtime detects the version conflict and throws `FileLoadException`
   - The stricter `AssemblyLoadContext` in .NET 8/9 enforces exact version matching

## Why NUnit3TestAdapter 5.2.0 Worked

- Targeted `netcoreapp3.1` which had more lenient assembly version resolution
- Older NUnit.Engine v3.18.1 with less strict AssemblyLoadContext behavior
- .NET Core 3.1-7 runtime was more forgiving about version mismatches

## Why the NUnit Engine Can't Use Application Runtime Resolution

The NUnit Engine creates its own isolated `AssemblyLoadContext` rather than using the application runtime's resolution:

### Why Isolation is Needed
1. **Test Isolation**: Multiple test assemblies can be run with different dependencies without interference
2. **Engine Protection**: The engine itself doesn't conflict with test dependencies
3. **Multi-targeting**: Different test assemblies can target different frameworks

### The Problem This Creates

**Key Principle:** Isolated `AssemblyLoadContext` + conflicting deps.json entries = always fails, regardless of target framework.

When the test engine creates its own isolated `AssemblyLoadContext`:

1. **Explicit deps.json Entries**: The test engine sees the deps.json which explicitly lists both version requirements:
   - Framework provides `Microsoft.AspNetCore.Http.Extensions` 9.0.0.0
   - ApplicationInsights deps.json declares `Microsoft.AspNetCore.Http.Extensions` 2.1.1 as a dependency

2. **Strict Loading Rules**: .NET 8/9 runtime rule - **You cannot load two different versions of the same assembly in one `AssemblyLoadContext`**

3. **Conflict Detection**: When the engine tries to honor both requirements, the runtime throws `FileLoadException`

4. **Why Application Works But Tests Fail**:
   - **Application runtime**: Uses version forwarding - the 2.1.1 reference is satisfied by the 9.0.0 framework assembly in the default context
   - **Test context**: Isolated `AssemblyLoadContext` sees conflicting explicit version requirements and cannot auto-redirect

### Why This Wasn't a Problem Before

- **.NET Core 3.1-7**: More lenient `AssemblyLoadContext` would allow version mismatches to "slide" and unify to compatible versions
- **.NET 8/9**: Stricter version enforcement - won't silently unify incompatible versions

### Engine Resolution Strategies

The NUnit Engine (v3.21.0) implements sophisticated assembly resolution strategies:

- **TrustedPlatformAssembliesStrategy**: Loads from framework assemblies
- **RuntimeLibrariesStrategy**: Uses deps.json and DependencyContext for application dependencies
- **AspNetCoreStrategy**: Special handling for ASP.NET Core runtime dependencies

The engine **does mimic runtime resolution behavior** through these strategies. However, when deps.json explicitly declares conflicting versions AND .NET 8+ enforces strict version matching, the engine cannot perform automatic binding redirects without violating the runtime's security model.

### Important: Target Framework Doesn't Matter

**Downgrading from .NET 9 to .NET 8 does NOT fix this issue**. The problem is:

- **Not** about the version gap between framework versions (8.0 vs 9.0)
- **IS** about assembly identity conflict in isolated context - two versions of the same assembly cannot coexist
- Affects .NET 8 and .NET 9 equally when using isolated `AssemblyLoadContext`
- The adapter's target framework (net8.0) matters because .NET 8+ enforces stricter isolation rules

### Why the Engine Can't Just Use Application Context

Using the application's runtime resolution would:

- Break test isolation (tests could interfere with each other)
- Cause the engine to conflict with test dependencies
- Prevent running multiple test assemblies with different dependency versions

The fundamental issue is that the engine's isolated context loads assemblies referencing incompatible framework versions before the application runtime can establish proper bindings.

## Solution

Upgrade `Microsoft.ApplicationInsights.AspNetCore` to a version compatible with .NET 9:

- Version 2.23.0 targets ASP.NET Core 2.1
- Newer versions target modern ASP.NET Core and won't bring in conflicting dependencies

### Alternative Workaround

Downgrade to `NUnit3TestAdapter` v5.2.0 (or earlier) which targets `netcoreapp3.1` and has more lenient assembly loading. However, this is not recommended as a long-term solution.

## Potential Engine Fixes

### Key Architectural Insight

The testhost starts **one adapter/engine instance per test assembly**, each in its own isolated process. When a solution has multiple test assemblies, they run in separate processes. This means:

- **Inter-assembly isolation is NOT needed** - each test assembly already has its own process
- The engine's custom `AssemblyLoadContext` is primarily isolating the engine itself from test dependencies
- But this isolation creates the version conflict by loading assemblies in the wrong order

### Potential Fix Options

#### Option 1: Use Default AssemblyLoadContext (Recommended)
Since each test assembly runs in an isolated process, the engine could use the default `AssemblyLoadContext`:

- **Benefit**: Let the runtime's natural version unification handle conflicts
- **Benefit**: Application binding redirects/runtime config take precedence
- **Benefit**: Eliminates "early loading" problem - discovery and execution use same context
- **Implementation**: Detect isolated process mode and skip creating custom context

#### Option 2: Defer Assembly Loading Until Test Execution
Currently the engine loads assemblies during test discovery to scan metadata:

- **Change**: Do minimal metadata scanning without fully loading dependency assemblies
- **Change**: Only load the actual test assembly for discovery
- **Change**: Defer loading application dependencies until test execution starts
- **Benefit**: Let the application's `Program.Main` establish the runtime context first

#### Option 3: Use Application's AssemblyLoadContext as Parent
When creating its `AssemblyLoadContext`, make the test assembly's default context the parent:

- **Change**: Only isolate engine-specific assemblies (NUnit framework, etc.)
- **Change**: Let application assemblies resolve through the default context
- **Benefit**: Maintains some engine isolation while respecting application runtime resolution

#### Option 4: Assembly Version Unification Strategy
Implement smarter version resolution in `TestAssemblyResolver.OnResolving`:

- **Change**: When multiple versions are requested, prefer the higher version from shared framework
- **Change**: Detect shared framework assemblies and always use the runtime's version
- **Change**: Add binding redirect logic similar to what the runtime does naturally
- **Benefit**: Works within existing architecture

#### Option 5: Lazy Assembly Probing
Don't probe/load transitive dependencies during discovery:

- **Change**: Only load the direct test assembly for discovery
- **Change**: Mark transitive dependencies as "deferred"
- **Change**: Let them load naturally when test code actually references them
- **Benefit**: `WebApplicationFactory` and application code establish correct context first

### Recommended Approach

**Option 1 (Use Default AssemblyLoadContext)** is most straightforward given that:

1. Each test assembly already runs in complete process isolation
2. No cross-assembly dependency conflicts are possible
3. The engine would use the same resolution as the application runtime
4. Eliminates the root cause rather than working around it

## Repro Project Structure

- **App/** - Minimal ASP.NET Core 9.0 Web API with health checks and ApplicationInsights
- **App.Test/** - Test project using NUnit 4.4.0 and NUnit3TestAdapter 6.0.0

### To Reproduce

Run `dotnet test` - the test will fail with the `FileLoadException`

**Key Observation**: The diagnostic output shows `Microsoft.AspNetCore.Http, Version=3.1.32.0` is already loaded BEFORE the test code executes. This happens during NUnit Engine's test discovery/assembly loading phase, triggered by the `Microsoft.ApplicationInsights.AspNetCore` v2.23.0 dependency references.

### Key Dependencies

- `Microsoft.ApplicationInsights.AspNetCore` v2.23.0 (causes the issue)
- `Microsoft.AspNetCore.Mvc.Testing` v9.0.11
- `NUnit3TestAdapter` v6.0.0
- Target Framework: net9.0
