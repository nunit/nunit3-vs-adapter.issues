using NUnit.Framework;

namespace NUnit3VSIssue811
{
    [TestFixture]
    [Ignore("https://github.com/nunit/nunit3-vs-adapter/issues/811")]
    public class Ignore
    {
        [Test]
        public void IgnoredTest()
        {
        }
    }
}
