using System.Collections.Generic;
using NUnit.Framework;

// Issue 623: the Azure DevOps rerun of failed tests sends
//   /TestCaseFilter:"FullyQualifiedName=Company.Tests.MarketplaceTesting(\"Chrome\").SubmitItemforSaleWithEmptyValues"
// which failed with "Incorrect format for TestCaseFilter".
namespace Company.Tests
{
    public static class BaseConfiguration
    {
        public static IEnumerable<string> GetAllBrowsersType()
        {
            yield return "Chrome";
            yield return "Firefox";
        }
    }

    [TestFixtureSource(typeof(BaseConfiguration), nameof(BaseConfiguration.GetAllBrowsersType))]
    public class MarketplaceTesting
    {
        private readonly string browser;

        public MarketplaceTesting(string browser) => this.browser = browser;

        [Test]
        public void SubmitItemforSaleWithEmptyValues() => Assert.That(browser, Is.Not.Empty);

        [Test]
        public void SubmitItemforSale() => Assert.That(browser, Is.Not.Empty);
    }
}
