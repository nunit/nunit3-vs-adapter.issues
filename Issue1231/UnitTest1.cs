using System.Runtime.CompilerServices;

namespace NUnitTestNamespace;

public class TestClass
{
    [Property("Priority","1")]
    [Test]
    public void Test1()
    {
        Assert.Pass();
    }

    [Property("Whatever", "SomeValue")]
    [Test]
    public void Test2()
    {
        Assert.Pass();
    }

    [Category("SomeCategory")]
    [Test]
    public void Test3()
    {
        Assert.Pass();
    }
}

public class TestClass2
{
    [Property("Priority","1")]
    [Test]
    public void Test1()
    {
        Assert.Pass();
    }
}
