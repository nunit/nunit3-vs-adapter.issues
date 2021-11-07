using NUnit.Framework;

namespace Issue909
{

    public class Tests
    {
        [Test,Explicit]
        public void TestAsExplicit()
        {
            Assert.Pass();
        }

        [Test, Ignore("")]
        public void TestAsIgnore()
        {
            Assert.Pass();
        }

        [Test]
        public void TestAsNormal()
        {
            Assert.Pass();
        }

    }
}