namespace csharp;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Test1()
    {
        Assert.Pass();
    }

    [Test]
    public void Test2([Range(0,8)] int r)
    {
        Assert.Pass();
    }
}