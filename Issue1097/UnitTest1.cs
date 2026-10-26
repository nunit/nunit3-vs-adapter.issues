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
        TestContext.Out.WriteLine(s);
        Assert.Pass();
    }

    // Issue 654: the ")." in the argument moved the test out of its class in Test Explorer.
    [TestCase("TestAttribute).")]
    public void Test654(string value)
    {
        TestContext.Out.WriteLine(value);
        Assert.Pass();
    }
}