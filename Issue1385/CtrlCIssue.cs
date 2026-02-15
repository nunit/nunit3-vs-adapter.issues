using NUnit.Framework;
using System.Threading;

namespace CtrlCIssue
{
    [TestFixture]
    public class UnitTests
    {
        [Test]
        public void Test()
        {
            TestContext.Progress.WriteLine("Starting sleep...");
            Thread.Sleep(10000);
            TestContext.Progress.WriteLine("Finished sleep.");
        }
    }
}