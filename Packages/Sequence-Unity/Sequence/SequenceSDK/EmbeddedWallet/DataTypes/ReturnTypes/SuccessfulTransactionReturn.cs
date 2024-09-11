using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;
using System;

namespace Sequence.EmbeddedWallet
{
    [System.Serializable]
    public class SuccessfulTransactionReturn : TransactionReturn
    {
        public const string IdentifyingCode = "transactionReceipt";
        public string txHash { get; private set; }
        public string metaTxHash { get; private set; }
        public IntentPayload request { get; private set; }
        public MetaTxnReceipt receipt { get; private set; }

        [Obsolete("nativeReceipt is deprecated. Please use nativeTransactionReceipt instead.")]
        public JObject nativeReceipt { get; private set; }
        public TransactionReceipt nativeTransactionReceipt { get; private set; }
        public SimulateResult[] simulations { get; private set; }

        public SuccessfulTransactionReturn(string txHash, string metaTxHash, IntentPayload request, MetaTxnReceipt receipt, JObject nativeReceipt = null, SimulateResult[] simulations = null)
        {
            this.txHash = txHash;
            this.metaTxHash = metaTxHash;
            this.request = request;
            this.receipt = receipt;
            this.nativeTransactionReceipt = nativeReceipt?.ToObject<TransactionReceipt>(new JsonSerializer { Converters = { new TransactionReceiptConverter() } });
            this.simulations = simulations;
        }
    }
    public class TransactionReceiptConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(TransactionReceipt);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jsonObject;
            try
            {
                jsonObject = JObject.Load(reader);
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError($"Failed to load JSON: {ex.Message}");
                throw;
            }

            TransactionReceipt receipt = new TransactionReceipt();

            try
            {
                receipt.transactionHash = jsonObject["transactionHash"]?.ToString();
                receipt.transactionIndex = jsonObject["transactionIndex"]?.ToString();
                receipt.blockHash = jsonObject["blockHash"]?.ToString();
                receipt.blockNumber = jsonObject["blockNumber"]?.ToString();
                receipt.from = jsonObject["from"]?.ToString();
                receipt.to = jsonObject["to"]?.ToString();
                receipt.cumulativeGasUsed = jsonObject["cumulativeGasUsed"]?.ToString();
                receipt.effectiveGasPrice = jsonObject["effectiveGasPrice"]?.ToString();
                receipt.gasUsed = jsonObject["gasUsed"]?.ToString();
                receipt.contractAddress = jsonObject["contractAddress"]?.ToString();
                receipt.logsBloom = jsonObject["logsBloom"]?.ToString();
                receipt.type = jsonObject["type"]?.ToString();
                receipt.root = jsonObject["root"]?.ToString();
                receipt.status = jsonObject["status"]?.ToString();
                receipt.logs = jsonObject["logs"]?.ToObject<List<Log>>(serializer);
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError($"Error during JSON conversion: {ex.Message}");
                throw;
            }

            return receipt;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            TransactionReceipt receipt = (TransactionReceipt)value;

            JObject receiptObject = new JObject
        {
            { "transactionHash", JToken.FromObject(receipt.transactionHash) },
            { "transactionIndex", JToken.FromObject(receipt.transactionIndex) },
            { "blockHash", JToken.FromObject(receipt.blockHash) },
            { "blockNumber", JToken.FromObject(receipt.blockNumber) },
            { "from", JToken.FromObject(receipt.from) },
            { "to", JToken.FromObject(receipt.to) },
            { "cumulativeGasUsed", JToken.FromObject(receipt.cumulativeGasUsed) },
            { "effectiveGasPrice", JToken.FromObject(receipt.effectiveGasPrice) },
            { "gasUsed", JToken.FromObject(receipt.gasUsed) },
            { "contractAddress", JToken.FromObject(receipt.contractAddress) },
            { "logsBloom", JToken.FromObject(receipt.logsBloom) },
            { "type", JToken.FromObject(receipt.type) },
            { "root", JToken.FromObject(receipt.root) },
            { "status", JToken.FromObject(receipt.status) },
            { "logs", JToken.FromObject(receipt.logs, serializer) }
        };

            receiptObject.WriteTo(writer);
        }
    }
}
