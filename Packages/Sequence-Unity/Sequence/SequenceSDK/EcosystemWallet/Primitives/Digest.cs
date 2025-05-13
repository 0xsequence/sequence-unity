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
            byte[] encoded = new FixedBytesCoder().Encode(new FixedByte(32, digest.HexStringToByteArray()));
            return encoded;
        }
    }
}