namespace Issue918;

public class Tests
{
     [TestFixtureSource(typeof(FixtureSources), nameof(FixtureSources.Types))]
    public class SomeTest<T>
    {
        [Test]
        public void Foo()
        {
            Assert.Pass();
        }
    }

    public static class FixtureSources
    {
        public static Type[] Types =
        {
            typeof(object)
        };
    }
}