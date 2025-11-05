using NUnit.Framework;
using System.Collections;

namespace Issue1351
{
    //[TestFixture(1)]
    //[TestFixture(2)]
    //[TestFixtureSource(nameof(GetTestCases))]
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

        private static IEnumerable<TestFixtureData> GetTestCases()
        {
            yield return new TestFixtureData(1);
            yield return new TestFixtureData(2);
        }

        private static IEnumerable<TestFixtureData> GetTestCasesWithNames()
        {
            yield return new TestFixtureData(1).SetArgDisplayNames("One");
            yield return new TestFixtureData(2).SetArgDisplayNames("Two");
        }
    }
}