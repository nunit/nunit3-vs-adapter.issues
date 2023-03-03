using System;
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Builders;

namespace NUnitDiscoveryNameChange
{
    [AttributeUsage(AttributeTargets.Class)]
    public class MyTestFixtureAttribute : NUnitAttribute, IFixtureBuilder
    {
        private static readonly NUnitTestCaseBuilder TestCaseBuilder = new NUnitTestCaseBuilder();

        public IEnumerable<TestSuite> BuildFrom(ITypeInfo typeInfo)
        {
            var testSuite = (MyTestSuite) typeInfo.Construct(null);

            var runMethodInfo = typeInfo.Type.GetMethod(nameof(MyTestSuite.RunStepUnderNUnit));
            if (runMethodInfo == null)
                throw new ApplicationException($"Failed to find {nameof(MyTestSuite.RunStepUnderNUnit)} method on {typeInfo.Type}.");

            var steps = testSuite.GenerateSteps();

            var fixture = new TestFixture(typeInfo);

            foreach (var step in steps)
            {
                var stepParams = new TestCaseParameters(new object[] {step});
                stepParams.TestName = step.Name;
                
                var nUnitTestMethod = TestCaseBuilder.BuildTestMethod(new MethodWrapper(typeInfo.Type, runMethodInfo), fixture, stepParams);
                fixture.Add(nUnitTestMethod);
            }

            return new[] {fixture};
        }
    }

    public class TestDefinition
    {
        public string Name { get; }

        public TestDefinition(string name) => Name = name;

        public void RunUnderUnit() => Console.WriteLine("Running test: " + Name);
    }

    [MyTestFixture]
    public class MyTestSuite
    {
        public void RunStepUnderNUnit(TestDefinition step)
        {
            step.RunUnderUnit();
        }

        public List<TestDefinition> GenerateSteps() => new List<TestDefinition>
            {
                new TestDefinition("Test with fixed name - always runs"),
                new TestDefinition("Test with random zero or one - sometimes runs: " + new Random().Next(2)),
                new TestDefinition("Test with current time - never runs: " + DateTime.Now)
            };
    }
}
