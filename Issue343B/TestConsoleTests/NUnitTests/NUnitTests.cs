using System;
using System.Diagnostics;
using NUnit.Framework;

namespace NUnitTests
{
    public class NUnitTests
    {
        [Test]
        public void ConsoleTest()
        {
            Console.WriteLine("Hello world from NUnit");
            Debug.WriteLine("Debug from NUnit");
        }
    }
}