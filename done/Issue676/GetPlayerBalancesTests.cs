using System;
using NUnit.Framework;

namespace NUnitTest
{
    public class GetPlayerBalancesTests
    {
        protected const string CurrencyCode = "GBP";

        protected const long PlayerId = 5390412;

        protected const string SiteCode = "SiteCode";

        protected const string GameToken = "c38cb9ef-27ef-e911-80e1-000d3ab693ab";

        protected const string ValidToken = "abc";

        protected static string CorrelationToken => Guid.NewGuid().ToString();

        protected static Guid BaseToken = new Guid(GameToken); 

        protected static string ModifiedCorrelationToken
        {
            get
            {
                BaseToken = BaseToken.NextGuid();
                return BaseToken.ToString();
            }
        } 

        private static string startString = "abc";
        private static int next = 'd';
        private static int i = 1;

        protected static string WhateverToken
        {
            get
            {
                startString += (char)next;
                startString += i;
                next++;
                i++;
                return startString;
            }
        }


        [TestCaseSource(nameof(CreateInvalidPlayerBalancesRequests))]
        public void PlayerBalancesTest(PlayerBalancesRequest request, string siteCode, string correlationToken)
        {
            Assert.Pass();
        }

        [TestCaseSource(nameof(CreateValidPlayerBalancesRequests))]
        public void ValidPlayerBalancesTest(PlayerBalancesRequest request, string siteCode, string correlationToken)
        {
            Assert.Pass();
        }

        [TestCaseSource(nameof(CreateModifiedPlayerBalancesRequests))]
        public void ModifiedPlayerBalancesTest(PlayerBalancesRequest request, string siteCode, string correlationToken)
        {
            Assert.Pass();
        }

