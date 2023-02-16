namespace Issue1053;

public class Tests
{
    [Test]
    public void Test1()
    {
        Console.WriteLine("test1"); //<-- Warning will occur (see log below)
        Console.WriteLine("test2"); //<-- No Warning
        Assert.Pass();
    }
}