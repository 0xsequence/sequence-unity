using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Sequence.ABI
{
    public class FixedByte : IList
    {
        public byte[] Data { get; set; }
        public int Count { get; set; }
        public bool IsSynchronized { get; }
        public object SyncRoot { get; }
        public bool IsFixedSize { get; } = true;
        public bool IsReadOnly { get; }
        private static readonly string ConstructorExceptionMessage = "ABIByte type length should be [0,32]";

        public FixedByte(int count, string str)
        {
            if (count >= 0 && count <= 32)
            {
                Count = count;

                Data = new byte[count];
                Data = Encoding.ASCII.GetBytes(str);
            }
            else
            {
                throw new ABITypeException(ConstructorExceptionMessage);
            }
        }
        
        public FixedByte(int count, byte[] bytes)
        {
            if (count >= 0 && count <= 32)
            {
                Count = count;
                Data = bytes;
            }
            else
            {
                throw new ABITypeException(ConstructorExceptionMessage);
            }
        }

        public IEnumerator GetEnumerator()
        {
            return Data.GetEnumerator();
        }
        
        public void CopyTo(Array array, int index)
        {
            Data.CopyTo(array, index);
        }
        
        public int Add(object value)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(object value)
        {
            return Array.IndexOf(Data, value) >= 0;
        }

        public int IndexOf(object value)
        {
            return Array.IndexOf(Data, value);
        }

        public void Insert(int index, object value)
        {
            throw new NotImplementedException();
        }

        public void Remove(object value)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        public object this[int index]
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
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
