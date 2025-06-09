using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using NUnit.Framework;
using Sequence.ABI;
using Sequence.Utils;
using UnityEngine.Scripting;
using StringExtensions = Sequence.Utils.StringExtensions;

namespace Sequence.EcosystemWallet.Primitives
{
    [Serializable]
    public class Calls : Payload
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
        
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is Calls))
            {
                return false;
            }
            
            Calls other = (Calls)obj;
            if (space != other.space || nonce != other.nonce || calls.Length != other.calls.Length)
            {
                return false;
            }
            for (int i = 0; i < calls.Length; i++)
            {
                if (!calls[i].Equals(other.calls[i]))
                {
                    return false;
                }
            }
            return true;
        }

        public override byte[] GetEIP712EncodeData()
        {
            byte[] spaceEncoded = new NumberCoder().Encode(space);
            byte[] nonceEncoded = new NumberCoder().Encode(nonce);
            byte[] callsEncoded = new byte[] { };
            if (calls != null)
            {
                foreach (var call in calls)
                {
                    callsEncoded = ByteArrayExtensions.ConcatenateByteArrays(callsEncoded, call.HashStruct());
                }
            }
            callsEncoded = SequenceCoder.KeccakHash(callsEncoded);
            byte[] encoded = ByteArrayExtensions.ConcatenateByteArrays(
                spaceEncoded,
                nonceEncoded,
                callsEncoded
            );
            return encoded;
        }

        public override string ToString()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            
            sb.AppendLine($"Calls {{ space: {space}, nonce: {nonce}, calls: [");
            
            if (calls != null)
            {
                for (int i = 0; i < calls.Length; i++)
                {
                    sb.AppendLine($"    {calls[i]},");
                }
            }
            
            string parentWalletsStr = parentWallets != null ? string.Join(", ", parentWallets.Select(w => w.ToString()).ToArray()) : "null";
            sb.AppendLine($"], parentWallets: [{parentWalletsStr}] }}");
            
            return sb.ToString();
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

            int globalFlag = SetGlobalFlag(nonceBytesNeeded, (uint)callsLength);
            
            // Start building the output
            // We'll accumulate in a Bytes object as we go
            byte[] outBytes = globalFlag.ByteArrayFromNumber(1);
            outBytes = SetSpaceBytes(outBytes);
            outBytes = SetNonceBytes(outBytes, nonceBytesNeeded);
            outBytes = StoreCallsLength(outBytes, callsLength);
            outBytes = EncodeCalls(outBytes, callsLength, self);

            return outBytes;
        }

        private int SetGlobalFlag(int nonceBytesNeeded, uint callsLength)
        {
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
            
            if (GetCallsCountSize(callsLength) > 1)
            {
                globalFlag |= 0x20; // We need more than one byte for the calls count
                                    // bit [5] => callsCountSizeFlag: 1 => 2 bytes, 0 => 1 byte
            }

            return globalFlag;
        }

        private int GetCallsCountSize(uint callsLength)
        {
            /*
              If there's more than one call, we decide if we store the #calls in 1 or 2 bytes.
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
                }
                else
                {
                    throw new ArgumentException($"{calls} is too large, cannot have more than {MaxCalls} calls, given {callsLength}");
                }
            }
            return callsCountSize;
        }

        private byte[] SetSpaceBytes(byte[] outBytes)
        {
            // If space isn't 0, store it as exactly 20 bytes (like uint160)
            if (space != BigInteger.Zero)
            {
                byte[] spaceBytes = space.ByteArrayFromNumber().PadLeft(20);
                outBytes = ByteArrayExtensions.ConcatenateByteArrays(outBytes, spaceBytes);
            }

            return outBytes;
        }
        
        private byte[] SetNonceBytes(byte[] outBytes, int nonceBytesNeeded)
        {
            // Encode nonce in nonceBytesNeeded
            if (nonceBytesNeeded > 0)
            {
                byte[] nonceBytes = nonce.ByteArrayFromNumber().PadLeft(nonceBytesNeeded);
                outBytes = ByteArrayExtensions.ConcatenateByteArrays(outBytes, nonceBytes);
            }


            return outBytes;
        }

        private byte[] StoreCallsLength(byte[] outBytes, int callsLength)
        {
            int callsCountSize = GetCallsCountSize((uint)callsLength);
            if (callsLength != 1)
            {
                if (callsCountSize > 2 || callsCountSize <= 0)
                {
                    throw new SystemException(
                        $"If {nameof(callsLength)} != 1, then {callsCountSize} must be 1 or 2, given {callsCountSize}");
                }
                outBytes = ByteArrayExtensions.ConcatenateByteArrays(outBytes, callsLength.ByteArrayFromNumber(callsCountSize));
            }

            return outBytes;
        }

        private byte[] EncodeCalls(byte[] outBytes, int callsLength, Address self)
        {
            for (int i = 0; i < callsLength; i++)
            {
                Call call = calls[i];
                outBytes = call.Encode(outBytes, self);
            }

            return outBytes;
        }

        public static Calls Decode(byte[] packed, Address self = null)
        {
            int pointer = 0;
            if (packed == null || packed.Length == 0)
            {
                throw new Exception("Invalid packed data: missing globalFlag; given empty byte[]");
            }
            
            // Read globalFlag 
            int globalFlag = packed[pointer];
            pointer++;
            
            // bit 0 => spaceZeroFlag
            bool spaceZeroFlag = (globalFlag & 0x01) == 0x01;
            BigInteger space = BigInteger.Zero;

            if (!spaceZeroFlag)
            {
                if (pointer + 20 > packed.Length)
                {
                    throw new Exception("Invalid packed data: not enough bytes for space");
                }

                byte[] spaceBytes = packed.Slice(pointer, 20);
                space = new BigInteger(spaceBytes, isUnsigned: true, isBigEndian: true);
                pointer += 20;
            }
            
            
            // bits [1..3] => nonceSize
            int nonceSize = (globalFlag >> 1) & 0x07;
            BigInteger nonce = BigInteger.Zero;
            if (nonceSize > 0)
            {
                if (pointer + nonceSize > packed.Length)
                {
                    throw new Exception("Invalid packed data: not enough bytes for nonce");
                }

                byte[] nonceBytes = packed.Slice(pointer, nonceSize);
                nonce = new BigInteger(nonceBytes, isUnsigned: true, isBigEndian: true);
                pointer += nonceSize;
            }

            // bit [4] => singleCallFlag
            BigInteger callsCount = 1;
            bool singleCallFlag = (globalFlag & 0x10) == 0x10;
            if (!singleCallFlag)
            {
                // bit [5] => callsCountSizeFlag => 1 => 2 bytes, 0 => 1 byte
                int countSize = (globalFlag & 0x20) == 0x20 ? 2 : 1;
                if (pointer + countSize > packed.Length)
                {
                    throw new Exception("Invalid packed data: not enough bytes for callsCount");
                }

                byte[] callsCountBytes = packed.Slice(pointer, countSize);
                callsCount = new BigInteger(callsCountBytes, isUnsigned: true, isBigEndian: true);
                pointer += countSize;
            }

            List<Call> calls = new List<Call>();
            for (int i = 0; i < callsCount; i++)
            {
                if (pointer + 1 > packed.Length)
                {
                    throw new Exception("Invalid packed data: missing call flags");
                }
                int flags = packed[pointer];
                pointer++;
                
                // bit 0 => toSelf
                Address to;
                if ((flags & 0x01) == 0x01)
                {
                    if (self == null)
                    {
                        throw new Exception("Missing 'self' address for toSelf call");
                    }
                    to = self;
                }
                else
                {
                    if (pointer + 20 > packed.Length)
                    {
                        throw new Exception("Invalid packed data: not enough bytes for address");
                    }

                    byte[] toBytes = packed.Slice(pointer, 20);
                    to = new Address(toBytes);
                    pointer += 20;
                }
                
                // bit 1 => hasValue
                BigInteger value = BigInteger.Zero;
                if ((flags & 0x02) == 0x02)
                {
                    if (pointer + 32 > packed.Length)
                    {
                        throw new Exception("Invalid packed data: not enough bytes for value");
                    }

                    byte[] valueBytes = packed.Slice(pointer, 32);
                    value = new BigInteger(valueBytes, isUnsigned: true, isBigEndian: true);
                    pointer += 32;
                }
                
                // bit 2 => hasData
                byte[] data = "0x".HexStringToByteArray();
                if ((flags & 0x04) == 0x04)
                {
                    if (pointer + 3 > packed.Length)
                    {
                        throw new Exception("Invalid packed data: not enough bytes for data length");
                    }

                    byte[] dataLengthBytes = packed.Slice(pointer, 3);
                    int dataLength = (int)new BigInteger(dataLengthBytes, isUnsigned: true, isBigEndian: true);
                    pointer += 3;

                    if (pointer + dataLength > packed.Length)
                    {
                        throw new Exception("Invalid packed data: not enough bytes for call data");
                    }

                    data = packed.Slice(pointer, dataLength);
                    pointer += dataLength;
                }
                
                // bit 3 => hasGasLimit
                BigInteger gasLimit = BigInteger.Zero;
                if ((flags & 0x08) == 0x08)
                {
                    if (pointer + 32 > packed.Length)
                    {
                        throw new Exception("Invalid packed data: not enough bytes for gasLimit");
                    }

                    byte[] gasLimitBytes = packed.Slice(pointer, 32);
                    gasLimit = new BigInteger(gasLimitBytes, isUnsigned: true, isBigEndian: true);
                    pointer += 32;
                }
                
                // bits 4..5 => delegateCall, onlyFallback
                bool delegateCall = (flags & 0x10) == 0x10;
                bool onlyFallback = (flags & 0x20) == 0x20;
                
                // bits 6..7 => behaviorOnError
                BehaviourOnError behaviorOnError = (BehaviourOnError)((flags & 0xc0) >> 6);

                Call call = new Call(
                    to,
                    value,
                    data,
                    gasLimit,
                    delegateCall,
                    onlyFallback,
                    behaviorOnError
                );
                calls.Add(call);
            }

            return new Calls(space, nonce, calls.ToArray());
        }
        
        internal static Calls FromSolidityEncoding(SolidityDecoded decoded)
        {
            return new Calls(decoded.space, decoded.nonce, decoded.calls);
        }
    }
}