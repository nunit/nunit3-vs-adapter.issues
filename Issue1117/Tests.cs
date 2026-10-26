using NUnit.Framework;

namespace Issue1117
{
    namespace Many
    {
        // 2100 test cases: more than the default AssemblySelectLimit of 2000.
        public class ManyTests
        {
            private static IEnumerable<int> Numbers() => Enumerable.Range(1, 2100);

            [TestCaseSource(nameof(Numbers))]
            public void Case(int n) => Assert.That(n, Is.Positive);
        }
    }

    namespace Other
    {
        public class OtherTests
        {
            // Not selected by FullyQualifiedName~Issue1117.Many. If it runs, ran.txt lists it.
            [Test]
            public void MustNotRun()
            {
                Ran.Mark();
                Assert.Fail("Selected by the filter only when the adapter dropped the filter");
            }
        }
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
