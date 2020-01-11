using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace ParametrizedIssue
{
    public class MiscTests
    {
        [TestCase("aaa")]
        [TestCase("bbb")]
        [TestCase("ccc")]
        public void TestCasesWithSimpleStrings(string s)
        {

        }

        //[Test]
        //public void TestCasesWithRandom([Random(3)] int someRandomInt)
        //{

        //}
    }
}
