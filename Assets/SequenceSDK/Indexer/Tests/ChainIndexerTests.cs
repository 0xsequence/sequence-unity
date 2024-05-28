using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using NUnit.Framework;
using Sequence;
using Sequence.Utils;

namespace Sequence.Indexer.Tests
{
    public class ChainIndexerTests
    {
        private static List<Chain> chainIdCases = EnumExtensions.GetEnumValuesAsList<Chain>();

        private Address _address = new Address("0x8e3E38fe7367dd3b52D1e281E4e8400447C8d8B9"); // From the Sequence Docs - this address has a fair amount of tokens held on Polygon and works great for our test cases
    
        [TestCaseSource(nameof(chainIdCases))]
        public void TestCreateChainIndexerForChain(Chain chain)
        {
            if (chain == Chain.None) return;
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
            if (chain == Chain.None) return;
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
            if (chain == Chain.None) return;
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
        
        [TestCaseSource(nameof(chainIdCases))]
        public async Task TestRuntimeStatus(Chain chain)
        {
            if (chain == Chain.None) return;
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
            Assert.IsFalse(string.IsNullOrWhiteSpace(result.ver));
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
    }
}