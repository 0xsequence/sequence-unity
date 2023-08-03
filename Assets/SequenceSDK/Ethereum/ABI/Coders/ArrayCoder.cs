using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

        /// <summary>
        /// Encodes the list of objects into a byte array.
        /// </summary>
        /// <typeparam name="T">The type of objects in the list.</typeparam>
        /// <param name="value">The list of objects to encode.</param>
        /// <returns>The encoded byte array.</returns>
        public byte[] Encode<T>(List<T> value, string evmType)
        {
            try
            {
                string encodedStr = EncodeToString(value, evmType);
                return SequenceCoder.HexStringToByteArray(encodedStr);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error encoding array: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Encodes the list of objects into a string representation.
        /// </summary>
        /// <typeparam name="T">The type of objects in the list.</typeparam>
        /// <param name="value">The list of objects to encode.</param>
        /// <returns>The encoded string representation.</returns>
        public string EncodeToString<T>(List<T> value, string evmType)
        {
            try
            {
                List<object> valueWrapper = new List<object>();
                var valueList = value.Cast<object>().ToList();
                valueWrapper.Add(valueList);
                return _tupleCoder.EncodeToString(valueWrapper, new string[]{evmType});
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error encoding array to string: {ex.Message}");
                return null;
            }
        }
        
        public byte[] Encode(object value)
        {
            throw new System.NotImplementedException();
        }

        public object Decode(byte[] encoded)
        {
            throw new System.NotImplementedException();
        }
    }
}
