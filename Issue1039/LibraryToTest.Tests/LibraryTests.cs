using NUnit.Framework;

using System.Collections.Generic;
using System.Diagnostics;

namespace TherapySelector.Tests
{
    public class LibraryTests
    {
        private int i = 0;
        public LibraryTests()
        {
            TestContext.WriteLine("Ctor");
            i++;
            Process notepad = new Process();
            notepad.StartInfo.FileName = "notepad.exe";
            notepad.StartInfo.Arguments = "DemoText";
            notepad.Start();
        }
        
        [Test]
        public void SampleTest()
        {
            Assert.IsTrue(true);
            var x = new List<int>();
            Assert.That(42, Is.GreaterThan(41));
            TestContext.WriteLine($"Sampletest {i}");
        }
    }
}