using System.Net.Http;
using System.Numerics;
using System.Threading.Tasks;

namespace Sequence
{
    public interface IIndexer
    {
        /// <summary>
        /// Retrive indexer status
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

        /// <summary>
        /// Retrieve the chain ID for a given BlockChainType
        /// </summary>

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
        /// Retrieve the token supply for a given contract
        /// </summary>
        /// <exception cref="HttpRequestException">If the network request fails</exception>
        public Task<GetTokenSuppliesReturn> GetTokenSupplies(GetTokenSuppliesArgs args);

        /// <summary>
        /// Retrieve <see cref="GetTokenSuppliesMapReturn"/>
        /// </summary>
        /// <exception cref="HttpRequestException">If the network request fails</exception>
        public Task<GetTokenSuppliesMapReturn> GetTokenSuppliesMap(GetTokenSuppliesMapArgs args);

        /// <summary>
        /// Retrieve <see cref="GetBalanceUpdatesReturn"/>
        /// </summary>
        /// <exception cref="HttpRequestException">If the network request fails</exception>
        public Task<GetBalanceUpdatesReturn> GetBalanceUpdates(GetBalanceUpdatesArgs args);

        /// <summary>
        /// Retrieve transaction history <see cref="GetTransactionHistoryReturn"/>
        /// </summary>
        /// <exception cref="HttpRequestException">If the network request fails</exception>
        public Task<GetTransactionHistoryReturn> GetTransactionHistory(GetTransactionHistoryArgs args);
    }
}