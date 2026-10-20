namespace Issue1504;

public class NullableTestCaseTests
{
    [TestCase(1)]
    public void NonNullable(decimal value)
    {
        Assert.Pass();
    }

    [TestCase(1)]
    public void Nullable(decimal? value)
    {
        Assert.Pass();
    }
}