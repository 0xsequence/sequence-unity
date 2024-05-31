using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Sequence;
using Sequence.Demo;
using UnityEngine;

namespace SequenceExamples.Scripts.Tests
{
    public class SearchableQuerierTests
    {
        public static IEnumerable<TestCaseData> ReturnRatioTestCases()
        {
            SearchableCollection[] allCollections = BuildSearchableCollections(5);
            TokenElement[] allTokens = BuildTokenElements(5);
            ISearchable[] expected = BuildExpected(allCollections, allTokens, 3, 3);
            yield return new TestCaseData(allCollections, allTokens, 6, expected);
            allCollections = BuildSearchableCollections(1);
            allTokens = BuildTokenElements(4);
            expected = BuildExpected(allCollections, allTokens, 1, 4);
            yield return new TestCaseData(allCollections, allTokens, 6, expected);
            allCollections = BuildSearchableCollections(10);
            allTokens = BuildTokenElements(0);
            expected = BuildExpected(allCollections, allTokens, 4, 0);
            yield return new TestCaseData(allCollections, allTokens, 4, expected);
            allCollections = BuildSearchableCollections(8);
            allTokens = BuildTokenElements(2);
            expected = BuildExpected(allCollections, allTokens, 4, 2);
            yield return new TestCaseData(allCollections, allTokens, 6, expected);
        }

        private static SearchableCollection[] BuildSearchableCollections(int amountOfCollections)
        {
            FetchNftContentResult fetchNftContentResult = GetFetchNftResult(amountOfCollections);
            return AssembleCollection(fetchNftContentResult);
        }

        private static SearchableCollection[] AssembleCollection(FetchNftContentResult fetchNftContentResult)
        {
            CollectionNftMapper mapper = new CollectionNftMapper();
            mapper.HandleNftFetch(fetchNftContentResult);
            CollectionInfo[] collections = mapper.GetCollections();
            int collectionsLength = collections.Length;
            SearchableCollection[] searchableCollections = new SearchableCollection[collectionsLength];
            for (int i = 0; i < collectionsLength; i++)
            {
                searchableCollections[i] = new SearchableCollection(collections[i], mapper);
            }

            return searchableCollections;
        }

        private static FetchNftContentResult GetFetchNftResult(int collections)
        {
            FetchNftContentResult result = new FetchNftContentResult(new NftElement[collections], false);
            for (uint i = 0; i < collections; i++)
            {
                Texture2D texture = MockNftContentFetcher.CreateMockTexture();
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(.5f, .5f));
                result.Content[i] = new NftElement(new Address("0xc683a014955b75F5ECF991d4502427c8fa1Aa249"), sprite,
                    $"Token{i}", sprite, $"Collection{i}", i, Chain.Ethereum, i, i, new MockCurrencyConverter());
            }

            return result;
        }

        private static TokenElement[] BuildTokenElements(int length)
        {
            MockTokenContentFetcher tokenFetcher = new MockTokenContentFetcher();
            TokenElement[] tokens = new TokenElement[length];
            for (int i = 0; i < length; i++)
            {
                tokens[i] = tokenFetcher.CreateMockElement();
            }

            return tokens;
        }

        private static ISearchable[] BuildExpected(SearchableCollection[] allCollections, TokenElement[] allTokens,
            int collections, int tokens)
        {
            ISearchable[] expected = new ISearchable[collections + tokens];
            for (int i = 0; i < collections; i++)
            {
                expected[i] = allCollections[i];
            }

            for (int i = 0; i < tokens; i++)
            {
                expected[collections + i] = allTokens[i];
            }

            return expected;
        }
        
        [TestCaseSource(nameof(ReturnRatioTestCases))]
        public void TestReturnRatio(SearchableCollection[] allCollections, TokenElement[] allTokens, int maxToReturn, ISearchable[] expected)
        {
            SearchableQuerier querier = new SearchableQuerier(allCollections, allTokens, maxToReturn);

            Queue<ISearchable> results = new Queue<ISearchable>();
            for (ISearchable result = querier.GetNextValid(); result != null; result = querier.GetNextValid())
            {
                results.Enqueue(result);
            }
            
            CollectionAssert.AreEqual(expected, results);
        }

        public static object[] SearchCriteriaFilteringTestCases = new[]
        {
            new object[] { "Collection", new string[] {"CollectionName", "Collection3", "Collection2", "Collection1", "CollectionLaunch", "collectionlowercase"}},
            new object[] { "CollectionName", new string[] {"CollectionName", "Collection Name With Whitespace"}},
            new object[] { " \n c   o l ", new string[] {"CollectionName", "Collection3", "Collection2", "Collection1", "CollectionLaunch", "collectionlowercase"}},
            new object[] { "Coo", new string[] {"Cool"}},
            new object[] { "N", new string[] {"Name", "Name"}},
            new object[] { "_", new string[] {"_snake_"}},
            new object[] { "#", new string[] {"#hashtag"}},
            new object[] { "token", new string[] {"TokenName", "Token3", "Token2", "Token1", "TokenLaunch", "tokenlowercase" }},
            new object[] {"t", new string[]{"TokenName", "TotallyAwesomeName", "Token3", "Token2", "Token1", "TokenLaunch"}},
            new object[] { "tot", new string[] {"TotallyAwesomeName"}},
            new object[] { "TokenName", new string[] {"TokenName", "Token Name With Whitespace"}},
            new object[] { "Nothing matching", new string[] {}}
        };
        
