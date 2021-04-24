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

        [Test]
        public void Test1()
        {
            Assert.Pass();
        }

        [TestCase("sv-SE")]
        [TestCase("nb-NO")]
        [TestCase("nn-NO")]
        [TestCase("da-DK")]
        public void TestCulture(string culture)
        {
            var input = new List<string>
            {
                "XXX","AAA", "BBB", "AAB", "ABA"
            };
            var cult = new CultureInfo(culture);
            var result = input.OrderBy(f => f, StringComparer.Create(cult, true)).ToList();
            Assert.Multiple(() =>
            {
                Assert.That(result.First(), Is.EqualTo("AAA"));
                Assert.That(result, Is.Not.EqualTo(input).AsCollection);
            });
            
        }
    }
}