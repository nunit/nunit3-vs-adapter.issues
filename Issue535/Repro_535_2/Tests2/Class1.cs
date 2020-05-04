using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Tests2
{
    [TestFixture]
    public class Class1
    {

        [Test]
        public void TestTrivial2()
        {
        }

        static readonly string _sourcesDir = "cases.source";

        [TestCaseSource(nameof(GetAllFileNames))]
        public void TestForFile2(string sourceFileNameWithExtension)
        {
            Assert.Pass();
        }

        public static List<string> GetAllFileNames()
        {

            var allSourceFilePaths = Directory.GetFiles(Directory.GetCurrentDirectory() + @"\Tests2\" + _sourcesDir);
            var allSourceFileNamesWithExtensions = allSourceFilePaths.Select(Path.GetFileName);

            return allSourceFileNamesWithExtensions.ToList();
        }
    }
}
