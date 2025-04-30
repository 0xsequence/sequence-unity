using System;
using System.Numerics;
using UnityEngine.Scripting;

namespace Sequence.EcosystemWallet.Primitives
{
    [Serializable]
    internal class Calls : Payload
    {
        public override PayloadType type => PayloadType.Call;
        public BigInteger space;
        public BigInteger nonce;
        public Call calls;

        [Preserve]
        public Calls(BigInteger space, BigInteger nonce, Call calls)
        {
            this.space = space;
            this.nonce = nonce;
            this.calls = calls;
        }
    }
}