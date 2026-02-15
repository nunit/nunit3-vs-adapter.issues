using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Issue1332
{
    public sealed class QuotesTests
    {
        [TestCase("\"C:\\Path\\File.txt\"")]
        [TestCase("C:\\Path\\File.txt\"")]
        public void Test(string input) { }
    }
}
