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
                var checkpointerBytes = configuration.checkpointer.Value.HexStringToByteArray();
                output = ByteArrayExtensions.ConcatenateByteArrays(output, checkpointerBytes.PadLeft(20));
                Debug.Log(checkpointerBytes.ByteArrayToHexStringWithPrefix());
                
                if (!skipCheckpointerData)
                {
                    int checkpointerDataSize = checkpointerData?.Length ?? 0;
                    if (checkpointerDataSize > 16777215)
                        throw new Exception("Checkpointer data too large");

                    output = ByteArrayExtensions.ConcatenateByteArrays(output, checkpointerData ?? new byte[0]);
                }
            }

            var checkpointBytes = configuration.checkpoint.ByteArrayFromNumber(bytesForCheckpoint);
            output = ByteArrayExtensions.ConcatenateByteArrays(output, checkpointBytes);

            var thresholdBytes = configuration.threshold.ByteArrayFromNumber(bytesForThreshold);
            output = ByteArrayExtensions.ConcatenateByteArrays(output, thresholdBytes);
            
            var topologyBytes = configuration.topology.Encode(noChainId, checkpointerData);
            output = ByteArrayExtensions.ConcatenateByteArrays(output, topologyBytes);

            return erc6492 != null ? Erc6492Helper.Wrap(output, erc6492.to, erc6492.data) : output;
        }

        public static RawSignature Decode(byte[] erc6492Signature)
        {
            var (signature, erc6492) = Erc6492Helper.Decode(erc6492Signature);
            
            if (signature.Length < 1)
                throw new Exception("Signature is empty");

            int index = 1;
            byte flag = signature[0];
            bool noChainId = (flag & 0x02) == 0x02;

            Address checkpointerAddress = null;
            byte[]? checkpointerData = null;

            if ((flag & 0x40) == 0x40)
            {
                if (index + 20 > signature.Length)
                    throw new Exception("Not enough bytes for checkpointer address");

                var checkpointer = signature[index..(index + 20)].ByteArrayToHexStringWithPrefix();
                checkpointerAddress = new Address(checkpointer);
                index += 20;

                if (index + 3 > signature.Length)
                    throw new Exception("Not enough bytes for checkpointer data size");

                var dataSize = signature[index..(index + 3)].ToInteger();
                index += 3;

                if (index + dataSize > signature.Length)
                    throw new Exception("Not enough bytes for checkpointer data");

                checkpointerData = signature[index..(index + dataSize)];
                index += dataSize;
            }

            int checkpointSize = (flag & 0x1C) >> 2;
            if (index + checkpointSize > signature.Length)
                throw new Exception("Not enough bytes for checkpoint");

            var checkpoint = signature[index..(index + checkpointSize)].ToBigInteger();
            index += checkpointSize;

            int thresholdSize = ((flag & 0x20) >> 5) + 1;
            if (index + thresholdSize > signature.Length)
                throw new Exception("Not enough bytes for threshold");

            var threshold = signature[index..(index + thresholdSize)].ToBigInteger();
            index += thresholdSize;

            if ((flag & 0x01) == 0x01)
            {
                var subsignatures = new List<RawSignature>();

                while (index < signature.Length)
                {
                    if (index + 3 > signature.Length)
                        throw new Exception("Not enough bytes for chained subsignature size");

                    int subSize = signature[index..(index + 3)].ToInteger();
                    index += 3;

                    if (index + subSize > signature.Length)
                        throw new Exception("Not enough bytes for chained subsignature");

                    var subSignature = Decode(signature[index..(index + subSize)]);
                    index += subSize;

                    if (subSignature.checkpointerData != null)
                        throw new Exception("Chained subsignature has checkpointer data");

                    subSignature.checkpointerData = null;
                    subsignatures.Add(subSignature);
                }

                if (subsignatures.Count == 0)
                    throw new Exception("Chained signature has no subsignatures");

                return new RawSignature
                {
                    noChainId = subsignatures[0].noChainId,
                    checkpointerData = null,
                    configuration = subsignatures[0].configuration,
                    suffix = subsignatures.GetRange(1, subsignatures.Count - 1).ToArray(),
                    erc6492 = erc6492.HasValue ? new Erc6492
                    {
                        to = erc6492.Value.To,
                        data = erc6492.Value.Data
                    } : null
                };
            }

            var (nodes, leftover) = SignatureUtils.ParseBranch(signature[index..]);
            if (leftover.Length != 0)
                throw new Exception("Leftover bytes in signature");

            var topology = SignatureUtils.FoldNodes(nodes);

            return new RawSignature
            {
                noChainId = noChainId,
                checkpointerData = checkpointerData,
                configuration = new Config
                {
                    threshold = threshold,
                    checkpoint = checkpoint,
                    topology = topology,
                    checkpointer = checkpointerAddress
                },
                erc6492 = erc6492.HasValue ? new Erc6492
                {
                    to = erc6492.Value.To,
                    data = erc6492.Value.Data
                } : null
            };
        }
    }
}