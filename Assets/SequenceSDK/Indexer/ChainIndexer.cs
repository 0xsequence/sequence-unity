using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using UnityEngine;

namespace Sequence
{
    public class ChainIndexer : IIndexer
    {
        private BigInteger _chainId;

        public ChainIndexer(BigInteger chainId, bool logErrors = true, bool logWarnings = true)
        {
            this._chainId = chainId;
            SetupLogging(logErrors, logWarnings);
        }
        
        public ChainIndexer(Chain chain, bool logErrors = true, bool logWarnings = true)
        {
            this._chainId = BigInteger.Parse(chain.GetChainId());
            SetupLogging(logErrors, logWarnings);
        }

        private void SetupLogging(bool logErrors, bool logWarnings)
        {
            if (logErrors)
            {
                Indexer.OnIndexerQueryFailed += HandleQueryFailed;
            }

            if (logWarnings)
            {
                Indexer.OnIndexerQueryIssue += HandleQueryIssue;
            }
        }

        private void HandleQueryFailed(string error)
        {
            Debug.LogError("Indexer query failed: " + error);
        }

        private void HandleQueryIssue(string error)
        {
            Debug.LogWarning("Indexer query encountered an issue: " + error);
        }
        
        public Task<bool> Ping()
        {
            return Indexer.Ping(_chainId);
        }

        public Task<Version> Version()
        {
            return Indexer.Version(_chainId);
        }

        public Task<RuntimeStatus> RuntimeStatus()
        {
            return Indexer.RuntimeStatus(_chainId);
        }

        public BigInteger GetChainID()
        {
            return _chainId;
        }

        public Chain GetChain()
        {
            return (Chain)(int)_chainId;
        }

        public Task<EtherBalance> GetEtherBalance(string accountAddress)
        {
            return Indexer.GetEtherBalance(_chainId, accountAddress);
        }

        public Task<GetTokenBalancesReturn> GetTokenBalances(GetTokenBalancesArgs args)
        {
            return Indexer.GetTokenBalances(_chainId, args);
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
            return Indexer.GetTokenSupplies(_chainId, args);
        }

        public Task<GetTokenSuppliesMapReturn> GetTokenSuppliesMap(GetTokenSuppliesMapArgs args)
        {
            return Indexer.GetTokenSuppliesMap(_chainId, args);
        }

        public Task<GetBalanceUpdatesReturn> GetBalanceUpdates(GetBalanceUpdatesArgs args)
        {
            return Indexer.GetBalanceUpdates(_chainId, args);
        }

        public Task<GetTransactionHistoryReturn> GetTransactionHistory(GetTransactionHistoryArgs args)
        {
            return Indexer.GetTransactionHistory(_chainId, args);
        }
    }
}