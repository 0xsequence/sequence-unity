namespace Sequence.Integrations.Transak
{
    public class OnOffRampQueryParameters
    {
        public const string apiKey = "5911d9ec-46b5-48fa-a755-d59a715ff0cf"; // This can be hardcoded as it is a public API key
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
#if UNITY_2017_1_OR_NEWER
            this.referrerDomain = "sequence-unity: " + UnityEngine.Application.productName;
#else
            this.referrerDomain = "sequence-unity";
#endif
            this.fiatCurrency = addFundsSettings.fiatCurrency;
            this.disableWalletAddressForm = disableWalletAddressForm;
            this.defaultFiatAmount = addFundsSettings.defaultFiatAmount;
            this.defaultCryptoCurrency = addFundsSettings.defaultCryptoCurrency;
            this.networks = addFundsSettings.networks;
        }

        public string AsQueryParameters()
        {
            string url = $"apiKey={apiKey}&referrerDomain={referrerDomain}&walletAddress={walletAddress}&fiatCurrency={fiatCurrency}&disableWalletAddressForm={disableWalletAddressForm}&defaultFiatAmount={defaultFiatAmount}&defaultCryptoCurrency={defaultCryptoCurrency}&networks={networks}";
            url = url.Replace(" ", "");
            return url;
        }
    }
}