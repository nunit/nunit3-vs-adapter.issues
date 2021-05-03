using NUnit.Framework;

namespace Issue530
{
    public class Tests
    {
        [Ignore("reasons")]
public void IgnoredTest() => Assert.Fail();

[Explicit("reasons")]
public void ExplicitTest() => Assert.Fail();

[TestCase(null, Explicit = true, Reason = "because")]
public void ExplicitTestCase(object ignored) => Assert.Fail("This test should not be run.");
    }
}