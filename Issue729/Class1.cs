using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace NUnitAdapterIssue729TestProject
{
    [TestFixture(TestName = "Fixture in Class1")]
    class Class1
    {
        [TestCase(TestName = "Test Case 1 in Class1")]
        public void TestCase1()
        { 
        
        }

    }
}
