using NUnit.Framework;
using System.Collections.Generic;

namespace TestProject1
{
    public class Tests
    {
        [TestCaseSource(nameof(MyTestData))]
        public void Test1(bool input1)
        {
            Assert.Pass();
        }

        private static IEnumerable<TestCaseData> MyTestData
        {
            get
            {
                yield return createTestCaseData(true);
                yield return createTestCaseData(false);
            }
        }

        private static TestCaseData createTestCaseData(bool input1)
        {
            return new TestCaseData(input1).SetName($"{nameof(Test1)}:input1={input1}");
        }


    }
}