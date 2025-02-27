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

        // Todo consider re-write such that we first check what currencies they have in their wallet before checking the swap price
        public async Task<string> GetApproximateTotalInCurrencyIfAffordable(string currencyContractAddress)
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

            if (currencyContractAddress != Currency.NativeCurrencyAddress)
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
                    int decimals = 18;
                    if (balance.contractInfo == null)
                    {
                        Debug.LogWarning($"No contract info found for {balance.contractAddress}, attempting to fetch from contract...");
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
                        decimals = balance.contractInfo.decimals;
                    }

                    double balanceAmount = DecimalNormalizer.ReturnToNormal(balance.balance, decimals);
                    if (balanceAmount >= price)
                    {
                        return total;
                    }
                    else
                    {
                        return "";
                    }
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
                    BigInteger balance = balancesReturn.balanceWei;
                    double balanceAmount = DecimalNormalizer.ReturnToNormal(balance,1);
                    if (balanceAmount >= price)
                    {
                        return total;
                    }
                    else
                    {
                        return "";
                    }
                }
                catch (Exception e)
                {
                    string error = $"Error fetching native balance for {_chain}: {e.Message}";
                    return error;
                }
            }
        }
    }
}