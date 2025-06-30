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
            return Value;
        }
    }
}