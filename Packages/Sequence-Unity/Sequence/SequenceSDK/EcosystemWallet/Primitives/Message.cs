using System;
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
    }
}