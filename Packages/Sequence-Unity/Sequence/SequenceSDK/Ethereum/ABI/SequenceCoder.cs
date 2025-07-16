using System;
using System.Text;
using Org.BouncyCastle.Crypto.Digests;
using UnityEngine;
using Sequence;
using Sequence.Utils;

namespace Sequence.ABI
{
    
    public class SequenceCoder
    {

        /// Implemented based on  https://github.com/ethereum/EIPs/blob/master/EIPS/eip-55.md
        /// <summary>
        /// Computes the checksum address for the given Ethereum address.
        /// </summary>
        /// <param name="address">The Ethereum address.</param>
        /// <returns>The checksum address.</returns>
        public static string AddressChecksum(string address)
        {
            try
            {
                if (address.StartsWith("0x"))
                {
                    address = address.Substring(2);
                    if (address.Length == 0)
                    {
                        return StringExtensions.ZeroAddress;
                    }
                }
                string hashedAddress = KeccakHashASCII(address);
                string checksumAddress = "";
                int idx = 0;
                foreach (char c in address)
                {
                    if ("0123456789".Contains(c))
                    {
                        checksumAddress += c;
                    }
                    else if ("abcdefABCDEF".Contains(c))
                    {
                        int hashedAddressNibble = Convert.ToInt32(hashedAddress[idx].ToString(), 16);
                        if (hashedAddressNibble > 7)
                        {
                            checksumAddress += Char.ToUpper(c);
                        }
                        else
                        {
                            checksumAddress += c;
                        }
                    }
                    else
                    {
                        throw new Exception($"Unrecognized hex character '{c}' at position {idx}");
                    }
                    idx++;
                }
                return "0x" + checksumAddress;
            }
            catch (Exception ex)
            {
                SequenceLog.Error($"Error computing checksum address({address}): {ex.Message}");
                return address;
            }
        }


        /// <summary>
        /// Computes the Keccak-256 hash of an ASCII input string.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <returns>The hash string.</returns>
        public static string KeccakHashASCII(string input)
        {
            try
            {
                var keccak256 = new KeccakDigest(256);
                byte[] inputBytes = Encoding.ASCII.GetBytes(input);
                keccak256.BlockUpdate(inputBytes, 0, inputBytes.Length);
                byte[] result = new byte[keccak256.GetByteLength()];
                keccak256.DoFinal(result, 0);

                string hashString = BitConverter.ToString(result, 0, 32);
                hashString = hashString.Replace("-", "").ToLowerInvariant();
                return hashString;
            }
            catch (Exception ex)
            {
                SequenceLog.Error($"Error computing Keccak-256 hash: {ex.Message}");
                return string.Empty;
            }
        }

        /// <summary>
        /// Computes the Keccak-256 hash of a byte array.
        /// </summary>
        /// <param name="input">The input byte array.</param>
        /// <returns>The hash byte array.</returns>
        public static byte[] KeccakHash(byte[] input)
        {
            try
            {
                var keccak256 = new KeccakDigest(256);
                keccak256.BlockUpdate(input, 0, input.Length);
                byte[] result = new byte[keccak256.GetByteLength()];
                keccak256.DoFinal(result, 0);

                byte[] result64 = new byte[32];
                Array.Copy(result, 0, result64, 0, 32);
                return result64;
            }
            catch (Exception ex)
            {
                SequenceLog.Error($"Error computing Keccak-256 hash: {ex.Message}");
                return new byte[0];
            }
        }

        /// <summary>
        /// Computes the Keccak-256 hash of a hexadecimal string.
        /// </summary>
        /// <param name="input">The input hexadecimal string.</param>
        /// <returns>The hash string.</returns>
        public static string KeccakHash(string input)
        {
            try
            {
                byte[] inputByte = HexStringToByteArray(input);
                byte[] keccak = KeccakHash(inputByte);
                return ByteArrayToHexString(keccak);
            }
            catch (Exception ex)
            {
                SequenceLog.Error($"Error computing Keccak-256 hash: {ex.Message}");
                return string.Empty;
            }
        }



        // Hex string to byte array and vice versa
        // Ref:
        //https://stackoverflow.com/questions/311165/how-do-you-convert-a-byte-array-to-a-hexadecimal-string-and-vice-versa
        /// <summary>
        /// Converts a hexadecimal string to a byte array.
        /// </summary>
        /// <param name="hexString">The hexadecimal string.</param>
        /// <returns>The byte array.</returns>
        public static byte[] HexStringToByteArray(string hexString)
        {
            try
            {
                if (hexString == null || hexString == "")
                {
                    return new byte[] { };
                }
                if (hexString.StartsWith("0x"))
                {
                    hexString = hexString.Remove(0, 2);
                }
                if (hexString.Length % 2 != 0)
                {
                    hexString = "0" + hexString;
                }

                byte firstByte = Convert.ToByte(hexString.Substring(0, 2), 16);
                int firstInt = Convert.ToInt32(firstByte);
                if (firstInt < 0)
                {
                    int numberChars = hexString.Length;
                    byte[] bytes = new byte[numberChars / 2];
                    int curr = 0;
                    for (int i = 0; i < numberChars - 1; i += 2)
                    {
                        curr = Convert.ToInt32(hexString.Substring(i, 2), 16);
                        bytes[i / 2] = Convert.ToByte(~curr);
                    }
                    curr = Convert.ToInt32(hexString.Substring(numberChars - 1, 2), 16);
                    bytes[(numberChars - 1) / 2] = Convert.ToByte(~curr + 1);
                    return bytes;
                }
                else
                {
                    int numberChars = hexString.Length;
                    byte[] bytes = new byte[numberChars / 2];
                    for (int i = 0; i < numberChars; i += 2)
                        bytes[i / 2] = Convert.ToByte(hexString.Substring(i, 2), 16);
                    return bytes;
                }
            }
            catch (Exception ex)
            {
                SequenceLog.Error($"Error converting hexadecimal string to byte array: {ex.Message}");
                return new byte[0];
            }
        }


        /// <summary>
        /// Converts a byte array to a hexadecimal string.
        /// </summary>
        /// <param name="ba">The byte array.</param>
        /// <returns>The hexadecimal string.</returns>
        public static string ByteArrayToHexString(byte[] ba)
        {
            return ba.ByteArrayToHexString();
        }

        public static string HexStringToHumanReadable(string hexString)
        {
            return hexString.HexStringToHumanReadable();
        }
    }
}