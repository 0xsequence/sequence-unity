using System;
using System.Numerics;
using Sequence.ABI;
using Sequence.Utils;
using UnityEngine.Scripting;
using StringExtensions = Sequence.Utils.StringExtensions;

namespace Sequence.EcosystemWallet.Primitives
{
    [Serializable]
    public class Call
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
        
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is Call))
            {
                return false;
            }
            
            Call call = (Call)obj;
            return to.Equals(call.to) &&
                   value == call.value &&
                   data.Equals(call.data) &&
                   gasLimit == call.gasLimit &&
                   delegateCall == call.delegateCall &&
                   onlyFallback == call.onlyFallback &&
                   behaviorOnError == call.behaviorOnError;
        }
        
        public string Hash()
        {
            byte[] typeHash = new StaticBytesCoder().Encode(CALL_TYPEHASH);
            byte[] toHash = new AddressCoder().Encode(to);
            byte[] valueHash = new NumberCoder().Encode(value);
            byte[] dataHash = new StaticBytesCoder().Encode(data);
            byte[] gasLimitHash = new NumberCoder().Encode(gasLimit);
            byte[] delegateCallHash = new BooleanCoder().Encode(delegateCall);
            byte[] onlyFallbackHash = new BooleanCoder().Encode(onlyFallback);
            byte[] behaviorOnErrorHash = new NumberCoder().Encode((int)behaviorOnError);

            byte[] encoded = ByteArrayExtensions.ConcatenateByteArrays(typeHash, toHash, valueHash, dataHash,
                gasLimitHash, delegateCallHash, onlyFallbackHash, behaviorOnErrorHash);

            return SequenceCoder.KeccakHash(encoded).ByteArrayToHexStringWithPrefix();
        }

        public byte[] HashStruct()
        {
            return Hash().HexStringToByteArray();
        }
    }
}