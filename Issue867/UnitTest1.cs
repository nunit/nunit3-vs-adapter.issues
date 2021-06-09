using NUnit.Framework;

namespace Issue867
{
    [TestFixture]
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [TestFixture(3)]
        [TestFixture(2)]
        [TestFixture(1)]
        internal class CommonTests : SomeBase
        {
            public int X { get; }
            public CommonTests(int x)
            {
                X = x;
            }

            [Test]
            public void TestIt()
            {
                Assert.That(X, Is.GreaterThan(0));
            }
        }
    }

    [TestFixture]
    public class SomeBase
    {
        [Test]
        public void BaseTest()
        {
            Assert.Pass();
        }

    }
}