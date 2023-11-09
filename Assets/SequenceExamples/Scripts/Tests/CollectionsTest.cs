using System.Collections.Generic;
using NUnit.Framework;
using Sequence;
using Sequence.Demo;
using UnityEngine;

namespace SequenceExamples.Scripts.Tests
{
    public class CollectionsTest
    {
        private Sprite _sprite;
        
        private FetchNftContentResult GetFetchNftResult()
        {
            _sprite = Sprite.Create(_texture, new Rect(0, 0, _texture.width, _texture.height), new Vector2(.5f, .5f));
            return new FetchNftContentResult(
                new NftElement[]
                {
                    new NftElement(new Address("0xc683a014955b75F5ECF991d4502427c8fa1Aa249"), _sprite, "Token", _sprite, "Collection", 5, Chain.Ethereum, 3, 5, new MockCurrencyConverter()),
                    new NftElement(new Address("0xc683a014955b75F5ECF991d4502427c8fa1Aa249"), _sprite, "Token2", _sprite, "Collection", 5, Chain.Ethereum, 4, 5, new MockCurrencyConverter()),
                    new NftElement(new Address("0xc683a014955b75F5ECF991d4502427c8fa1Aa249"), _sprite, "Token3", _sprite, "Collection", 5, Chain.Ethereum, 1, 5, new MockCurrencyConverter()),
                    new NftElement(new Address("0xc683a014955b75F5ECF991d4502427c8fa1Aa249"), _sprite, "Token4", _sprite, "Collection", 5, Chain.Ethereum, 7, 5, new MockCurrencyConverter()),
                    new NftElement(new Address("0xc683a014955b75F5ECF991d4502427c8fa1Aa249"), _sprite, "Token5", _sprite, "Collection", 5, Chain.Ethereum, 2, 5, new MockCurrencyConverter()),
                    new NftElement(new Address("0xc683a014955b75F5ECF991d4502427c8fa1Aa249"), _sprite, "Token6", _sprite, "Collection", 5, Chain.Ethereum, 27, 5, new MockCurrencyConverter()),
                    new NftElement(new Address("0xc683a014955b75F5ECF991d4502427c8fa1Aa249"), _sprite, "Token7", _sprite, "Collection", 5, Chain.Ethereum, 42, 5, new MockCurrencyConverter()),
                    new NftElement(new Address("0xc683a014955b75F5ECF991d4502427c8fa1Aa249"), _sprite, "Token8", _sprite, "Collection", 5, Chain.Ethereum, 30, 5, new MockCurrencyConverter()),
                    new NftElement(new Address("0xc683a014955b75F5ECF991d4502427c8fa1Aa249"), _sprite, "Token9", _sprite, "Another Collection", 5, Chain.Base, 31, 5, new MockCurrencyConverter()),
                },
                false);
        } 
        private Texture2D _texture = MockNftContentFetcher.CreateMockTexture();
        
        [Test]
        public void TestCollectionNftMapper()
        {
            int nftsInOtherCollections = 1;
            FetchNftContentResult expected = GetFetchNftResult();
            CollectionNftMapper mapper = new CollectionNftMapper();
            mapper.HandleNftFetch(expected);
            uint expectedTotalOwned = 116; // Sum the balances in expected by hand

            List<NftElement> result = mapper.GetNftsFromCollection(expected.Content[0].Collection);

            Assert.AreEqual(expected.Content.Length - nftsInOtherCollections, result.Count);
            int count = result.Count;
            for (int i = 0; i < count; i++)
            {
                Assert.AreEqual(expected.Content[i], result[i]);
            }

            uint totalOwnedResult = NftElement.CalculateTotalNftsOwned(result);
            Assert.AreEqual(expectedTotalOwned, totalOwnedResult);
        }

        [Test]
        public void TestCollectionNftMapper_returnsEmptyListWhenGivenNewCollection()
        {
            CollectionNftMapper mapper = new CollectionNftMapper();

            List<NftElement> result = mapper.GetNftsFromCollection(GetFetchNftResult().Content[0].Collection);
            
            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public void TestCollectionNftMapper_GetCollections()
        {
            FetchNftContentResult nfts = GetFetchNftResult();
            CollectionNftMapper mapper = new CollectionNftMapper();
            mapper.HandleNftFetch(nfts);

            CollectionInfo[] result = mapper.GetCollections();
            
            Assert.AreEqual(new CollectionInfo[]
            { 
                CollectionInfo.GetCollectionInfo(new Address("0xc683a014955b75F5ECF991d4502427c8fa1Aa249"), _sprite, "Collection", Chain.Ethereum),
                CollectionInfo.GetCollectionInfo(new Address("0xc683a014955b75F5ECF991d4502427c8fa1Aa249"), _sprite, "Another Collection", Chain.Base),
            }, result);
        }
    }
}