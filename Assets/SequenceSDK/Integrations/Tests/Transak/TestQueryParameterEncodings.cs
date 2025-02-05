using System;
using System.ComponentModel;
using System.Threading.Tasks;
using NUnit.Framework;
using Sequence.Integrations.Transak;
using Sequence.Marketplace;

namespace Sequence.Integrations.Tests.Transak
{
    public class TestQueryParameterEncodings
    {
        [Test]
        public async Task TestNftDataEncoder()
        {
            IMarketplaceReader reader = new MarketplaceReader(Chain.Polygon);
            Address collection = new Address("0xdeb398f41ccd290ee5114df7e498cf04fac916cb");
            Marketplace.TokenMetadata metadata = await reader.GetCollectible(collection, "1");
            TransakNftData data = new TransakNftData(metadata.image, metadata.name, collection, new[] { "1" },
                new decimal[] { Decimal.Parse("0.02") }, 1, NFTType.ERC1155);
            
            string encodedData = NftDataEncoder.Encode(data);

            string expected =
                "W3siaW1hZ2VVUkwiOiJodHRwczovL2Rldi1tZXRhZGF0YS5zZXF1ZW5jZS5hcHAvcHJvamVjdHMvMTAxMC9jb2xsZWN0aW9ucy8zOTQvdG9rZW5zLzEvaW1hZ2Uud2VicCIsIm5mdE5hbWUiOiJLZWF0b24gVC0zMjIiLCJjb2xsZWN0aW9uQWRkcmVzcyI6IjB4ZGViMzk4ZjQxY2NkMjkwZWU1MTE0ZGY3ZTQ5OGNmMDRmYWM5MTZjYiIsInRva2VuSUQiOlsiMSJdLCJwcmljZSI6WzAuMDJdLCJxdWFudGl0eSI6MSwibmZ0VHlwZSI6IkVSQzExNTUifV0%3D";
            Assert.AreEqual(expected, encodedData);
        }

        [Test]
        public void TestCallDataCompressor()
        {
            string calldata =
                "0x60e606f60000000000000000000000004a598b7ec77b1562ad0df7dc64a162695ce4c78a00000000000000000000000000000000000000000000000000000000000000e0000000000000000000000000000000000000000000000000000000000000012000000000000000000000000000000000000000000000000000000000000001600000000000000000000000003c499c542cef5e3811e1192ce70d8cc03d5c33590000000000000000000000000000000000000000000000000000000000004e2000000000000000000000000000000000000000000000000000000000000001a000000000000000000000000000000000000000000000000000000000000000010000000000000000000000000000000000000000000000000000000000000001000000000000000000000000000000000000000000000000000000000000000100000000000000000000000000000000000000000000000000000000000000010000000000000000000000000000000000000000000000000000000000000001000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000010000000000000000000000000000000000000000000000000000000000000000";
            
            string compressed = CallDataCompressor.Compress(calldata);

            string expected =
                "eJztkMsNxDAIBVsC8zPlOIBrSPmbBiytxCGXzI3DvAfArVAKuhUO8BKfl1WYXSg6VkJuy1BeqENdojhsrpP%2FH9XTcTT94%2FlAwe4hPKK2FE3EQvRnMsgZAZQSROKdeq7u%2Fs33Pwmf%2Fyrt%2Fh9Zdb5R";
            Assert.AreEqual(expected, compressed);
        }
    }
}