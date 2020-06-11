using NUnit.Framework;

namespace Issue760
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test_Serialize()
        {
        }
        
        [TestCase(TestName ="Test_Serialize")]
        //[Test]
        public void Test_Serialize__ArgumentNullException()
        {
           // Assert.Fail();
        }

        [TestCase(2, 3, 5, TestName = "Whatever it is")]
        public void Test744(int a, int b, int c)
        {

        }
    }
}