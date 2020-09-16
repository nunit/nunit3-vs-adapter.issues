using System;
using NUnit.Framework;

namespace Issue774
{
    
        [TestFixture]
        public class Tests
        {
            [Test]
            public void DoesNotWorkIn4alpha1()
            {
                Console.WriteLine();
                Assert.Pass();
            }

            [Test]
            public void Works()
            {
                Console.WriteLine("content");
                Assert.Pass();
            }

            [Test]
            public void MultipleLines()
            {
                Console.WriteLine("Whatever 1");
                Console.WriteLine();
                Console.WriteLine("Whatever 2");
                Assert.Pass();
            }
        }
    
}