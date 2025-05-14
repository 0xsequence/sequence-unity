using System;
using System.Numerics;
using Sequence.Utils;

namespace Sequence.EcosystemWallet.Primitives
{
    [Serializable]
    internal class EncodeSapient
    {
        public int kind;
        public bool noChainId;
        public EncodedCall[] calls;
        public BigInteger space;
        public BigInteger nonce;
        public string message;
        public string imageHash;
        public string digest;
        public Address[] parentWallets;

        public class EncodedCall
        {
            public BigInteger gasLimit;
            public bool delegateCall;
            public string data;
            public bool onlyFallback;
            public Address to;
            public BigInteger value;
            public BigInteger behaviorOnError;

            public EncodedCall(Call call)
            {
                to = call.to;
                value = call.value;
                data = call.data.ByteArrayToHexStringWithPrefix();
                gasLimit = call.gasLimit;
                delegateCall = call.delegateCall;
                onlyFallback = call.onlyFallback;
                behaviorOnError = (int)call.behaviorOnError;
            }
        }
    }
}