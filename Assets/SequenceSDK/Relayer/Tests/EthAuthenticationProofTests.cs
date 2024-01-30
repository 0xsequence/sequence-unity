using System.Threading.Tasks;
using NUnit.Framework;
using Sequence.Wallet;

namespace Sequence.Relayer.Tests
{
    public class EthAuthenticationProofTests
    {
        [Test]
        public async Task TestEoaAuthenticationProofGeneration()
        {
            EthWallet wallet = new EthWallet();
            EthAuthenticationProof proof = new EthAuthenticationProof(wallet, Chain.Polygon);

            string ethAuthenticationProof = await proof.GenerateProof();
            
            Assert.NotNull(ethAuthenticationProof);

            try
            {
                bool isValid = await EthAuthenticationProofValidator.IsValidEthAuthProof(Chain.Polygon, wallet.GetAddress(), ethAuthenticationProof);
                Assert.True(isValid);
            }
            catch (System.Exception e)
            {
                Assert.Fail("Unexpected exception: " + e.Message);
            }
        }
    }
}