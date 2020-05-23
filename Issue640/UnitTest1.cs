using NUnit.Framework;

namespace FqnNameSpace
{
    public class TestClassName
    {
        [Test]
        public void TestMethod()
        {
            Assert.Pass();
        }

        [TestCase(42)]
        public void ParametrizedTestMethod(int x)
        {
            Assert.That(x, Is.EqualTo(42));
        }

        public class NestedTestClass
        {
            [Test]
            public void NestedTestMethod()
            {
                Assert.Pass();
            }
        }
    }

    [TestFixture("First",5)]
    [TestFixture("Second",6)]
    public class ParametrizedTestClass
    {
        private string p;
        private int l;
        public ParametrizedTestClass(string p,int l)
        {
            this.p = p;
            this.l = l;
        }

        [Test]
        public void PTCMethod()
        {
            Assert.That(p.Length, Is.EqualTo(l));
        }
    }
}