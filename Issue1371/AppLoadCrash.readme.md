# Assembly Load Crash Analysis - NUnit3TestAdapter 6.0.0 with .NET 9

## Problem Summary

Integration tests fail with `FileLoadException` when using NUnit3TestAdapter 6.0.0 on .NET 9.0:

```
System.IO.FileLoadException : Could not load file or assembly 'Microsoft.AspNetCore.Http.Extensions, Version=9.0.0.0'
The requested assembly version conflicts with what is already bound in the app domain
```

## Root Cause

**Two conflicting versions** of `Microsoft.AspNetCore.Http.Extensions` are being loaded:

1. **Version 9.0.0.0** - From .NET 9 shared framework at:
   - `C:\Program Files\dotnet\shared\Microsoft.AspNetCore.App\9.0.11\Microsoft.AspNetCore.Http.Extensions.dll`

2. **Version 2.1.1** - Transitively referenced through the dependency chain:
   ```
   Fhi.Grunndata.OppslagWeb.csproj
   └─ Microsoft.ApplicationInsights.AspNetCore 2.23.0 (targets .NETStandard2.0)
      ├─ Microsoft.AspNetCore.Hosting 2.1.1
      └─ Microsoft.AspNetCore.Http 2.1.22
         └─ Microsoft.AspNetCore.Http.Extensions 2.1.1 ⚠️ CONFLICT
   ```

## Why This Didn't Happen with NUnit3TestAdapter 5.2.0

The key difference is the **target framework** of the adapter build:

### NUnit3TestAdapter 5.2.0
- Targets: **`netcoreapp3.1`** and `net462`
- .NET 9 projects load the **`netcoreapp3.1`** build
- Uses Microsoft.Testing.Platform.MSBuild **1.9.0**
- NUnit.Engine version: **3.18.1**
- Runtime behavior: More lenient assembly version resolution

### NUnit3TestAdapter 6.0.0
- Targets: **`net8.0`** and `net462`
- .NET 9 projects load the **`net8.0`** build
- Uses Microsoft.Testing.Platform.MSBuild **2.0.2**
- NUnit.Engine version: **3.21.0**
- Runtime behavior: **Stricter assembly version checking**

### Technical Details

The .NET 8+ runtime (including .NET 9) has **stricter assembly loading and binding redirect enforcement** in the `AssemblyLoadContext`. When NUnit.Engine attempts to resolve assemblies:

- .NET Core 3.1-7: More forgiving about version mismatches, often allows compatible versions to coexist
- .NET 8+: Enforces strict version checking and throws `FileLoadException` when encountering conflicts

## The Issue with Microsoft.ApplicationInsights.AspNetCore 2.23.0

This package is **outdated for modern .NET**:

```xml
<dependencies>
  <group targetFramework=".NETStandard2.0">
    <dependency id="Microsoft.AspNetCore.Hosting" version="2.1.1" />
    <dependency id="Microsoft.AspNetCore.Http" version="2.1.22" />
    <!-- Other dependencies -->
  </group>
</dependencies>
```

The package only targets `.NETStandard2.0` and pulls in ASP.NET Core 2.1 dependencies, which are incompatible with .NET 9's framework assemblies.

## Solution

### Option 1: Upgrade Microsoft.ApplicationInsights.AspNetCore (Recommended)

**Version 3.0.0-beta1** (and newer) properly supports .NET 8, 9, and 10:

```xml
<PackageVersion Include="Microsoft.ApplicationInsights.AspNetCore" Version="3.0.0-beta1" />
```

**Key changes in version 3.0.0:**
- Native support for `net8.0`, `net9.0`, and `net10.0` target frameworks
- No longer depends on old ASP.NET Core 2.x packages
- Built on OpenTelemetry infrastructure (breaking change - review migration guide)
- Dependencies:
  - `OpenTelemetry.Extensions.Hosting` 1.14.0
  - `OpenTelemetry.Instrumentation.AspNetCore` 1.14.0
  - `OpenTelemetry.Instrumentation.Http` 1.14.0
  - `OpenTelemetry.Instrumentation.SqlClient` 1.14.0-beta.1

**Note:** Version 3.0.0 is a major version with breaking changes. Review the migration documentation before upgrading.

### Option 2: Downgrade NUnit3TestAdapter (Temporary Workaround)

