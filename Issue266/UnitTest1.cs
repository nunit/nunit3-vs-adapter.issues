using NUnit.Framework;
using System;

namespace Issue266
{

    public class Tests
    {
        [TestFixture]
        public class TestClass
        {
            [OneTimeSetUp]
            public void FixtureSetup()
            {
                Console.WriteLine("FixSetUp");
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

            [OneTimeTearDown]
            public void FixtureTearDown()
            {
                Console.WriteLine("FixTearDown");
            }

            [Test]
            public void Test()
            {
                Console.WriteLine("Test");
            }
        }
    }
}