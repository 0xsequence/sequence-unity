using System;
using System.Linq;
using Sequence.ABI;
using Sequence.Utils;
using UnityEngine.Scripting;

namespace Sequence.EcosystemWallet.Primitives
{
    [Serializable]
    public class Message : Payload
    {
        public override PayloadType type => PayloadType.Message;
        public byte[] message;

        [Preserve]
        public Message(byte[] message)
        {
            this.message = message;
        }
        
        public override byte[] GetEIP712EncodeData()
        {
            byte[] encoded = SequenceCoder.KeccakHash(message);
            return encoded;
        }
        
        public override string ToString()
        {
            string messageHex = message != null ? message.ByteArrayToHexStringWithPrefix() : "null";
            string parentWalletsStr = parentWallets != null ? string.Join(", ", parentWallets.Select(w => w.ToString()).ToArray()) : "null";
            return $"Message {{ message: {messageHex}, parentWallets: [{parentWalletsStr}] }}";
        }
        
        internal static Message FromSolidityEncoding(SolidityDecoded decoded)
        {
            return new Message(decoded.message.HexStringToByteArray());
        }
    }
}