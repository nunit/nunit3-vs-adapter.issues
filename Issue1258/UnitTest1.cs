namespace NUnitMTP
{
    public class Tests
    {
        [Test, Category("Cat1")]
        public void Test1()
        {
            Assert.Pass();
        }

        [Property("Name","Whatever")]
        [Test, Category("Cat2")]
        public void Test2()
        {
            Assert.Pass();
        }
    }
}
