using NUnit.Framework;

namespace TestProject3
{
    public sealed class Test1
    {
        [Test]
        [TestCaseSource(nameof(GetInvalidCharacters))]
        public void TestMethod1(string s)
        {
        }

        private static IEnumerable<string> GetInvalidCharacters => ["\""];


        [TestCase(@"FOO\BAR")]
        public void Test2(string s)
        {
            Assert.That(s, Is.Not.Null);
        }
    }

    /// <summary>
    /// From NUNit Issue 5046
    /// </summary>
    public static class TestIssue
    {
        [TestCase("public class Test { public void Foo(string a = \"c\") { } }", false)]
        public static void TestLength(string input, bool expected) =>
            Assert.That(input.Length % 2 == 0, Is.EqualTo(expected));
    }
}
