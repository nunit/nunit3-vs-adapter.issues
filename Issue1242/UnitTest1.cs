namespace Issue1242;

public class MyItem
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
}

public static class TestData
{
    public static IEnumerable<TestFixtureData> Data
    {
        get
        {
            yield return new TestFixtureData(new MyItem { Id = Guid.NewGuid(), Name = "my name" });
            yield return new TestFixtureData(new MyItem { Id = Guid.NewGuid(), Name = "my other name" });
            yield return new TestFixtureData(new MyItem { Id = Guid.NewGuid(), Name = "my old name" });

            yield return new TestFixtureData(1, new MyItem { Id = Guid.NewGuid(), Name = "my name" });
            yield return new TestFixtureData(2, new MyItem { Id = Guid.NewGuid(), Name = "my other name" });
            yield return new TestFixtureData(3, new MyItem { Id = Guid.NewGuid(), Name = "my old name" });
        }
    }
}

[TestFixtureSource(typeof(TestData), nameof(TestData.Data))]
public class Tests
{
    private readonly MyItem _item;

    public Tests(MyItem item)
    {
        _item = item;
    }

    public Tests(int _, MyItem item)
    {
        _item = item;
    }

    [Test]
    public void Test1()
    {
        Assert.Pass();
    }
}
