using System;
using System.Threading.Tasks;
using NUnit.Framework;
using Sequence.Contracts;
using Sequence.EmbeddedWallet;
using Sequence.EmbeddedWallet.Tests;
using Sequence.Integrations.Tests.Mocks;
using Sequence.Integrations.Transak;
using Sequence.Marketplace;
using Sequence.Provider;
using UnityEngine;
using HttpClient = Sequence.Marketplace.HttpClient;

namespace Sequence.Integrations.Tests.Transak
{
    public class TransakNFTCheckoutTests
    {
        private IWallet _testWallet =
            new SequenceWallet(new Address("0xD2eFbb2f18bfE3D265b26D2ACe83400A65335a07"), "", null);

        [Test]
        public async Task TestGetSupportedCountries()
        {
            try
            {
                SupportedCountry[] supportedCountries = await TransakNFTCheckout.GetSupportedCountries();
                Assert.IsNotNull(supportedCountries);
                Assert.Greater(supportedCountries.Length, 0);
            }
            catch (Exception e)
            {
                Assert.Fail("Expected no exception, but got: " + e.Message);
            }
        }
        
        [Test]
        public async Task TestGetNFTCheckoutLink_Marketplace()
        {
            CollectibleOrder[] collectibleOrders = await OrderFetcher.FetchListings(Chain.Polygon, "0x079294e6ffec16234578c672fa3fbfd4b6c48640");
            TransakNFTCheckout transakCheckout =
                new TransakNFTCheckout(_testWallet, Chain.Polygon, new MockEthClientForGasEstimation());

            string transakNFTCheckoutLink = await transakCheckout.GetNFTCheckoutLink(collectibleOrders[0], 1, 
                SequenceTransakContractIdRepository.SequenceContractIds[Chain.Polygon][OrderbookKind.sequence_marketplace_v2], NFTType.ERC1155);

            Debug.Log(transakNFTCheckoutLink);
            Assert.IsNotNull(transakNFTCheckoutLink);
        }

        [Test]
        public async Task TestGetNFTCheckoutLink_PrimarySale_ERC1155()
        {
            TransakNFTCheckout transakCheckout =
                new TransakNFTCheckout(_testWallet, Chain.Polygon);

            string transakNFTCheckoutLink = await transakCheckout.GetNFTCheckoutLink(
                new ERC1155Sale("0xe65b75eb7c58ffc0bf0e671d64d0e1c6cd0d3e5b"),
                new Address("0xdeb398f41ccd290ee5114df7e498cf04fac916cb"), 1, 1, 
                new TransakContractId("674eb5613d739107bbd18ed2", new Address("0xe65b75eb7c58ffc0bf0e671d64d0e1c6cd0d3e5b"), Chain.Polygon, "POL"));
            
            Debug.Log(transakNFTCheckoutLink);
            Assert.IsNotNull(transakNFTCheckoutLink);
            string expectedLinkFromKit =
                "https://global.transak.com/?apiKey=5911d9ec-46b5-48fa-a755-d59a715ff0cf&isNFT=true&calldata=eJztkMsNxDAIBVsC8zPlOIBrSPmbBiytxCGXzI3DvAfArVAKuhUO8BKfl1WYXSg6VkJuy1BeqENdojhsrpP%2FH9XTcTT94%2FlAwe4hPKK2FE3EQvRnMsgZAZQSROKdeq7u%2Fs33Pwmf%2Fyrt%2Fh9Zdb5R&contractId=674eb5613d739107bbd18ed2&cryptoCurrencyCode=POL&estimatedGasLimit=500000&nftData=W3siaW1hZ2VVUkwiOiJodHRwczovL2Rldi1tZXRhZGF0YS5zZXF1ZW5jZS5hcHAvcHJvamVjdHMvMTAxMC9jb2xsZWN0aW9ucy8zOTQvdG9rZW5zLzEvaW1hZ2Uud2VicCIsIm5mdE5hbWUiOiJLZWF0b24gVC0zMjIiLCJjb2xsZWN0aW9uQWRkcmVzcyI6IjB4ZGViMzk4ZjQxY2NkMjkwZWU1MTE0ZGY3ZTQ5OGNmMDRmYWM5MTZjYiIsInRva2VuSUQiOlsiMSJdLCJwcmljZSI6WzAuMDJdLCJxdWFudGl0eSI6MSwibmZ0VHlwZSI6IkVSQzExNTUifV0%3D&walletAddress=0x5cFE1189E36dacf78A434f42F1d1104C0bf4cafB&disableWalletAddressForm=true&partnerOrderId=0x5cFE1189E36dacf78A434f42F1d1104C0bf4cafB-1738782811397";
            Assert.AreEqual(expectedLinkFromKit, transakNFTCheckoutLink);
        }

        [Test]
        public async Task TestGetNFTCheckoutLink_PrimarySale_ERC1155_InvalidContractId()
        {
            TransakNFTCheckout transakCheckout =
                new TransakNFTCheckout(_testWallet, Chain.Polygon);

            try
            {
                string transakNFTCheckoutLink = await transakCheckout.GetNFTCheckoutLink(
                    new ERC1155Sale("0xe65b75eb7c58ffc0bf0e671d64d0e1c6cd0d3e5b"),
                    new Address("0xdeb398f41ccd290ee5114df7e498cf04fac916cb"), 1, 1, 
                    new TransakContractId("674eb5613d739107bbd18ed2", new Address("0xe65b75eb7c58ffc0bf0e671d64d0e1c6cd0d3e5b"), Chain.Avalanche, "POL"));
                Assert.Fail("Expected exception but none was thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.Message.Contains("contractId is not for the same chain"));
            }
        }

        // Todo fix test once I have a testable contractId
        [Test]
        public async Task TestGetNFTCheckoutLink_PrimarySale_ERC721()
        {
            TransakNFTCheckout transakCheckout =
                new TransakNFTCheckout(_testWallet, Chain.Polygon);
        
            string transakNFTCheckoutLink = await transakCheckout.GetNFTCheckoutLink(
                new ERC721Sale("0xe65b75eb7c58ffc0bf0e671d64d0e1c6cd0d3e5b"),
                new Address("0xdeb398f41ccd290ee5114df7e498cf04fac916cb"), 1, 1, null);  // todo replace contract
            
            Debug.Log(transakNFTCheckoutLink);
            Assert.IsNotNull(transakNFTCheckoutLink);
        }

        [Test]
        public void TestConstructionWithUnsupportedChain()
        {
            try
            {
                TransakNFTCheckout checkout = new TransakNFTCheckout(_testWallet, Chain.None);
                Assert.Fail("Expected exception but none was thrown");
            }
            catch (Exception e)
            {
                Assert.True(e.Message.Contains("provided chain is not supported"));
                Assert.True(e.Message.Contains("Ethereum"));
                Assert.True(e.Message.Contains("Polygon"));
                Debug.Log(e.Message);
            }
        }
    }
}