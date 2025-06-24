using System;
using System.Collections.Generic;
using System.Linq;
using Sequence.Utils;

namespace Sequence.EcosystemWallet.Primitives
{
    public class RawSignature
    {
        public bool noChainId;
        public byte[] checkpointerData;
        public RawConfig configuration;
        public RawSignature[] suffix;
        public Erc6492 erc6492;

        public class Erc6492
        {
            public Address to;
            public byte[] data;
        }

        public byte[] Encode(bool skipCheckpointerData = false, bool skipCheckpointerAddress = false)
        {
            if (suffix is { Length: > 0 })
            {
                var head = new RawSignature
                {
                    noChainId = this.noChainId,
                    checkpointerData = this.checkpointerData,
                    configuration = this.configuration,
                    erc6492 = this.erc6492
                };

                var chained = new List<RawSignature> { head };
                chained.AddRange(suffix);

                var chainedSignature = new ChainedSignature(chained.ToArray());
                return chainedSignature.Encode();
            }

            byte flag = 0;
            if (noChainId)
                flag |= 0x02;

            int bytesForCheckpoint = configuration.checkpoint.MinimumBytesNeeded();
            if (bytesForCheckpoint > 7)
                throw new Exception("Checkpoint too large");
            
            flag |= (byte)(bytesForCheckpoint << 2);

            int bytesForThreshold = configuration.threshold.MinimumBytesNeeded();
            bytesForThreshold = bytesForThreshold == 0 ? 1 : bytesForThreshold;
            if (bytesForThreshold > 2)
                throw new Exception("Threshold too large");

            flag |= bytesForThreshold == 2 ? (byte)0x20 : (byte)0x00;

            if (!string.IsNullOrEmpty(configuration.checkpointer) && !skipCheckpointerAddress)
            {
                flag |= 0x40;
            }

            byte[] output = new byte[] { flag };

            if (!string.IsNullOrEmpty(configuration.checkpointer) && !skipCheckpointerAddress)
            {
                output = output.Concat(configuration.checkpointer.Value.ToByteArray().PadLeft(20)).ToArray();
                if (!skipCheckpointerData)
                {
                    int checkpointerDataSize = checkpointerData?.Length ?? 0;
                    if (checkpointerDataSize > 16777215)
                        throw new Exception("Checkpointer data too large");

                    output = output.Concat(checkpointerData ?? new byte[0]).ToArray();
                }
            }

            var checkpointBytes = configuration.checkpoint.ToByteArray().PadLeft(bytesForCheckpoint);
            output = output.Concat(checkpointBytes).ToArray();

            var thresholdBytes = configuration.threshold.ToByteArray().PadLeft(bytesForThreshold);
            output = output.Concat(thresholdBytes).ToArray();
            
            // TODO: 
            /*
            var topologyBytes = EncodeTopology(configuration.topology, signature);
            output = Bytes.Concat(output, topologyBytes);

            return erc6492 != null ? Wrap(output, erc6492) : output;*/
            
            return output;
        }

        public static RawSignature Decode(byte[] erc6492Signature)
        {
            // TODO: 
            return new RawSignature();
        }
    }
}