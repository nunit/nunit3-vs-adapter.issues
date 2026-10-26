using NUnit.Framework;

namespace Issue969
{
    [TestFixture("chrome")]
    [TestFixture("edge")]
    [TestFixture("chromeheadless")]
    public class SampleClass
    {
        private readonly string _browser;

        public SampleClass(string browser) => _browser = browser;

        [TestCase("a", "b")]
        public void SampleMethod(string p1, string p2) => Ran.Mark();
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
