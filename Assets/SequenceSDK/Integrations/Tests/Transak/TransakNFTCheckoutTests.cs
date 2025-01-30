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
        private CollectibleOrder[] _collectibleOrders;

        private IWallet _testWallet =
            new SequenceWallet(new Address("0xD2eFbb2f18bfE3D265b26D2ACe83400A65335a07"), "", null);
        
        
        
        [SetUp]
        public async Task Setup()
        {
            _collectibleOrders = await OrderFetcher.FetchListings(Chain.ArbitrumNova, "0x0ee3af1874789245467e7482f042ced9c5171073");
        }
        
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
            TransakNFTCheckout transakCheckout =
                new TransakNFTCheckout(_testWallet, Chain.ArbitrumNova, new MockEthClientForGasEstimation());
            
            string transakNFTCheckoutLink = await transakCheckout.GetNFTCheckoutLink(_collectibleOrders[0].order, _collectibleOrders[0].metadata, 1, NFTType.ERC1155);

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
    }
}