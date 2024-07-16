using System.Threading.Tasks;
using Newtonsoft.Json;
using NUnit.Framework;
using Sequence.Wallet;

namespace Sequence.Relayer.Tests
{
    public class PermissionedMinterTests
    {
        [Test]
        public async Task TestBuildMintTokenRequestJson()
        {
            EOAWallet wallet = new EOAWallet();
            PermissionedMinter minter = new PermissionedMinter(new MintingRequestProver(wallet, Chain.Polygon),
                "", "0x75700a9dC31ff38b93EafDC380c28e1B816f6799");

            try
            {
                string mintingRequestJson = await minter.BuildMintTokenRequestJson("123", 3);
                MintTokenRequest mintingRequest = JsonConvert.DeserializeObject<MintTokenRequest>(mintingRequestJson);
            
                Assert.AreEqual(wallet.GetAddress().ToString(), mintingRequest.address);
                bool isValid =
                    await MintingRequestProofValidator.IsValidMintingRequestProof(Chain.Polygon,
                        new MintingRequestProof(mintingRequest));
                Assert.IsTrue(isValid);
            }
            catch (System.Exception e)
            {
                Assert.Fail("Unexpected exception: " + e.Message);
            }
        }
    }
}