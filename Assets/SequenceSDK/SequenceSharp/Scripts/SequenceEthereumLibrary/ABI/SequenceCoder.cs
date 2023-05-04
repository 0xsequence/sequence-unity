using System;
using System.Text;
using Org.BouncyCastle.Crypto.Digests;


namespace Sequence.ABI
{
    
    public class SequenceCoder
    {
        
        // Implemented based on  https://github.com/ethereum/EIPs/blob/master/EIPS/eip-55.md
        public static string AddressChecksum(string address)
        {
            if (address.StartsWith("0x"))
            {
                address = address.Substring(2);
            }
            string hashedAddress = KeccakHashASCII(address);
            string checksumAddress = "";
            int idx = 0;
            foreach(char c in address)
            {
                if("0123456789".Contains(c))
                {
                    checksumAddress += c;
                }
                else if("abcdef".Contains(c))
                {
                    int hashedAddressNibble = Convert.ToInt32(hashedAddress[idx].ToString(), 16);
                    
                    if(hashedAddressNibble > 7)
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

        

        public static string KeccakHashASCII(string input)
        {

            var keccak256 = new KeccakDigest(256);
            byte[] inputBytes = Encoding.ASCII.GetBytes(input);
            keccak256.BlockUpdate(inputBytes, 0, inputBytes.Length);
            byte[] result = new byte[keccak256.GetByteLength()];
            keccak256.DoFinal(result, 0);

            string hashString = BitConverter.ToString(result, 0,32);
            hashString = hashString.Replace("-", "").ToLowerInvariant();
            return hashString;
        }


        public static byte[] KeccakHash(byte[] input)
        {
            var keccak256 = new KeccakDigest(256);
            keccak256.BlockUpdate(input, 0, input.Length);
            byte[] result = new byte[keccak256.GetByteLength()];
            keccak256.DoFinal(result, 0);

            byte[] result64 = new byte[32];
            Array.Copy(result, 0, result64, 0, 32);
            return result64;

        }

        public static string KeccakHash(string input)
        {
            byte[] inputByte = HexStringToByteArray(input);
            byte[] keccak = KeccakHash(inputByte);
            return ByteArrayToHexString(keccak);           

        }



        // Hex string to byte array and vice versa
        // Ref:
        //https://stackoverflow.com/questions/311165/how-do-you-convert-a-byte-array-to-a-hexadecimal-string-and-vice-versa

        public static byte[] HexStringToByteArray(string hexString)
        {
            if (hexString == "") return new byte[] { };

            if (hexString.StartsWith("0x"))
            {
                hexString = hexString.Remove(0, 2);
            }
            if(hexString.Length %2 != 0)
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
                for (int i = 0; i < numberChars-1; i += 2)
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
         
        public static string ByteArrayToHexString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();

        }

       

    }
}