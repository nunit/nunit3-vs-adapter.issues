using System;
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
            Console.WriteLine("Hi testing Test1");
            TestContext.WriteLine("Context Test1");
            Assert.Pass();
        }
    }
}

