using NUnit.Framework;
using System.Collections;

namespace Issue1351
{
    /// <summary>
    /// Parameterized fixture via TestFixtureSource with display names.
    /// TRX should show className="Issue1351.Tests(One)" / "Issue1351.Tests(Two)".
    /// </summary>
    [TestFixtureSource(nameof(GetTestCasesWithNames))]
    public class Tests
    {
        private int _counter;

        public Tests(int counter)
        {
            _counter = counter;
        }

        [Test]
        public void Test1()
        {
            Console.WriteLine($"Test1:{_counter}");
            Assert.Pass();
        }

        private static IEnumerable<TestFixtureData> GetTestCasesWithNames()
        {
            yield return new TestFixtureData(1).SetArgDisplayNames("One");
            yield return new TestFixtureData(2).SetArgDisplayNames("Two");
        }
    }

    /// <summary>
    /// Parameterized fixture via TestFixtureSource without display names.
    /// TRX should show className="Issue1351.TestsWithGetCases(1)" / "(2)".
    /// </summary>
    [TestFixtureSource(nameof(GetTestCases))]
    public class TestsWithGetCases
    {
        private int _counter;

        public TestsWithGetCases(int counter)
        {
            _counter = counter;
        }

        [Test]
        public void Test1()
        {
            Console.WriteLine($"Test1:{_counter}");
            Assert.Pass();
        }

        private static IEnumerable<TestFixtureData> GetTestCases()
        {
            yield return new TestFixtureData(1);
            yield return new TestFixtureData(2);
        }
    }

    /// <summary>
    /// Method-level parameters via [TestCase].
    /// TRX should show className="Issue1351.TestsWithMethodParams"
    /// and the TestMethodIdentifierProperty should carry parameterTypeFullNames=["System.Int32"].
    /// </summary>
    public class TestsWithMethodParams
    {
        [TestCase(1)]
        [TestCase(2)]
        public void TestWithIntParam(int value)
        {
            Console.WriteLine($"TestWithIntParam:{value}");
            Assert.Pass();
        }

        [TestCase(1, "hello")]
        [TestCase(2, "world")]
        public void TestWithTwoParams(int value, string label)
        {
            Console.WriteLine($"TestWithTwoParams:{value},{label}");
            Assert.Pass();
        }
    }
}
