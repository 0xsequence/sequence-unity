using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using UnityEngine;

namespace Sequence
{
    public class ChainIndexer : IIndexer
    {
        public string ChainId { get; private set; }

        private IHttpHandler _customHttpHandler = null;
        
        private static int _counter = 0;
        private int _instanceId;

        public ChainIndexer(BigInteger chainId, bool logErrors = true, bool logWarnings = true)
        {
            this.ChainId = chainId.ToString();
            _instanceId = ++_counter;
            SetupLogging(logErrors, logWarnings);
        }
        
        public ChainIndexer(Chain chain, bool logErrors = true, bool logWarnings = true)
        {
            this.ChainId = chain.GetChainId();
            _instanceId = ++_counter;
            SetupLogging(logErrors, logWarnings);
        }

        /// <summary>
        /// Inject a custom http handler - useful for testing or customization
        /// </summary>
        /// <param name="httpHandler"></param>
        public void InjectCustomHttpHandler(IHttpHandler httpHandler)
        {
            _customHttpHandler = httpHandler;
        }

        private void SetupLogging(bool logErrors, bool logWarnings)
        {
            for (int i = 0; i < 1000; i++)
            {
                IIndexer.OnIndexerQueryError -= HandleQueryFailed;
                IIndexer.OnIndexerQueryIssue -= HandleQueryFailed;
            }
            
            if (logErrors)
            {
                IIndexer.OnIndexerQueryError += HandleQueryFailed;
            }

            if (logWarnings)
            {
                IIndexer.OnIndexerQueryIssue += HandleQueryIssue;
            }
        }

        private void HandleQueryFailed(string error)
        {
            Debug.LogError(_instanceId + "Indexer query failed: " + error);
        }

        private void HandleQueryIssue(string error)
        {
            Debug.LogWarning(_instanceId + "Indexer query encountered an issue: " + error);
        }
        
        public Task<bool> Ping()
        {
            return Indexer.Ping(ChainId, _customHttpHandler);
        }

        public Task<Version> Version()
        {
            return Indexer.Version(ChainId, _customHttpHandler);
        }

        public Task<RuntimeStatus> RuntimeStatus()
        {
            return Indexer.RuntimeStatus(ChainId, _customHttpHandler);
        }

        public BigInteger GetChainID()
        {
            return BigInteger.Parse(ChainId);
        }

        public Chain GetChain()
        {
            return ChainDictionaries.ChainById[ChainId];
        }

        public Task<EtherBalance> GetEtherBalance(string accountAddress)
        {
            return Indexer.GetEtherBalance(ChainId, accountAddress, 0, _customHttpHandler);
        }

        public Task<GetTokenBalancesReturn> GetTokenBalances(GetTokenBalancesArgs args)
        {
            return Indexer.GetTokenBalances(ChainId, args, 0, _customHttpHandler);
        }

        public async Task<Dictionary<BigInteger, TokenBalance>> GetTokenBalancesOrganizedInDictionary(
            string accountAddress, string contractAddress, bool includeMetadata = false)
        {
            GetTokenBalancesReturn tokenBalances = await GetTokenBalances(new GetTokenBalancesArgs(accountAddress, contractAddress, includeMetadata));
            Dictionary<BigInteger, TokenBalance> result =
                new Dictionary<BigInteger, TokenBalance>();
            result = await AddBalancesToDictionary(result, tokenBalances.balances);
            while (tokenBalances.page.more)
            {
                tokenBalances = await GetTokenBalances(new GetTokenBalancesArgs(accountAddress, contractAddress, includeMetadata, tokenBalances.page));
                result = await AddBalancesToDictionary(result, tokenBalances.balances);
            }

            return result;
        }

        private async Task<Dictionary<BigInteger, TokenBalance>> AddBalancesToDictionary(
            Dictionary<BigInteger, TokenBalance> dictionary, TokenBalance[] balances)
        {
            int length = balances.Length;
            for (int i = 0; i < length; i++)
            {
                dictionary[balances[i].tokenID] = balances[i];
            }

            return dictionary;
        }

        public Task<GetTokenSuppliesReturn> GetTokenSupplies(GetTokenSuppliesArgs args)
        {
            return Indexer.GetTokenSupplies(ChainId, args, 0, _customHttpHandler);
        }

        public Task<GetTokenSuppliesMapReturn> GetTokenSuppliesMap(GetTokenSuppliesMapArgs args)
        {
            return Indexer.GetTokenSuppliesMap(ChainId, args, 0, _customHttpHandler);
        }

        [Obsolete]
        public Task<GetBalanceUpdatesReturn> GetBalanceUpdates(GetBalanceUpdatesArgs args)
        {
            return Indexer.GetBalanceUpdates(ChainId, args);
        }

        public Task<GetTransactionHistoryReturn> GetTransactionHistory(GetTransactionHistoryArgs args)
        {
            return Indexer.GetTransactionHistory(ChainId, args, 0, _customHttpHandler);
        }
    }
}