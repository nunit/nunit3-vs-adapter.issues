namespace Issue873;

public class SomeTests
{
    private static IEnumerable<TestCaseData> DataCases
    {
        get
        {
            yield return new TestCaseData(1, new[] { (1, 2) });
        }
    }
    [TestCaseSource(nameof(DataCases))]
    public void SomeTest(int x1, (int, int)[] expected)
    {
        var actual = new[] { (1, 2) };
        Assert.AreEqual(expected, actual);
    }
}