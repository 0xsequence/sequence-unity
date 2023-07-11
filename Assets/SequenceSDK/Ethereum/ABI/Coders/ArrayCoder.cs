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
        /// Decodes the byte array into a list of objects.
        /// </summary>
        /// <param name="encoded">The byte array to decode.</param>
        /// <param name="types">The list of types for decoding.</param>
        /// <returns>The decoded list of objects.</returns>
        public List<object> Decode(byte[] encoded, List<object> types)
        {
            try
            {
                string encodedString = SequenceCoder.ByteArrayToHexString(encoded);
                return DecodeFromString(encodedString, types);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error decoding array: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Encodes the list of objects into a byte array.
        /// </summary>
        /// <typeparam name="T">The type of objects in the list.</typeparam>
        /// <param name="value">The list of objects to encode.</param>
        /// <returns>The encoded byte array.</returns>
        public byte[] Encode<T>(List<T> value)
        {
            try
            {
                string encodedStr = EncodeToString(value);
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
        public string EncodeToString<T>(List<T> value)
        {
            try
            {
                List<object> valueWrapper = new List<object>();
                var valueList = value.Cast<object>().ToList();
                valueWrapper.Add(valueList);
                return _tupleCoder.EncodeToString(valueWrapper);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error encoding array to string: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Decodes the encoded string into a list of objects.
        /// </summary>
        /// <param name="encodedString">The encoded string to decode.</param>
        /// <param name="types">The list of types for decoding.</param>
        /// <returns>The decoded list of objects.</returns>
        public List<object> DecodeFromString(string encodedString, List<object> types)
        {
            try
            {
                return _tupleCoder.DecodeFromString(encodedString, types);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error decoding array from string: {ex.Message}");
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
