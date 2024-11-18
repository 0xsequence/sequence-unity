using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;

namespace Sequence.Marketplace
{
    public class FilterAffordableOrders
    {
        private Address _buyer;
        private IIndexer _indexer;
        private CollectibleOrder[] _orders;
        
        public FilterAffordableOrders(Address buyer, IIndexer indexer, CollectibleOrder[] orders)
        {
            _buyer = buyer;
            _indexer = indexer;
            _orders = orders;
        }
        
        public async Task<CollectibleOrder[]> RemoveListingsThatUserCannotAfford()
        {
            if (_orders == null || _orders.Length == 0)
            {
                return _orders;
            }
            
            string[] currencies = GetCurrenciesNeeded(_orders);
            Dictionary<string, BigInteger> balances = await GetCurrencyBalances(currencies);
            
            List<CollectibleOrder> affordableOrders = new List<CollectibleOrder>();
            for (int i = 0; i < _orders.Length; i++)
            {
                Order order = _orders[i].order;
                BigInteger price = BigInteger.Parse(order.priceAmount);
                if (price <= balances[order.priceCurrencyAddress])
                {
                    affordableOrders.Add(_orders[i]);
                }
            }

            return affordableOrders.ToArray();
        }
        
        
        
        private string[] GetCurrenciesNeeded(CollectibleOrder[] orders)
        {
            int length = orders.Length;
            List<string> addresses = new List<string>();
            for (int i = 0; i < length; i++)
            {
                string currencyAddress = orders[i].order.priceCurrencyAddress;
                if (!addresses.Contains(currencyAddress))
                {
                    addresses.Add(currencyAddress);
                }
            }

            return addresses.ToArray();
        }
        
        private async Task<Dictionary<string, BigInteger>> GetCurrencyBalances(string[] currencyAddresses)
        {
            Dictionary<string, BigInteger> balances = new Dictionary<string, BigInteger>();
            for (int i = 0; i < currencyAddresses.Length; i++)
            {
                GetTokenBalancesReturn result = await _indexer.GetTokenBalances(new GetTokenBalancesArgs(_buyer, currencyAddresses[i]));
                balances[currencyAddresses[i]] = BigInteger.Zero;
                if (result != null && result.balances!= null && result.balances.Length > 0)
                {
                    balances[currencyAddresses[i]] = result.balances[0].balance;
                }
            }
            
            return balances;
        }
    }
}