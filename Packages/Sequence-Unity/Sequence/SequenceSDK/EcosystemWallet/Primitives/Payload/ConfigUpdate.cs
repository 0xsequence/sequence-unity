using System;
using System.Linq;
using Sequence.ABI;
using Sequence.Utils;
using UnityEngine.Scripting;

namespace Sequence.EcosystemWallet.Primitives
{
    [Serializable]
    public class ConfigUpdate : Payload
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
            string asHex = encoded.ByteArrayToHexStringWithPrefix();
            return encoded;
        }
        
        public override string ToString()
        {
            string parentWalletsStr = parentWallets != null ? string.Join(", ", parentWallets.Select(w => w.ToString()).ToArray()) : "null";
            return $"ConfigUpdate {{ imageHash: {imageHash}, parentWallets: [{parentWalletsStr}] }}";
        }
        
        internal static ConfigUpdate FromSolidityEncoding(SolidityDecoded decoded)
        {
            return new ConfigUpdate(decoded.imageHash.EnsureHexPrefix());
        }
    }
}