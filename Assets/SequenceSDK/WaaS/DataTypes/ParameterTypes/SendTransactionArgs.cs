using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SequenceSDK.WaaS;
using UnityEditor;

namespace Sequence.WaaS
{
    [System.Serializable]
    public class SendTransactionArgs
    {
        public string code { get; private set; } = "sendTransaction";
        public uint expires { get; private set; }
        public string identifier { get; private set; } = Guid.NewGuid().ToString();
        public uint issued { get; private set; }
        public string network { get; private set; }
        public SequenceSDK.WaaS.Transaction[] transactions { get; private set; }
        public string wallet { get; private set; }

        public static readonly string transactionTypeIdentifier = "type";

        public SendTransactionArgs(string walletAddress, string network, SequenceSDK.WaaS.Transaction[] transactions, uint timeBeforeExpiry = 30)
        {
            this.wallet = walletAddress;
            this.network = network;
            this.transactions = transactions;
            this.issued = (uint)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            this.expires = this.issued + timeBeforeExpiry;
        }
        
        public SendTransactionArgs(string walletAddress, Chain network, SequenceSDK.WaaS.Transaction[] transactions, uint timeBeforeExpiry = 30)
        {
            uint networkId = (uint)network;
            this.wallet = walletAddress;
            this.network = networkId.ToString();
            this.transactions = transactions;
            this.issued = (uint)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            this.expires = this.issued + timeBeforeExpiry;
        }

        [JsonConstructor]
        public SendTransactionArgs(string code, uint expires, uint issued, string network, JObject[] transactions, string wallet)
        {
            this.code = code;
            this.expires = expires;
            this.issued = issued;
            this.network = network;
            this.wallet = wallet;
            int transactionCount = transactions.Length;
            this.transactions = new SequenceSDK.WaaS.Transaction[transactionCount];
            for (int i = 0; i < transactionCount; i++)
            {
                if (transactions[i].TryGetValue(transactionTypeIdentifier, out var type))
                {
                    string typeName = type.Value<string>();
                    switch (typeName)
                    {
                        case RawTransaction.TypeIdentifier:
                            this.transactions[i] = transactions[i].ToObject<RawTransaction>();
                            break;
                        case SendERC20.TypeIdentifier:
                            this.transactions[i] = transactions[i].ToObject<SendERC20>();
                            break;
                        case SendERC721.TypeIdentifier:
                            this.transactions[i] = transactions[i].ToObject<SendERC721>();
                            break;
                        case SendERC1155.TypeIdentifier:
                            this.transactions[i] = transactions[i].ToObject<SendERC1155>();
                            break;
                        case DelayedEncode.TypeIdentifier:
                            this.transactions[i] = transactions[i].ToObject<DelayedEncode>();
                            break;
                        default:
                            throw new JsonSerializationException($"Unknown transaction type {typeName} in transaction {i}: {transactions[i]}");
                    }
                }
                else
                {
                    throw new JsonSerializationException(
                        $"No '{transactionTypeIdentifier}' found in transaction {i}: {transactions[i]}");
                }
            }
        }
    }
}