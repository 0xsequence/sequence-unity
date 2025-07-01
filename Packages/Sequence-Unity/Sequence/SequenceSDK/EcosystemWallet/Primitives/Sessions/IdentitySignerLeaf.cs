using Sequence.Utils;

namespace Sequence.EcosystemWallet.Primitives
{
    public class IdentitySignerLeaf : SessionLeaf
    {
        public Address identitySigner;

        public override object ToJsonObject()
        {
            return new
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