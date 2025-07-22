using System;
using System.Collections;
using System.Collections.Generic;
using Sequence.Utils;
using UnityEngine;

namespace Sequence.ABI
{
    public class StaticBytesCoder : ICoder
    {

        /// <summary>
        /// Decodes the byte array into its original value.
        /// </summary>
        /// <param name="encoded">The byte array to decode.</param>
        /// <returns>The decoded object.</returns>
        public object Decode(byte[] encoded)
        {
            try
            {
                string encodedStr = SequenceCoder.ByteArrayToHexString(encoded);
                return SequenceCoder.HexStringToByteArray(DecodeFromString(encodedStr));
            }
            catch (Exception ex)
            {
                SequenceLog.Error("Exception occurred during Decode: " + ex.Message);
                throw;
            }
        }



        /// <summary>
        /// Encodes the value into a byte array.
        /// </summary>
        /// <param name="value">The value to encode.</param>
        /// <returns>The encoded byte array.</returns>
        public byte[] Encode(object value)
        {
            try
            {
                /// bytes<M>: binary type of M bytes, 0 < M <= 3
                /// bytes, of length k (which is assumed to be of type uint256):
                ///enc(X) = enc(k) pad_right(X), i.e.the number of bytes is encoded as a uint256 followed by the actual value of X as a byte sequence, followed by the minimum number of zero-bytes such that len(enc(X)) is a multiple of 32.
                string encodedString = EncodeToString(value);
                byte[] encoded = SequenceCoder.HexStringToByteArray(encodedString);
                return encoded;
            }
            catch (Exception ex)
            {
                SequenceLog.Error("Exception occurred during Encode: " + ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Encodes the value into a string representation.
        /// </summary>
        /// <param name="value">The value to encode.</param>
        /// <returns>The encoded string representation.</returns>
        public string EncodeToString(object value)
        {
            try
            {
                string valueStr = SequenceCoder.ByteArrayToHexString(((FixedByte)value).Data);
                string encodedStr = (valueStr).PadRight(64, '0');
                return encodedStr;
            }

            catch (Exception ex)
            {
                SequenceLog.Error("Exception occurred during EncodeToString: " + ex.Message);
                throw;
            }
        }


        /// <summary>
        /// Decodes the string representation into its original value.
        /// </summary>
        /// <param name="encodedString">The encoded string representation.</param>
        /// <returns>The decoded string.</returns>
        public string DecodeFromString(string encodedString)
        {
            try
            {
                int trailingZero = 0;
                for (int i = encodedString.Length - 1; i > 0; i--)
                {
                    if (encodedString[i] == '0') trailingZero++;
                    else break;
                }
                string byteStr = EnsureEvenLength(encodedString.Substring(0, encodedString.Length - trailingZero));

                return byteStr;
            }

            catch (Exception ex)
            {
                SequenceLog.Error("Exception occurred during DecodeFromString: " + ex.Message);
                throw;
            }
        }

        private string EnsureEvenLength(string value)
        {
            if (value.Length % 2 == 1)
            {
                value += "0";
            }

            return value;
        }
    }

    public static class StaticBytesCoderExtensions
    {
        private static StaticBytesCoder _coder = new StaticBytesCoder();
        public static byte[] Decode(string encoded)
        {
            return SequenceCoder.HexStringToByteArray(_coder.DecodeFromString(encoded.Replace("0x", "")));
        }
    }
}
