using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NUnit.Framework;
using Sequence;
using Sequence.Utils;
using UnityEngine;
using UnityEngine.TestTools;

namespace Sequence.Indexer.Tests
{
    public class ChainIndexerTests
    {
        private static List<Chain> chainIdCases = EnumExtensions.GetEnumValuesAsList<Chain>();

        private Address _address = new Address("0x8e3E38fe7367dd3b52D1e281E4e8400447C8d8B9"); // From the Sequence Docs - this address has a fair amount of tokens held on Polygon and works great for our test cases
    
        [TestCaseSource(nameof(chainIdCases))]
        public void TestCreateChainIndexerForChain(Chain chain)
        {
            if (!chain.IsActive()) return;
            try
            {
                ChainIndexer chainIndexer = new ChainIndexer(chain);
                Assert.AreEqual(chain.GetChainId(), chainIndexer.ChainId);
                Assert.AreEqual(chain, chainIndexer.GetChain());
            }
            catch (Exception e)
            {
                Assert.Fail("Encountered exception when none was expected: " + e.Message);
            }
        }
        
        [TestCaseSource(nameof(chainIdCases))]
        public async Task TestPingChain(Chain chain)
        {
            if (!chain.IsActive()) return;
            try
            {
                IIndexer chainIndexer = new ChainIndexer(chain);
                bool result = await chainIndexer.Ping();
                Assert.IsTrue(result);
            }
            catch (Exception e)
            {
                Assert.Fail("Encountered exception when none was expected: " + e.Message);
            }
        }

        [TestCaseSource(nameof(chainIdCases))]
        public async Task TestVersion(Chain chain)
        {
            if (!chain.IsActive()) return;
            try
            {
                IIndexer indexer = new ChainIndexer(chain);
                Version version = await indexer.Version();
                Assert.IsNotNull(version);
                Assert.IsFalse(string.IsNullOrWhiteSpace(version.appVersion));
                Assert.IsFalse(string.IsNullOrWhiteSpace(version.schemaVersion));
                Assert.IsFalse(string.IsNullOrWhiteSpace(version.webrpcVersion));
                Assert.IsFalse(string.IsNullOrWhiteSpace(version.schemaHash));
            }
            catch (Exception e)
            {
                Assert.Fail("Encountered exception when none was expected: " + e.Message);
            }
        }


        private bool ChainIsInactive(Chain chain)
        {
            return chain == Chain.None || chain == Chain.AstarZKEvm || chain == Chain.TestnetAstarZKyoto || chain == Chain.TestnetBorne;
        }
        
        [TestCaseSource(nameof(chainIdCases))]
        public async Task TestRuntimeStatus(Chain chain)
        {
            if (!chain.IsActive()) return;
            RuntimeStatus result = null;
            try
            {
                IIndexer chainIndexer = new ChainIndexer(chain);
                result = await chainIndexer.RuntimeStatus();
            }
            catch (Exception e)
            {
                Assert.Fail("Encountered exception when none was expected: " + e.Message);
            }
            
            Assert.IsNotNull(result);
            Assert.IsTrue(result.indexerEnabled);
            Assert.IsTrue(result.healthOK);
            Assert.IsFalse(string.IsNullOrWhiteSpace(result.startTime));
            Assert.IsFalse(string.IsNullOrWhiteSpace(result.branch));
            Assert.IsFalse(string.IsNullOrWhiteSpace(result.commitHash));
            Assert.AreEqual(chain.GetChainId(), result.chainID);
            Assert.Greater(result.uptime, 0);
            Assert.IsNotNull(result.checks);
            Assert.IsTrue(result.checks.running);
            Assert.IsFalse(string.IsNullOrWhiteSpace(result.checks.syncMode));
            Assert.Greater(result.checks.lastBlockNum, BigInteger.Zero);
        }

        [Test]
        public async Task TestGetEtherBalance()
        {
            IIndexer indexer = new ChainIndexer(Chain.Polygon);
            EtherBalance balance = null;
            try
            {
                balance = await indexer.GetEtherBalance(_address);
            }
            catch (Exception e)
            {
                Assert.Fail("Encountered exception when none was expected: " + e.Message);
            }

            Assert.IsNotNull(balance);
            Assert.AreEqual(_address.Value.ToLower(), balance.accountAddress);
            Assert.Greater(balance.balanceWei, BigInteger.Zero);
        }

        [Test]
        public async Task TestGetTokenBalances()
        {
            IIndexer indexer = new ChainIndexer(Chain.Polygon);
            GetTokenBalancesReturn balances = null;
            try
            {
                balances = await indexer.GetTokenBalances(new GetTokenBalancesArgs(_address));
            }
            catch (Exception e)
            {
                Assert.Fail("Encountered exception when none was expected: " + e.Message);
            }

            Assert.IsNotNull(balances);
            Assert.IsNotNull(balances.page);
            Assert.IsNotNull(balances.balances);
            Assert.Greater(balances.balances.Length, 0);
        }

