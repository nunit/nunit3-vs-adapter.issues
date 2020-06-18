using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace FQN
{
    public  class Issue761
    {
        [TestCase("\ufff0")]
        [TestCase("\uffff")]
        [TestCase("\u1111")]
        public void Test_01(string value)
        {
            TestContext.WriteLine(value);
        }

        [TestCase("Whatever")]
        public void Test_02(string value)
        {
            TestContext.WriteLine(value);
        }
    }
}
