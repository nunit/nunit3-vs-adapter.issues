using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace nunit_error
{
    [TestFixture]
    public class TestClass
    {
        private class TestCases
        {
            private static IEnumerable<TestCaseData> TestData
            {
                get
                {
                    yield return new TestCaseData("Test 1").SetName("TestData 1");
                    yield return new TestCaseData("Test 2").SetName("TestData 2");
                    yield return new TestCaseData("Test 3").SetName("TestData 3");
                    yield return new TestCaseData("Test 4").SetName("TestData 4");
                    yield return new TestCaseData("Test 5").SetName("TestData 5");
                }
            }
        }

        [SetUp]
        public void Setup()
        {
            //
        }

        [TestCaseSource(typeof(TestCases), "TestData")]
        public void TestMethod(string data)
        {
            Console.WriteLine(data);
        }
    }
}