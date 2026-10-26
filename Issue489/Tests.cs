using System.Collections.Generic;
using NUnit.Framework;

// Issue 489: tests with names set by TestName or SetName showed twice in Test Explorer:
// once from source discovery, never run, and once from the adapter.
namespace NUnitSetName
{
    public class Class1
    {
        [Test]
        public void SimpleTest() { }

        [TestCase(1)]
        public void TestCase(int i) { }

        [TestCase(1, TestName = "{m} with a 1")]
        [TestCase(2, TestName = "an overriden test name with a 2")]
        public void NamedTestCase(int i) { }

        [TestCaseSource(nameof(Sauce))]
        public void SourcedTestCase(int i) { }

        public static IEnumerable<TestCaseData> Sauce
        {
            get
            {
                yield return new TestCaseData(1).SetName("totally overriden name");
                yield return new TestCaseData(2).SetName("{m} - enhanced name");
            }
        }
    }
}
