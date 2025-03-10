using System;
using System.Numerics;
using Sequence.EmbeddedWallet;

namespace Sequence.ABI
{

    public class NumberCoder : ICoder
    {
        /// <summary>
        /// Decodes the byte array into a number object.
        /// </summary>
        /// <param name="encoded">The byte array to decode.</param>
        /// <returns>The decoded number object.</returns>
        public object Decode(byte[] encoded)
        {
            try
            {
                int encodedLength = encoded.Length;
                byte[] decoded = new byte[encodedLength];
                //check sign
                if (Convert.ToInt32(encoded[0]) < 0)
                {
                    //reverse two's complement

                    for (int i = 0; i < encodedLength - 1; i++)
                    {
                        byte onesComplement = (byte)~encoded[i];
                        decoded[i] = onesComplement;
                    }
                    byte lastOnesComplement = (byte)~encoded[encodedLength - 1];
                    decoded[encodedLength - 1] = (byte)(lastOnesComplement + 1);
                }
                else
                {
                    decoded = encoded;
                }
                string decodedString = SequenceCoder.ByteArrayToHexString(decoded);

                return DecodeFromString(decodedString);
            }
            catch (Exception ex)
            {
                LogHandler.Error($"Error decoding number: {ex.Message}");
                return null;
            }
        }



        /// <summary>
        /// Encodes the number object into a byte array.
        /// </summary>
        /// <param name="number">The number object to encode.</param>
        /// <returns>The encoded byte array.</returns>
        public byte[] Encode(object number)
        {
            try
            {
                //The BigInteger structure does not include constructors with a parameter of type Byte, Int16, SByte, or UInt16. However, the Int32 type supports the implicit conversion of 8-bit and 16-bit signed and unsigned integers to signed 32-bit integers.
                byte[] encoded = { };
                if ((number is int) || (number is BigInteger))
                {
                    encoded = EncodeSignedInt((BigInteger)number, 32);

                }
                // TODO: Make sure Big Endian
                /*if (BitConverter.IsLittleEndian)
                {
                    Array.Reverse(encodedInt); 
                }*/


                return encoded;
            }
            catch (Exception ex)
            {
                LogHandler.Error($"Error encoding number: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Encodes the number object into a string representation.
        /// </summary>
        /// <param name="number">The number object to encode.</param>
        /// <returns>The encoded string representation.</returns>
        public string EncodeToString(object number)
        {
            try
            {
                BigInteger bgNumber;
                if (number.GetType() == typeof(int))
                {
                    bgNumber = new BigInteger((int)number);

                }
                else if (number.GetType() == typeof(uint))
                {
                    bgNumber = new BigInteger((uint)number);
                }
                else
                {
                    bgNumber = (BigInteger)number;
                }
                string encoded = EncodeSignedIntString(bgNumber, 64);
                return encoded;
            }
            catch (Exception ex)
            {
                LogHandler.Error($"Error encoding number to string: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Decodes the encoded string into a number object.
        /// </summary>
        /// <param name="encodedString">The encoded string to decode.</param>
        /// <returns>The decoded number object.</returns>
        public object DecodeFromString(string encodedString)
        {
            try
            {
                BigInteger decodedNumber = BigInteger.Parse(encodedString, System.Globalization.NumberStyles.HexNumber);
                return decodedNumber;
            }
            catch (Exception ex)
            {
                LogHandler.Error($"Error decoding number from string: {ex.Message}");
                return null;
            }
        }


        /// <summary>
        /// Encodes the signed integer into a byte array.
        /// </summary>
        /// <param name="number">The signed integer to encode.</param>
        ///
        public byte[] EncodeSignedInt(BigInteger number, int length)
        {
            try
            {
                /// int<M>: enc(X) is the big-endian two�s complement encoding of X, padded on the higher-order (left) side with 0xff bytes for negative X and with zero-bytes for non-negative X such that the length is 32 bytes.
                string encodedString = EncodeSignedIntString(number, length * 2);
                byte[] encoded = SequenceCoder.HexStringToByteArray(encodedString);
                return encoded;
            }
            catch (Exception ex)
            {
                LogHandler.Error("Exception occurred during EncodeSignedInt: " + ex.Message);
                throw;
            }
        }


        /// <summary>
        /// Encodes the signed integer into a string representation.
        /// </summary>
        /// <param name="number">The signed integer to encode.</param>
        /// <param name="length">The desired length of the encoded string representation.</param>
        /// <returns>The encoded string representation.</returns>
        public string EncodeSignedIntString(BigInteger number, int length)
        {
            try
            {
                var hex = number.ToString("x");
                string encodedString;
                if (number.Sign > -1)
                {
                    encodedString = new string('0', length - hex.Length) + hex;
                }
                else
                {
                    encodedString = new string('f', length - hex.Length) + hex;
                }
                return encodedString;
            }
            catch (Exception ex)
            {
                LogHandler.Error("Exception occurred during EncodeSignedIntString: " + ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Encodes the unsigned integer into a byte array.
        /// </summary>
        /// <param name="number">The unsigned integer to encode.</param>
        /// <param name="length">The desired length of the encoded byte array.</param>
        /// <returns>The encoded byte array.</returns>
        public byte[] EncodeUnsignedInt(BigInteger number, int length)
        {
            try
            {
                /// uint<M>: enc(X) is the big-endian encoding of X, padded on the higher-order (left) side with zero-bytes such that the length is 32 bytes.
                string encodedString = EncodeUnsignedIntString(number, length * 2);
                byte[] encoded = SequenceCoder.HexStringToByteArray(encodedString);
                return encoded;
            }
            catch (Exception ex)
            {
                LogHandler.Error("Exception occurred during EncodeUnsignedInt: " + ex.Message);
                throw;
            }
        }


        /// <summary>
        /// Encodes the unsigned integer into a string representation.
        /// </summary>
        /// <param name="number">The unsigned integer to encode.</param>
        /// <param name="length">The desired length of the encoded string representation.</param>
        /// <returns>The encoded string representation.</returns>
        public string EncodeUnsignedIntString(BigInteger number, int length)
        {
            try
            {
                var hex = number.ToString("x");
                string encodedString = new string('0', length - hex.Length) + hex;
                return encodedString;
            }
            catch (Exception ex)
            {
                LogHandler.Error("Exception occurred during EncodeUnsignedIntString: " + ex.Message);
                throw;
            }
        }

        public byte[] EncodeSignedFloat(float number)
        {
            //fixed<M>x<N>: enc(X) is enc(X * 10**N) where X * 10**N is interpreted as a int256
            throw new NotImplementedException();
        }

        public string EncodeSignedFloatString(float number)
        {
            throw new NotImplementedException();

        }

        public byte[] EncodeUnsignedFloat(float number)
        {
            //ufixed<M>x<N>: enc(X) is enc(X * 10**N) where X * 10**N is interpreted as a uint256
            throw new NotImplementedException();
        }

        public string EncodeUnsignedFloatString(float number)
        {
            throw new NotImplementedException();

        }


    }
}