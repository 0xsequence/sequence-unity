using System;
using System.Numerics;
using System.Threading.Tasks;
using Sequence.Contracts;
using Sequence.EmbeddedWallet;
using Sequence.Provider;
using Sequence.Utils;
using UnityEngine;

namespace Sequence.Marketplace
{
    internal class AffordabilityCalculator
    {
        private readonly Func<Address, Task<string>> _getApproximateTotalInCurrency;
        private readonly IIndexer _indexer;
        private readonly IWallet _wallet;
        private readonly IEthClient _client;
        private readonly Chain _chain;

        public AffordabilityCalculator(Func<Address, Task<string>> getApproximateTotalInCurrency, IIndexer indexer, IWallet wallet, IEthClient client, Chain chain)
        {
            _getApproximateTotalInCurrency = getApproximateTotalInCurrency;
            _indexer = indexer;
            _wallet = wallet;
            _client = client;
            _chain = chain;
        }

        public async Task<string> GetApproximateTotalInCurrencyIfAffordable(string currencyContractAddress, TokenBalance userBalance = null)
        {
            string total = await _getApproximateTotalInCurrency(new Address(currencyContractAddress));
            if (string.IsNullOrWhiteSpace(total) || total.StartsWith("Error"))
            {
                return total;
            }
            
            double price = double.Parse(total);
            if (price <= 0)
            {
                return "";
            }

            BigInteger balanceAmount = 0;
            int decimals = 18;
            
            if (userBalance != null)
            {
                balanceAmount = userBalance.balance;
                decimals = await GetDecimals(userBalance, currencyContractAddress);
            }
            else if (currencyContractAddress != Currency.NativeCurrencyAddress)
            {
                try
                {
                    GetTokenBalancesReturn balancesReturn = await _indexer.GetTokenBalances(new GetTokenBalancesArgs(_wallet.GetWalletAddress(), currencyContractAddress));
                    if (balancesReturn == null || balancesReturn.balances == null)
                    {
                        throw new Exception("Received unexpected null response from indexer");
                    }
                    TokenBalance[] balances = balancesReturn.balances;
                    if (balances.Length == 0)
                    {
                        return "";
                    }
                    TokenBalance balance = balances[0];
                    decimals = await GetDecimals(balance, currencyContractAddress);
                    balanceAmount = balance.balance;
                }
                catch (Exception e)
                {
                    string error = $"Error fetching token balances for {currencyContractAddress}: {e.Message}";
                    return error;
                }
            }
            else
            {
                try
                {
                    EtherBalance balancesReturn = await _indexer.GetEtherBalance(_wallet.GetWalletAddress());
                    if (balancesReturn == null)
                    {
                        throw new Exception("Received unexpected null response from indexer");
                    }
                    balanceAmount = balancesReturn.balanceWei;
                }
                catch (Exception e)
                {
                    string error = $"Error fetching native balance for {_chain}: {e.Message}";
                    return error;
                }
            }
            double balanceNormalized = DecimalNormalizer.ReturnToNormal(balanceAmount,decimals);
            if (balanceNormalized >= price)
            {
                return total;
            }
            else
            {
                return "";
            }
        }

        private async Task<int> GetDecimals(TokenBalance balance, string currencyContractAddress)
        {
            int decimals = 18;
            if (balance.contractInfo == null && balance.tokenMetadata == null) 
            {
                Debug.LogWarning($"No contract info or metadata found for {balance.contractAddress}, attempting to fetch from contract...");
                try
                {
                    var decimalsFromContract = await new ERC20(currencyContractAddress).Decimals(_client);
                    decimals = (int)decimalsFromContract;
                }
                catch (Exception e)
                {
                    Debug.LogError($"Error fetching decimals from contract for {currencyContractAddress}: {e.Message}\nUsing default of 18 decimals");
                }
            }
            else
            {
                if (balance.contractInfo != null)
                {
                    decimals = balance.contractInfo.decimals;
                }
                else
                {
                    decimals = (int)balance.tokenMetadata.decimals;
                }
            }

            return decimals;
        }
    }
}