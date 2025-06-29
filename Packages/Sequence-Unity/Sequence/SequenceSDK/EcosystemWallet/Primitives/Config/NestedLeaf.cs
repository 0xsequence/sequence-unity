using System;
using System.Linq;
using System.Numerics;
using System.Text;
using Sequence.ABI;
using Sequence.Utils;

namespace Sequence.EcosystemWallet.Primitives
{
    public class NestedLeaf : Leaf
    {
        public Topology tree;
        public BigInteger weight;
        public BigInteger threshold;
        
        public override object Parse()
        {
            return new
            {
                type = Nested,
                tree = tree.Parse(),
                weight = weight.ToString(),
                threshold = threshold.ToString()
            };
        }

        public override byte[] Encode(bool noChainId, byte[] checkpointerData)
        {
            var nested = tree.Encode(noChainId, checkpointerData);
            int flag = Topology.FlagNested << 4;

            var weightBytes = new byte[0];
            if (weight <= 3 && weight > 0)
                flag |= (int)weight << 2;
            else if (weight <= 255)
                weightBytes = weight.ByteArrayFromNumber(weight.MinBytesFor());
            else
                throw new Exception("Weight too large");

            var thresholdBytes = new byte[0];
            if (threshold <= 3 && threshold > 0)
                flag |= (int)threshold;
            else if (threshold <= 65535)
                thresholdBytes = threshold.ByteArrayFromNumber(threshold.MinBytesFor()).PadLeft(2);
            else
                throw new Exception("Threshold too large");

            if (nested.Length > 0xFFFFFF)
                throw new Exception("Nested tree too large");

            return ByteArrayExtensions.ConcatenateByteArrays(
                flag.ByteArrayFromNumber(flag.MinBytesFor()),
                weightBytes,
                thresholdBytes,
                nested.Length.ByteArrayFromNumber(nested.Length.MinBytesFor()).PadLeft(3),
                nested);
        }

        public override byte[] HashConfiguration()
        {
            byte[] prefix = Encoding.UTF8.GetBytes("Sequence nested config:\n");
            byte[] treeHash = tree.HashConfiguration();
            byte[] threshold = this.threshold.ByteArrayFromNumber().PadLeft(32);
            byte[] weight = this.weight.ByteArrayFromNumber().PadLeft(32);
                
            return SequenceCoder.KeccakHash(ByteArrayExtensions.ConcatenateByteArrays(prefix, treeHash, threshold, weight));
        }
    }
}