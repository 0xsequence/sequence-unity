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
        public TransactionReceipt nativeReceipt { get; private set; }
        public SimulateResult[] simulations { get; private set; }

        public SuccessfulTransactionReturn(string txHash, string metaTxHash, IntentPayload request, MetaTxnReceipt receipt, JObject nativeReceipt = null, SimulateResult[] simulations = null)
        {
            this.txHash = txHash;
            this.metaTxHash = metaTxHash;
            this.request = request;
            this.receipt = receipt;
            this.nativeReceipt = nativeReceipt.ToObject<TransactionReceipt>(new JsonSerializer { Converters = { new TransactionReceiptConverter() } });
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
            JObject jsonObject = JObject.Load(reader);

            var enumerator = jsonObject.Properties().GetEnumerator();
            if (!enumerator.MoveNext())
            {
                throw new JsonSerializationException("Invalid JSON format for TransactionReceipt.");
            }

            JProperty dynamicKeyProperty = enumerator.Current;

            JObject receiptObject = (JObject)dynamicKeyProperty.Value;

            TransactionReceipt receipt = new TransactionReceipt
            {
                transactionHash = receiptObject["transactionHash"]?.ToObject<string>(),
                transactionIndex = receiptObject["transactionIndex"]?.ToObject<string>(),
                blockHash = receiptObject["blockHash"]?.ToObject<string>(),
                blockNumber = receiptObject["blockNumber"]?.ToObject<string>(),
                from = receiptObject["from"]?.ToObject<string>(),
                to = receiptObject["to"]?.ToObject<string>(),
                cumulativeGasUsed = receiptObject["cumulativeGasUsed"]?.ToObject<string>(),
                effectiveGasPrice = receiptObject["effectiveGasPrice"]?.ToObject<string>(),
                gasUsed = receiptObject["gasUsed"]?.ToObject<string>(),
                contractAddress = receiptObject["contractAddress"]?.ToObject<string>(),
                logs = receiptObject["logs"]?.ToObject<List<Log>>(serializer),
                logsBloom = receiptObject["logsBloom"]?.ToObject<string>(),
                type = receiptObject["type"]?.ToObject<string>(),
                root = receiptObject["root"]?.ToObject<string>(),
                status = receiptObject["status"]?.ToObject<string>()
            };

            return receipt;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            TransactionReceipt receipt = (TransactionReceipt)value;

            // Create a JObject to hold the TransactionReceipt properties
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
        { "logs", JArray.FromObject(receipt.logs, serializer) },
        { "logsBloom", JToken.FromObject(receipt.logsBloom) },
        { "type", JToken.FromObject(receipt.type) },
        { "root", JToken.FromObject(receipt.root) },
        { "status", JToken.FromObject(receipt.status) }
    };

            // Use the transaction hash as the dynamic key
            string dynamicKey = receipt.transactionHash;

            // Create the final JSON object with the dynamic key wrapping the receiptObject
            JObject jsonObject = new JObject
    {
        { dynamicKey, receiptObject }
    };

            // Write the JSON to the writer
            jsonObject.WriteTo(writer);
        }
    }
}