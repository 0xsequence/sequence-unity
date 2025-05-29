using System;
using Sequence.ABI;
using Sequence.Utils;
using UnityEngine.Scripting;

namespace Sequence.EcosystemWallet.Primitives
{
    [Serializable]
    internal class Digest : Payload
    {
        public override PayloadType type => PayloadType.Digest;
        public string digest;

        [Preserve]
        public Digest(string digest)
        {
            this.digest = digest;
        }
        
        public override byte[] GetEIP712EncodeData()
        {
            FixedByte bytes = new FixedByte(32, digest.HexStringToByteArray());
            byte[] encoded = new FixedBytesCoder().Encode(bytes);
            return encoded;
        }
        
        internal static Digest FromSolidityEncoding(SolidityDecoded decoded)
        {
            return new Digest(decoded.digest.EnsureHexPrefix());
        }
    }
}