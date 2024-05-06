using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Issue718
{
    public class Tests
    {

        [Test]
        public async Task Test1()
        {
            await Task.Delay(1000);
            Debug.WriteLine("This is Debug.WriteLine");
            Trace.WriteLine("This is Trace.WriteLine");
            Console.WriteLine("This is Console.Writeline");
            TestContext.WriteLine("This is TestContext.WriteLine");
            TestContext.Out.WriteLine("This is TestContext.Out.WriteLine");
            TestContext.Error.WriteLine("This is TestContext.Error.WriteLine");
            TestContext.Progress.WriteLine("This is TestContext.Progress.WriteLine");
            Assert.Pass();
        }

        [Test]
        public void Test2()
        {
            TestContext.Progress.WriteLine("This is TestContext.Progress.WriteLine");
            Debug.WriteLine("This is Debug.WriteLine");
            Trace.WriteLine("This is Trace.WriteLine");
            Console.WriteLine("This is Console.Writeline");
            TestContext.WriteLine("This is TestContext.WriteLine");
            TestContext.Out.WriteLine("This is TestContext.Out.WriteLine");
            TestContext.Error.WriteLine("This is TestContext.Error.WriteLine");
            Assert.Pass();
        }


    }

    
}

[SetUpFixture]
public class SetupTrace
{
    [OneTimeSetUp]
    public void Setup()
    {
        Trace.Listeners.Add(new ConsoleTraceListener());
    }

    [OneTimeTearDown]
    public void EndTest()
    {
        Trace.Flush();
    }
}