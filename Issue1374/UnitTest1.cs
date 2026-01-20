using System.Diagnostics;

namespace Issue1374;

public class UnitTest1
{

    //[SetUp]
    //public void SetUp()
    //{
    //    Trace.Listeners.Clear();
    //    Trace.Listeners.Add(new DefaultTraceListener()
    //    {
    //        AssertUiEnabled = false,
    //    });
    //}

    [Test]
    public void TraceAssert_WithFalseCondition_ShouldThrowException()
    {
        var exception = Assert.Catch(() => System.Diagnostics.Trace.Assert(false));
        Console.WriteLine(exception);
        Assert.That(exception, Is.Not.Null);
    }
}
