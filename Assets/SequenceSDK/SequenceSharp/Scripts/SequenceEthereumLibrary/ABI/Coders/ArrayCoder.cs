using System.Collections.Generic;
using System.Linq;

namespace Sequence.ABI
{
    /// <summary>
    /// T[k] for any T and k:
    /// enc(X) = enc((X[0], ..., X[k-1]))
    /// i.e. it is encoded as if it were a tuple with k elements of the same type.
    /// T[] where X has k elements (k is assumed to be of type uint256):
    /// enc(X) = enc(k) enc([X[0], ..., X[k-1]])
    /// i.e. it is encoded as if it were an array of static size k, prefixed with the number of elements.
    /// </summary>
    public class ArrayCoder : ICoder
    {
        TupleCoder _tupleCoder = new TupleCoder();
        public List<object> Decode(byte[] encoded, List<object> types)
        {
            string encodedString = SequenceCoder.ByteArrayToHexString(encoded);
            return DecodeFromString(encodedString, types);
        }

        public T DefaultValue<T>()
        {
            throw new System.NotImplementedException();
        }

        public byte[] Encode<T>(List<T> value)
        {
            string encodedStr = EncodeToString(value);
            return SequenceCoder.HexStringToByteArray(encodedStr);
            
        }

        public string EncodeToString<T>(List<T> value)
        {
            List<object> valueWrapper = new List<object>();         
            var valueList = value.Cast<object>().ToList();
            valueWrapper.Add(valueList);
            return _tupleCoder.EncodeToString(valueWrapper);
        }

        public List<object> DecodeFromString(string encodedString, List<object> types)
        {
            return _tupleCoder.DecodeFromString(encodedString, types);
        }

        public bool IsSupportedType()
        {
            throw new System.NotImplementedException();
        }

        public byte[] Encode(object value)
        {
            throw new System.NotImplementedException();
        }

        object ICoder.Decode(byte[] encoded)
        {
            throw new System.NotImplementedException();
        }
    }
}
