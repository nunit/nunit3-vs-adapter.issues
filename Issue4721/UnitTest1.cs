namespace Issue4721;

public class Tests
{

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        Assert.Fail("I am in one time setup");
    }

    [OneTimeTearDown]
    public void Teardown()
    {
        Assert.Fail("I am in one time teardown");
    }

    [Test]
    [Explicit]
    public void FailedTest()
    {
        Assert.Fail();
    }

    [Test]
    [Explicit]
    public void SuccessTest()
    {
        Assert.Pass();
    }

    [Test]
    public void DummySuccessTest()
    {
        Assert.Pass();
    }
}
