using System;
using Sequence.ABI;
using Sequence.Utils;
using UnityEngine.Scripting;
using System.Linq;

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
            byte[] encoded = SequenceCoder.KeccakHash(digest.HexStringToByteArray());
            return encoded;
        }
        
        public override string ToString()
        {
            string parentWalletsStr = parentWallets != null ? string.Join(", ", parentWallets.Select(w => w.ToString()).ToArray()) : "null";
            return $"Digest {{ digest: {digest}, parentWallets: [{parentWalletsStr}] }}";
        }
        
        internal static Digest FromSolidityEncoding(SolidityDecoded decoded)
        {
            return new Digest(decoded.digest.EnsureHexPrefix());
        }
    }
}