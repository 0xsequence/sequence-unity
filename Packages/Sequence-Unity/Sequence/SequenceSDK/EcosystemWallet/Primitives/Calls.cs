using System;
using System.Collections.Generic;
using System.Numerics;
using Sequence.ABI;
using Sequence.Utils;
using UnityEngine.Scripting;

namespace Sequence.EcosystemWallet.Primitives
{
    [Serializable]
    internal class Calls : Payload
    {
        public const uint MaxNonceBytes = 15;
        public const uint MaxCalls = 65536;
        
        public override PayloadType type => PayloadType.Call;

        public BigInteger space;
        public BigInteger nonce;
        public Call[] calls;

        [Preserve]
        public Calls(BigInteger space, BigInteger nonce, Call[] calls)
        {
            this.space = space;
            this.nonce = nonce;
            this.calls = calls;
        }
        
        public override byte[] GetEIP712EncodeData()
        {
            byte[] spaceEncoded = new NumberCoder().Encode(space);
            byte[] nonceEncoded = new NumberCoder().Encode(nonce);
            byte[] callsEncoded = new byte[] { };
            foreach (var call in calls)
            {
                callsEncoded = ByteArrayExtensions.ConcatenateByteArrays(callsEncoded, call.HashStruct());
            }
            callsEncoded = SequenceCoder.KeccakHash(callsEncoded);
            byte[] encoded = ByteArrayExtensions.ConcatenateByteArrays(
                spaceEncoded,
                nonceEncoded,
                callsEncoded
            );
            return encoded;
        }

        // Todo: once we're able to test the following code, let's refactor it into separate classes and methods instead of giant methods
        
