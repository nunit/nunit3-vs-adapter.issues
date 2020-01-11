using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;


namespace ParametrizedIssue
{
    public class Bulldetector_Issue694
    {

        private static readonly List<string> RandomStrings = new List<string>
        {
            "abc", "def", "ghi"
        };

        [Test]
        public void Is_applicable_when_no_constraints_are_set(
            [ValueSource(nameof(RandomStrings))] string vehicleGroup,
            [ValueSource(nameof(RandomStrings))] string fuelType,
            [ValueSource(nameof(RandomStrings))] string carBody,
            [Random(3)] int grossWeight)
        {
            // removed - not relevant
        }
    }
}
