using System;
using System.Threading.Tasks;
using NUnit.Framework;
using Sequence.Integrations.Transak;

namespace Sequence.Integrations.Tests.Transak
{
    public class TransakOnRampTests
    {
        [Test]
        public async Task TestGetSupportedCountries()
        {
            try
            {
                SupportedCountry[] supportedCountries = await TransakOnRamp.GetSupportedCountries();
                Assert.IsNotNull(supportedCountries);
                Assert.Greater(supportedCountries.Length, 0);
            }
            catch (Exception e)
            {
                Assert.Fail("Expected no exception, but got: " + e.Message);
            }
        }

        [Test]
        public void TestGetTransakLink()
        {
            TransakOnRamp onRamp = new TransakOnRamp("0xc683a014955b75F5ECF991d4502427c8fa1Aa249");
            string transakOnRampLink = onRamp.GetTransakLink();
            Assert.IsNotNull(transakOnRampLink);
        }
    }
}