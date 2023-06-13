namespace Issue1089;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Test1()
    {
        TestContext.Progress.WriteLine("Test progress");
    }
}