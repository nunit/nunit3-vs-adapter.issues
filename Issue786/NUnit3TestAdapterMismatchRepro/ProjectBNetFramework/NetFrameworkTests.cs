namespace ProjectBNetFramework
{
    using System;

    using NUnit.Framework;

    [TestFixture]
    public class NetFrameworkTests
    {
        [Test]
        public void ShouldRunNetFrameworkTest()
        {
            Console.WriteLine($"Running {nameof(ShouldRunNetFrameworkTest)}");
            Assert.That(1, Is.GreaterThan(0));
        }
    }
}