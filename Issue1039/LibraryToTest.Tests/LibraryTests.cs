using NUnit.Framework;
using NUnit.Framework.Legacy;

using System.Collections.Generic;
using System.Diagnostics;

namespace TherapySelector.Tests
{
    public class LibraryTests
    {
        private int i = 0;
        public LibraryTests()
        {
            TestContext.Out.WriteLine("Ctor");
            i++;
            
        }
        
        [Test]
        public void SampleTest()
        {
            ClassicAssert.IsTrue(true);
            var x = new List<int>();
            Assert.That(42, Is.GreaterThan(41));
            TestContext.Out.WriteLine($"Sampletest {i}");
        }
    }
}