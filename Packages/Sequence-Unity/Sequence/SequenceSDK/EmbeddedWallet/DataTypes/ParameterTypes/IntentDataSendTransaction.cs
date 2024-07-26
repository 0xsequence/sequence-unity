using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Sequence.EmbeddedWallet
{
    [System.Serializable]
    public class IntentDataSendTransaction
    {
        public string identifier { get; private set; } = Guid.NewGuid().ToString();
        public string network { get; private set; }
        public Transaction[] transactions { get; private set; }
        public string transactionsFeeQuote { get; private set; }
        public string wallet { get; private set; }

        public static readonly string transactionTypeIdentifier = "type";

        public IntentDataSendTransaction(string walletAddress, string network, Transaction[] transactions, string transactionsFeeQuote = "")
        {
            this.wallet = walletAddress;
            this.network = network;
            this.transactions = transactions;
            if (!string.IsNullOrWhiteSpace(transactionsFeeQuote))
            {
                this.transactionsFeeQuote = transactionsFeeQuote;
            }
        }
        
        public IntentDataSendTransaction(string walletAddress, Chain network, Transaction[] transactions, string transactionsFeeQuote = "")
        {
            this.wallet = walletAddress;
            this.network = network.GetChainId();
            this.transactions = transactions;
            if (!string.IsNullOrWhiteSpace(transactionsFeeQuote))
            {
                this.transactionsFeeQuote = transactionsFeeQuote;
            }
        }

        [JsonConstructor]
        public IntentDataSendTransaction(string code, uint expires, uint issued, string network, JObject[] transactions, string transactionsFeeQuote, string wallet)
        {
            this.network = network;
            this.wallet = wallet;
            this.transactionsFeeQuote = transactionsFeeQuote;
            int transactionCount = transactions.Length;
            this.transactions = new Transaction[transactionCount];
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