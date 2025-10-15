namespace NewEngine;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Test1()
    {
        int x = 42;
        Assert.That(x, Is.EqualTo(42));
    }
}
