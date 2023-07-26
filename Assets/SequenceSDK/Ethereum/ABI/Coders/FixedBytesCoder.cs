using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sequence.ABI
{
    public class FixedBytesCoder : ICoder
    {
        NumberCoder _numberCoder = new NumberCoder();

        /// <summary>
        /// Decodes the byte array into a byte array value.
        /// </summary>
        /// <param name="encoded">The byte array to decode.</param>
        /// <returns>The decoded byte array value.</returns>
        public object Decode(byte[] encoded)
        {
            try
            {
                string encodedStr = SequenceCoder.ByteArrayToHexString(encoded);
                return SequenceCoder.HexStringToByteArray(DecodeFromString(encodedStr));
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error decoding byte array: {ex.Message}");
                return null;
            }
        }

            /// <summary>
        /// Encodes the byte array value into a byte array.
        /// </summary>
        /// <param name="value">The byte array value to encode.</param>
        /// <returns>The encoded byte array.</returns>
        public byte[] Encode(object value)
        {
            /// bytes<M>: binary type of M bytes, 0 < M <= 3
            /// bytes, of length k (which is assumed to be of type uint256):
            ///enc(X) = enc(k) pad_right(X), i.e.the number of bytes is encoded as a uint256 followed by the actual value of X as a byte sequence, followed by the minimum number of zero-bytes such that len(enc(X)) is a multiple of 32.
            try
            {
                string encodedString = EncodeToString(value);
                byte[] encoded = SequenceCoder.HexStringToByteArray(encodedString);
                return encoded;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error encoding byte array: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Encodes the byte array value into a string representation.
        /// </summary>
        /// <param name="value">The byte array value to encode.</param>
        /// <returns>The encoded string representation.</returns>
        public string EncodeToString(object value)
        {
            try
            {
                int numberOfBytes = ((byte[])value).Length;
                string valueStr = SequenceCoder.ByteArrayToHexString(((byte[])value));
                string numberOfBytesStr = _numberCoder.EncodeUnsignedIntString(numberOfBytes, 64);
                // followed by the minimum number of zero-bytes such that len(enc(X)) is a multiple of 32
                int currentTotalLength = 64 + numberOfBytes;
                int zeroBytesNeeded = 64 - currentTotalLength % 64;
                int totalLength = currentTotalLength + zeroBytesNeeded;

                string encodedStr = (numberOfBytesStr + valueStr).PadRight(totalLength, '0');
                return encodedStr;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error encoding byte array to string: {ex.Message}");
                return null;
            }
        }
        /// <summary>
        /// Decodes the encoded string into a byte array value.
        /// </summary>
        /// <param name="encodedString">The encoded string to decode.</param>
        /// <returns>The decoded byte array value.</returns>
        public string DecodeFromString(string encodedString)
        {
            try
            {
                string byteStr = encodedString.Substring(64, encodedString.Length - 64);

                return byteStr;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error decoding byte array from string: {ex.Message}");
                return null;
            }
        }
    }

    public static class FixedBytesCoderWrapper
    {
        private static FixedBytesCoder _coder = new FixedBytesCoder();
        public static byte[] Decode(string encoded)
        {
            return SequenceCoder.HexStringToByteArray(_coder.DecodeFromString(encoded.Replace("0x", "")));
        }
    }
}