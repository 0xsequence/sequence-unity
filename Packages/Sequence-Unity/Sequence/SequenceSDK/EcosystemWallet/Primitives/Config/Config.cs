using System.Collections.Generic;
using System.Numerics;
using Newtonsoft.Json;
using Sequence.ABI;
using Sequence.Utils;
using UnityEngine;

namespace Sequence.EcosystemWallet.Primitives
{
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
            
            Debug.Log($"1 SequenceCoder.KeccakHash: {root.ByteArrayToHexStringWithPrefix()}");
            
            byte[] checkpointBytes = checkpoint.ByteArrayFromNumber(checkpoint.MinBytesFor()).PadLeft(32);
            root = SequenceCoder.KeccakHash(ByteArrayExtensions.ConcatenateByteArrays(root, checkpointBytes));
            
            string checkpointerAddress = checkpointer?.Value ?? "0x0000000000000000000000000000000000000000";
            byte[] checkpointerBytes = checkpointerAddress.HexStringToByteArray().PadLeft(32);
            root = SequenceCoder.KeccakHash(ByteArrayExtensions.ConcatenateByteArrays(root, checkpointerBytes));
            
            Debug.Log($"thresholdBytes: {thresholdBytes.ByteArrayToHexStringWithPrefix()}");
            Debug.Log($"checkpointBytes: {checkpointBytes.ByteArrayToHexStringWithPrefix()}");
            Debug.Log($"checkpointerBytes: {checkpointerBytes.ByteArrayToHexStringWithPrefix()}");
            
            return root;
        }

        public string ToJson()
        {
            var jsonObject = new
            {
                threshold = threshold.ToString(),
                checkpoint = checkpoint.ToString(),
                topology = topology?.Parse(),
                checkpointer = checkpointer?.Value
            };

            return JsonConvert.SerializeObject(jsonObject);
        }

        public static Config FromJson(string json)
        {
            var input = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
            
            var threshold = input["threshold"].ToString();
            var checkpoint = input["checkpoint"].ToString();
            var checkpointer = new Address(input["checkpointer"].ToString());
            var topology = input["topology"].ToString();
            
            return new Config
            {
                threshold = BigInteger.Parse(threshold),
                checkpoint = BigInteger.Parse(checkpoint),
                checkpointer = checkpointer,
                topology = Topology.Decode(topology)
            };
        }
    }
}