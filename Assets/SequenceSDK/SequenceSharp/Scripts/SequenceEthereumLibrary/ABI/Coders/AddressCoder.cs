
using System.Globalization;
using System.Numerics;

namespace Sequence.ABI
{
    /// <summary>
    /// address: equivalent to uint160, except for the assumed interpretation and language typing. For computing the function selector, address is used.
    /// </summary>
    public class AddressCoder : ICoder
    {
        public object Decode(byte[] encoded)
        {
            string encodedString = SequenceCoder.ByteArrayToHexString(encoded);
            string decoded = SequenceCoder.AddressChecksum(DecodeFromString(encodedString));
            return decoded;
        }

        public T DefaultValue<T>()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// address: as in the uint160 case
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public byte[] Encode(object value)
        {
            //Trim 0x at the start
            string encodedString = EncodeToString(value);
            byte[] encoded = SequenceCoder.HexStringToByteArray(encodedString);
            return encoded;
        }

        public string EncodeToString(object value)
        {
            string address = (string)value;
            if (address.StartsWith("0x"))
            {
                address = address.Remove(0, 2);
            }

            string encodedString = new string('0', 64 - address.Length) + address;
            return encodedString.ToLower();
        }

        public string DecodeFromString(string encodedString)
        {
            //cut leading zeros
            encodedString = encodedString.TrimStart('0');
            return "0x" + encodedString;
        }

        public bool IsSupportedType()
        {
            throw new System.NotImplementedException();
        }
    }
}