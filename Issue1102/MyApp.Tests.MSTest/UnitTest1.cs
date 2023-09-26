namespace MyApp.Tests.MSTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            Program.Main(new string[] { });
            Assert.IsTrue(true);
        }
    }
}