using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Sequence;
using Sequence.Utils;
using Random = UnityEngine.Random;
using Version = Sequence.Version;

namespace Temp
{
    public class MockIndexerReturnsHighBalances : IIndexer
    {
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
                balanceWei = DecimalNormalizer.NormalizeAsBigInteger(Random.Range(10000f, 100000f))
            };
        }

        public async Task<GetTokenBalancesReturn> GetTokenBalances(GetTokenBalancesArgs args)
        {
            int decimals = Random.Range(0, 19);
            return new GetTokenBalancesReturn()
            {
                balances = new TokenBalance[]
                {
                    new TokenBalance()
                    {
                        balance = DecimalNormalizer.NormalizeAsBigInteger(Random.Range(10000f, 100000f), decimals),
                        contractInfo = new ContractInfo()
                        {
                            address = args.contractAddress,
                            decimals = decimals,
                        },
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

        public bool ChainMatched(Chain chain)
        {
            return true;
        }
    }
}