        public byte[] Encode(Address self = null)
        {
            int callsLength = calls.Length;
            int nonceBytesNeeded = nonce.MinimumBytesNeeded();
            if (nonceBytesNeeded > MaxNonceBytes)
            {
                throw new ArgumentException(
                    $"{nameof(nonce)} is too large, need {nonceBytesNeeded} bytes to represent it, but max is {MaxNonceBytes}");
            }

            /*
                globalFlag layout:
                  bit 0: spaceZeroFlag => 1 if space == 0, else 0
                  bits [1..3]: how many bytes we use to encode nonce
                  bit 4: singleCallFlag => 1 if there's exactly one call
                  bit 5: callsCountSizeFlag => 1 if #calls stored in 2 bytes, 0 if in 1 byte
                  (bits [6..7] are unused/free)
              */
            int globalFlag = 0;
            
            if (space == BigInteger.Zero)
            {
                globalFlag |= 0x01;
            }

            // bits [1..3] => how many bytes for the nonce
            globalFlag |= nonceBytesNeeded << 1;

            // bit [4] => singleCallFlag
            if (callsLength == 1)
            {
                globalFlag |= 0x10;
            }

            /*
              If there's more than one call, we decide if we store the #calls in 1 or 2 bytes.
              bit [5] => callsCountSizeFlag: 1 => 2 bytes, 0 => 1 byte
            */
            int callsCountSize = 0;
            if (callsLength != 1)
            {
                if (callsLength < 256)
                {
                    callsCountSize = 1;
                }
                else if (callsLength < MaxCalls)
                {
                    callsCountSize = 2;
                    globalFlag |= 0x20;
                }
                else
                {
                    throw new ArgumentException($"{calls} is too large, cannot have more than {MaxCalls} calls, given {callsLength}");
                }
            }
            
            // Start building the output
            // We'll accumulate in a Bytes object as we go
            byte[] outBytes = globalFlag.ByteArrayFromNumber(1);
            
            // If space isn't 0, store it as exactly 20 bytes (like uint160)
            if (space != BigInteger.Zero)
            {
                byte[] spaceBytes = space.ByteArrayFromNumber().PadLeft(20);
                outBytes = ByteArrayExtensions.ConcatenateByteArrays(outBytes, spaceBytes);
            }
            
            // Encode nonce in nonceBytesNeeded
            if (nonceBytesNeeded > 0)
            {
                // We'll store nonce in exactly nonceBytesNeeded bytes
                byte[] nonceBytes = nonce.ByteArrayFromNumber().PadLeft(nonceBytesNeeded);
                outBytes = ByteArrayExtensions.ConcatenateByteArrays(outBytes, nonceBytes);
            }
            
            // Store callsLength if not single-call
            if (callsLength != 1)
            {
                if (callsCountSize > 2 || callsCountSize <= 0)
                {
                    throw new SystemException(
                        $"If {nameof(callsLength)} != 1, then {callsCountSize} must be 1 or 2, given {callsCountSize}");
                }
                outBytes = ByteArrayExtensions.ConcatenateByteArrays(outBytes, callsLength.ByteArrayFromNumber(callsCountSize));
            }
            
            // Now encode each call
            for (int i = 0; i < callsLength; i++)
            {
                Call call = calls[i];
                /*
                  call flags layout (1 byte):
                    bit 0 => toSelf (call.to == this)
                    bit 1 => hasValue (call.value != 0)
                    bit 2 => hasData (call.data.length > 0)
                    bit 3 => hasGasLimit (call.gasLimit != 0)
                    bit 4 => delegateCall
                    bit 5 => onlyFallback
                    bits [6..7] => behaviorOnError => 0=ignore, 1=revert, 2=abort
                */
                int flags = 0;
                if (self != null && self.Equals(call.to))
                {
                    flags |= 0x01;
                }
                
                if (call.value != BigInteger.Zero)
                {
                    flags |= 0x02;
                }
                
                if (call.data != null && call.data.Length > 0)
                {
                    flags |= 0x04;
                }
                
                if (call.gasLimit != BigInteger.Zero)
                {
                    flags |= 0x08;
                }
                
                if (call.delegateCall)
                {
                    flags |= 0x10;
                }

                if (call.onlyFallback)
                {
                    flags |= 0x20;
                }

                flags |= (int)call.behaviorOnError << 6;

                outBytes = ByteArrayExtensions.ConcatenateByteArrays(outBytes, flags.ByteArrayFromNumber(1));
                
                // If toSelf bit not set, store 20-byte address
                if ((flags & 0x01) == 0)
                {
                    byte[] toAddressBytes = call.to.Value.HexStringToByteArray();
                    if (toAddressBytes.Length != 20)
                    {
                        throw new SystemException($"Invalid '{nameof(call.to)}' address {call.to}, must be 20 bytes long but got {toAddressBytes.Length} bytes");
                    }
                    outBytes = ByteArrayExtensions.ConcatenateByteArrays(outBytes, toAddressBytes);
                }
                
                // If hasValue, store 32 bytes of value
                if ((flags & 0x02) != 0)
                {
                    byte[] valueBytes = call.value.ByteArrayFromNumber().PadLeft(32);
                    outBytes = ByteArrayExtensions.ConcatenateByteArrays(outBytes, valueBytes);
                }
                
                // If hasData, store 3 bytes of data length + data
                if ((flags & 0x04) != 0)
                {
                    if (call.data == null)
                    {
                        throw new SystemException($"If ({nameof(flags)} & 0x04) != 0, then {nameof(call.data)} cannot be null");
                    }
                    int callDataLength = call.data!.Length;
                    if (callDataLength > 0xffffff)
                    {
                        throw new ArgumentException($"{nameof(call.data)} is too large, length cannot exceed 0xffffff, given {callDataLength.ToHexadecimal()}");
                    }
                    // 3 bytes => up to 16,777,215
                    byte[] callDataLengthBytes = callDataLength.ByteArrayFromNumber(3);
                    outBytes = ByteArrayExtensions.ConcatenateByteArrays(outBytes, callDataLengthBytes, call.data);
                }
                
                // If hasGasLimit, store 32 bytes of gasLimit
                if ((flags & 0x08) != 0)
                {
                    byte[] gasBytes = call.gasLimit.ByteArrayFromNumber().PadLeft(32);
                    outBytes = ByteArrayExtensions.ConcatenateByteArrays(outBytes, gasBytes);
                }
            }

            return outBytes;
        }
    }
}