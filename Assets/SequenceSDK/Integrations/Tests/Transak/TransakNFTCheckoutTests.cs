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
            
            string transakNFTCheckoutLink = await transakCheckout.GetNFTCheckoutLink(collectibleOrders[0].order, collectibleOrders[0].metadata, 1, NFTType.ERC1155);

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
                new Address("0xdeb398f41ccd290ee5114df7e498cf04fac916cb"), 1, 1);
            
            Debug.Log(transakNFTCheckoutLink);
            Assert.IsNotNull(transakNFTCheckoutLink);
        }

        [Test]
        public async Task TestGetNFTCheckoutLink_PrimarySale_ERC721()
        {
            TransakNFTCheckout transakCheckout =
                new TransakNFTCheckout(_testWallet, Chain.Polygon);

            string transakNFTCheckoutLink = await transakCheckout.GetNFTCheckoutLink(
                new ERC721Sale("0xe65b75eb7c58ffc0bf0e671d64d0e1c6cd0d3e5b"),
                new Address("0xdeb398f41ccd290ee5114df7e498cf04fac916cb"), 1, 1); // Todo replace with actual 721 contracts
            
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