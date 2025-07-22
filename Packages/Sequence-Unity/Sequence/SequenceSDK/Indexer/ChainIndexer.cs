using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Sequence.Utils;
using UnityEngine;

namespace Sequence
{
    public class ChainIndexer : IIndexer
    {
        public string ChainId { get; private set; }

        private IHttpHandler _customHttpHandler = null;
        
        public Action<string> OnQueryError;
        public Action<string> OnQueryIssue;

        private bool _logErrors;
        private bool _logWarnings;

        [Obsolete("Use the constructor that takes a Chain object instead")]
        public ChainIndexer(BigInteger chainId, bool logErrors = true, bool logWarnings = true)
        {
            this.ChainId = chainId.ToString();
            _logErrors = logErrors;
            _logWarnings = logWarnings;
        }
        
        public ChainIndexer(Chain chain, bool logErrors = true, bool logWarnings = true)
        {
            this.ChainId = chain.GetChainId();
            _logErrors = logErrors;
            _logWarnings = logWarnings;
        }

        /// <summary>
        /// Inject a custom http handler - useful for testing or customization
        /// </summary>
        /// <param name="httpHandler"></param>
        public void InjectCustomHttpHandler(IHttpHandler httpHandler)
        {
            _customHttpHandler = httpHandler;
        }

        public void OnQueryFailed(string error)
        {
            error = "Indexer query failed: " + error;
            if (_logErrors)
            {
                SequenceLog.Error(error);
            }
            
            OnQueryError?.Invoke(error);
        }

        public void OnQueryEncounteredAnIssue(string error)
        {
            string issue = "Indexer query encountered an issue: " + error;
            if (_logWarnings)
            {
                SequenceLog.Warning(issue);
            }

            OnQueryIssue?.Invoke(issue);
        }

        public Task<bool> Ping()
        {
            return Indexer.Ping(ChainId, _customHttpHandler, this);
        }

        public Task<Version> Version()
        {
            return Indexer.Version(ChainId, _customHttpHandler, this);
        }

        public Task<RuntimeStatus> RuntimeStatus()
        {
            return Indexer.RuntimeStatus(ChainId, _customHttpHandler, this);
        }

        [Obsolete]
        public BigInteger GetChainID()
        {
            return BigInteger.Parse(ChainId);
        }

        public Chain GetChain()
        {
            return ChainDictionaries.ChainById[ChainId];
        }
        
        public Task<EtherBalance> GetNativeTokenBalance(string accountAddress)
        {
            return Indexer.GetNativeTokenBalance(ChainId, accountAddress, 0, _customHttpHandler, this);
        }

        [Obsolete("Call GetNativeTokenBalance instead.")]
        public Task<EtherBalance> GetEtherBalance(string accountAddress)
        {
            return Indexer.GetNativeTokenBalance(ChainId, accountAddress, 0, _customHttpHandler, this);
        }

        public Task<GetTokenBalancesReturn> GetTokenBalances(GetTokenBalancesArgs args)
        {
            return Indexer.GetTokenBalances(ChainId, args, 0, _customHttpHandler, this);
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
            return Indexer.GetTokenSupplies(ChainId, args, 0, _customHttpHandler, this);
        }

        public Task<GetTokenSuppliesMapReturn> GetTokenSuppliesMap(GetTokenSuppliesMapArgs args)
        {
            return Indexer.GetTokenSuppliesMap(ChainId, args, 0, _customHttpHandler, this);
        }

        [Obsolete]
        public Task<GetBalanceUpdatesReturn> GetBalanceUpdates(GetBalanceUpdatesArgs args)
        {
            return Indexer.GetBalanceUpdates(ChainId, args);
        }

        public Task<GetTransactionHistoryReturn> GetTransactionHistory(GetTransactionHistoryArgs args)
        {
            return Indexer.GetTransactionHistory(ChainId, args, 0, _customHttpHandler, this);
        }

        public void SubscribeReceipts(SubscribeReceiptsArgs args, WebRPCStreamOptions<SubscribeReceiptsReturn> options)
        {
            Indexer.SubscribeReceipts(ChainId, args, options, this);
        }
        
        public void SubscribeEvents(SubscribeEventsArgs args, WebRPCStreamOptions<SubscribeEventsReturn> options)
        {
            Indexer.SubscribeEvents(ChainId, args, options, this);
        }
        
        public void SubscribeBalanceUpdates(SubscribeBalanceUpdatesArgs args, WebRPCStreamOptions<SubscribeBalanceUpdatesReturn> options)
        {
            Indexer.SubscribeBalanceUpdates(ChainId, args, options, this);
        }

        public void AbortStreams()
        {
            Indexer.AbortStreams(this);
        }

        public bool ChainMatched(Chain chain)
        {
            return chain == GetChain();
        }
    }
}