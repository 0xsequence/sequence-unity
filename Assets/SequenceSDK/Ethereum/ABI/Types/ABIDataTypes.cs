using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Sequence.ABI
{
    public class FixedByte
    {
        public byte[] Data { get; set; }
        public int Length { get; set; }
        private static readonly string ConstructorExceptionMessage = "ABIByte type length should be [0,32]";

        public FixedByte(int _length, string str)
        {
            if (_length >= 0 && _length <= 32)
            {
                Length = _length;

                Data = new byte[_length];
                Data = Encoding.ASCII.GetBytes(str);
            }
            else
            {
                throw new ABITypeException(ConstructorExceptionMessage);
            }
        }
        
        public FixedByte(int _length, byte[] bytes)
        {
            if (_length >= 0 && _length <= 32)
            {
                Length = _length;
                Data = bytes;
            }
            else
            {
                throw new ABITypeException(ConstructorExceptionMessage);
            }
        }
        
    }
    


    public class ABITypeException : Exception
    {
        public ABITypeException():base("ABI length does not match")
        {
            
        }

        public ABITypeException(string message)
            : base(message)
        {
        }

        
    }
}
