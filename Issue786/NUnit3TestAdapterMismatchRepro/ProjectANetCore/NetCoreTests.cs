namespace ProjectANetCore
{
    using System;

    using NUnit.Framework;

    [TestFixture]
    public class NetCoreTests
    {
        [Test]
        public void ShouldRunNetCoreTest()
        {
            Console.WriteLine($"Running {nameof(ShouldRunNetCoreTest)}");
            Assert.That(1, Is.GreaterThan(0));
        }
    }
}