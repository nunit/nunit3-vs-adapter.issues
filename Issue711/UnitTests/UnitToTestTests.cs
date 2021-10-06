using System.Collections.Generic;
using ApplicationToTest;
using NUnit.Framework;

namespace UnitTests
{
    [TestFixture]
    public class Tests
    {
        [TestCaseSource(typeof(UnitToTestCaseData), nameof(UnitToTestCaseData.Cases))]
        public void TestUnit(string input)
        {
            var unit = new UnitToTest();
            var output = unit.TestMe(input);
            Assert.AreEqual(input, output);
        }

        [Test]
        public void TestUnitDirect()
        {
            const string Input = "Input A";

            var unit = new UnitToTest();
            var output = unit.TestMe(Input);
            Assert.AreEqual(Input, output);
        }


    }


    [TestFixture]
    public class TestFilterBug
    {
        [TestCase(TestName = "Invalid = TestNameEquals")]
        public void ReservedNames()
        {
            Assert.True(true);
        }
    }

    public class TestFrom549
    {
        [TestCaseSource("Testcases")]
        public void foo()
        {
        }
        public static IEnumerable<TestCaseData> Testcases { get; } = new[] { new TestCaseData().SetName("&") };
    }

    public class TestsFrom691
    {
        [TestCase("Works fine", ")")]
        [TestCase("Works fine", "()")]
        [TestCase("Works fine", "(")]
        [TestCase("Works fine", "\"(")]
        [TestCase("Works fine", ")\"")]
        [TestCase("Works fine", "(\"")]
        [TestCase("Works fine", "()\"")]
        [TestCase("Breaks test executor when using Run All", "\"()")]
        [TestCase("Breaks test executor when using Run All", "\")")]
        public void TestName(string expectedBehaviour, string data)
        {
            Assert.Pass();
        }
    }
}
// BackLook.Apps.WorkInProgress.Specs.Computing work in progress Project.No Workpackages exist"

/// <summary>
/// Issue 876
/// </summary>
namespace BackLook.Apps.WorkInProgress
{
    public class Specs
    {
        [TestCase(TestName = "Computing work in progress Project.No Workpackages exist")]
        public void Test876()
        {
            Assert.Pass();
        }
}
}