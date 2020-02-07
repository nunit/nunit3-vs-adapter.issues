using System;
using System.Threading.Tasks;
using NUnit.Framework;

namespace NUnitTest
{
    public class GetPlayerBalancesTests
    {
        protected const string CurrencyCode = "GBP";

        protected const long PlayerId = 5390412;

        protected const string SiteCode = "SiteCode";

        protected const string GameToken = "c38cb9ef-27ef-e911-80e1-000d3ab693ab";

        protected static string CorrelationToken => Guid.NewGuid().ToString();

        [Test]
        [TestCaseSource(nameof(CreateInvalidPlayerBalancesRequests))]
        public void PlayerBalancesTest(PlayerBalancesRequest request, string siteCode, string correlationToken)
        {
            Assert.Pass();
        }

        private static object[] CreateInvalidPlayerBalancesRequests()
        {
            return new object[]
            {
                new object[] { new PlayerBalancesRequest(), null, null },
                new object[] { new PlayerBalancesRequest { PlayerId = -1, Token = GameToken, CurrencyCode = CurrencyCode }, SiteCode, CorrelationToken },
                new object[] { new PlayerBalancesRequest { PlayerId = 0, Token = GameToken, CurrencyCode = CurrencyCode }, SiteCode, CorrelationToken },
                new object[] { new PlayerBalancesRequest { PlayerId = PlayerId, Token = null, CurrencyCode = CurrencyCode }, SiteCode, CorrelationToken },
                new object[] { new PlayerBalancesRequest { PlayerId = PlayerId, Token = string.Empty, CurrencyCode = CurrencyCode }, SiteCode, CorrelationToken },
                new object[] { new PlayerBalancesRequest { PlayerId = PlayerId, Token = new string('*', 51), CurrencyCode = CurrencyCode }, SiteCode, CorrelationToken },
                new object[] { new PlayerBalancesRequest { PlayerId = PlayerId, Token = GameToken, CurrencyCode = CurrencyCode }, null, CorrelationToken },
                new object[] { new PlayerBalancesRequest { PlayerId = PlayerId, Token = GameToken, CurrencyCode = CurrencyCode }, string.Empty, CorrelationToken },
                new object[] { new PlayerBalancesRequest { PlayerId = PlayerId, Token = GameToken, CurrencyCode = CurrencyCode }, new string('*', 51), CorrelationToken },
                new object[] { new PlayerBalancesRequest { PlayerId = PlayerId, Token = GameToken, CurrencyCode = CurrencyCode }, SiteCode, null },
                new object[] { new PlayerBalancesRequest { PlayerId = PlayerId, Token = GameToken, CurrencyCode = CurrencyCode }, SiteCode, string.Empty }
            };
        }
    }
}
