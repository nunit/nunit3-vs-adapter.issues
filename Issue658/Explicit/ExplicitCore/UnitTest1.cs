// NUnit 3 tests
// See documentation : https://github.com/nunit/docs/wiki/NUnit-Documentation
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;

namespace ExplicitCore
{
    [TestFixture]
    public class TestClassCore
    {
        [Explicit]
        [Test]
        public void TestMethodCoreIsExplicit()
        {
            // TODO: Add your test code here
            var answer = 42;
            Assert.That(answer, Is.EqualTo(42), "Some useful error message");
        }

        [Test]
        public void TestMethodCoreNotExplicit()
        {
            // TODO: Add your test code here
            var answer = 42;
            Assert.That(answer, Is.EqualTo(42), "Some useful error message");
        }

        [Ignore("ignored")]
        [Test]
        public void AnIgnoredTest()
        {

        }


        [Test, Category("Slow")]
        public void SlowAndFailsTestMethod()
        {
            Assert.Fail();
        }

        [Test]
        public void FailsTestMethod()
        {
            Assert.That(1,Is.EqualTo(2));
        }
    }
}