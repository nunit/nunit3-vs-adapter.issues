using System;
using System.Diagnostics;
using NUnit.Framework;

namespace NUnitTestProject
{
    [TestFixture]
    public class UnitTest1
    {

        [OneTimeSetUp]
        public void Init()
        {
            Trace.Listeners.Add(new ConsoleTraceListener());
            Console.WriteLine("Console Writeline - In OneTimeSetup");
            TestContext.Out.WriteLine("Testcontext out");
        }

        [Test]
        public void TestMethod1()
        {
            Console.Error.WriteLine("console error");
            Console.WriteLine("console writeline");
            Trace.WriteLine("Trace");
            Debug.WriteLine("Debug");
            Assert.IsTrue(true);
        }

        [Test]
        public void TestMethod2()
        {
            Assert.IsTrue(true);
        }
    }
}
