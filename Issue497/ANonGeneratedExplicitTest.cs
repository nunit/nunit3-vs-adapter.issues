using NUnit.Framework;

namespace Filtering
{
    public class ANonGeneratedExplicitTest
    {
        [Category("IsExplicit")]
        [Explicit]
        [Test]
        public void TestExplicitTest()
        {
            Assert.Pass();
        }

        [Test]
        public void TestNotExplicitTest()
        {
            Assert.Pass();
        }

        [Test, Category("Slow")]
        public void RealSlowTestThatDoesntFail()
        {
            Assert.Pass();
        }

        [Explicit]
        [Test, Category("Slow")]
        public void TestExplicitTestSlow()
        {
            Assert.Pass();
        }

    }
}
