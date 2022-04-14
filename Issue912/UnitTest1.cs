
using NUnit.Framework;

namespace Issue912
{


public class TestExplicit
{

    [Test]
    [Explicit]
    public void Test1()
    {
        Assert.Fail();
    }

    [Category("FOO")]
    [Test]
    [Explicit]
    public void Test2()
    {
        Assert.Fail();
    }

    [Test]
    [Category("FOO")]
    public void TestFoo()
    {
        Assert.Pass();
    }

    [Test]
    [Category("BAR")]
    public void TestBar()
    {
        Assert.Pass();
    }

}
}