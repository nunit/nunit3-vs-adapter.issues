using System;
using NUnit.Framework;

namespace Issue671
{
    public class Tests
    {
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            var dd = new SomeWhereDeepDown();
            dd.WhatDoWeDoHere();
        }

        [SetUp]
        public void Setup()
        {
            throw new Exception("oops In Setup");
        }

        [Test]
        public void Test1()
        {
            Assert.Pass();
        }

        [Test]
        public void Test2()
        {
            Assert.Pass();
        }
    }


    public class SomeWhereDeepDown
    {
        public SomeWhereDeepDown()
        {
            
        }

        public void WhatDoWeDoHere()
        {
            throw new Exception("oops Deep Down");
        }
    }
}