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
            if (topology == null)
            {
                return null;
            }

            return topology.FindSignerLeaf(address);
        }

        public byte[] HashConfiguration()
        {
            if (topology == null)
            {
                return null;
            }

            byte[] root = topology.HashConfiguration();
            
            byte[] thresholdBytes = threshold.ByteArrayFromNumber().PadLeft(32);
            root = SequenceCoder.KeccakHash(ByteArrayExtensions.ConcatenateByteArrays(root, thresholdBytes));
            
            byte[] checkpointBytes = checkpoint.ByteArrayFromNumber().PadLeft(32);
            root = SequenceCoder.KeccakHash(ByteArrayExtensions.ConcatenateByteArrays(root, checkpointBytes));
            
            string checkpointerAddress = checkpointer?.Value ?? "0x0000000000000000000000000000000000000000";
            byte[] checkpointerBytes = checkpointerAddress.HexStringToByteArray().PadLeft(32);
            root = SequenceCoder.KeccakHash(ByteArrayExtensions.ConcatenateByteArrays(root, checkpointerBytes));
            
            return root;
        }

        public string ToJson()
        {
            var jsonObject = new
            {
                threshold = threshold.ToString(),
                checkpoint = checkpoint.ToString(),
                topology = topology?.Encode(),
                checkpointer = checkpointer?.Value
            };

            return JsonConvert.SerializeObject(jsonObject);
        }
    }
}