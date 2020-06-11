using System;
using NUnit.Framework;

namespace Issue740
{
    public class Tests
    {

       
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test0()
        {

        }


        [Test]
        public void Test1()
        {
            //TestContext.WriteLine("Just writeline from TestContext");
            //Console.WriteLine("Console.Writeline doing this");
            Assert.Warn("Some nice warning");
            Assert.That(true,"Assert failed");
        }

        [Test]
        public void Test2()
        {

        }
    }
}