using System;
using System.Collections.Generic;
using Sequence.EcosystemWallet.Utils;
using Sequence.Utils;
using UnityEngine;

namespace Sequence.EcosystemWallet.Primitives
{
    public class RawSignature
    {
        public class Erc6492
        {
            public Address to;
            public byte[] data;
        }
        
        public bool noChainId;
        public byte[] checkpointerData;
        public Config configuration;
        public RawSignature[] suffix;
        public Erc6492 erc6492;

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

            int bytesForCheckpoint = configuration.checkpoint.MinBytesFor();
            Debug.Log($"{configuration.checkpoint} - {bytesForCheckpoint}");
            
            if (bytesForCheckpoint > 7)
                throw new Exception("Checkpoint too large");
            
            flag |= (byte)(bytesForCheckpoint << 2);

            int bytesForThreshold = configuration.threshold.MinBytesFor();
            bytesForThreshold = bytesForThreshold == 0 ? 1 : bytesForThreshold;
            if (bytesForThreshold > 2)
                throw new Exception("Threshold too large");

            flag |= bytesForThreshold == 2 ? (byte)0x20 : (byte)0x00;

            if (configuration.checkpointer != null && !skipCheckpointerAddress)
            {
                flag |= 0x40;
            }

            byte[] output = new byte[] { flag };

            if (configuration.checkpointer != null && !skipCheckpointerAddress)
            {
                output = ByteArrayExtensions.ConcatenateByteArrays(output, configuration.checkpointer.Value.ToByteArray().PadLeft(20));
                
                if (!skipCheckpointerData)
                {
                    int checkpointerDataSize = checkpointerData?.Length ?? 0;
                    if (checkpointerDataSize > 16777215)
                        throw new Exception("Checkpointer data too large");

                    output = ByteArrayExtensions.ConcatenateByteArrays(output, checkpointerData ?? new byte[0]);
                }
            }

            var checkpointBytes = configuration.checkpoint.ToByteArray().PadLeft(bytesForCheckpoint);
            output = ByteArrayExtensions.ConcatenateByteArrays(output, checkpointBytes);

            var thresholdBytes = configuration.threshold.ToByteArray().PadLeft(bytesForThreshold);
            output = ByteArrayExtensions.ConcatenateByteArrays(output, thresholdBytes);
            
            var topologyBytes = configuration.topology.Encode(noChainId, checkpointerData);
            output = ByteArrayExtensions.ConcatenateByteArrays(output, topologyBytes);

            return erc6492 != null ? Erc6492Helper.Wrap(output, erc6492.to, erc6492.data) : output;
        }

        public static RawSignature Decode(byte[] erc6492Signature)
        {
            // TODO: 
            return new RawSignature();
        }
    }
}