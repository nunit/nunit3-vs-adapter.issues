using NUnit.Framework;

namespace Issue919
{
//  public class Tests
//  {
    public class Foo
    {
        [TestCase(1)]
        public void Baz(int a)
        {
            Assert.Pass();
        }

        [Test]
        public void Barr()
        {
            Assert.Pass();
        }
    }
}
//}