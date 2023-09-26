namespace MyApp.Tests.NUnit
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
            Program.Main(new string[] { });
            Assert.Pass();
        }
    }
}