        [TestCaseSource(nameof(SearchCriteriaFilteringTestCases))]
        public void TestSearchCriteriaFiltering(string searchCriteria, string[] expectedNames)
        {
            Texture2D texture = MockNftContentFetcher.CreateMockTexture();
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(.5f, .5f));
            FetchNftContentResult fetchNftContentResult = new FetchNftContentResult(
                new NftElement[]
                {
                    new NftElement(new Address("0xc683a014955b75F5ECF991d4502427c8fa1Aa249"), sprite, "TokenName", sprite, "CollectionName", 5, Chain.Ethereum, 1, 1, new MockCurrencyConverter()),
                    new NftElement(new Address("0xc683a014955b75F5ECF991d4502427c8fa1Aa249"), sprite, "TokenName", sprite, "Cool", 5, Chain.Ethereum, 1, 1, new MockCurrencyConverter()),
                    new NftElement(new Address("0xc683a014955b75F5ECF991d4502427c8fa1Aa249"), sprite, "TokenName", sprite, "Name", 5, Chain.Ethereum, 1, 1, new MockCurrencyConverter()),
                    new NftElement(new Address("0xc683a014955b75F5ECF991d4502427c8fa1Aa249"), sprite, "TokenName", sprite, "Collection3", 5, Chain.Ethereum, 1, 1, new MockCurrencyConverter()),
                    new NftElement(new Address("0xc683a014955b75F5ECF991d4502427c8fa1Aa249"), sprite, "TokenName", sprite, "Collection2", 5, Chain.Ethereum, 1, 1, new MockCurrencyConverter()),
                    new NftElement(new Address("0xc683a014955b75F5ECF991d4502427c8fa1Aa249"), sprite, "TokenName", sprite, "Collection1", 5, Chain.Ethereum, 1, 1, new MockCurrencyConverter()),
                    new NftElement(new Address("0xc683a014955b75F5ECF991d4502427c8fa1Aa249"), sprite, "TokenName", sprite, "CollectionLaunch", 5, Chain.Ethereum, 1, 1, new MockCurrencyConverter()),
                    new NftElement(new Address("0xc683a014955b75F5ECF991d4502427c8fa1Aa249"), sprite, "TokenName", sprite, "_snake_", 5, Chain.Ethereum, 1, 1, new MockCurrencyConverter()),
                    new NftElement(new Address("0xc683a014955b75F5ECF991d4502427c8fa1Aa249"), sprite, "TokenName", sprite, "collectionlowercase", 5, Chain.Ethereum, 1, 1, new MockCurrencyConverter()),
                    new NftElement(new Address("0xc683a014955b75F5ECF991d4502427c8fa1Aa249"), sprite, "TokenName", sprite, "Collection Name With Whitespace", 5, Chain.Ethereum, 1, 1, new MockCurrencyConverter()),
                }, false);
            SearchableCollection[] searchableCollections = AssembleCollection(fetchNftContentResult);
            TokenElement[] tokenElements = new TokenElement[]
            {
                new TokenElement("0xc683a014955b75F5ECF991d4502427c8fa1Aa249", sprite, "TokenName", Chain.Ethereum, 1, "STT", new MockCurrencyConverter()),
                new TokenElement("0xc683a014955b75F5ECF991d4502427c8fa1Aa249", sprite, "TotallyAwesomeName", Chain.Ethereum, 1, "STT", new MockCurrencyConverter()),
                new TokenElement("0xc683a014955b75F5ECF991d4502427c8fa1Aa249", sprite, "Name", Chain.Ethereum, 1, "STT", new MockCurrencyConverter()),
                new TokenElement("0xc683a014955b75F5ECF991d4502427c8fa1Aa249", sprite, "Token3", Chain.Ethereum, 1, "STT", new MockCurrencyConverter()),
                new TokenElement("0xc683a014955b75F5ECF991d4502427c8fa1Aa249", sprite, "Token2", Chain.Ethereum, 1, "STT", new MockCurrencyConverter()),
                new TokenElement("0xc683a014955b75F5ECF991d4502427c8fa1Aa249", sprite, "Token1", Chain.Ethereum, 1, "STT", new MockCurrencyConverter()),
                new TokenElement("0xc683a014955b75F5ECF991d4502427c8fa1Aa249", sprite, "TokenLaunch", Chain.Ethereum, 1, "STT", new MockCurrencyConverter()),
                new TokenElement("0xc683a014955b75F5ECF991d4502427c8fa1Aa249", sprite, "#hashtag", Chain.Ethereum, 1, "STT", new MockCurrencyConverter()),
                new TokenElement("0xc683a014955b75F5ECF991d4502427c8fa1Aa249", sprite, "tokenlowercase", Chain.Ethereum, 1, "STT", new MockCurrencyConverter()),
                new TokenElement("0xc683a014955b75F5ECF991d4502427c8fa1Aa249", sprite, "Token Name With Whitespace", Chain.Ethereum, 1, "STT", new MockCurrencyConverter()),
            };

            SearchableQuerier querier = new SearchableQuerier(searchableCollections, tokenElements, 6);
            querier.SetNewCriteria(searchCriteria);
            
            Queue<ISearchable> results = new Queue<ISearchable>();
            for (ISearchable result = querier.GetNextValid(); result != null; result = querier.GetNextValid())
            {
                results.Enqueue(result);
            }

            int expectedLength = expectedNames.Length;
            Assert.AreEqual(expectedLength, results.Count);
            for (int i = 0; i < expectedLength; i++)
            {
                Assert.AreEqual(expectedNames[i], results.Dequeue().GetName());
            }
        }
    }
}