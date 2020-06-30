using NUnit.Framework;

namespace Filtering
{
    public class ANonGeneratedExplicitTest
    {
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

    }
}
