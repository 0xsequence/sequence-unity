using System;
using Newtonsoft.Json;

namespace Sequence.EmbeddedWallet
{
    public class IntentDataFeeOptions
    {
        public string identifier { get; private set; }
        public string network { get; private set; }
        public Transaction[] transactions { get; private set; }
        public string wallet { get; private set; }
        
        [JsonConstructor]
        public IntentDataFeeOptions(string identifier, string network, Transaction[] transactions, string wallet)
        {
            this.identifier = identifier;
            this.network = network;
            this.transactions = transactions;
            this.wallet = wallet;
        }

        public IntentDataFeeOptions(Chain network, string walletAddress, params Transaction[] transactions)
        {
            this.identifier = Guid.NewGuid().ToString();
            this.network = network.GetChainId();
            this.wallet = walletAddress;
            this.transactions = transactions;
        }
    }
}