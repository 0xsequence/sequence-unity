using Sequence.ABI;
using Sequence.Utils;

namespace Sequence.EcosystemWallet.Primitives
{
    public static class AddressFactory
    {
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
                                    creationCode.HexStringToByteArray(), 
                                    module.HexStringToByteArray(32))
                            )
                        )
                    )
                    .SubArray(12)
            );
        }
    }
}