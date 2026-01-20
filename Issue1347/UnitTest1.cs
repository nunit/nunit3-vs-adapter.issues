using NUnit.Framework;

namespace NUnitIssue;

public static class TestIssue
{
    [TestCase("public class Test { public void Foo(string a = c) { } }", false)]
    [TestCase("public class Test { public void Foo(string a = \"c\") { } }", false)]
    public static void TestLength(string input, bool expected) =>
        Assert.That(input.Length % 2 == 0, Is.EqualTo(expected));
}
