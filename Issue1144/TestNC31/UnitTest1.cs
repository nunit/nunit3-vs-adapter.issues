using ClassLibNetC21;
using NUnit.Framework;

namespace TestNC31
{
    public class Tests
    {

        [Test]
        public void Test1()
        {
            var sut = new Matte();
            var result = sut.Add(2, 3);
            Assert.That(result, Is.EqualTo(5));
        }
    }

}