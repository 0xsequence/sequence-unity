using System;
using System.Collections.Generic;
using System.Numerics;
using Sequence.Utils;

namespace Sequence.EcosystemWallet.Primitives
{
    internal class CallsDecoder
    {
        private int _globalFlag;
        private int _pointer;
        private byte[] _packed;
        
        public CallsDecoder(byte[] packed)
        {
            if (packed == null || packed.Length == 0)
            {
                throw new ArgumentNullException("Invalid packed data: missing globalFlag; given empty byte[]");
            }
            this._packed = packed;
            this._pointer = 0;
            
            this._globalFlag = packed[_pointer];
            this._pointer++;
        }

        public Calls Decode(Address self)
        {
            BigInteger space = GetSpace();
            BigInteger nonce = GetNonce();
            Call[] calls = GetCalls(self);
            return new Calls(space, nonce, calls);
        }

        private BigInteger GetSpace()
        {
            bool spaceZeroFlag = (_globalFlag & 0x01) == 0x01;
            BigInteger space = BigInteger.Zero;

            if (!spaceZeroFlag)
            {
                if (_pointer + 20 > _packed.Length)
                {
                    throw new Exception("Invalid packed data: not enough bytes for space");
                }

                byte[] spaceBytes = _packed.Slice(_pointer, 20);
                space = new BigInteger(spaceBytes, isUnsigned: true, isBigEndian: true);
                _pointer += 20;
            }

            return space;
        }

        private BigInteger GetNonce()
        {
            int nonceSize = (_globalFlag >> 1) & 0x07;
            BigInteger nonce = BigInteger.Zero;
            if (nonceSize > 0)
            {
                if (_pointer + nonceSize > _packed.Length)
                {
                    throw new Exception("Invalid packed data: not enough bytes for nonce");
                }

                byte[] nonceBytes = _packed.Slice(_pointer, nonceSize);
                nonce = new BigInteger(nonceBytes, isUnsigned: true, isBigEndian: true);
                _pointer += nonceSize;
            }

            return nonce;
        }

        private BigInteger GetCallsCount()
        {
            BigInteger callsCount = 1;
            // bit [4] => singleCallFlag
            bool singleCallFlag = (_globalFlag & 0x10) == 0x10;
            if (!singleCallFlag)
            {
                // bit [5] => callsCountSizeFlag => 1 => 2 bytes, 0 => 1 byte
                int countSize = (_globalFlag & 0x20) == 0x20 ? 2 : 1;
                if (_pointer + countSize > _packed.Length)
                {
                    throw new Exception("Invalid packed data: not enough bytes for callsCount");
                }

                byte[] callsCountBytes = _packed.Slice(_pointer, countSize);
                callsCount = new BigInteger(callsCountBytes, isUnsigned: true, isBigEndian: true);
                _pointer += countSize;
            }
            return callsCount;
        }

        private Call[] GetCalls(Address self)
        {
            int callsCount = (int)GetCallsCount();
            CallDecoder decoder = new CallDecoder(_pointer, _packed);
            List<Call> calls = new List<Call>();
            for (int i = 0; i < callsCount; i++)
            {
                Call call = decoder.Decode(self);
                calls.Add(call);
            }

            return calls.ToArray();
        }
    }
}