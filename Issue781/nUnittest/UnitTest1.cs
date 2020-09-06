using NUnit.Framework;
using System;

namespace nUnittest
{
    [TestFixture]
    public class UnitTest1
    {
        [OneTimeSetUp]
        public static void OneTimeSetUp()
        {
            Console.WriteLine("OneTimeSetUp");
        }
        [SetUp]
        public void SetUp()
        {
            Console.WriteLine("SetUp");
        }
        [TearDown]
        public void TearDown()
        {
            Console.WriteLine("TearDown");
        }
        [Test]
        public void Test()
        {
            Console.WriteLine("Test");
        }

        [OneTimeTearDown]
        public static void OneTimeTearDown()
        {
            Console.WriteLine("OneTimeTearDown");
        }
    }
}
