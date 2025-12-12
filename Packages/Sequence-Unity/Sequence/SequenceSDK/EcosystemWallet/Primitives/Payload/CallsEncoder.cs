using System;
using System.Numerics;
using Sequence.Utils;
using UnityEngine;

namespace Sequence.EcosystemWallet.Primitives
{
    internal class CallsEncoder
    {
        private Address self;
        private int globalFlag;

        private BigInteger space;
        private BigInteger nonce;
        private Call[] calls;
        private int callsLength;
        private int nonceBytesNeeded;
        
        public CallsEncoder(Address self, Calls calls)
        {
            this.self = self;
            this.space = calls.space;
            this.nonce = calls.nonce;
            this.calls = calls.calls;
        }

        public byte[] Encode()
        {
            callsLength = calls.Length;
            nonceBytesNeeded = nonce.MinimumBytesNeeded();
            if (nonceBytesNeeded > Calls.MaxNonceBytes)
            {
                throw new ArgumentException(
                    $"{nameof(nonce)} is too large, need {nonceBytesNeeded} bytes to represent it, but max is {Calls.MaxNonceBytes}");
            }

            SetGlobalFlag();
            
            // Start building the output
            // We'll accumulate in a byte[] as we go
            byte[] outBytes = globalFlag.ByteArrayFromNumber(1);
            outBytes = AddSpaceBytes(outBytes);
            outBytes = AddNonceBytes(outBytes);
            outBytes = AddCallsLength(outBytes);
            outBytes = AddCalls(outBytes);

            return outBytes;
        }

        private void SetGlobalFlag()
        {
            /*
                globalFlag layout:
                  bit 0: spaceZeroFlag => 1 if space == 0, else 0
                  bits [1..3]: how many bytes we use to encode nonce
                  bit 4: singleCallFlag => 1 if there's exactly one call
                  bit 5: callsCountSizeFlag => 1 if #calls stored in 2 bytes, 0 if in 1 byte
                  (bits [6..7] are unused/free)
              */
            globalFlag = 0;
            
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
            
            if (GetCallsCountSize() > 1)
            {
                globalFlag |= 0x20; // We need more than one byte for the calls count
                // bit [5] => callsCountSizeFlag: 1 => 2 bytes, 0 => 1 byte
            }
        }

        private int GetCallsCountSize()
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
                else if (callsLength < Calls.MaxCalls)
                {
                    callsCountSize = 2;
                }
                else
                {
                    throw new ArgumentException($"{calls} is too large, cannot have more than {Calls.MaxCalls} calls, given {callsLength}");
                }
            }
            return callsCountSize;
        }
        
        private byte[] AddSpaceBytes(byte[] outBytes)
        {
            // If space isn't 0, store it as exactly 20 bytes (like uint160)
            if (space != BigInteger.Zero)
            {
                byte[] spaceBytes = space.ByteArrayFromNumber().PadLeft(20);
                outBytes = ByteArrayExtensions.ConcatenateByteArrays(outBytes, spaceBytes);
            }

            return outBytes;
        }
        
        private byte[] AddNonceBytes(byte[] outBytes)
        {
            // Encode nonce in nonceBytesNeeded
            if (nonceBytesNeeded > 0)
            {
                byte[] nonceBytes = nonce.ByteArrayFromNumber().PadLeft(nonceBytesNeeded);
                outBytes = ByteArrayExtensions.ConcatenateByteArrays(outBytes, nonceBytes);
            }

            return outBytes;
        }
        
        private byte[] AddCallsLength(byte[] outBytes)
        {
            int callsCountSize = GetCallsCountSize();
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
        
        private byte[] AddCalls(byte[] outBytes)
        {
            for (int i = 0; i < callsLength; i++)
            {
                CallEncoder encoder = new CallEncoder(outBytes, calls[i]);
                outBytes = encoder.Encode(self);
            }

            return outBytes;
        }
    }
}