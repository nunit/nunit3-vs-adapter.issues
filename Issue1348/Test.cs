using AwesomeAssertions;
using NUnit.Framework;


public sealed class ExceptionTests2
{
    [Test]
    public void TestMethod_AwesomeAssertions()
    {
        Action act = () => "foo".Should().BeNull();

        act.Should().Throw<Exception>();
        var exception = act.Should().Throw<Exception>().Which;
        exception.GetType().FullName.Should().Be("NUnit.Framework.AssertionException");
    }
}


