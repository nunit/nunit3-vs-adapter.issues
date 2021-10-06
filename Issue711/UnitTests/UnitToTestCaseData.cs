using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace UnitTests
{
    public static class UnitToTestCaseData
    {
        public static IEnumerable<TestCaseData> Cases
        {
            get
            {
                yield return new TestCaseData("Input A").SetName("Unit To Test Input A equals itself");
                yield return new TestCaseData("Input B").SetName("Unit To Test Input B = itself");
            }
        }
    }
}