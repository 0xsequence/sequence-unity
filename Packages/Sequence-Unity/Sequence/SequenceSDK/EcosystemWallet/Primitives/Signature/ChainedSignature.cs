using System;
using System.Linq;
using Sequence.Utils;

namespace Sequence.EcosystemWallet.Primitives
{
    public class ChainedSignature
    {
        public RawSignature[] signatures;
        
        public ChainedSignature(RawSignature[] signatures)
        {
            this.signatures = signatures;
        }

        public byte[] Encode()
        {
            if (signatures == null || signatures.Length == 0)
                throw new ArgumentException("Signatures list cannot be empty.");

            byte flag = 0x01;

            var sigForCheckpointer = signatures[^1];
            if (sigForCheckpointer.configuration.checkpointer != null)
            {
                flag |= 0x40;
            }

            byte[] output = new byte[] { flag };

            if (sigForCheckpointer.configuration.checkpointer != null)
            {
                output = sigForCheckpointer.configuration.checkpointer.Value.HexStringToByteArray().PadLeft(20).Concat(output).ToArray();
                output = ByteArrayExtensions.ConcatenateByteArrays(
                    sigForCheckpointer.configuration.checkpointer.Value.HexStringToByteArray().PadLeft(20),
                    output);
                
                var checkpointerDataSize = sigForCheckpointer.checkpointerData?.Length ?? 0;
                if (checkpointerDataSize > 16777215)
                    throw new Exception("Checkpointer data too large");

                output = ByteArrayExtensions.ConcatenateByteArrays(output,
                    sigForCheckpointer.checkpointerData ?? new byte[0]);
            }

            for (int i = 0; i < signatures.Length; i++)
            {
                var signature = signatures[i];
                bool isLast = i == signatures.Length - 1;

                var encoded = signature.Encode(skipCheckpointerData: true, skipCheckpointerAddress: isLast);
                if (encoded.Length > 16777215)
                    throw new Exception("Chained signature too large");

                output = ByteArrayExtensions.ConcatenateByteArrays(output, encoded);
            }

            return output;
        }
    }
}