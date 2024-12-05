using System.Diagnostics;
using NUnit.Framework.Interfaces;

namespace Issue1225;

public class MyFixture
{
    private int i = 0;

    [SetUp]
    public void AtStartOfEachTest()
    {
        i++;
        Console.Error.WriteLine($"{TestContext.CurrentContext.Test.FullName} runs as number {i}");
    }

    [Test, Order(42)]
    public void TestA_42()
    {
    
    }

    [Test, Order(13)]
    public void TestB_13()
    {
    }

    [Test, Order(5)]
    public void TestC_5()
    {
    }

    
}