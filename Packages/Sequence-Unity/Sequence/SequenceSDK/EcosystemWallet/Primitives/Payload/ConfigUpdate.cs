using System;
using System.Collections.Generic;
using System.Linq;
using Sequence.ABI;
using Sequence.Utils;
using UnityEngine.Scripting;

namespace Sequence.EcosystemWallet.Primitives
{
    [Serializable]
    public class ConfigUpdate : Payload
    {
        public static readonly Dictionary<string, NamedType[]> Types = new Dictionary<string, NamedType[]>
        {
            ["ConfigUpdate"] = new[]
            {
                new NamedType("imageHash", "bytes32"),
                new NamedType("wallets", "address[]"),
            }
        };
        
        public override PayloadType type => PayloadType.ConfigUpdate;
        public string imageHash;

        [Preserve]
        public ConfigUpdate(string imageHash)
        {
            this.imageHash = imageHash;
        }
        
        public override byte[] GetEIP712EncodeData()
        {
            byte[] encoded = SequenceCoder.KeccakHash(imageHash.HexStringToByteArray());
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