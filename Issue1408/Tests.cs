using System;
using System.Collections.Generic;
using NUnit.Framework;

// Issue 1408: a TestCaseSource with an array of tuples gave an extra entry in Test Explorer,
// without parameters, that was never run.
namespace NUnitTestExplorerFailure
{
    public class Tests
    {
        static IEnumerable<object[]> DemonstrationSource()
        {
            yield return [new[] { new Tuple<int, int>(0, 0) }];
            yield return [new[] { new Tuple<int, int>(1, 2), new Tuple<int, int>(3, 4) }];
        }

        [TestCaseSource(nameof(DemonstrationSource))]
        public void Demonstration(Tuple<int, int>[] expected) => Assert.That(expected, Is.Not.Empty);
    }
}
