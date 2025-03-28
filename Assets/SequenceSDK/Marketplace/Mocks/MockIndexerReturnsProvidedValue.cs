using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;

namespace Sequence.Marketplace.Mocks
{
    public class MockIndexerReturnsProvidedValue : IIndexer
    {
        private string _given;
        private int _decimals;
        
        public MockIndexerReturnsProvidedValue(string given, int decimals)
        {
            _given = given;
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

        public async Task<EtherBalance> GetEtherBalance(string accountAddress)
        {
            return new EtherBalance()
            {
                accountAddress = accountAddress,
                balanceWei = BigInteger.Parse(_given)
            };
        }

        public async Task<GetTokenBalancesReturn> GetTokenBalances(GetTokenBalancesArgs args)
        {
            return new GetTokenBalancesReturn()
            {
                balances = new TokenBalance[]
                {
                    new TokenBalance()
                    {
                        accountAddress = args.accountAddress,
                        contractAddress = args.contractAddress,
                        balance = BigInteger.Parse(_given),
                        contractInfo = new ContractInfo()
                        {
                            decimals = _decimals,
                        }
                    }
                }
            };
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

        public void SubscribeReceipts(SubscribeReceiptsArgs args, WebRPCStreamOptions<SubscribeReceiptsReturn> options)
        {
            throw new System.NotImplementedException();
        }

        public void SubscribeEvents(SubscribeEventsArgs args, WebRPCStreamOptions<SubscribeEventsReturn> options)
        {
            throw new System.NotImplementedException();
        }

        public void SubscribeBalanceUpdates(SubscribeBalanceUpdatesArgs args, WebRPCStreamOptions<SubscribeBalanceUpdatesReturn> options)
        {
            throw new System.NotImplementedException();
        }

        public void AbortStreams()
        {
            throw new System.NotImplementedException();
        }

        public bool ChainMatched(Chain chain)
        {
            throw new System.NotImplementedException();
        }
    }
}