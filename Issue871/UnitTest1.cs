using NUnit.Framework;

namespace Issue871
{
    [TestFixture]
    public class Tests
    {
        bool b1 = true;
        bool b2 = false;

        [Test]
        public void NUnitTest()
        {
            b1 = true;
            b2 = false;
        }

        [TearDown]
        public void TearDownTest()
        {
            if (b1 != b2)
            {
                Assert.Fail("Test failed!");
            }
        }
    }
}