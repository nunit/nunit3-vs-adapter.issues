namespace Issue1097;

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

    [TestCase("I am a good test case (the best, even).")]
    public void Test1097(string s)
    {
        TestContext.WriteLine(s);
        Assert.Pass();
    }
}