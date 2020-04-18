using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NUnit.Framework;

namespace TestCaseSourceJumpToGivenLocation
{
    public class TestSourcing
    {
        [TestCaseSource(nameof(GetTestFiles))]
        public void TestHeader(FileInfo file)
        {
            Assert.Pass();
        }

        public static IEnumerable<TestCaseData> GetTestFiles
        {
            get
            {
                var testFiles = Directory.GetFiles(TestContext.CurrentContext.WorkDirectory,"*.txt");
                foreach (var f in testFiles)
                {
                    var td = new TestCaseData(new FileInfo(f))
                        .SetName(Path.GetFileNameWithoutExtension(f).Replace(".", "_"))
                        .SetDescription($"Description for: {f}")
                        .SetCategory("TextFiles");
                    td.Properties.Add("CodeFilePath", f);
                    td.Properties.Add("LineNumber", 1);
                    yield return td;
                        

                    /* How to setup Jump-File here??? */
                }
            }
        }
    }
}
