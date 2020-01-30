using NUnit.Framework;

namespace Issue710
{
    public class Tests
    {
        [TestCaseSource("MyTestCases")]
        public void MyTest(string path, (string type, string name)[] expectedResults)
        {
        }

        static object[] MyTestCases =
        {
            // Folders
            new object[] { "pathA", new[] { (type: "TypeA", name: "A") } },
            new object[] { "pathB", new[] { (type: "TypeA", name: "A"), (type: "TypeB", name: "B") } },
        };
    }
}