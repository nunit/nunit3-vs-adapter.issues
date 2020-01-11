using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace ParametrizedIssue
{
    public enum StatusCodes
    {
        Status200OK,
        Status404NotFound,

    }

    public enum LicenseVerificationError
    {
        UnknownLicenseKey
    }


    public class DaRosenBerg_Issue696
    {
        private static string mIntegrationTestProductId = "353";

        [TestCase("67DC-FFFB-4803-4D3D", "mIntegrationTestProductId", "anything", StatusCodes.Status200OK, null,
            TestName = "Unlimited (correct)")]
        [TestCase("67DC-FFFB-4803-4D3D", "mIntegrationTestWrongProductId", "anything", StatusCodes.Status404NotFound,
            LicenseVerificationError.UnknownLicenseKey, TestName = "Unlimited (wrong product)")]
        [TestCase("AB13-D3B4-1234-54A8", "mIntegrationTestProductId", "anything", StatusCodes.Status404NotFound,
            LicenseVerificationError.UnknownLicenseKey, TestName = "Non-existent license key")]
        public void VerifyTestAsync(string licenseKey, string productId, string domain, int expectedStatusCode,
            LicenseVerificationError? expectedError)
        {
        }
    }
}
