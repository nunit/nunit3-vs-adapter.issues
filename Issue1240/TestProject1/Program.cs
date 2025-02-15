using System.Threading;
using NUnit.Framework;

namespace TestProject1;

public  class TestProgram
{
    [Test]
    public static void Test1()
    {
        // ensure the execution is long enough to see it in Task manager
        Thread.Sleep(5000);
    }
}
