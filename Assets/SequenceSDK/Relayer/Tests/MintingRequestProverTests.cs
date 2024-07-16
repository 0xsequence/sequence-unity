using System.Threading.Tasks;
using NUnit.Framework;
using Sequence.Wallet;

namespace Sequence.Relayer.Tests
{
    public class MintingRequestProverTests
    {
        [Test]
        public async Task TestEOAAuthenticationProofGeneration()
        {
            EOAWallet wallet = new EOAWallet();
            MintingRequestProver prover = new MintingRequestProver(wallet, Chain.Polygon);

            MintingRequestProof mintingRequestProof = await prover.GenerateProof("0x75700a9dC31ff38b93EafDC380c28e1B816f6799", "tokenId", 5);
            
            Assert.NotNull(mintingRequestProof);

            try
            {
                bool isValid = await MintingRequestProofValidator.IsValidMintingRequestProof(Chain.Polygon, mintingRequestProof);
                Assert.True(isValid);
            }
            catch (System.Exception e)
            {
                Assert.Fail("Unexpected exception: " + e.Message);
            }
        }
    }
}