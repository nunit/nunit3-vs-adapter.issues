using NUnit.Framework;

namespace Issue1197
{
    [TestFixture, Category("MyCustomCategory")]
    [Explicit]
    [Ignore("Ignoring")]
    public class MyTestClass
    {
        [Test]
        public void MyTest() => Ran.Mark();
    }

    [TestFixture, Category("MyCustomCategory")]
    [Explicit]
    public class AnotherTestClass
    {
        [Test]
        public void AnotherTest() => Ran.Mark();
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
