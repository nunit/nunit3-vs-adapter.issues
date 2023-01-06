namespace Issue425;
using NUnit.Framework;

public class Tests
{
    [Test]
    [Category("Foo")]
    public void Test1()
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
    public void MyTest()
    {
      Assert.Pass();
    }

    [Test]
    [Category("Bar")]
    [Category("Foo")]
    public void Test2()
    {
      Assert.Pass();
    }

    [Test]
    [Category("Bar")]
    public void MyTest3()
    {
      Assert.Pass();
    }
    [Test]
    [Category("Foo")]
    public void MyTest4()
    {
      Assert.Pass();
    }
}

