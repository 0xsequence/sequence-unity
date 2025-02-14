using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Numerics;
using System.Threading.Tasks;

namespace Sequence
{
    public interface IIndexer
    {
        /// <summary>
        /// Handle a failed indexer query
        ///
        /// If provided to Indexer static class when calling a function, Indexer should call this method when needed
        /// </summary>
        /// <param name="error"></param>
        public void OnQueryFailed(string error);
        
        /// <summary>
        /// Handle an issue during an indexer query
        ///
        /// If provided to Indexer static class when calling a function, Indexer should call this method when needed
        /// </summary>
        /// <param name="error"></param>
        public void OnQueryEncounteredAnIssue(string error);
        
        /// <summary>
        /// Retrieve indexer status
        /// </summary>
        /// <returns>true if this chain's indexer is good, false otherwise</returns>
        /// <exception cref="HttpRequestException">If the network request fails</exception>
        public Task<bool> Ping();

        /// <summary>
        /// Retrieve indexer version information.
        /// </summary>
        /// <exception cref="HttpRequestException">If the network request fails</exception>
        public Task<Version> Version();

        /// <summary>
        /// Retrieve indexer runtime status information
        /// </summary>
        /// <exception cref="HttpRequestException">If the network request fails</exception>
        public Task<RuntimeStatus> RuntimeStatus();

        [Obsolete("If needed, access the ChainId variable on the implementing object")]
        public BigInteger GetChainID();

        /// <summary>
        /// Retrieve the Chain for this indexer
        /// </summary>
        /// <returns></returns>
        public Chain GetChain();

        /// <summary>
        /// Retrieve the balance of a network's native token for a given account address
        /// </summary>
        /// <exception cref="HttpRequestException">If the network request fails</exception>
        public Task<EtherBalance> GetEtherBalance(string accountAddress);

        /// <summary>
        /// Retrieve an account's token balances, optionally for a specific contract
        /// </summary>
        /// <exception cref="HttpRequestException">If the network request fails</exception>
        public Task<GetTokenBalancesReturn> GetTokenBalances(GetTokenBalancesArgs args);

        /// <summary>
        /// Retrieve an account's token balances organized in a Dictionary mapping token id to the associated TokenBalance
        /// Useful for easy lookup of token balances
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public Task<Dictionary<BigInteger, TokenBalance>> GetTokenBalancesOrganizedInDictionary(
            string accountAddress, string contractAddress, bool includeMetadata = false);

        /// <summary>
        /// Retrieve the token supply for a given contract
        /// </summary>
        /// <exception cref="HttpRequestException">If the network request fails</exception>
        public Task<GetTokenSuppliesReturn> GetTokenSupplies(GetTokenSuppliesArgs args);

        /// <summary>
        /// Retrieve <see cref="GetTokenSuppliesMapReturn"/>
        /// </summary>
        /// <exception cref="HttpRequestException">If the network request fails</exception>
        public Task<GetTokenSuppliesMapReturn> GetTokenSuppliesMap(GetTokenSuppliesMapArgs args);

        [Obsolete]
        public Task<GetBalanceUpdatesReturn> GetBalanceUpdates(GetBalanceUpdatesArgs args);

        /// <summary>
        /// Retrieve transaction history <see cref="GetTransactionHistoryReturn"/>
        /// </summary>
        /// <exception cref="HttpRequestException">If the network request fails</exception>
        public Task<GetTransactionHistoryReturn> GetTransactionHistory(GetTransactionHistoryArgs args);

        /// <summary>
        /// Retrieve transaction history <see cref="GetTransactionHistoryReturn"/>
        /// </summary>
        /// <exception cref="HttpRequestException">If the network request fails</exception>
        public Task<bool> SubscribeReceipts();
        
        /// <summary>
        /// Retrieve transaction history <see cref="GetTransactionHistoryReturn"/>
        /// </summary>
        /// <exception cref="HttpRequestException">If the network request fails</exception>
        public Task<bool> SubscribeEvents();
        
        /// <summary>
        /// Retrieve transaction history <see cref="GetTransactionHistoryReturn"/>
        /// </summary>
        /// <exception cref="HttpRequestException">If the network request fails</exception>
        public Task<bool> SubscribeBalanceUpdates();
    }
}