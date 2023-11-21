using System;
using UnityEditor;

namespace Sequence.WaaS
{
    [System.Serializable]
    public class SendTransactionArgs
    {
        public string code { get; private set; } = "sendTransaction";
        public string identifier { get; private set; } = Guid.NewGuid().ToString();
        public string wallet { get; private set; }
        public string network { get; private set; }
        public SequenceSDK.WaaS.Transaction[] transactions { get; private set; }

        public SendTransactionArgs(string walletAddress, string network, SequenceSDK.WaaS.Transaction[] transactions)
        {
            this.wallet = walletAddress;
            this.network = network;
            this.transactions = transactions;
        }
    }
}