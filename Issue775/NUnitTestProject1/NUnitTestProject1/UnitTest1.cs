using NUnit.Framework;
using System.Collections.Generic;

namespace NUnitTestProject1
{
    [TestFixture]
    public class Tests
    {
        public class CustomTestData
        {
            public double[] numbers;
        }

        public static IEnumerable<TestCaseData> TestDataFactory()
        {
            yield return new TestCaseData(new CustomTestData
            {
                numbers = new[] { 0.0, 1.0, 2.1 }
            }).SetName("A) First scenario");

            yield return new TestCaseData(new CustomTestData
            {
                numbers = new[] { 0.1, 1.1, 2.2 }
            }).SetName("B) Second scenario");
        }

        [TestCaseSource("TestDataFactory")]
        public void TestCaseSOurceTest(CustomTestData data)
        {
            Assert.That(data.numbers, Is.Not.Empty);
        }

        [Test]
        public void SimpleTest()
        {
            Assert.Pass();
        }
    }
}