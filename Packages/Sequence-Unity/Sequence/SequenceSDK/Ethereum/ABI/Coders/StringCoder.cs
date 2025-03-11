using System;
using System.Text;
using Sequence.Utils;

namespace Sequence.ABI
{

    public class StringCoder : ICoder
    {
        BytesCoder _bytesCoder = new BytesCoder();

        /// <summary>
        /// Decodes the byte array into a string representation.
        /// </summary>
        /// <param name="encoded">The byte array to decode.</param>
        /// <returns>The decoded string.</returns>
        public object Decode(byte[] encoded)
        {
            try
            {
                string encodedString = SequenceCoder.ByteArrayToHexString(encoded);
                string decodedString = DecodeFromString(encodedString);
                return decodedString;
            }
            catch (Exception ex)
            {
                LogHandler.Error("Exception occurred during Decode: " + ex.Message);
                throw;
            }
        }


        /// <summary>
        /// Encodes the string into a byte array using UTF-8 encoding.
        /// </summary>
        /// <param name="value">The string value to encode.</param>
        /// <returns>The encoded byte array.</returns>
        public byte[] Encode(object value)
        {
            try
            {
                /// string: dynamic sized unicode string assumed to be UTF-8 encoded.
                /// string:
                ///       enc(X) = enc(enc_utf8(X)), i.e.X is UTF-8 encoded and this value is interpreted as of bytes type and encoded further.Note that the length used in this subsequent encoding is the number of bytes of the UTF-8 encoded string, not its number of characters.
                Encoding utf8 = Encoding.UTF8;

                return _bytesCoder.Encode(utf8.GetBytes((string)value));
            }
            catch (Exception ex)
            {
                LogHandler.Error("Exception occurred during Encode: " + ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Encodes the string into a string representation.
        /// </summary>
        /// <param name="value">The string value to encode.</param>
        /// <returns>The encoded string representation.</returns>
        public string EncodeToString(object value)
        {
            try
            {
                return SequenceCoder.ByteArrayToHexString(Encode(value));
            }
            catch (Exception ex)
            {
                LogHandler.Error("Exception occurred during EncodeToString: " + ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Decodes the string representation into its original string value.
        /// </summary>
        /// <param name="encodedString">The encoded string representation.</param>
        /// <returns>The decoded string.</returns>
        public string DecodeFromString(string encodedString)
        {
            try
            {
                string decodedStr = _bytesCoder.DecodeFromString(encodedString);
                return SequenceCoder.HexStringToHumanReadable(decodedStr);
            }
            catch (Exception ex)
            {
                LogHandler.Error("Exception occurred during DecodeFromString: " + ex.Message);
                throw;
            }
        }


    }

    public static class StringCoderExtensions
    {
        private static StringCoder _stringCoder = new StringCoder();
        public static string DecodeFromString(string encodedString)
        {
            return _stringCoder.DecodeFromString(encodedString);
        }
    }
}