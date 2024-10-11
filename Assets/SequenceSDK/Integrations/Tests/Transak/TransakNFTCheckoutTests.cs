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
            Chain chain = Chain.Polygon;
            MarketplaceReader marketplaceReader = new MarketplaceReader(chain);
            string contractAddress = "0x44b3f42e2BF34F62868Ff9e9dAb7C2F807ba97Cb";
            CollectiblesFilter filter = new CollectiblesFilter(false);
            
            ListCollectiblesReturn collectiblesResponse = await marketplaceReader.ListCollectibleListingsWithLowestPricedListingsFirst(contractAddress, filter);
            Assert.IsNotNull(collectiblesResponse);
            Assert.IsNotNull(collectiblesResponse.collectibles);
            int length = collectiblesResponse.collectibles.Length;
            Assert.Greater(length, 0);
            _collectibleOrders = collectiblesResponse.collectibles;

            for (int i = 0; i < length; i++)
            {
                Assert.IsNotNull(_collectibleOrders[i]);
                Assert.IsNotNull(_collectibleOrders[i].order);
            }
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