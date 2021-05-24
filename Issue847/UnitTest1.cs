using NUnit.Framework;

namespace Issue847
{
    public class Tests
    {
        [Test]
        [Category("Whatever")]
        [Description("some text here")]
        public void Test1()
        {
            Assert.Pass();
        }
    }
}

