using Sequence.Utils;

namespace Sequence.EcosystemWallet.Primitives
{
    public class SessionNodeLeaf : INode
    {
        public byte[] Value { get; set; }

        public object ToJson()
        {
            return Value.ByteArrayToHexStringWithPrefix();
        }

        public byte[] Encode()
        {
            var flag  = SessionsTopology.FlagNode << 4;
            return ByteArrayExtensions.ConcatenateByteArrays(flag.ByteArrayFromNumber(flag.MinBytesFor()), Value);
        }

        public byte[] EncodeRaw()
        {
            return Value;
        }

        public SessionsTopology ToTopology()
        {
            return new SessionsTopology(this);
        }
    }
}