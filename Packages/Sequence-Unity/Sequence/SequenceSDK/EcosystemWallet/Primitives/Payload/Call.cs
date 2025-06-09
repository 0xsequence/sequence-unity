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
            bool toEqual = to.Equals(call.to);
            int dataLength = data.Length;
            bool dataEqual = dataLength == call.data.Length;
            if (dataEqual)
            {
                for (int i = 0; i < dataLength; i++)
                {
                    dataEqual &= data[i] == call.data[i];
                }
            }
            return toEqual &&
                   value == call.value &&
                   dataEqual &&
                   gasLimit == call.gasLimit &&
                   delegateCall == call.delegateCall &&
                   onlyFallback == call.onlyFallback &&
                   behaviorOnError == call.behaviorOnError;
        }
        
        public override string ToString()
        {
            string dataHex = data != null ? data.ByteArrayToHexStringWithPrefix() : "null";
            // Truncate data if it's too long for display
            if (dataHex != null && dataHex.Length > 50)
            {
                dataHex = dataHex.Substring(0, 47) + "...";
            }
            
            return $"Call {{ to: {to}, value: {value}, data: {dataHex}, gasLimit: {gasLimit}, " +
                   $"delegateCall: {delegateCall}, onlyFallback: {onlyFallback}, behaviorOnError: {behaviorOnError} }}";
        }

        internal byte[] Encode(byte[] outBytes, Address self)
        {
            int flags = SetFlags(self);
            outBytes = ByteArrayExtensions.ConcatenateByteArrays(outBytes, flags.ByteArrayFromNumber(1));
            outBytes = SetToAddress(outBytes, flags);
            outBytes = SetHasValue(outBytes, flags);
            outBytes = SetData(outBytes, flags);
            outBytes = SetGasLimit(outBytes, flags);
            return outBytes;
        }
        
        /// <summary>
        /// Set flags in this format
        ///     call flags layout (1 byte):
        ///     bit 0 => toSelf (call.to == this)
        ///     bit 1 => hasValue (call.value != 0)
        ///     bit 2 => hasData (call.data.length > 0)
        ///     bit 3 => hasGasLimit (call.gasLimit != 0)
        ///     bit 4 => delegateCall
        ///     bit 5 => onlyFallback
        ///     bits [6..7] => behaviorOnError => 0=ignore, 1=revert, 2=abort
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        private int SetFlags(Address self)
        {
            int flags = 0;
            if (self != null && self.Equals(to))
            {
                flags |= 0x01;
            }
                
            if (value != BigInteger.Zero)
            {
                flags |= 0x02;
            }
                
            if (data != null && data.Length > 0)
            {
                flags |= 0x04;
            }
                
            if (gasLimit != BigInteger.Zero)
            {
                flags |= 0x08;
            }
                
            if (delegateCall)
            {
                flags |= 0x10;
            }

            if (onlyFallback)
            {
                flags |= 0x20;
            }

            flags |= (int)behaviorOnError << 6;
            
            return flags;
        }

        private byte[] SetToAddress(byte[] outBytes, int flags)
        {
            // If toSelf bit not set, store 20-byte address
            if ((flags & 0x01) == 0)
            {
                byte[] toAddressBytes = to.Value.HexStringToByteArray();
                if (toAddressBytes.Length != 20)
                {
                    throw new SystemException($"Invalid '{nameof(to)}' address {to}, must be 20 bytes long but got {toAddressBytes.Length} bytes");
                }
                outBytes = ByteArrayExtensions.ConcatenateByteArrays(outBytes, toAddressBytes);
            }

            return outBytes;
        }

        private byte[] SetHasValue(byte[] outBytes, int flags)
        {
            // If hasValue, store 32 bytes of value
            if ((flags & 0x02) != 0)
            {
                byte[] valueBytes = value.ByteArrayFromNumber().PadLeft(32);
                outBytes = ByteArrayExtensions.ConcatenateByteArrays(outBytes, valueBytes);
            }

            return outBytes;
        }

        private byte[] SetData(byte[] outBytes, int flags)
        {
            // If hasData, store 3 bytes of data length + data
            if ((flags & 0x04) != 0)
            {
                if (data == null)
                {
                    throw new SystemException($"If ({nameof(flags)} & 0x04) != 0, then {nameof(data)} cannot be null");
                }
                int callDataLength = data!.Length;
                if (callDataLength > 0xffffff)
                {
                    throw new ArgumentException($"{nameof(data)} is too large, length cannot exceed 0xffffff, given {callDataLength.ToHexadecimal()}");
                }
                // 3 bytes => up to 16,777,215
                byte[] callDataLengthBytes = callDataLength.ByteArrayFromNumber(3);
                outBytes = ByteArrayExtensions.ConcatenateByteArrays(outBytes, callDataLengthBytes, data);
            }

            return outBytes;
        }
        
        private byte[] SetGasLimit(byte[] outBytes, int flags)
        {
            // If hasGasLimit, store 32 bytes of gasLimit
            if ((flags & 0x08) != 0)
            {
                byte[] gasBytes = gasLimit.ByteArrayFromNumber().PadLeft(32);
                outBytes = ByteArrayExtensions.ConcatenateByteArrays(outBytes, gasBytes);
            }
            return outBytes;
        }
        
        public string Hash()
        {
            byte[] typeHash = new StaticBytesCoder().Encode(CALL_TYPEHASH);
            byte[] toHash = new AddressCoder().Encode(to);
            byte[] valueHash = new NumberCoder().Encode(value);
            byte[] dataHash = SequenceCoder.KeccakHash(data);
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