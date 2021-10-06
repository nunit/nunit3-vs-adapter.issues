using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace UnitTests.SourceLines
{
    /// <summary>
    /// Issue 423
    /// </summary>
    public class MyDataClass
    {
        public static IEnumerable<TestCaseData> TestCases
        {
            get
            {
                yield return new TestCaseData(12, 3).SetName("NewTestName").Returns(4);
                yield return new TestCaseData(12, 4).Returns(3);
            }
        }
    }
    [TestFixture]
    public class MyTests2
    {
        [Test, TestCaseSource(typeof(MyDataClass), "TestCases")]
        public int DivideTest(int n, int d)
        {
            return n / d;
        }
    }
}
