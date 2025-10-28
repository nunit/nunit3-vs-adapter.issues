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