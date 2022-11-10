using NUnit.Framework;
using System;

namespace UnitTestProject1
{
    [TestFixture]
    public class UnitTest1
    {
        [Test]
        public void TestMethod1()
        {
            var d = Environment.CurrentDirectory;
            TestContext.WriteLine($"Current directory : {d}");
            Assert.AreEqual(1, 1);
        }
    }
}
