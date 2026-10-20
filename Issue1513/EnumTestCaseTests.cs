using NUnit.Framework;

namespace NUnitEnumTestCaseRepro;

public enum TopLevelEnum
{
    First,
    Second,
}

// Control: no enum parameter.
public class IntParameterTests
{
    [TestCase(1)]
    [TestCase(2)]
    public void IntParameter(int value) => Assert.Pass();
}

// Enum declared at namespace level.
public class TopLevelEnumTests
{
    [TestCase(TopLevelEnum.First)]
    [TestCase(TopLevelEnum.Second)]
    public void TopLevelEnumParameter(TopLevelEnum value) => Assert.Pass();
}

// Enum declared as a nested type of the test class.
public class NestedEnumTests
{
    public enum NestedEnum
    {
        First,
        Second,
    }

    [TestCase(NestedEnum.First)]
    [TestCase(NestedEnum.Second)]
    public void NestedEnumParameter(NestedEnum value) => Assert.Pass();

    [TestCase(NestedEnum.First)]
    [TestCase(NestedEnum.Second)]
    public async Task NestedEnumParameterAsync(NestedEnum value)
    {
        await Task.Yield();
        Assert.Pass();
    }
}

// Same as the original failing code: nested enum in a parameterized fixture with a primary constructor.
public enum FixtureArg
{
    A,
    B,
}

[TestFixture(FixtureArg.A)]
[TestFixture(FixtureArg.B)]
public class ParameterizedFixtureWithNestedEnumTests(FixtureArg fixtureArg)
{
    public enum NestedEnum
    {
        First,
        Second,
    }

    [Test]
    public void NoParameter() => Assert.That(fixtureArg, Is.Not.Null);

    [TestCase(NestedEnum.First)]
    [TestCase(NestedEnum.Second)]
    public void NestedEnumParameter(NestedEnum value) => Assert.Pass();
}
