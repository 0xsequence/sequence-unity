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
        /// Subscribe to receipt events.
        /// </summary>
        /// <param name="args">Specify the account or event you want to receive events from by defining a filter.</param>
        /// <param name="options">Actions triggered whenever a message or error event is received.</param>
        public void SubscribeReceipts(SubscribeReceiptsArgs args, WebRPCStreamOptions<SubscribeReceiptsReturn> options);
        
        /// <summary>
        /// Subscribe to smart contract events.
        /// </summary>
        /// <param name="args">Specify the accounts or contracts you want to receive events from by defining a filter.</param>
        /// <param name="options">Actions triggered whenever a message or error event is received.</param>
        public void SubscribeEvents(SubscribeEventsArgs args, WebRPCStreamOptions<SubscribeEventsReturn> options);
        
        /// <summary>
        /// Subscribe to balance update events for a given contract address.
        /// </summary>
        /// <param name="args">Define the contract address you want to receive events from.</param>
        /// <param name="options">Actions triggered whenever a message or error event is received.</param>
        public void SubscribeBalanceUpdates(SubscribeBalanceUpdatesArgs args, WebRPCStreamOptions<SubscribeBalanceUpdatesReturn> options);

        /// <summary>
        /// Aborts all running streams.
        /// </summary>
        public void AbortStreams();

        /// <summary>
        /// Returns true if the provided Chain matches the Chain of this Indexer, false otherwise
        /// </summary>
        /// <param name="chain"></param>
        /// <returns></returns>
        public bool ChainMatched(Chain chain);
    }
}