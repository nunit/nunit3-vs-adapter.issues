using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Issue1332
{
    public sealed class MyTest
    {
        private static IEnumerable<object[]> TestData()
        {
            // Edge cases
            yield return new object[] { "", new List<string>() { } };
            yield return new object[] { "\"", new List<string>() { "\"" } };
            yield return new object[] { "\"\"", new List<string>() { "" } };
            yield return new object[] { "\"   \"", new List<string>() { "" } };
            yield return new object[] { "null", new List<string>() { "null" } };

            // Common cases (only non-quotation marked tokens)
            yield return new object[] { "Code", new List<string>() { "Code" } };
            yield return new object[] { "Code @", new List<string>() { "Code", "@" } };
            yield return new object[] { "Code @ Test", new List<string>() { "Code", "@", "Test" } };

            // Quotation cases (only quotation marked tokens)
            yield return new object[] { "\"Code\" \"block\"", new List<string>() { "Code", "block" } };
            yield return new object[] { "\"Code\" \" block\" ", new List<string>() { "Code", "block" } };
            yield return new object[] { "\"Code block\" \"This is a test\" ", new List<string>() { "Code block", "This is a test" } };
            yield return new object[] { "\"Code\"\"block\"", new List<string>() { "Code", "block" } };

            // Mixed cases
            yield return new object[] { "\'Code block\' @ \"Test\" \" app", new List<string>() { "\'Code", "block\'", "@", "Test", "\" app" } };
            yield return new object[] { "\"Code block\" @ \"Test\" \" app", new List<string>() { "Code block", "@", "Test", "\" app" } };
            yield return new object[] { " \"Code block\", @ \"Test\"    app", new List<string>() { "Code block", ",", "@", "Test", "app" } };
        }

        [Test]
        [TestCaseSource(nameof(TestData))]
        public void TestMethod(string searchString, List<string> resultList)
        {
            // do nothing, it's only about the method parameters.
        }
    }
}
