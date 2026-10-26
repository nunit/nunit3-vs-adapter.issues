using NUnit.Framework;

namespace Issue1202
{
    public class Tests
    {
        [Test]
        public void FailedTestName() => Ran.Mark();

        [Test]
        public void OtherTest() => Ran.Mark();
    }

    internal static class Ran
    {
        private static readonly object Lock = new();

        public static void Mark()
        {
            var path = Path.Combine(TestContext.CurrentContext.WorkDirectory, "ran.txt");
            lock (Lock)
                File.AppendAllText(path, TestContext.CurrentContext.Test.FullName + Environment.NewLine);
        }
    }
}