Revert to version 5.2.0 until Application Insights can be updated:

```xml
<PackageVersion Include="NUnit3TestAdapter" version="5.2.0" />
```

## Why Renovate Hasn't Flagged This

Microsoft.ApplicationInsights.AspNetCore version 3.0.0 is currently in **beta** (3.0.0-beta1). By default:

1. **Renovate's default behavior:** Does not suggest pre-release versions (alpha, beta, rc) unless explicitly configured
2. **Your renovate.json:** Does not have any specific rules for ApplicationInsights packages, so it follows the default stable-only policy
3. **Version 2.23.0 is the latest stable:** Renovate considers the project up-to-date with the latest stable version

### To Enable Beta/Pre-release Updates

Add to `renovate.json`:

```json
{
  "packageRules": [
    {
      "description": "Allow pre-release versions for Application Insights v3",
      "matchManagers": ["nuget"],
      "matchPackageNames": ["Microsoft.ApplicationInsights.AspNetCore"],
      "ignoreUnstable": false
    }
  ]
}
```

Or wait for the stable 3.0.0 release.

## Understanding the Runtime vs Test Context Behavior

### Why Does the Application Work Normally?

Your application runs fine in production/development because:

1. **ASP.NET Core hosting handles binding redirects automatically** - The web host infrastructure in .NET 9 performs automatic version forwarding
2. **Framework assemblies take precedence** - When the app runs, .NET 9's built-in assemblies satisfy the ApplicationInsights dependencies through version forwarding
3. **No strict version checking at runtime** - The runtime loader is more flexible about compatible versions

### Why Does It Fail in Tests?

The test context is different:

1. **NUnit Engine creates isolated AssemblyLoadContext** - This is by design to isolate test execution
2. **.NET 8+ has stricter AssemblyLoadContext behavior** - The isolation mechanisms enforce stricter version checking than regular app hosting
3. **The conflict is explicitly declared in deps.json** - The test engine sees both version requirements and cannot auto-redirect when they conflict

### Is This a Generic Engine Problem?

**No - it's a .NET 8+ runtime behavior change** affecting all test frameworks:

- NUnit Engine (NUnit3TestAdapter 6.0+ using net8.0 build)
- xUnit runners on .NET 8+
- MSTest on .NET 8+
- Any code using strict AssemblyLoadContext isolation on .NET 8+

The stricter assembly loading behavior is a **deliberate security and reliability improvement** in .NET 8+, not a bug.

### How NUnit Engine Attempts Resolution

The NUnit Engine (3.21.0 in adapter 6.0.0) implements sophisticated assembly resolution strategies:

1. **TrustedPlatformAssembliesStrategy** - Loads from `TRUSTED_PLATFORM_ASSEMBLIES` (framework assemblies)
2. **RuntimeLibrariesStrategy** - Uses `deps.json` and `DependencyContext` for application dependencies
3. **AspNetCoreStrategy** - Special handling for ASP.NET Core runtime dependencies
4. **WindowsDesktopStrategy** - Special handling for Windows Desktop runtime dependencies

The engine **does mimic runtime resolution behavior** through these strategies. However:

- When `deps.json` explicitly declares conflicting versions (2.1.1 from ApplicationInsights vs 9.0.0 from framework)
- And .NET 8+ `AssemblyLoadContext` enforces strict version matching
- The engine **cannot perform automatic binding redirects** that would violate the runtime's security model

### Why Downgrading the Adapter "Works"

NUnit3TestAdapter 5.2.0 targets `netcoreapp3.1`, which means:

- Tests run using .NET Core 3.1-7 runtime behavior (even when testing a .NET 9 app)
- Those runtimes had **more lenient assembly binding redirect behavior**
- The conflict gets **silently resolved via automatic version forwarding**

**This is NOT a proper fix** - it's masking the problem by using older, more permissive runtime behavior.

### The Real Issue

The conflict **IS in your dependency graph**, and while it's currently only surfacing in the test context, it could manifest in other scenarios:

- Future .NET versions with even stricter loading
- Different hosting scenarios (serverless, containers, etc.)
- Other tooling that uses .NET 8+ runtime isolation
- Third-party libraries that use strict AssemblyLoadContext

### Understanding What Actually Happens

#### Runtime Behavior (Application Works Fine)

When your web application runs normally:

