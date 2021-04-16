using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;

namespace NUnitTest
{
    public class TestCaseSourceWithGuidTests
    {

        private static readonly Guid FixedPayerId = Guid.NewGuid();
        private static TestCaseData[] _payerMatchCases =
        {
            new TestCaseData(Guid.NewGuid(), Guid.NewGuid()).SetName("DifferentPayerIds"),
            new TestCaseData(FixedPayerId, FixedPayerId).SetName("SamePayerId"),

        };

        [TestCaseSource(nameof(_payerMatchCases))]
        public void Process_ShouldMatchExistingGuids(Guid payerId, Guid existingPayerId)
        {
            Assert.Pass();
        }
    }
}
