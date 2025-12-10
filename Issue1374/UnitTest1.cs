namespace Issue1374;

public class UnitTest1
{
    [Test]
    public void TraceAssert_WithFalseCondition_ShouldThrowException()
    {
        var exception = Assert.Catch(() => System.Diagnostics.Trace.Assert(false));
        Assert.That(exception, Is.Not.Null);
    }
}
