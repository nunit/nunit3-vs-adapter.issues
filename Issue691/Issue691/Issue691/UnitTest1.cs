using NUnit.Framework;

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
        [TestCase("Works fine", "\"")]
        //[TestCase("Breaks test executor when using Run All", "\"()")]
        [TestCase("Breaks test executor when using Run All", "\")")]
        public void TestName(string expectedBehaviour, string data)
        {
            //Irrelevant
        }
    }
}