using NUnit.Framework;

namespace Issue530
{
    public class Tests
    {
        [Test]
        [Ignore("reasons")]
        public void IgnoredTest() => Assert.Fail();

        [Test]
        [Explicit("reasons")]
        public void ExplicitTest() => Assert.Fail();

        [TestCase(null, Explicit = true, Reason = "because")]
        public void ExplicitTestCase(object ignored) => Assert.Fail("This test should not be run.");
    }
}