using Sequence.Utils;

namespace Sequence.EcosystemWallet.Primitives
{
    public class SessionNodeLeaf : SessionLeaf
    {
        public byte[] Value;

        public override object ToJson()
        {
            return Value.ByteArrayToHexStringWithPrefix();
        }

        public override byte[] Encode()
        {
            var flag  = SessionsTopology.FlagNode << 4;
            return ByteArrayExtensions.ConcatenateByteArrays(flag.ByteArrayFromNumber(flag.MinBytesFor()), Value);
        }

        public override byte[] EncodeGeneric()
        {
            return Value;
        }
    }
}