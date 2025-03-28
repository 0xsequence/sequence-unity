using System;
using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace Sequence.Pay.Transak
{
    [Serializable]
    public class AddFundsSettings
    {
        public string walletAddress;
        public string fiatCurrency;
        public string defaultFiatAmount;
        public string defaultCryptoCurrency;
        public string networks;

        public const string DefaultNetworks =
            "ethereum,mainnet,arbitrum,optimism,polygon,polygonzkevm,zksync,base,bnb,oasys,astar,avaxcchain";
        public const string DefaultCryptoCurrency = "USDC";

        [Preserve]
        public AddFundsSettings(string walletAddress, string fiatCurrency = "USD", string defaultFiatAmount = "50", string defaultCryptoCurrency = DefaultCryptoCurrency, string networks = DefaultNetworks)
        {
            this.walletAddress = walletAddress;
            this.fiatCurrency = fiatCurrency;
            this.defaultFiatAmount = defaultFiatAmount;
            this.defaultCryptoCurrency = defaultCryptoCurrency;
            this.networks = networks;
        }
    }
}