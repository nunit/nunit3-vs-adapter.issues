using System.Collections;
using System.IO;
using NUnit.Framework;

namespace NUnitFilterSample;

[TestFixture]
[Sample]
public class FixtureWithCategory
{
    public static IEnumerable Chars => Path.GetInvalidFileNameChars();

    [TestCaseSource(nameof(Chars))]
    public void Test(char c)
    {
        Assert.Fail("in sample category");
    }
}