using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;

// Issue 677, from https://github.com/ovebastiansen/nunitadapterissue:
//   dotnet test --filter "FullyQualifiedName=TestFixtureSourceError.TestsBroken(Windows Edgium).Test1"
// found 0 tests. The fixture argument is an object whose ToString has a space.
namespace TestFixtureSourceError
{
    public class TestCaseDefinition
    {
        public string Os { get; set; }
        public string Browser { get; set; }

        public override string ToString() => $"{Os} {Browser}";
    }

    public static class TestFactory
    {
        public static IEnumerable TestCases => new List<TestCaseDefinition>
        {
            new() { Os = "Windows", Browser = "Google" },
            new() { Os = "Windows", Browser = "Edgium" },
        };
    }

    [TestFixtureSource(typeof(TestFactory), nameof(TestFactory.TestCases))]
    public class TestsBroken
    {
        public TestsBroken(TestCaseDefinition definition) => Definition = definition;

        public TestCaseDefinition Definition { get; }

        [Test]
        public void Test1() => Assert.That(Definition.Browser, Is.Not.Empty);
    }

    public class TestsGood
    {
        [Test]
        public void Test1() => Assert.Pass();
    }
}
