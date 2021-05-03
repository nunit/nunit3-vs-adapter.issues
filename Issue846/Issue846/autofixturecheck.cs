using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using NUnit.Framework;

namespace Issue846
{
    public class Autofixturecheck
    {
        private IFixture _fixture;
        [SetUp]
        public void Init()
        {
            _fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
        }

        [Test]
        public void TestDateTime()
        {
            var dt = _fixture.Create<DateTime>();
        }
    }
}
