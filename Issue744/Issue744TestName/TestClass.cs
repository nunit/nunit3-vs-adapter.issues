// NUnit 3 tests
// See documentation : https://github.com/nunit/docs/wiki/NUnit-Documentation
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;

namespace Issue744TestName
{
    [TestFixture]
    public class TestClass
    {
        [Test]
        public void TestMethod()
        {
            // TODO: Add your test code here
            var answer = 42;
            Assert.That(answer, Is.EqualTo(42), "Some useful error message");
        }

        [TestCase(1, 2, 3, TestName = "Adding 1 and 2")]
        public void SumTest(int x, int y, int expectedresult)
        {
            Assert.That(x + y, Is.EqualTo(expectedresult));
        }

        [TestCase(1, 2, 3)]
        public void TestCaseTest(int x, int y, int expectedresult)
        {
            Assert.That(x + y, Is.EqualTo(expectedresult));
        }


    }
}
