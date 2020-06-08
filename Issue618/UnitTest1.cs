using System.IO;
using NUnit.Framework;

namespace Issue618
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
            var file = Path.Combine(TestContext.CurrentContext.TestDirectory, "Image1.bmp");
            TestContext.AddTestAttachment(file, "Description screenshot");
        }
    }
}