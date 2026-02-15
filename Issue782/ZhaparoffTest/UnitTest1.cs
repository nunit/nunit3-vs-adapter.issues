using System.Collections.Generic;
using NUnit.Framework;

namespace ZhaparoffTest;


public record SampleTestCase(int? A, int? B, int ExpectedResult, string Because = null);

public class SampleTestCases
{
    public static IEnumerable<SampleTestCase> GetTestCases(bool includeNullValuesForA)
    {
        if (includeNullValuesForA)
        {
            yield return new SampleTestCase(null, null, 0);
            yield return new SampleTestCase(null, 2, -1, "null value should have precedence over others");
        }

        yield return new SampleTestCase(1, null, 1, "null value should have precedence over others");
        yield return new SampleTestCase(1, 2, -1);
        yield return new SampleTestCase(4, 2, 1);
        yield return new SampleTestCase(4, 4, 0);
    }
}

public class SampleComparer
{
    public int Compare(int? a, int? b)
    {
        if (a == b) return 0;
        if (a is null) return -1;
        if (b is null) return 1;
        return a.Value.CompareTo(b.Value);
    }
}

[TestFixture]
public class SampleComparerTests
{
    private readonly SampleComparer testObj = new SampleComparer();

    [TestCaseSource(
        typeof(SampleTestCases),
        nameof(SampleTestCases.GetTestCases),
        new object[] { true })]
    public void SampleComparableTestA(SampleTestCase testCase)
    {
        Assert.That(testObj.Compare(testCase.A, testCase.B), Is.EqualTo(testCase.ExpectedResult), testCase.Because);
    }

    [TestCaseSource(
        typeof(SampleTestCases),
        nameof(SampleTestCases.GetTestCases),
        new object[] { false })]
    public void SampleComparableTestB(SampleTestCase testCase)
    {
        Assert.That(testObj.Compare(testCase.A, testCase.B), Is.EqualTo(testCase.ExpectedResult), testCase.Because);
    }
}