using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestNunit
{
    [TestFixture]
    public class StupidTest
    {
        [TestCase(',')]
        [TestCase('1')]
        [TestCase('2')]
        [TestCase('3')]
        [TestCase('4')]
        [TestCase('5')]
        [TestCase('6')]
        [TestCase('7')]
        [TestCase('8')]
        [TestCase('9')]
        [TestCase('0')]
        public void Verify_re_run(char parameter)
        {
            if (parameter == ',')
            {
                Assert.Fail("This test will always fails with ,");
            }
        }
    }
}
