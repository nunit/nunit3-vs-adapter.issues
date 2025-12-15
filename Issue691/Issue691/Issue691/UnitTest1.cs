using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace Issue691
{
    public class Tests
    {
        [TestCase("Works fine", ")")]
        [TestCase("Works fine", "()")]
        [TestCase("Works fine", "(")]
        [TestCase("Works fine", "\"(")]
        [TestCase("Works fine", ")\"")]
        [TestCase("Works fine", "(\"")]
        [TestCase("Works fine", "()\"")]
        [TestCase("Works fine", "a b c")]
       // [TestCase("Works fine", "\"")]
       // [TestCase("Breaks test executor when using Run All", "\"()")]
       // [TestCase("Breaks test executor when using Run All", "\")")]
        public void TestName(string expectedBehaviour, string data)
        {
            //Irrelevant
        }
    }


    [TestFixture]
    public class UnitTest1
    {
        [Test]
        public void TestMethod1()
        {
            ClassicAssert.AreEqual("hi", "hi");
        }
    }
}