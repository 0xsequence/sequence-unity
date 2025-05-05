using System;
using System.Numerics;
using Sequence.ABI;
using Sequence.Utils;
using UnityEngine.Scripting;
using StringExtensions = Sequence.Utils.StringExtensions;

namespace Sequence.EcosystemWallet.Primitives
{
    [Serializable]
    internal class Call
    {
        public static readonly byte[] CALL_TYPEHASH = SequenceCoder.KeccakHash(
            StringExtensions.ToByteArray(
                "Call(address to,uint256 value,bytes data,uint256 gasLimit,bool delegateCall,bool onlyFallback,uint256 behaviorOnError)"));
        
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
        
        public string Hash()
        {
            byte[] encoded = AbiParameters.Encode(
                new[]
                {
                    "bytes32",
                    "address",
                    "uint256",
                    "bytes32",
                    "uint256",
                    "bool",
                    "bool",
                    "uint256"
                },
                new object[]
                {
                    CALL_TYPEHASH,
                    to,
                    value,
                    SequenceCoder.KeccakHash(data),
                    gasLimit,
                    delegateCall,
                    onlyFallback,
                    (int)behaviorOnError
                }
            );

            return SequenceCoder.KeccakHash(encoded).ByteArrayToHexStringWithPrefix();
        }
    }
}