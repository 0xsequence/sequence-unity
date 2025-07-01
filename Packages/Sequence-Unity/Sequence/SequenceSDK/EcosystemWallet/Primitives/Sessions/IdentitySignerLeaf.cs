using Sequence.Utils;
using UnityEngine;

namespace Sequence.EcosystemWallet.Primitives
{
    public class IdentitySignerLeaf : SessionLeaf
    {
        public Address identitySigner;

        public override object ToJson()
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
            Debug.Log($"IdentitySigner {ByteArrayExtensions.ConcatenateByteArrays(flag.ByteArrayFromNumber(flag.MinBytesFor()), identitySigner.Value.HexStringToByteArray()).ByteArrayToHexStringWithPrefix()}");
            return ByteArrayExtensions.ConcatenateByteArrays(flag.ByteArrayFromNumber(flag.MinBytesFor()),
                identitySigner.Value.HexStringToByteArray());
        }
        
        public override byte[] EncodeGeneric()
        {
            return ByteArrayExtensions.ConcatenateByteArrays(
                SessionsTopology.FlagIdentitySigner.ByteArrayFromNumber(1), 
                identitySigner.Value.HexStringToByteArray(20));
        }
    }
}