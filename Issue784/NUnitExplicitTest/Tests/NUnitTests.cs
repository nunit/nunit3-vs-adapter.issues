using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Tests
{
    public class NUnitTests
    {
        [Test]
        public void TestThatWorks()
        { }

        [Test, Ignore("Does not work")]
        public void IgnoredTestDoesntWork()
        {
            throw new Exception();
        }

        [Test, Explicit]
        public void ExplicitTestDoesntWork()
        {
            throw new Exception();
        }

        [TestCaseSource(nameof(TestCases))]
        [Explicit]
        public void TestCaseSourceStillDoesntWork()
        {
            throw new Exception();
        }

        private static readonly IEnumerable<string> TestCases = new[]
        {
            "case1"
        };
    }
}
