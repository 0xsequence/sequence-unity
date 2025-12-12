using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Sequence.Utils;
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
                SequenceLog.Error($"Error decoding byte array: {ex.Message}");
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
                SequenceLog.Error($"Error encoding byte array: {ex.Message}");
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
            if (value is byte[] valueBytes)
            {
                return EncodeBytesToString(valueBytes);
            }

            if (value is FixedByte valueFixedBytes)
            {
                return EncodeFixedBytesToString(valueFixedBytes);
            }

            throw new ArgumentException(
                $"Encountered unexpected value type: {value.GetType().Name}. Expected {nameof(FixedByte)} or byte[].");
        }

        private string EncodeBytesToString(byte[] value)
        {
            try
            {
                return DoEncodeAsString(value, value.Length);
            }
            catch (Exception ex)
            {
                SequenceLog.Error($"Error encoding byte array to string: {ex.Message}");
                return null;
            }
        }

        private string EncodeFixedBytesToString(FixedByte value)
        {
            try
            {
                return DoEncodeAsString(value.Data, value.Count);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error encoding byte array to string: {ex.Message}");
                return null;
            }
        }

        private string DoEncodeAsString(byte[] data, int length)
        {
            string valueString = data.ByteArrayToHexString();
            string numberOfBytesStr = _numberCoder.EncodeUnsignedIntString(length, 64);
            // followed by the minimum number of zero-bytes such that len(enc(X)) is a multiple of 32
            int currentTotalLength = length;
            int zeroBytesNeeded = 64 - currentTotalLength % 64;
            int totalLength = currentTotalLength + zeroBytesNeeded;

            valueString = valueString.PadRight(totalLength, '0');

            string encodedStr = numberOfBytesStr + valueString;
            return encodedStr;
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
                string lengthString = encodedString.Substring(0, 64);
                var length = (BigInteger)_numberCoder.DecodeFromString(lengthString);
                string byteStr = encodedString.Substring(64, (int)length * 2);

                return byteStr;
            }
            catch (Exception ex)
            {
                SequenceLog.Error($"Error decoding byte array from string: {ex.Message}");
                return null;
            }
        }
    }
}