using AwesomeAssertions;
using NUnit.Framework;

public sealed class ExceptionTests
{
    [Test]
    public void TestMethod_AwesomeAssertions()
    {
        Action act = () => "foo".Should().BeNull();

        act.Should().Throw<AssertionException>();
    }
}