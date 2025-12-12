using System;
using Sequence.Utils;
using UnityEngine;

namespace Sequence.ABI
{
    /// <summary>
    /// Note that in the dynamic case, head(X(i)) is well-defined since the lengths of the head parts only depend on the types and not the values. The value of head(X(i)) is the offset of the beginning of tail(X(i)) relative to the start of enc(X).
    /// </summary>
    public class BytesCoder : ICoder
    {
        FixedBytesCoder _fixedBytesCoder = new FixedBytesCoder();
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
            try
            {
                string bytesStr = _fixedBytesCoder.EncodeToString(value);
                return bytesStr;
            }
            catch (Exception ex)
            {
                SequenceLog.Error($"Error encoding byte array to string: {ex.Message}");
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
                string fixedStr = EnsureEvenLength(encodedString);
                return _fixedBytesCoder.DecodeFromString(fixedStr);
            }
            catch (Exception ex)
            {
                SequenceLog.Error($"Error decoding byte array from string: {ex.Message}");
                return null;
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

    public static class BytesCoderExtensions
    {
        private static BytesCoder _coder = new BytesCoder();
        public static byte[] Decode(string encoded)
        {
            return SequenceCoder.HexStringToByteArray(_coder.DecodeFromString(encoded.Replace("0x", "")));
        }
    }
}