1. **Assembly Unification** - The ASP.NET Core hosting layer doesn't actually load the 2.1.1 assembly from NuGet packages
2. **Framework Satisfaction** - The 2.1.1 reference in ApplicationInsights is just metadata saying "I need at least 2.1.1"
3. **Version Forwarding** - The runtime provides the newer framework version (8.0 or 9.0) and ApplicationInsights works fine with it
4. **Single AssemblyLoadContext** - Everything loads in the default context with unified assembly resolution

#### Test Behavior (Tests Fail)

In the isolated test context with NUnit3TestAdapter 6.0.0:

1. **Explicit deps.json Entries** - The test engine sees the deps.json which explicitly lists both version requirements:
   - Framework provides Microsoft.AspNetCore.Http.Extensions 9.0.0.0
   - ApplicationInsights deps.json declares Microsoft.AspNetCore.Http.Extensions 2.1.1 as a dependency
2. **Isolated AssemblyLoadContext** - The engine creates isolation to prevent test interference
3. **Strict Loading Rules** - .NET runtime rule: **You cannot load two different versions of the same assembly in one AssemblyLoadContext**
4. **Conflict Detection** - When the engine tries to honor both requirements, the runtime throws `FileLoadException`

#### Why Target Framework Doesn't Matter

Testing shows that **downgrading from .NET 9 to .NET 8 doesn't fix the issue**. This confirms:

- The problem is **not about the version gap** between framework (8.0 vs 9.0) and ApplicationInsights (2.1)
- The problem is **assembly identity conflict in isolated context** - two versions of the same assembly cannot coexist
- This affects **.NET 8 and .NET 9 equally** when using isolated AssemblyLoadContext
- The adapter's target framework (net8.0 for adapter 6.0.0) matters because .NET 8+ enforces stricter isolation rules

**Key principle:** Isolated AssemblyLoadContext + conflicting deps.json entries = always fails, regardless of target framework.

#### Why Adapter 5.2.0 Works

NUnit3TestAdapter 5.2.0 targets `netcoreapp3.1`, which means:

- Uses **.NET Core 3.1-7 runtime behavior** for assembly loading (even when testing .NET 8/9 apps)
- Those runtimes were **more lenient** about assembly version conflicts in isolated contexts
- The conflict still exists in deps.json, but the older runtime allows it to resolve through version forwarding
- **This is a workaround, not a fix** - the dependency graph still has the conflict

### The Only True Resolution

**Remove the conflicting dependency from your graph** by upgrading `Microsoft.ApplicationInsights.AspNetCore` to 3.x when stable:

- Eliminates ASP.NET Core 2.1.x transitive dependencies completely (no more 2.1.1 in deps.json)
- Aligns your entire dependency graph with .NET 8/9+
- Future-proofs against stricter assembly loading in future .NET versions
- Provides proper OpenTelemetry integration
- Works with any test adapter version and any target framework

### Summary of Options

| Option | Status | Pros | Cons |
|--------|--------|------|------|
| **Downgrade to NUnit3TestAdapter 5.2.0** | Temporary workaround | Tests pass immediately | Hides the real problem, uses older runtime behavior |
| **Wait for ApplicationInsights 3.0.0 stable** | Long-term solution | Proper fix, no beta risk | Requires waiting for release, eventual migration work |
| **Use ApplicationInsights 3.0.0-beta1** | Immediate proper fix | Resolves root cause now | Beta risk for production monitoring |

## Investigation Commands Used

```powershell
# Check transitive dependencies
dotnet list package --include-transitive | Select-String -Pattern "Microsoft.AspNetCore.Http"

# Check NuGet package dependencies
Get-Content "C:\Users\<username>\.nuget\packages\microsoft.applicationinsights.aspnetcore\2.23.0\microsoft.applicationinsights.aspnetcore.nuspec"

# Check NUnit adapter versions
[System.Reflection.AssemblyName]::GetAssemblyName($dll.FullName).Version
```

## Related Files

- Test file: `Fhi.Grunndata.OppslagWeb.IntegrationTests\EnvironmentTests.cs`
- Package versions: `Directory.Packages.props`
- Main project: `Fhi.Grunndata.OppslagWeb\Fhi.Grunndata.OppslagWeb.csproj`

## Date of Analysis

December 11, 2025
