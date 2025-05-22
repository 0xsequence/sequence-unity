using System;
using Sequence.ABI;
using Sequence.Utils;
using UnityEngine.Scripting;

namespace Sequence.EcosystemWallet.Primitives
{
    [Serializable]
    internal class Message : Payload
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
            byte[] encoded = new BytesCoder().Encode(message);
            return encoded;
        }
        
        internal static Message FromSolidityEncoding(SolidityDecoded decoded)
        {
            return new Message(decoded.message.HexStringToByteArray());
        }
    }
}