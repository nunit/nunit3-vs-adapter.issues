using NUnit.Framework;
using Project;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Tests
{
    [TestFixture]
    public class Class1
    {

        [Test]
        public void TestTrivial()
        {
            Assert.AreEqual(2, Program.Return2());
        }

        static readonly string _prefix = "..\\..\\..\\";    // Path relative from tests build to project root.
        static readonly string _sourcesDir = "cases.source";

        [TestCaseSource(nameof(GetAllFileNames))]
        public void TestForFile(string sourceFileNameWithExtension)
        {
            Assert.Pass();
        }

        public static List<string> GetAllFileNames()
        {

            var allSourceFilePaths = Directory.GetFiles(_prefix + _sourcesDir);
            var allSourceFileNamesWithExtensions = allSourceFilePaths.Select(Path.GetFileName);

            return allSourceFileNamesWithExtensions.ToList();
        }
    }
}
