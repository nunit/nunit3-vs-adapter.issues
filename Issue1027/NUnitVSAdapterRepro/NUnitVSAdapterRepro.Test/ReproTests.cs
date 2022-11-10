using System;
using NUnit.Framework;

namespace NUnitVSAdapterRepro.Test
{
    [TestFixture]
    public class ReproTests
    {
        [Test]
        public void TestRepro()
        {
            var sut = new Repro();
            Assert.That(sut.Foo(), Is.EqualTo(5));

            var d = Environment.CurrentDirectory;
            TestContext.WriteLine($"Current directory : {d}");
        }
    }
}
