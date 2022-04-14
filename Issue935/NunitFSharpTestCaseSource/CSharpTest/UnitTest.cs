using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;

using static FSharpLibrary.FSharpTypes;

namespace CSharpTest
{
    public class MyTestCases
        : IEnumerable<TestCaseData>
    {
        public IEnumerator<TestCaseData> GetEnumerator()
        {
            var myItem = new MyTestItem(DU2.NewDU2MemberOfDU1(DU1.NewDU1Member("aaa")), DU3.NewDU3Member("bbb"));
            var testCaseData = new TestCaseData(myItem);
            //testCaseData.TestName = "Setting test name work around the bug";
            yield return testCaseData;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();


        

    }

    [TestFixture]
    public class MyTests
    {
        [TestCaseSource(typeof(MyTestCases))]
        public void RunSomeTests(MyTestItem testItem) => Assert.Pass();


        [TestCaseSource(nameof(GetBadTest2Data))]
        public void BadTest2(IEnumerable<(string, string)> collection) => Assert.Pass();

        private static IEnumerable<TestCaseData> GetBadTest2Data()
        {
            yield return new TestCaseData(new[] { ("ccc", "ddd") });
        }
    }

}