        [Test]
        public async Task TestGetTokenBalancesOrganizedInDictionary()
        {
            string skyweaverContractAddress = "0x631998e91476DA5B870D741192fc5Cbc55F5a52E";
            IIndexer indexer = new ChainIndexer(Chain.Polygon);
            Dictionary<BigInteger, TokenBalance> tokenDictionary = null;
            try
            {
                tokenDictionary =
                    await indexer.GetTokenBalancesOrganizedInDictionary(_address, skyweaverContractAddress);
            }
            catch (Exception e)
            {
                Assert.Fail("Encountered exception when none was expected: " + e.Message);
            }
            
            Assert.IsNotNull(tokenDictionary);
            Assert.Greater(tokenDictionary.Count, 0);
        }

        [Test]
        public async Task TestGetTokenSupplies()
        {
            string skyweaverContractAddress = "0x631998e91476DA5B870D741192fc5Cbc55F5a52E";
            IIndexer indexer = new ChainIndexer(Chain.Polygon);
            GetTokenSuppliesReturn tokenSupplies = null;
            try
            {
                tokenSupplies = await indexer.GetTokenSupplies(new GetTokenSuppliesArgs(skyweaverContractAddress));
            }
            catch (Exception e)
            {
                Assert.Fail("Encountered exception when none was expected: " + e.Message);
            }
            
            Assert.IsNotNull(tokenSupplies);
            Assert.IsNotNull(tokenSupplies.page);
            Assert.AreEqual(ContractType.ERC1155, tokenSupplies.contractType);
            Assert.IsNotNull(tokenSupplies.tokenIDs);
            Assert.Greater(tokenSupplies.tokenIDs.Length, 0);
        }

        [Test]
        public async Task TestGetTransactionHistory()
        {
            IIndexer indexer = new ChainIndexer(Chain.Polygon);
            GetTransactionHistoryReturn history = null;
            try
            {
                history = await indexer.GetTransactionHistory(
                    new GetTransactionHistoryArgs(new TransactionHistoryFilter()
                    {
                        accountAddress = _address
                    }));
            }
            catch (Exception e)
            {
                Assert.Fail("Encountered exception when none was expected: " + e.Message);
            }
            
            Assert.IsNotNull(history);
            Assert.IsNotNull(history.page);
            Assert.IsNotNull(history.transactions);
            Assert.Greater(history.transactions.Length, 0);
        }
    
