using System;
using Sequence.ABI;
using Sequence.Utils;

namespace Sequence.EcosystemWallet.Primitives
{
    public static class AddressFactory
    {
        private static byte[] DefaultCreationCode =
            "0x6041600e3d396021805130553df33d3d36153402601f57363d3d373d363d30545af43d82803e903d91601f57fd5bf3"
                .HexStringToByteArray();
        
        public static Address Create(Topology topology, string factory, string module, string creationCode = null)
        {
            return Create(topology.HashConfiguration(), factory, module, creationCode);
        }
        
        public static Address Create(byte[] imageHash, string factory, string module, string creationCode = null)
        {
            return new Address(
                SequenceCoder.KeccakHash(
                        ByteArrayExtensions.ConcatenateByteArrays("0xff".HexStringToByteArray(),
                            factory.HexStringToByteArray(),
                            imageHash,
                            SequenceCoder.KeccakHash(
                                ByteArrayExtensions.ConcatenateByteArrays(
                                    creationCode?.HexStringToByteArray() ?? DefaultCreationCode, 
                                    module.HexStringToByteArray(32))
                            )
                        )
                    )
                    .SubArray(12)
            );
        }
    }
}