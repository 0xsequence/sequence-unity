using System;
using System.Threading.Tasks;
using NUnit.Framework;
using Sequence.Contracts;
using Sequence.EmbeddedWallet;
using Sequence.EmbeddedWallet.Tests;
using Sequence.Pay.Tests.Mocks;
using Sequence.Pay.Transak;
using Sequence.Marketplace;
using Sequence.Provider;
using UnityEngine;
using HttpClient = Sequence.Marketplace.HttpClient;

namespace Sequence.Pay.Tests.Transak
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

            string transakNFTCheckoutLink = await transakCheckout.GetNFTCheckoutLink(collectibleOrders[0], 1, NFTType.ERC1155);

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
                new TransakContractId("674eb5613d739107bbd18ed2", new Address("0xe65b75eb7c58ffc0bf0e671d64d0e1c6cd0d3e5b"), Chain.Polygon, "USDC"));
            
            Debug.Log(transakNFTCheckoutLink);
            Assert.IsNotNull(transakNFTCheckoutLink);
            // Expected link validated on February 6, 2025
            // This assertion will confirm that we are still generating the same link from the same data. If primary sale erc1155 with Transak checkout stops working, but this test is still passing, then Transak has made a change on their end that we need to account for 
            string expectedLink =
                "https://global.transak.com/?apiKey=5911d9ec-46b5-48fa-a755-d59a715ff0cf&isNFT=true&calldata=eJzlT0kSwzAI%2BxKrMM9xMHlDn9%2Bk90w742N1kgABoheoQThBD7DpOY7oijjYIXPROmMVbDIE6dVWMeaT%2Fzf0np1l0%2F8Yn7Qss9yk%2BvTWwdzMeamgNapIl5eq5855693%2Fp%2B4tIP5vv6gtJDoMDhMKD7014uKGARdyhQeHXxMN%2FVRulmGRd9fMjs0UX%2FEGjwG76g%3D%3D&contractId=674eb5613d739107bbd18ed2&cryptoCurrencyCode=USDC&estimatedGasLimit=500000&nftData=W3siaW1hZ2VVUkwiOiJodHRwczovL2Rldi1tZXRhZGF0YS5zZXF1ZW5jZS5hcHAvcHJvamVjdHMvMTAxMC9jb2xsZWN0aW9ucy8zOTQvdG9rZW5zLzEvaW1hZ2Uud2VicCIsIm5mdE5hbWUiOiJLZWF0b24gVC0zMjIiLCJjb2xsZWN0aW9uQWRkcmVzcyI6IjB4ZGViMzk4ZjQxY2NkMjkwZWU1MTE0ZGY3ZTQ5OGNmMDRmYWM5MTZjYiIsInRva2VuSUQiOlsiMSJdLCJwcmljZSI6WzAuMDIwMDAwXSwicXVhbnRpdHkiOjEsIm5mdFR5cGUiOiJFUkMxMTU1In1d&walletAddress=0xD2eFbb2f18bfE3D265b26D2ACe83400A65335a07&disableWalletAddressForm=true&partnerOrderId=0xD2eFbb2f18bfE3D265b26D2ACe83400A65335a07-";
            Assert.True(transakNFTCheckoutLink.StartsWith(expectedLink)); // Check starts with as the timestamp at the end will differ
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