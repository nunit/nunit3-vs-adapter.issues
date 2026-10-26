using System.Collections.Generic;
using NUnit.Framework;

// Issue 1414: the test case names come from TestContext.CurrentContext.Random. The adapter saves the
// seed in nunit_random_seed.tmp at discovery, so a later run selects the same names. To see if
// discovery runs, look for a new discovery dump (DumpXmlTestDiscovery is on in .runsettings).
namespace Issue1414
{
    public class RandomTests
    {
        public static IEnumerable<int> RandomValues()
        {
            for (int i = 0; i < 3; i++)
                yield return TestContext.CurrentContext.Random.Next(1000);
        }

        [TestCaseSource(nameof(RandomValues))]
        public void Random(int value) => Assert.That(value, Is.LessThan(1000));
    }
}
