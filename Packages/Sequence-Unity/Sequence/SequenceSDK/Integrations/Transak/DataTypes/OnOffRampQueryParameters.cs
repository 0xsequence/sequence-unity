using System;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Scripting;

namespace Sequence.Integrations.Transak
{
    public class OnOffRampQueryParameters
    {
        public string referrerDomain;
        public string walletAddress;
        public string fiatCurrency;
        public bool disableWalletAddressForm;
        public string defaultFiatAmount;
        public string defaultCryptoCurrency;
        public string networks;

        public OnOffRampQueryParameters(Address walletAddress, AddFundsSettings addFundsSettings,
            bool disableWalletAddressForm = true)
        {
            this.walletAddress = walletAddress;
            this.referrerDomain = "sequence-unity: " + Application.productName;
            this.fiatCurrency = addFundsSettings.fiatCurrency;
            this.disableWalletAddressForm = disableWalletAddressForm;
            this.defaultFiatAmount = addFundsSettings.defaultFiatAmount;
            this.defaultCryptoCurrency = addFundsSettings.defaultCryptoCurrency;
            this.networks = addFundsSettings.networks;
        }

        public string AsQueryParameters()
        {
            string url = $"apiKey={SequenceTransakContractIdRepository.ApiKey}&referrerDomain={referrerDomain}&walletAddress={walletAddress}&fiatCurrency={fiatCurrency}&disableWalletAddressForm={disableWalletAddressForm}&defaultFiatAmount={defaultFiatAmount}&defaultCryptoCurrency={defaultCryptoCurrency}&networks={networks}";
            url = url.Replace(" ", "");
            return url;
        }
    }
}