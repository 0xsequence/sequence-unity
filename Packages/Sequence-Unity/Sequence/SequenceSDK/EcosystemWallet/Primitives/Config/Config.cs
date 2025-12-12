using System;
using System.Collections.Generic;
using System.Numerics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Utilities.Encoders;
using Sequence.ABI;
using Sequence.Utils;
using UnityEngine;
using UnityEngine.Scripting;

namespace Sequence.EcosystemWallet.Primitives
{
    [JsonConverter(typeof(ConfigJsonConverter))]
    public class Config
    {
        public BigInteger threshold;
        public BigInteger checkpoint;
        public Topology topology;
        public Address checkpointer;

        public Leaf FindSignerLeaf(Address address)
        {
            return topology?.FindSignerLeaf(address);
        }

        public byte[] HashConfiguration()
        {
            if (topology == null)
                return null;

            byte[] root = topology.HashConfiguration();
            
            byte[] thresholdBytes = threshold.ByteArrayFromNumber(threshold.MinBytesFor()).PadLeft(32);
            root = SequenceCoder.KeccakHash(ByteArrayExtensions.ConcatenateByteArrays(root, thresholdBytes));
            
            byte[] checkpointBytes = checkpoint.ByteArrayFromNumber(checkpoint.MinBytesFor()).PadLeft(32);
            root = SequenceCoder.KeccakHash(ByteArrayExtensions.ConcatenateByteArrays(root, checkpointBytes));
            
            string checkpointerAddress = checkpointer?.Value ?? "0x0000000000000000000000000000000000000000";
            byte[] checkpointerBytes = checkpointerAddress.HexStringToByteArray().PadLeft(32);
            root = SequenceCoder.KeccakHash(ByteArrayExtensions.ConcatenateByteArrays(root, checkpointerBytes));
            
            return root;
        }

        public static Config FromJson(string json)
        {
            var input = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
            
            var threshold = input["threshold"].ToString();
            var checkpoint = input["checkpoint"].ToString();
            var topology = input["topology"].ToString();
            
            var checkpointer = input.TryGetValue("checkpointer", out var value) && 
                               value is string valueStr ? new Address(valueStr) : null;
            
            return new Config
            {
                threshold = BigInteger.Parse(threshold),
                checkpoint = BigInteger.Parse(checkpoint),
                checkpointer = checkpointer,
                topology = Topology.Decode(topology)
            };
        }
    }
    
    [Preserve]
    public class ConfigJsonConverter : JsonConverter<Config>
    {
        public override void WriteJson(JsonWriter writer, Config value, JsonSerializer serializer)
        {
            var obj = new JObject
            {
                ["threshold"] = value.threshold.ToString(),
                ["checkpoint"] = value.checkpoint.ToString(),
                ["topology"] = JToken.FromObject(value.topology.Parse(), serializer),
                ["checkpointer"] = value.checkpointer?.Value
            };

            obj.WriteTo(writer);
        }

        public override Config ReadJson(JsonReader reader, Type objectType, Config existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var obj = JObject.Load(reader);
            return new Config
            {
                threshold = BigInteger.Parse((string)obj["threshold"]),
                checkpoint = BigInteger.Parse((string)obj["checkpoint"]),
                topology = Topology.Decode(obj["topology"].ToString()),
                checkpointer = obj["checkpointer"] != null ? new Address((string)obj["checkpointer"]) : null
            };
        }
    }
}