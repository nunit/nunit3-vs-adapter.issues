namespace igntest;

[Ignore("Not in use")]
public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Test1()
    {
        Assert.Fail();
    }

     [Test]
    public void Test2()
    {
        Assert.Fail();
    }
}