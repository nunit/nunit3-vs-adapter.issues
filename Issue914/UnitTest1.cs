using NUnit.Framework;

namespace Issue914
{

    public class Tests
    {
        [Test]
        public void TestNunitAttachment()
        {
            TestContext.AddTestAttachment(@"C:\Windows\WindowsUpdate.log");
        }
    }
}