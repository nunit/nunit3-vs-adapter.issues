using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

// Issue 607, from https://github.com/NetMatch/nunit-adapter-testcase:
// test cases with names from SetName were not shown as run, and the method names stayed as not run.
namespace Testcase.Tests
{
    [TestFixture]
    public class Tests
    {
        public static IEnumerable<TestCaseData> AddCases
        {
            get
            {
                return Enumerate().Select(testCase => testCase.SetName($"{{m}}{{a}} : {testCase.ExpectedResult}"));

                IEnumerable<TestCaseData> Enumerate()
                {
                    yield return new TestCaseData(1, 1).Returns(2);
                    yield return new TestCaseData(1, 2).Returns(3);
                    yield return new TestCaseData(1, 3).Returns(4);
                }
            }
        }

        public static IEnumerable<TestCaseData> ConcatCases
        {
            get
            {
                return Enumerate().Select(testCase => testCase.SetName($"{{m}}{{a}} : \"{testCase.ExpectedResult}\""));

                IEnumerable<TestCaseData> Enumerate()
                {
                    yield return new TestCaseData("a", "b").Returns("ab");
                    yield return new TestCaseData("a", "c").Returns("ac");
                    yield return new TestCaseData("a", "d").Returns("ad");
                }
            }
        }

        [TestCaseSource(nameof(AddCases))]
        public int Add(int a, int b) => a + b;

        [TestCaseSource(nameof(ConcatCases))]
        public string Concat(string a, string b) => a + b;
    }
}
