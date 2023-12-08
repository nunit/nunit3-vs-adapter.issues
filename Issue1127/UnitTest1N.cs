namespace Issue1127.N;
using NUnit.Framework;
public class Tests
{
    [TestCase(" ", false, false)]
    [TestCase(" ", true, true)]
    [TestCase(" ", false, true)]
    [TestCase(" ", true, false)]
    [TestCase("", false, false)]
    [TestCase("", true, true)]
    [TestCase("", false, true)]
    [TestCase("",true,false)]
    public void Test1(string a, bool b, bool c)
    {
        Assert.That(b, Is.EqualTo(c));
        
    }

    [Test]
    public void Test2()
    {
        Assert.Pass();
    }

    [Test]
    public void Test3()
    {
        Assert.Fail("This fails");
    }
}