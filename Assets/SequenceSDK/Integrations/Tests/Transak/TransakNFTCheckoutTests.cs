using System;
using System.Threading.Tasks;
using NUnit.Framework;
using Sequence.Contracts;
using Sequence.EmbeddedWallet;
using Sequence.Integrations.Tests.Mocks;
using Sequence.Integrations.Transak;
using Sequence.Marketplace;
using Sequence.Provider;
using UnityEngine;

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
            _collectibleOrders = await OrderFetcher.FetchListings();
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
        public async Task TestGetNFTCheckoutLink()
        {
            TransakNFTCheckout transakCheckout =
                new TransakNFTCheckout(_testWallet, Chain.Polygon, new MockEthClientForGasEstimation());
            
            string transakNFTCheckoutLink = await transakCheckout.GetNFTCheckoutLink(_collectibleOrders[0].order, _collectibleOrders[0].metadata, 1, NFTType.ERC1155);

            Debug.Log(transakNFTCheckoutLink);
            Assert.IsNotNull(transakNFTCheckoutLink);
        }
    }
}