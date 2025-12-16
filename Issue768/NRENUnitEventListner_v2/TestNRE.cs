namespace NRENUnitEventListner_v2
{
    using NUnit.Framework;
    using NUnit.Framework.Legacy;

    [TestFixture]
    public class TestNRE
    {
        [TestCase]
        public void SimpleTest()
        {
            ClassicAssert.True(true);
        }
    }
}
