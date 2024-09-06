using System.Threading.Tasks;
using NUnit.Framework;
using Sequence.Integrations.Sardine;

namespace Sequence.Integrations.Tests.Sardine
{
    public class SardineCheckoutTests
    {
        [Test]
        public async Task TestCheckSardineWhitelistStatus()
        {
            SardineCheckout sardine = new SardineCheckout(Chain.Polygon);

            bool result =
                await sardine.CheckSardineWhitelistStatus(new Address("0x44b3f42e2BF34F62868Ff9e9dAb7C2F807ba97Cb"));

            Assert.False(result);
        }
    }
}