using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SequenceSDK.WaaS;
using UnityEditor;

namespace Sequence.WaaS
{
    [System.Serializable]
    public class IntentDataSendTransaction
    {
        public string identifier = Guid.NewGuid().ToString();
        public string network;
        public Transaction[] transactions;
        public string wallet;

        public static readonly string transactionTypeIdentifier = "type";

        public IntentDataSendTransaction(string walletAddress, string network, Sequence.WaaS.Transaction[] transactions)
        {
            this.wallet = walletAddress;
            this.network = network;
            this.transactions = transactions;
        }
        
        public IntentDataSendTransaction(string walletAddress, Chain network, Sequence.WaaS.Transaction[] transactions)
        {
            uint networkId = (uint)network;
            this.wallet = walletAddress;
            this.network = networkId.ToString();
            this.transactions = transactions;
        }

        [JsonConstructor]
        public IntentDataSendTransaction(string code, uint expires, uint issued, string network, JObject[] transactions, string wallet)
        {
            this.network = network;
            this.wallet = wallet;
            int transactionCount = transactions.Length;
            this.transactions = new Sequence.WaaS.Transaction[transactionCount];
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