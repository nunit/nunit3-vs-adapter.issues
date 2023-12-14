using ClassLibFW45;

namespace Issue1144
{
    using NUnit.Framework;

    public class Tests
    {
        
        [Test]
        public void Test1()
        {
            var sut = new Matte();
            var result = sut.Add(2, 3);
            Assert.That(result,Is.EqualTo(5));
        }
    }
}