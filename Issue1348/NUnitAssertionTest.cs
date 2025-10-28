
using NUnit.Framework;

namespace Issue1348
{
    public class NUnitAssertionTest
    {
        [Test]
        public void TestMethod_NUnitAssertion()
        {
            Assert.Throws<AssertionException>(() =>
            {
                Assert.That("foo",Is.Null);
            });
        }
    }
}
