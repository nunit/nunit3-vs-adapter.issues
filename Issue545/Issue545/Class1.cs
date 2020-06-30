using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Issue545
{
    [TestFixture]
    public class FooTests
    {
        private static readonly TestCaseData[] Values =
        {
            new TestCaseData(1),
            new TestCaseData(2),
        };

        [Explicit("Don't run this")]
        [TestCaseSource(nameof(Values))]
        public void Test1(object value)
        {
        }

        [Test]
        public void Test2()
        {
            Assert.Pass();
        }

    }
}
