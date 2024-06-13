using System;
using UnityEngine;

namespace Sequence.ABI
{

    public class BooleanCoder : ICoder
    {
        /// <summary>
        /// Decodes the byte array into a boolean value.
        /// </summary>
        /// <param name="encoded">The byte array to decode.</param>
        /// <returns>The decoded boolean value.</returns>
        public object Decode(byte[] encoded)
        {
            try
            {
                int length = encoded.Length;
                if (encoded[length - 1] == 1) return true;
                return false;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error decoding boolean: {ex.Message}");
                return null;
            }
        }



        /// <summary>
        /// Encodes the boolean value into a byte array.
        /// </summary>
        /// <param name="value">The boolean value to encode.</param>
        /// <returns>The encoded byte array.</returns>
        public byte[] Encode(object value)
        {
            try
            {
                string encodedStr = EncodeToString(value);
                return SequenceCoder.HexStringToByteArray(encodedStr);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error encoding boolean: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Encodes the boolean value into a string representation.
        /// </summary>
        /// <param name="value">The boolean value to encode.</param>
        /// <returns>The encoded string representation.</returns>
        public string EncodeToString(object value)
        {
            try
            {
                if ((bool)value)
                {
                    return new string('0', 63) + '1';
                }
                return new string('0', 63) + '0';
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error encoding boolean to string: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Decodes the encoded string into a boolean value.
        /// </summary>
        /// <param name="encodedString">The encoded string to decode.</param>
        /// <returns>The decoded boolean value.</returns>
        public bool DecodeFromString(string encodedString)
        {
            try
            {
                int length = encodedString.Length;
                if (encodedString[length - 1] == '1') return true;
                return false;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error decoding boolean from string: {ex.Message}");
                return false;
            }
        }


    }
}