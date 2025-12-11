using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace App.Test;

public class Tests
{
    [TestCase("IntegrationTests")]
    public void ThatWeCanGetEnvironment(string env)
    {
        // Try to force load the old version to trigger the conflict
        try
        {
            // This mimics what ApplicationInsights dependencies might try to do
            var oldHttpType = Type.GetType("Microsoft.AspNetCore.Http.HttpContext, Microsoft.AspNetCore.Http, Version=2.1.1.0, Culture=neutral, PublicKeyToken=adb9793829ddae60");
            Console.WriteLine($"Loaded old type: {oldHttpType?.AssemblyQualifiedName ?? "null"}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception loading old type: {ex.Message}");
        }

        // Diagnostic: Log all loaded assemblies before test execution
        Console.WriteLine("=== Loaded Assemblies Before Test ===");
        var loadedBefore = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => a.FullName.Contains("Microsoft.AspNetCore.Http"))
            .OrderBy(a => a.FullName);
        foreach (var asm in loadedBefore)
        {
            Console.WriteLine($"  {asm.FullName}");
            Console.WriteLine($"    Location: {asm.Location}");
        }

        // Diagnostic: Hook into assembly resolution to see what's being requested
        AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
        {
            if (args.Name.Contains("Microsoft.AspNetCore.Http.Extensions"))
            {
                Console.WriteLine($"=== Assembly Resolution Requested ===");
                Console.WriteLine($"  Requested: {args.Name}");
                Console.WriteLine($"  Requesting Assembly: {args.RequestingAssembly?.FullName ?? "null"}");
                
                // Try to find what's loaded
                var loaded = AppDomain.CurrentDomain.GetAssemblies()
                    .FirstOrDefault(a => a.FullName.StartsWith("Microsoft.AspNetCore.Http.Extensions"));
                if (loaded != null)
                {
                    Console.WriteLine($"  Already Loaded: {loaded.FullName}");
                    Console.WriteLine($"  Location: {loaded.Location}");
                }
            }
            return null;
        };

        var factory = new TestOptionsFactory(env);
        var environment = factory.Services.GetRequiredService<IWebHostEnvironment>();

        // Diagnostic: Log all loaded assemblies after test execution
        Console.WriteLine("=== Loaded Assemblies After Test ===");
        var loadedAfter = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => a.FullName.Contains("Microsoft.AspNetCore.Http"))
            .OrderBy(a => a.FullName);
        foreach (var asm in loadedAfter)
        {
            Console.WriteLine($"  {asm.FullName}");
            Console.WriteLine($"    Location: {asm.Location}");
        }

        Assert.That(environment.EnvironmentName, Is.EqualTo(env));
    }
}
