using NUnit.Framework;

public sealed class UnitTest1
{
    [TestCase("\"C:\\Path\\File.txt\"")]
    [TestCase("C:\\Path\\File.txt\"")]
    public void Test(string input)
    {
        Assert.Pass("It's all about the parameters");
    }
}

// From the issue comments (MaceWindu, 2026-01-14).
// To check in Test Explorer: run each test on its own, then "Run all", and read
// bin\Debug\<tfm>\Dump\E_Issue1349.dll.dump (dump.runsettings turns the dump on).
public sealed class ValuesTests
{
    // Reported: cannot be run, either on its own or with "Run all".
    [Test]
    public void Test1([Values("|")] string arg) => Assert.Pass();

    // Reported: runs on its own, but "Run all" throws
    // "Filter string 'Tests.Test2\("\\\"\)' includes unrecognized escape sequence".
    [Test]
    public void Test2([Values("\\")] string arg) => Assert.Pass();
}
