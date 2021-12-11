namespace Issue918;

public class Tests
{
    [TestFixtureSource(typeof(FixtureSources), nameof(FixtureSources.Types))]
    public class SomeTest<T>
    {
        [Test]
        public void Foo()
        {
            DateTime? dateTime = DateTime.Now;
            Assert.That(dateTime, Is.Not.Null);
            DateTime x = dateTime.Value;
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