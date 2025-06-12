using System;
using System.Numerics;
using Sequence.Utils;

namespace Sequence.EcosystemWallet.Primitives
{
    internal class CallDecoder
    {
        private int _flags;
        private int _pointer;
        private byte[] _packed;
        
        public CallDecoder(int pointer, byte[] packed)
        {
            if (packed == null || packed.Length == 0)
            {
                throw new ArgumentNullException("Invalid packed data: missing globalFlag; given empty byte[]");
            }
            this._pointer = pointer;
            this._packed = packed;
            
            
            if (this._pointer + 1 > this._packed.Length)
            {
                throw new Exception("Invalid packed data: missing call flags");
            }
            _flags = packed[pointer];
            this._pointer++;
        }
        
        public Call Decode(Address self)
        {
            Address to = GetTo(self);
            BigInteger value = GetValue();
            byte[] data = GetData();
            BigInteger gasLimit = GetGasLimit();
            bool delegateCall = GetDelegateCall();
            bool onlyFallback = GetOnlyFallback();
            BehaviourOnError behaviorOnError = GetBehaviorOnError();

            return new Call(to, value, data, gasLimit, delegateCall, onlyFallback, behaviorOnError);
        }

        private Address GetTo(Address self)
        {
            // bit 0 => toSelf
            Address to;
            if ((_flags & 0x01) == 0x01)
            {
                if (self == null)
                {
                    throw new Exception("Missing 'self' address for toSelf call");
                }
                to = self;
            }
            else
            {
                if (_pointer + 20 > _packed.Length)
                {
                    throw new Exception("Invalid packed data: not enough bytes for address");
                }

                byte[] toBytes = _packed.Slice(_pointer, 20);
                to = new Address(toBytes);
                _pointer += 20;
            }

            return self;
        }

        private BigInteger GetValue()
        {
            // bit 1 => hasValue
            BigInteger value = BigInteger.Zero;
            if ((_flags & 0x02) == 0x02)
            {
                if (_pointer + 32 > _packed.Length)
                {
                    throw new Exception("Invalid packed data: not enough bytes for value");
                }

                byte[] valueBytes = _packed.Slice(_pointer, 32);
                value = new BigInteger(valueBytes, isUnsigned: true, isBigEndian: true);
                _pointer += 32;
            }

            return value;
        }

        private byte[] GetData()
        {
            // bit 2 => hasData
            byte[] data = "0x".HexStringToByteArray();
            if ((_flags & 0x04) == 0x04)
            {
                if (_pointer + 3 > _packed.Length)
                {
                    throw new Exception("Invalid packed data: not enough bytes for data length");
                }

                byte[] dataLengthBytes = _packed.Slice(_pointer, 3);
                int dataLength = (int)new BigInteger(dataLengthBytes, isUnsigned: true, isBigEndian: true);
                _pointer += 3;

                if (_pointer + dataLength > _packed.Length)
                {
                    throw new Exception("Invalid packed data: not enough bytes for call data");
                }

                data = _packed.Slice(_pointer, dataLength);
                _pointer += dataLength;
            }

            return data;
        }
        
        private BigInteger GetGasLimit()
        {
            // bit 3 => hasGasLimit
            BigInteger gasLimit = BigInteger.Zero;
            if ((_flags & 0x08) == 0x08)
            {
                if (_pointer + 32 > _packed.Length)
                {
                    throw new Exception("Invalid packed data: not enough bytes for gasLimit");
                }

                byte[] gasLimitBytes = _packed.Slice(_pointer, 32);
                gasLimit = new BigInteger(gasLimitBytes, isUnsigned: true, isBigEndian: true);
                _pointer += 32;
            }

            return gasLimit;
        }
        
        private bool GetDelegateCall()
        {
            // bit 4 => delegateCall
            return (_flags & 0x10) == 0x10;
        }
        
        private bool GetOnlyFallback()
        {
            // bit 5 => onlyFallback
            return (_flags & 0x20) == 0x20;
        }

        private BehaviourOnError GetBehaviorOnError()
        {
            // bits 6..7 => behaviorOnError
            BehaviourOnError behaviorOnError = (BehaviourOnError)((_flags & 0xc0) >> 6);
            return behaviorOnError;
        }
    }
}