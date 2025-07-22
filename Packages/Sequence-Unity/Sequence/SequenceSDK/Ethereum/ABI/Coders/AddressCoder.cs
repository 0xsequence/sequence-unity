
using System;
using Sequence.Utils;
using UnityEngine;

namespace Sequence.ABI
{
    /// <summary>
    /// address: equivalent to uint160, except for the assumed interpretation and language typing. For computing the function selector, address is used.
    /// </summary>
    public class AddressCoder : ICoder
    {
        /// <summary>
        /// Decodes the byte array into an address string.
        /// </summary>
        /// <param name="encoded">The byte array to decode.</param>
        /// <returns>The decoded address string.</returns>
        public object Decode(byte[] encoded)
        {
            try
            {
                string encodedString = SequenceCoder.ByteArrayToHexString(encoded);
                string decoded = SequenceCoder.AddressChecksum(DecodeFromString(encodedString));
                return decoded;
            }
            catch (Exception ex)
            {
                SequenceLog.Error($"Failed to decode address: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Encodes the address object into a byte array. the uint160 case.
        /// </summary>
        /// <param name="value">The address object to encode.</param>
        /// <returns>The encoded byte array.</returns>
        public byte[] Encode(object value)
        {
            try
            {
                //Trim 0x at the start
                string encodedString = EncodeToString(value);
                byte[] encoded = SequenceCoder.HexStringToByteArray(encodedString);
                return encoded;
            }
            catch (Exception ex)
            {
                SequenceLog.Error($"Failed to encode address: {ex.Message}");
                return new byte[0];
            }
        }

        /// <summary>
        /// Helper function, 3ncodes the address object into a string representation.
        /// </summary>
        /// <param name="value">The address object to encode.</param>
        /// <returns>The encoded string representation of the address.</returns>
        public string EncodeToString(object value)
        {
            try
            {
                string address;
                // We can't explicitly cast an Address to a string, but we can implicitly cast it
                if (value.GetType() == typeof(Address))
                {
                    address = (Address)value;
                }else
                {
                    address = (string)value;
                }

                if (address.StartsWith("0x"))
                {
                    address = address.Remove(0, 2);
                }

                string encodedString = new string('0', 64 - address.Length) + address;
                return encodedString.ToLower();
            }
            catch (Exception ex)
            {
                SequenceLog.Error($"Failed to encode address to string: {ex.Message}");
                return string.Empty;
            }
        }

        /// <summary>
        /// Decodes the encoded string into an address string.
        /// </summary>
        /// <param name="encodedString">The encoded string to decode.</param>
        /// <returns>The decoded address string.</returns>
        public string DecodeFromString(string encodedString)
        {
            if (encodedString.IsZeroAddress())
            {
                return StringExtensions.ZeroAddress;
            }
            
            try
            {
                encodedString = encodedString.Replace("0x", "").TrimStart('0');
                return "0x" + encodedString;
            }
            catch (Exception ex)
            {
                SequenceLog.Error($"Failed to decode address from string: {ex.Message}");
                return string.Empty;
            }
        }
    }

    public static class AddressCoderExtensions
    {
        private static AddressCoder _addressCoder = new AddressCoder();
        public static string Decode(string encodedString)
        {
            
            try
            {
                string decoded = SequenceCoder.AddressChecksum(_addressCoder.DecodeFromString(encodedString));
                return decoded;
            }
            catch (Exception ex)
            {
                SequenceLog.Error($"Failed to decode address: {ex.Message}");
                return null;
            }
        }
    }
}