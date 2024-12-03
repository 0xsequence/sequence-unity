using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sequence.EmbeddedWallet;
using UnityEngine;

namespace Sequence.Marketplace
{
    public interface ICheckoutHelper
    {
        public static event Action<Marketplace.Currency> OnSelectedCurrency;

        public static void SelectCurrency(Currency currency)
        {
            OnSelectedCurrency?.Invoke(currency);
        }

        public string GetApproximateTotalInUSD();
        public Task<string> GetApproximateTotalInCurrency(Address currencyAddress);
        public Task<Currency[]> GetCurrencies();
        public Task<Sprite> GetCurrencyIcon(Currency currency);
        public Task<string> GetApproximateTotalInCurrencyIfAffordable(string currencyContractAddress);
        public Task<TransactionReturn> Checkout();
        public Task<Currency> GetBestCurrency();
        public CollectibleOrder[] GetListings();
        public IWallet GetWallet();
        public Dictionary<string, Sprite> GetCollectibleImagesByOrderId();
        public Dictionary<string, uint> GetAmountsRequestedByOrderId();
        public void SetAmountRequested(string orderId, uint amount);
    }
}