using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using NUnit.Framework;

namespace Issue846
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [TestCase(42)]
        public void Test1(int x)
        {
            Assert.Pass();
            Assert.That(x == 42);
        }

        [TestCase("sv-SE")]
        [TestCase("nb-NO")]
        [TestCase("nn-NO",Explicit=true)]
        [TestCase("da-DK")]
        public void TestCulture(string culture)
        {

            var cultures = CultureInfo.GetCultures(CultureTypes.AllCultures).Select(o => o.Name).ToList();
            Assert.That(cultures, Does.Contain(culture), $"Specified culture {culture} doesn't exist");
            var input = new List<string>
            {
                "XXX","AAA", "BBB", "AAB", "ABA"
            };
            var cult = new CultureInfo(culture);
            // var result = input.OrderBy(f => f, StringComparer.Create(cult, true)).ToList();
            var result = input.OrderBy(f => f, StringComparer.InvariantCultureIgnoreCase).ToList();
            Assert.Multiple(() =>
            {
                Assert.That(result.First(), Is.EqualTo("AAA"));
                Assert.That(result.Last(), Is.EqualTo("XXX"));
                Assert.That(result, Is.Not.EqualTo(input).AsCollection, "Has not been sorted at all");
            });

        }
    }
}