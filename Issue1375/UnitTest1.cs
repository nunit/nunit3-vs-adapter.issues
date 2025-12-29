using CSScriptLib;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System.Reflection;
using System.Runtime.Loader;

namespace TestCase;

public class Point
{
    public int X { get; set; }
    public int Y { get; set; }
}

[TestFixture]
public class DynamicEvaluationTests
{
    [Test]
    public void TestCSScript()
    {
        dynamic script = CSScript.Evaluator.LoadCode(
            @"using TestCase;
             public class Script
             {
                 public Point CreatePoint(int x, int y)
                 {
                     return new Point { X = x, Y = y };
                 }
             }"
        );
        var result = script.CreatePoint(1, 2);
        var t = result.GetType();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.X, Is.EqualTo(1));
            Assert.That(result.Y, Is.EqualTo(2));
        }
        //result.X.ShouldBe(1);
        //result.Y.ShouldBe(2);
    }

    [Test]
    public async Task TestRoslyn()
    {
        var result = await CSharpScript.EvaluateAsync<dynamic>(
             @"new TestCase.Point { X = 1, Y = 2 }",
            ScriptOptions.Default.WithReferences(typeof(Point).Assembly).WithImports("TestCase")
        );
        var actualType = result.GetType();
        Console.WriteLine(actualType.FullName);  // Output: TestCase.Point
        Console.WriteLine(actualType.Assembly.FullName);
        Console.WriteLine(typeof(Point).FullName);  // Output: TestCase.Point
        Console.WriteLine(typeof(Point).Assembly.FullName);
        Type dynamicPointType = result.GetType();
        Type testPoint = typeof(Point);

        // These should be TRUE:
        Console.WriteLine($"Same Type: {dynamicPointType == testPoint}");
        Console.WriteLine($"Same Assembly: {dynamicPointType.Assembly == testPoint.Assembly}");

        var dynamicContext = AssemblyLoadContext.GetLoadContext(dynamicPointType.Assembly);
        var testContext = AssemblyLoadContext.GetLoadContext(testPoint.Assembly);

        Console.WriteLine($"Dynamic Context: {dynamicContext?.Name} (Hash: {dynamicContext?.GetHashCode()})");
        Console.WriteLine($"Test Context: {testContext?.Name} (Hash: {testContext?.GetHashCode()})");
        Console.WriteLine($"Same Context: {dynamicContext == testContext}");

        Console.WriteLine("-------------------------------------------------");
        

        Assembly dynamicAssembly = dynamicPointType.Assembly;
        Assembly testAssembly = testPoint.Assembly;

        Console.WriteLine("=== Assembly Comparison ===");
        Console.WriteLine($"Dynamic Assembly: {dynamicAssembly}");
        Console.WriteLine($"Test Assembly: {testAssembly}");
        Console.WriteLine($"Same Instance: {ReferenceEquals(dynamicAssembly, testAssembly)}");
        Console.WriteLine($"Hash Codes: {dynamicAssembly.GetHashCode()} vs {testAssembly.GetHashCode()}");

        Console.WriteLine("\n=== Identity ===");
        Console.WriteLine($"Dynamic FullName: {dynamicAssembly.FullName}");
        Console.WriteLine($"Test FullName: {testAssembly.FullName}");
        Console.WriteLine($"Dynamic CodeBase: {dynamicAssembly.Location}");
        Console.WriteLine($"Test CodeBase: {testAssembly.Location}");

        Console.WriteLine("\n=== Location ===");
        Console.WriteLine($"Dynamic Location: {dynamicAssembly.Location}");
        Console.WriteLine($"Test Location: {testAssembly.Location}");
        Console.WriteLine($"Same Location: {dynamicAssembly.Location == testAssembly.Location}");

        Console.WriteLine("\n=== Module Info ===");
        Console.WriteLine($"Dynamic Module: {dynamicAssembly.ManifestModule.FullyQualifiedName}");
        Console.WriteLine($"Test Module: {testAssembly.ManifestModule.FullyQualifiedName}");
        Console.WriteLine($"Dynamic Module Hash: {dynamicAssembly.ManifestModule.GetHashCode()}");
        Console.WriteLine($"Test Module Hash: {testAssembly.ManifestModule.GetHashCode()}");

        Console.WriteLine("\n=== Load Context ===");
        Console.WriteLine($"Dynamic Context: {dynamicContext?.Name} (Collectible: {dynamicContext?.IsCollectible})");
        Console.WriteLine($"Test Context: {testContext?.Name} (Collectible: {testContext?.IsCollectible})");

        Console.WriteLine("\n=== Metadata Token ===");
        Console.WriteLine($"Dynamic Point Token: {dynamicPointType.MetadataToken}");
        Console.WriteLine($"Test Point Token: {testPoint.MetadataToken}");

        Console.WriteLine("\n=== Type Comparison ===");
        Console.WriteLine($"Same Type: {dynamicPointType == testPoint}");
        Console.WriteLine($"Type Equals: {dynamicPointType.Equals(testPoint)}");
        Console.WriteLine($"AssignableFrom: {testPoint.IsAssignableFrom(dynamicPointType)}");
        Console.WriteLine($"Type Handle Equal: {dynamicPointType.TypeHandle.Value == testPoint.TypeHandle.Value}");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.X, Is.EqualTo(1));
            Assert.That(result.Y, Is.EqualTo(2));
            Assert.That(dynamicContext== testContext);
            Assert.That(dynamicAssembly == testAssembly);
        }
        //result.X.ShouldBe(1);
        //result.Y.ShouldBe(2);
    }
}