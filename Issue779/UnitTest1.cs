using NUnit.Framework;

namespace Issue779
{
    public class Tests
    {
        [Category("CategoryA")]
        [Property("Priority","3")]
        [Property("Bug", "12345")]
        [Test]
        public void Test1()
        {
            Assert.Pass();
        }

        [Category("CategoryB")]
        [Test]
        public void Test2()
        {
            Assert.Pass();
        }
    }
}