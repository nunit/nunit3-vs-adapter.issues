namespace Issue425;
using NUnit.Framework;

public class Tests
{
    [Test]
    [Category("Foo")]
    public void Test1Foo()
    {
      Assert.Pass();
    }

    [Test]
    public void MyTest2()
    {
      Assert.Pass();
    }
    [Test]
    [Category("Foo")]
    public void Test2Foo()
    {
      Assert.Pass();
    }

    [Test]
    [Category("Bar")]
    [Category("Foo")]
    public void Test2FooAndBar()
    {
      Assert.Pass();
    }

    [Test]
    [Category("Bar")]
    public void Test3Bar()
    {
      Assert.Pass();
    }
    [Test]
    [Category("Foo")]
    public void Test4Foo()
    {
      Assert.Pass();
    }
}

