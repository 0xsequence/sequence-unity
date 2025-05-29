using System;
using Sequence.ABI;
using Sequence.Utils;
using UnityEngine.Scripting;

namespace Sequence.EcosystemWallet.Primitives
{
    [Serializable]
    internal class ConfigUpdate : Payload
    {
        public override PayloadType type => PayloadType.ConfigUpdate;
        public string imageHash;

        [Preserve]
        public ConfigUpdate(string imageHash)
        {
            this.imageHash = imageHash;
        }
        
        public override byte[] GetEIP712EncodeData()
        {
            FixedByte bytes = new FixedByte(32, imageHash.HexStringToByteArray());
            byte[] encoded = new FixedBytesCoder().Encode(bytes);
            return encoded;
        }
        
        internal static ConfigUpdate FromSolidityEncoding(SolidityDecoded decoded)
        {
            return new ConfigUpdate(decoded.imageHash.EnsureHexPrefix());
        }
    }
}