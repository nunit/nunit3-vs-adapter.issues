using System;
namespace Issue933;

public class Tests
{
    [SetUp]
    public void Setup()
    {
        TestContext.Out.WriteLine("TestContext.Out.WriteLine Setup for any test is here");
    }



    [Test]
    public void Test1()
    {
        Console.WriteLine("Console.WriteLine for Test1 is here");
        TestContext.WriteLine("TestContext.WriteLine for Test1 is here");
        TestContext.Progress.WriteLine("TestContext.Progress.WriteLine for Test1 is here");
        TestContext.Out.WriteLine("TestContext.Out.WriteLine for Test1 is here");
        Assert.Pass();
    }

    [Test]
    public void Test2()
    {
        Assert.Pass();
    }

    [Test]
    public void Test3()
    {
        Assert.Pass();
    }

    [Category("Bug")]
    [Test]
    public void Test4()
    {
        Assert.Pass();
    }

    [Test]
    public void Test5()
    {
        Assert.Pass();
    }

    [Test]
    public void Test6()
    {
        Assert.Pass();
    }

    [Test]
    public void Test7()
    {
        Assert.Pass();
    }

    [Test]
    public void Test8()
    {
        Assert.Pass();
    }

    [Test]
    public void Test9()
    {
        Assert.Pass();
    }

    [Test]
    public void Test10()
    {
        Assert.Pass();
    }
}