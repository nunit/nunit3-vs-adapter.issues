using AwesomeAssertions;
using NUnit.Framework;
using System;

// [assembly: FixtureLifeCycle(LifeCycle.InstancePerTestCase)]


// [FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
public sealed class ExceptionTests
{
   
    [Test]
    public void TestMethod_AwesomeAssertions()
    {
        Action act = () => "foo".Should().BeNull();

        var exception = act.Should().Throw<Exception>().Which;
        exception.GetType().FullName.Should().Be("NUnit.Framework.AssertionException");
    }
}