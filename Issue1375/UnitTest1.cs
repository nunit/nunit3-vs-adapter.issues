using CSScriptLib;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

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
        Point result = script.CreatePoint(1, 2);
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
        var result = await CSharpScript.EvaluateAsync<Point>(
            "new Point { X = 1, Y = 2 }",
            ScriptOptions.Default.WithReferences(typeof(Point).Assembly).WithImports("TestCase")
        );
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.X, Is.EqualTo(1));
            Assert.That(result.Y, Is.EqualTo(2));
        }
        //result.X.ShouldBe(1);
        //result.Y.ShouldBe(2);
    }
}