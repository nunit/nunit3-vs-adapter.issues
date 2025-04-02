namespace Test.NS;

public class NUnitTest
{
    [TestCase("StringLiteral", true, TestName = $"Test_StringLiteral_True")]
    public void Test(string stringValue, bool boolValue)
    {
        Assert.That(stringValue, Is.Not.Null);
        Assert.That(boolValue, Is.True);
    }
}