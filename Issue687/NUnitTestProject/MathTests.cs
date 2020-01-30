using NUnit.Framework;

namespace NUnitTestProject
{
    using System;
    using System.Collections.Generic;
    using Math = NETStandardClassLibrary.Math;

    [TestFixture]
    public class MathTests
    {
        private const double _tolerance = 1e-14;

        [Test]
        public void SquareTest(
            [ValueSource(nameof(TestFunctions))] Func<double, double> func,
            [Values(-1.0, 1.0, 2.0)] double x)
        {
            Assert.That(Math.Square(func, x), Is.EqualTo(func(x) * func(x)).Within(_tolerance));
        }

        private static IEnumerable<Func<double, double>> TestFunctions()
        {
            yield return x => x * x;
            yield return x => 2.0 * x - 1.0;
        }
    }
}