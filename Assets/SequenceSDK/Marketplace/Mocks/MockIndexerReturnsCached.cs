using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace Sequence.Marketplace.Mocks
{
    public class MockIndexerReturnsCached : IIndexer
    {
        private Dictionary<string, GetTokenBalancesReturn> _cachedTokenBalances;
        public MockIndexerReturnsCached(Dictionary<string, GetTokenBalancesReturn> cachedTokenBalances)
        {
            _cachedTokenBalances = cachedTokenBalances;
        }    
        
        public void OnQueryFailed(string error)
        {
            throw new System.NotImplementedException();
        }

        public void OnQueryEncounteredAnIssue(string error)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> Ping()
        {
            throw new System.NotImplementedException();
        }

        public Task<Version> Version()
        {
            throw new System.NotImplementedException();
        }

        public Task<RuntimeStatus> RuntimeStatus()
        {
            throw new System.NotImplementedException();
        }

        public BigInteger GetChainID()
        {
            throw new System.NotImplementedException();
        }

        public Chain GetChain()
        {
            throw new System.NotImplementedException();
        }

        public Task<EtherBalance> GetEtherBalance(string accountAddress)
        {
            throw new System.NotImplementedException();
        }

        public async Task<GetTokenBalancesReturn> GetTokenBalances(GetTokenBalancesArgs args)
        {
            await Task.Yield();
            if (_cachedTokenBalances.TryGetValue(args.contractAddress, out var balances))
            {
                return balances;
            }

            return null;
        }

        public Task<Dictionary<BigInteger, TokenBalance>> GetTokenBalancesOrganizedInDictionary(string accountAddress, string contractAddress, bool includeMetadata = false)
        {
            throw new System.NotImplementedException();
        }

        public Task<GetTokenSuppliesReturn> GetTokenSupplies(GetTokenSuppliesArgs args)
        {
            throw new System.NotImplementedException();
        }

        public Task<GetTokenSuppliesMapReturn> GetTokenSuppliesMap(GetTokenSuppliesMapArgs args)
        {
            throw new System.NotImplementedException();
        }

        public Task<GetBalanceUpdatesReturn> GetBalanceUpdates(GetBalanceUpdatesArgs args)
        {
            throw new System.NotImplementedException();
        }

        public Task<GetTransactionHistoryReturn> GetTransactionHistory(GetTransactionHistoryArgs args)
        {
            throw new System.NotImplementedException();
        }

        public bool ChainMatched(Chain chain)
        {
            return true;
        }
    }
}