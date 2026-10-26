using NUnit.Framework;

namespace Issue1257
{
    namespace Core
    {
        // 2100 test cases, like the reporter's large project: more than the default AssemblySelectLimit of 2000.
        public class ManyTests
        {
            private static IEnumerable<int> Numbers() => Enumerable.Range(1, 2100);

            [TestCaseSource(nameof(Numbers))]
            public void Case(int n) => Assert.That(n, Is.Positive);
        }
    }

    namespace Multimedia
    {
        [TestFixture]
        [Platform("MacOsX")]
        public sealed class DevicesWatcherTests
        {
            [OneTimeSetUp]
            public void Setup() => Ran.Mark();

            [Test]
            public void CheckDeviceAddedRemoved() => Ran.Mark();
        }

        public class PlainMultimediaTests
        {
            [Test]
            public void NoPlatform() => Ran.Mark();
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
