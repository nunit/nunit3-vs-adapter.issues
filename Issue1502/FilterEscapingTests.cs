namespace Issue1502;

/// <summary>
/// Run each case on its own from Test Explorer, then read &lt;MsFilter&gt; and
/// &lt;ParsedTestCases&gt; in bin\Debug\net10.0\Dump\. See explicit.md.
/// </summary>
public class FilterEscapingTests
{
    [TestCase("a & b")]
    [TestCase("a &amp; b")]
    [TestCase("say \"hi\"")]
    [TestCase(@"C:\Temp")]
    [TestCase("a\nb")]
    [TestCase("x(y")]
    [TestCase("x)y")]
    [TestCase("a | b")]
    [TestCase("a = b")]
    [TestCase("<tag>")]
    public void StringArgument(string s) => Assert.Pass(s);

    [TestCase(1, TestName = "Name with & and &amp;")]
    public void CustomName(int i) => Assert.Pass();
}
