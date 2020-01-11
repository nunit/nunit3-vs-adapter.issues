using NUnit.Framework;

namespace ParametrizedIssue
{
    public class LorenH_Issue692
    {


        [TestCase(-180.001, 0, 1, 1)]
        [TestCase(0, -90.001, 1, 1)]
        [TestCase(0, 0, 180.001, 1)]
        [TestCase(0, 0, 1, 90.001)]
        public void BoxGeoFilter_Create_ShouldFailIfOutOfRange(double west, double south, double east, double north)
        {
            Assert.Pass();
        }
    }
}