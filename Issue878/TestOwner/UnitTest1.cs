using NUnit.Framework;

[assembly: NUnit.Framework.Property("Owner", "Kermit the frog")]

namespace TestOwner
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            Assert.Pass();
        }
    }
}