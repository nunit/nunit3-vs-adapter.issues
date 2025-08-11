namespace Issue1290;

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

    // [TearDown]
    public void TearDown()
    {
        string s = null!;
        var l = s.Length; // Cause a nullref exception, should be seen in the output
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        string s = null!;
        var l = s.Length; // Cause a nullref exception, should be seen in the output
    }

}
