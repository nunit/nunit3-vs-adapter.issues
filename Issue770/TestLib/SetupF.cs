using System;
using NUnit.Framework;

[SetUpFixture]
public class SetupF
{
    [OneTimeSetUp]
    public void RunBeforeAnyTests()
    {
        Console.Out.WriteLine("Do some work");
    }
}