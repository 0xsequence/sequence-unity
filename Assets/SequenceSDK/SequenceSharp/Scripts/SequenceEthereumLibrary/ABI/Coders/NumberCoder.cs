using System;
using System.Numerics;

namespace Sequence.ABI
{

    public class NumberCoder : ICoder
    {
        public object Decode(byte[] encoded)
        {
            int encodedLength = encoded.Length;
            byte[] decoded = new byte[encodedLength];
            //check sign
            if(Convert.ToInt32(encoded[0])<0)
            {
                //reverse two's complement
                
                for (int i = 0; i< encodedLength - 1;i++)
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



        /// <summary>
        /// Encode signed integer, unsigend integer call EncodeUnsigned
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public byte[] Encode(object number)
        {

            //The BigInteger structure does not include constructors with a parameter of type Byte, Int16, SByte, or UInt16. However, the Int32 type supports the implicit conversion of 8-bit and 16-bit signed and unsigned integers to signed 32-bit integers.
            
            byte[] encoded = EncodeSignedInt((BigInteger)number, 32);
            // TODO: Make sure Big Endian
            /*if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(encodedInt); 
            }*/


            return encoded;

        }

        public byte[] EncodeUnsigned(object number)
        {
            byte[] encoded = EncodeUnsignedInt(new BigInteger((int)number), 32);
            return encoded;
        }

        public string EncodeToString(object number)
        {
            BigInteger bgNumber;
            if(number.GetType() == typeof(int))
            {
                bgNumber = new BigInteger((int)number);

            }
            else if(number.GetType() == typeof(uint))
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

        public string EncodeUnsignedToString(object number)
        {
            string encoded = EncodeUnsignedIntString((BigInteger)number, 64);
            return encoded;
        }

        public object DecodeFromString(string encodedString)
        {
            BigInteger decodedNumber = BigInteger.Parse(encodedString, System.Globalization.NumberStyles.HexNumber);
            return decodedNumber;
            
            
        }


        /// <summary>
        /// int<M>: enc(X) is the big-endian two’s complement encoding of X, padded on the higher-order (left) side with 0xff bytes for negative X and with zero-bytes for non-negative X such that the length is 32 bytes.
        /// </summary>
        /// <param name="number"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public byte[] EncodeSignedInt(BigInteger number, int length)
        {
            string encodedString = EncodeSignedIntString(number, length * 2);
            byte[] encoded = SequenceCoder.HexStringToByteArray(encodedString);
            return encoded;
        }

        public string EncodeSignedIntString(BigInteger number, int length)
        {
            var hex = number.ToString("x");
            string encodedString;
            if (number.Sign > 0)
            {
                encodedString = new string('0', length - hex.Length) + hex;
            }
            else
            {
                encodedString = new string('f', length - hex.Length) + hex;
            }
            return encodedString;
        }

        /// <summary>
        /// uint<M>: enc(X) is the big-endian encoding of X, padded on the higher-order (left) side with zero-bytes such that the length is 32 bytes.
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public byte[] EncodeUnsignedInt(BigInteger number, int length)
        {
            string encodedString = EncodeUnsignedIntString(number, length * 2);
            byte[] encoded = SequenceCoder.HexStringToByteArray(encodedString);
            return encoded;
        }

        public string EncodeUnsignedIntString(BigInteger number, int length)
        {
            var hex = number.ToString("x");
            string encodedString = new string('0', length - hex.Length) + hex;
            return encodedString;
        }


    }
}