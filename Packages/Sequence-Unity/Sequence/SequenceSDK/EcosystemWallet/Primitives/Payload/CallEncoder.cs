using System;
using System.Numerics;
using Sequence.Utils;

namespace Sequence.EcosystemWallet.Primitives
{
    internal class CallEncoder
    {
        private byte[] outBytes;
        private Address to;
        private BigInteger value;
        private byte[] data;
        private BigInteger gasLimit;
        private bool delegateCall;
        private bool onlyFallback;
        private BehaviourOnError behaviorOnError;
        private int flags;

        public CallEncoder(byte[] outBytes, Call call)
        {
            this.outBytes = outBytes;
            this.to = call.to;
            this.value = call.value;
            this.data = call.data;
            this.gasLimit = call.gasLimit;
            this.delegateCall = call.delegateCall;
            this.onlyFallback = call.onlyFallback;
            this.behaviorOnError = call.behaviorOnError;
        }

        public byte[] Encode(Address self)
        {
            flags = SetFlags(self);
            outBytes = ByteArrayExtensions.ConcatenateByteArrays(outBytes, flags.ByteArrayFromNumber(1));
            outBytes = AddToAddress(outBytes);
            outBytes = AddValue(outBytes);
            outBytes = AddData(outBytes);
            outBytes = AddGasLimit(outBytes);
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
        
        private byte[] AddToAddress(byte[] bytes)
        {
            // If toSelf bit not set, store 20-byte address
            if ((flags & 0x01) == 0)
            {
                byte[] toAddressBytes = to.Value.HexStringToByteArray();
                if (toAddressBytes.Length != 20)
                {
                    throw new SystemException($"Invalid '{nameof(to)}' address {to}, must be 20 bytes long but got {toAddressBytes.Length} bytes");
                }
                bytes = ByteArrayExtensions.ConcatenateByteArrays(bytes, toAddressBytes);
            }

            return bytes;
        }
        
        private byte[] AddValue(byte[] bytes)
        {
            // If hasValue bit set, store 32-byte value
            if ((flags & 0x02) == 0x02)
            {
                byte[] valueBytes = value.ByteArrayFromNumber().PadLeft(32);
                bytes = ByteArrayExtensions.ConcatenateByteArrays(bytes, valueBytes);
            }

            return bytes;
        }

        private byte[] AddData(byte[] bytes)
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
                bytes = ByteArrayExtensions.ConcatenateByteArrays(bytes, callDataLengthBytes, data);
            }

            return bytes;
        }

        private byte[] AddGasLimit(byte[] bytes)
        {
            // If hasGasLimit, store 32-byte gas limit
            if ((flags & 0x08) != 0)
            {
                byte[] gasLimitBytes = gasLimit.ByteArrayFromNumber().PadLeft(32);
                bytes = ByteArrayExtensions.ConcatenateByteArrays(bytes, gasLimitBytes);
            }

            return bytes;
        }
    }
}