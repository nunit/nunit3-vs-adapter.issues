using NUnit.Framework;
using System;
using NUnit.Framework.Legacy;

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
            ClassicAssert.AreEqual(1, 1);
        }
    }
}
