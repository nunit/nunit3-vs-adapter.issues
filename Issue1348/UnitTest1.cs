using AwesomeAssertions;
using NUnit.Framework;

[assembly: FixtureLifeCycle(LifeCycle.InstancePerTestCase)]


//[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
public sealed class ExceptionTests
{
   
    [Test]
    public void TestMethod_AwesomeAssertions()
    {
        Action act = () => "foo".Should().BeNull();

        act.Should().Throw<AssertionException>();
    }
}