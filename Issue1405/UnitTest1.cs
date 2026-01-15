using NUnit.Framework;

namespace NUnitIssue;

public static class TestIssue
{

    [TestCase("This | That", false)]
    public static void TestLength(string input, bool expected) =>
        Assert.That(input.Length % 2 == 0, Is.EqualTo(expected));
}
