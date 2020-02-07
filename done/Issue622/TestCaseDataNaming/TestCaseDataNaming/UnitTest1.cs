using System.Collections;
using NUnit.Framework;

namespace TestCaseDataNaming
{
    [TestFixture]
    public class MyTests
    {
        [TestCaseSource(typeof(MyDataClass), "TestCases")]
        public int DivideTest(int n, int d)
        {
            return n / d;
        }
    }

    public class MyDataClass
    {
        public static IEnumerable TestCases
        {
            get
            {
                yield return new TestCaseData(12, 3).Returns(4);
                yield return new TestCaseData(12, 2).Returns(6);
                yield return new TestCaseData(12, 4).Returns(3);
            }
        }
    }
}