        [TestCaseSource(nameof(CreateTestContextRandomPlayerBalancesRequests))]
        public void TestContextRandomPlayerBalancesTest(PlayerBalancesRequest request, string siteCode, string correlationToken)
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
                new object[] { new PlayerBalancesRequest { PlayerId = PlayerId, Token = GameToken, CurrencyCode = CurrencyCode }, SiteCode, string.Empty },
            };
        }

        private static object[] CreateValidPlayerBalancesRequests()
        {
            return new object[]
            {
                new object[] { new PlayerBalancesRequest(), null, null },
                new object[] { new PlayerBalancesRequest { PlayerId = -1, Token = ValidToken, CurrencyCode = CurrencyCode }, SiteCode, WhateverToken },
                new object[] { new PlayerBalancesRequest { PlayerId = 0, Token = ValidToken, CurrencyCode = CurrencyCode }, SiteCode, WhateverToken },
                new object[] { new PlayerBalancesRequest { PlayerId = PlayerId, Token = null, CurrencyCode = CurrencyCode }, SiteCode, WhateverToken },
                new object[] { new PlayerBalancesRequest { PlayerId = PlayerId, Token = string.Empty, CurrencyCode = CurrencyCode }, SiteCode, WhateverToken },
                new object[] { new PlayerBalancesRequest { PlayerId = PlayerId, Token = new string('*', 51), CurrencyCode = CurrencyCode }, SiteCode, WhateverToken },
                new object[] { new PlayerBalancesRequest { PlayerId = PlayerId, Token = GameToken, CurrencyCode = CurrencyCode }, null, WhateverToken },
                new object[] { new PlayerBalancesRequest { PlayerId = PlayerId, Token = GameToken, CurrencyCode = CurrencyCode }, string.Empty, WhateverToken },
                new object[] { new PlayerBalancesRequest { PlayerId = PlayerId, Token = GameToken, CurrencyCode = CurrencyCode }, new string('*', 51), WhateverToken },
                new object[] { new PlayerBalancesRequest { PlayerId = PlayerId, Token = GameToken, CurrencyCode = CurrencyCode }, SiteCode, null },
                new object[] { new PlayerBalancesRequest { PlayerId = PlayerId, Token = GameToken, CurrencyCode = CurrencyCode }, SiteCode, string.Empty },
            };
        }

        private static object[] CreateModifiedPlayerBalancesRequests()
        {
            return new object[]
            {
                new object[] { new PlayerBalancesRequest(), null, null },
                new object[] { new PlayerBalancesRequest { PlayerId = -1, Token = GameToken, CurrencyCode = CurrencyCode }, SiteCode, ModifiedCorrelationToken },
                new object[] { new PlayerBalancesRequest { PlayerId = 0, Token = GameToken, CurrencyCode = CurrencyCode }, SiteCode, ModifiedCorrelationToken },
                new object[] { new PlayerBalancesRequest { PlayerId = PlayerId, Token = null, CurrencyCode = CurrencyCode }, SiteCode, ModifiedCorrelationToken },
                new object[] { new PlayerBalancesRequest { PlayerId = PlayerId, Token = string.Empty, CurrencyCode = CurrencyCode }, SiteCode, ModifiedCorrelationToken },
                new object[] { new PlayerBalancesRequest { PlayerId = PlayerId, Token = new string('*', 51), CurrencyCode = CurrencyCode }, SiteCode, ModifiedCorrelationToken },
                new object[] { new PlayerBalancesRequest { PlayerId = PlayerId, Token = GameToken, CurrencyCode = CurrencyCode }, null, ModifiedCorrelationToken },
                new object[] { new PlayerBalancesRequest { PlayerId = PlayerId, Token = GameToken, CurrencyCode = CurrencyCode }, string.Empty, ModifiedCorrelationToken },
                new object[] { new PlayerBalancesRequest { PlayerId = PlayerId, Token = GameToken, CurrencyCode = CurrencyCode }, new string('*', 51), ModifiedCorrelationToken },
                new object[] { new PlayerBalancesRequest { PlayerId = PlayerId, Token = GameToken, CurrencyCode = CurrencyCode }, SiteCode, null },
                new object[] { new PlayerBalancesRequest { PlayerId = PlayerId, Token = GameToken, CurrencyCode = CurrencyCode }, SiteCode, string.Empty },
            };
        }

        protected static string TestContextRandomGuid => TestContext.CurrentContext.Random.NextGuid().ToString();

        private static object[] CreateTestContextRandomPlayerBalancesRequests()
        {
            return new object[]
            {
                new object[] { new PlayerBalancesRequest(), null, null },
                new object[] { new PlayerBalancesRequest { PlayerId = -1, Token = GameToken, CurrencyCode = CurrencyCode }, SiteCode,TestContextRandomGuid  },
                new object[] { new PlayerBalancesRequest { PlayerId = 0, Token = GameToken, CurrencyCode = CurrencyCode }, SiteCode, TestContextRandomGuid },
                new object[] { new PlayerBalancesRequest { PlayerId = PlayerId, Token = null, CurrencyCode = CurrencyCode }, SiteCode, TestContextRandomGuid },
                new object[] { new PlayerBalancesRequest { PlayerId = PlayerId, Token = string.Empty, CurrencyCode = CurrencyCode }, SiteCode, TestContextRandomGuid },
                new object[] { new PlayerBalancesRequest { PlayerId = PlayerId, Token = new string('*', 51), CurrencyCode = CurrencyCode }, SiteCode, TestContextRandomGuid },
                new object[] { new PlayerBalancesRequest { PlayerId = PlayerId, Token = GameToken, CurrencyCode = CurrencyCode }, null, TestContextRandomGuid },
                new object[] { new PlayerBalancesRequest { PlayerId = PlayerId, Token = GameToken, CurrencyCode = CurrencyCode }, string.Empty, TestContextRandomGuid },
                new object[] { new PlayerBalancesRequest { PlayerId = PlayerId, Token = GameToken, CurrencyCode = CurrencyCode }, new string('*', 51), TestContextRandomGuid },
                new object[] { new PlayerBalancesRequest { PlayerId = PlayerId, Token = GameToken, CurrencyCode = CurrencyCode }, SiteCode, null },
                new object[] { new PlayerBalancesRequest { PlayerId = PlayerId, Token = GameToken, CurrencyCode = CurrencyCode }, SiteCode, string.Empty },
            };
        }
    }
}
