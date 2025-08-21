using Sequence.Utils;

namespace Sequence.EcosystemWallet.Primitives
{
    public class IdentitySignerLeaf : SessionLeaf
    {
        private struct Json
        {
            public string type;
            public string identitySigner;
        }
        
        public Address identitySigner;

        public override object ToJsonObject()
        {
            return new Json
            {
                type = IdentitySignerType,
                identitySigner = identitySigner.Value,
            };
        }

        public override byte[] Encode()
        {
            var flag = SessionsTopology.FlagIdentitySigner << 4;
            return ByteArrayExtensions.ConcatenateByteArrays(flag.ByteArrayFromNumber(flag.MinBytesFor()),
                identitySigner.Value.HexStringToByteArray());
        }
        
        public override byte[] EncodeRaw()
        {
            return ByteArrayExtensions.ConcatenateByteArrays(
                SessionsTopology.FlagIdentitySigner.ByteArrayFromNumber(1), 
                identitySigner.Value.HexStringToByteArray(20));
        }
    }
}