using Sequence.ABI;
using Sequence.Utils;

namespace Sequence.EcosystemWallet.Primitives.Passkeys
{
    public class PublicKey
    {
        public string x;
        public string y;
        public bool requireUserVerification;
        public PasskeyMetadata metadata;

        public string Hash()
        {
            var a = x.HexStringToByteArray(32);
            var b = y.HexStringToByteArray(32);
            var c = (requireUserVerification ? "0x01" : "0x00").HexStringToByteArray(32);
            var d = metadata.Encode();

            return SequenceCoder.KeccakHash(
                ByteArrayExtensions.ConcatenateByteArrays(
                    SequenceCoder.KeccakHash(ByteArrayExtensions.ConcatenateByteArrays(
                        a, b)), 
                    SequenceCoder.KeccakHash(ByteArrayExtensions.ConcatenateByteArrays(
                        c, d))
                )
            ).ByteArrayToHexStringWithPrefix();
        }
    }
}