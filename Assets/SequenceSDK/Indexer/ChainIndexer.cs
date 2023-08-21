using System.Numerics;
using System.Threading.Tasks;

namespace Sequence
{
    public class ChainIndexer : IIndexer
    {
        private BigInteger _chainId;

        public ChainIndexer(BigInteger chainId)
        {
            this._chainId = chainId;
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

        public Task<EtherBalance> GetEtherBalance(string accountAddress)
        {
            return Indexer.GetEtherBalance(_chainId, accountAddress);
        }

        public Task<GetTokenBalancesReturn> GetTokenBalances(GetTokenBalancesArgs args)
        {
            return Indexer.GetTokenBalances(_chainId, args);
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