using NUnit.Framework;

namespace Issue473
{
    public class Tests
    {
        [Test]
        public void Normal() => Ran.Mark();

        [Test, Explicit]
        public void ExplicitMethod() => Ran.Mark();

        [Test, Explicit, Category("Slow")]
        public void ExplicitWithCategory() => Ran.Mark();
    }

    [Explicit]
    public class ExplicitFixture
    {
        [Test]
        public void InExplicitFixture() => Ran.Mark();
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
