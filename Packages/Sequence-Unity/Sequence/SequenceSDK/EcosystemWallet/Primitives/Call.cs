using System;
using System.Numerics;
using UnityEngine.Scripting;

namespace Sequence.EcosystemWallet.Primitives
{
    [Serializable]
    internal class Call
    {
        public Address to;
        public BigInteger value;
        public byte[] data;
        public BigInteger gasLimit;
        public bool delegateCall;
        public bool onlyFallback;
        public BehaviourOnError behaviorOnError;

        [Preserve]
        public Call(Address to, BigInteger value, byte[] data, BigInteger gasLimit, bool delegateCall, bool onlyFallback, BehaviourOnError behaviorOnError)
        {
            this.to = to;
            this.value = value;
            this.data = data;
            this.gasLimit = gasLimit;
            this.delegateCall = delegateCall;
            this.onlyFallback = onlyFallback;
            this.behaviorOnError = behaviorOnError;
        }
    }
}