        [Test]
        public async Task TestGetTokenSuppliesMap()
        {
            IIndexer indexer = new ChainIndexer(Chain.Polygon);
            string usdcAddress = "0x3c499c542cEF5E3811e1192ce70d8cC03d5c3359".ToLower();
            string wmaticAddress = "0x0d500B1d8E8eF31E21C99d1Db9A6444d3ADf1270".ToLower();
            string skyweaverAddress = "0x631998e91476da5b870d741192fc5cbc55f5a52e".ToLower();
            string skyweaverTokenId1 = "68657";
            string skyweaverTokenId2 = "66669";
            string skyweaverTokenId3 = "66668";
            GetTokenSuppliesMapReturn suppliesMap = null;
            try
            {
                suppliesMap = await indexer.GetTokenSuppliesMap(new GetTokenSuppliesMapArgs(
                    new Dictionary<string, string[]>()
                    {
                        { usdcAddress, new string[] { } },
                        { wmaticAddress, new string[] { "0" } },
                        { skyweaverAddress, new string[] { skyweaverTokenId1, skyweaverTokenId2, skyweaverTokenId3 } },
                    }));
            }
            catch (Exception e)
            {
                Assert.Fail("Encountered exception when none was expected: " + e.Message);
            }

            Assert.IsNotNull(suppliesMap);
            Assert.IsNotNull(suppliesMap.supplies);
            Assert.AreEqual(3, suppliesMap.supplies.Count);
            Assert.IsTrue(suppliesMap.supplies.ContainsKey(usdcAddress));
            Assert.IsTrue(suppliesMap.supplies.ContainsKey(wmaticAddress));
            Assert.IsTrue(suppliesMap.supplies.ContainsKey(skyweaverAddress));
            Assert.IsNotNull(suppliesMap.supplies[usdcAddress]);
            Assert.IsNotNull(suppliesMap.supplies[wmaticAddress]);
            Assert.IsNotNull(suppliesMap.supplies[skyweaverAddress]);
            Assert.AreEqual(1, suppliesMap.supplies[usdcAddress].Length);
            Assert.AreEqual(1, suppliesMap.supplies[wmaticAddress].Length);
            Assert.AreEqual(3, suppliesMap.supplies[skyweaverAddress].Length, 0);
            Assert.Greater(suppliesMap.supplies[usdcAddress][0].supply.Length, 0);
            Assert.Greater(suppliesMap.supplies[wmaticAddress][0].supply.Length, 0);
            Assert.Greater(suppliesMap.supplies[skyweaverAddress][0].supply.Length, 0);
            Assert.Greater(suppliesMap.supplies[skyweaverAddress][1].supply.Length, 0);
            Assert.Greater(suppliesMap.supplies[skyweaverAddress][2].supply.Length, 0);
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task TestRequestErrorHandling(bool logError)
        {
            bool errorEventFired = false;

            ChainIndexer indexer = new ChainIndexer(Chain.Polygon, logError);
            indexer.OnQueryError += s =>
            {
                errorEventFired = true;
            };
            indexer.InjectCustomHttpHandler(new MockHttpHandler(new Exception("some error with request")));

            if (logError)
            {
                LogAssert.Expect(LogType.Error, new Regex("Indexer query failed:.* some error with request.*"));
            }

            await indexer.Ping();
            
            Assert.IsTrue(errorEventFired);
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task TestRequestDecodingErrorHandling(bool logError)
        {
            bool errorEventFired = false;

            ChainIndexer indexer = new ChainIndexer(Chain.Polygon, logError);
            indexer.OnQueryError += s =>
            {
                errorEventFired = true;
            };
            indexer.InjectCustomHttpHandler(new MockHttpHandler("some random garbage"));

            if (logError)
            {
                LogAssert.Expect(LogType.Error, new Regex("Indexer query failed:.* some random garbage.*"));
            }

            await indexer.GetEtherBalance(_address);
            
            Assert.IsTrue(errorEventFired);
        }

        [TestCase]
        public void TestSubscribeReceipts()
        {
            var indexer = new ChainIndexer(Chain.TestnetArbitrumSepolia);
            var streamOptions = new WebRPCStreamOptions<SubscribeReceiptsReturn>(
                OnSubscribeReceiptsMessageReceived,
                OnWebRPCErrorReceived);

            var filter = new TransactionFilter
            {
                contractAddress = "0x4ab3b16e9d3328f6d8025e71cefc64305ae4fe9c"
            };

            indexer.SubscribeReceipts(new SubscribeReceiptsArgs(filter), streamOptions);
        }
        
        [TestCase]
        public void TestSubscribeEvents()
        {
            var indexer = new ChainIndexer(Chain.TestnetArbitrumSepolia);
            var streamOptions = new WebRPCStreamOptions<SubscribeEventsReturn>(
                OnSubscribeEventsMessageReceived,
                OnWebRPCErrorReceived);

            var eventFilter = new EventFilter
            {
                accounts = Array.Empty<string>(),
                contractAddresses = new[] {"0x4ab3b16e9d3328f6d8025e71cefc64305ae4fe9c"},
                tokenIDs = new[] {"0"},
                events = new[] {"Transfer(address from, address to, uint256 value)"}
            };
            
            indexer.SubscribeEvents(new SubscribeEventsArgs(eventFilter), streamOptions);
        }
        
        [TestCase]
        public void TestSubscribeBalanceUpdates()
        {
            var indexer = new ChainIndexer(Chain.TestnetArbitrumSepolia);
            var streamOptions = new WebRPCStreamOptions<SubscribeBalanceUpdatesReturn>(
                OnSubscribeEventsMessageReceived,
                OnWebRPCErrorReceived);

            var contractAddress = "0x4ab3b16e9d3328f6d8025e71cefc64305ae4fe9c";
            indexer.SubscribeBalanceUpdates(new SubscribeBalanceUpdatesArgs(contractAddress), streamOptions);
        }
        
        private void OnSubscribeReceiptsMessageReceived(SubscribeReceiptsReturn @event)
        {
            Debug.Log($"Receipt Event Received, hash: {@event.receipt.transactionHash}");
        }

        private void OnSubscribeEventsMessageReceived(SubscribeEventsReturn @event)
        {
            Debug.Log($"Contract Event Received, address: {@event.log.contractAddress}");
        }
        
        private void OnSubscribeEventsMessageReceived(SubscribeBalanceUpdatesReturn @event)
        {
            Debug.Log($"Balance Update Received, balance: {@event.balance.balance}");
        }

        private void OnWebRPCErrorReceived(WebRPCError error)
        {
            Debug.LogError($"OnWebRPCErrorReceived: {error.msg}");
        }
    }
}