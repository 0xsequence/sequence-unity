using System;
using System.Collections;
using System.Collections.Generic;
//using System.Numerics;
using System.Text;
using Org.BouncyCastle.Crypto.Digests;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Math;

namespace SequenceSharp.ABI
{
    /// <summary>
    /// https://docs.soliditylang.org/en/v0.8.13/abi-spec.html
    /// </summary>
    public enum ABIType
    {
        TUPLE,
        FIXEDARRAY,
        DYNAMICARRAY,
        BYTES,
        STRING,
        NUMBER,
        ADDRESS,
        BOOLEAN,
        NONE

    }
    public class SequenceCoder
    {
        /// <summary>
        /// https://docs.soliditylang.org/en/v0.8.13/abi-spec.html
        /// For any ABI value X, we recursively define enc(X), depending on the type of X being different types
        /// </summary>
        /// <param name="abi"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public static byte[] EncodeParameter(string abi, object parameter)
        {
            //Switch

            return new byte[] { };
        }

        public static object DecodeParameter(string abi, byte[] encoded)
        {
            return new object{ };
        }

        public static List<object> GetParameterTypesFromABI(string abi)
        {
            List<object> parameterTypes = new List<object>();
            // Parse ABI JSON string
            JObject abiJson = JObject.Parse(abi);

            // Access properties of ABI JSON
            string name = (string)abiJson["name"];
            JArray inputs = (JArray)abiJson["inputs"];

            foreach (JObject input in inputs)
            {
                string inputName = (string)input["name"];
                string inputType = (string)input["type"];
                object type = GetParameterTypeByName(inputType);
                parameterTypes.Add(type);
            }
            
            return parameterTypes;

        }

        private static object GetParameterTypeByName(string paramName)
        {
            switch(paramName)
            {
                case "uint256":
                    return ABIType.NUMBER;
                case "string":
                    return ABIType.STRING;
                case "bytes":
                    return ABIType.BYTES;
                case "bool":
                    return ABIType.BOOLEAN;
                case "address":
                    return ABIType.ADDRESS;
                default:
                    break;
            }

            if(paramName.Contains("[]"))
            {
                switch (paramName)
                {
                    case "uint256[]":
                        List<ABIType> numberList = new List<ABIType>(new ABIType[] { ABIType.NUMBER });
                        return numberList;

                    case "string[]":
                        List<ABIType> stringList = new List<ABIType>(new ABIType[] { ABIType.STRING });
                        return stringList;

                    case "bytes[]":
                        List<ABIType> byteList = new List<ABIType>(new ABIType[] { ABIType.BYTES });
                        return byteList;

                    case "bool[]":
                        List<ABIType> boolList = new List<ABIType>(new ABIType[] { ABIType.BOOLEAN });
                        return boolList;

                    case "address[]":
                        List<ABIType> addressList = new List<ABIType>(new ABIType[] { ABIType.ADDRESS });
                        return addressList;

                    default:
                        break;
                }
            }
            return "";
        }

        // Implemented based on  https://github.com/ethereum/EIPs/blob/master/EIPS/eip-55.md
        public static string AddressChecksum(string address)
        {
            if (address.StartsWith("0x"))
            {
                address = address.Substring(2);
            }
            string hashedAddress = KeccakHash(address);
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

        public static string FunctionSelector(string functionSignature)
        {
            string hashed = KeccakHash(functionSignature);
            hashed = hashed.Substring(0, 8);
            return "0x" + hashed;
        }

        public static string KeccakHash(string input)
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


            // Hex string to byte array and vice versa
            // Ref:
            //https://stackoverflow.com/questions/311165/how-do-you-convert-a-byte-array-to-a-hexadecimal-string-and-vice-versa

            public static byte[] HexStringToByteArray(string hexString)
        {

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

        public static ABIType GetParameterType(object param)
        {
            //Start with static type
            //Boolean, Integer, Unsigned integer, Address
            if (param.GetType() == typeof(bool))
            {
                return ABIType.BOOLEAN;
            }
            else if (param.GetType() == typeof(BigInteger)||param.GetType() == typeof(int)|| param.GetType() == typeof(uint))
            {
                return ABIType.NUMBER;
            }
            else if (param.GetType() == typeof(string))
            {
                //TODO: make address a custom type  
                if(((string)param).Length == 42 && ((string)param).StartsWith("0x"))
                {
                    return ABIType.ADDRESS;
                }
                return ABIType.STRING;
            }
            else if (param is System.Runtime.CompilerServices.ITuple)
            {
                
                return ABIType.TUPLE;
            }
            else
            {
                IEnumerable paramEnumerable = param as IEnumerable;
                if (paramEnumerable != null)
                {
                    //IEnumerable types
                    var type = param.GetType();
                    string name = type.Name;
                    //Support ArrayList, List and Array (Bytes is considered byte array) as of now
                    if (param is IList &&
           param.GetType().IsGenericType &&
           param.GetType().GetGenericTypeDefinition().IsAssignableFrom(typeof(List<>)))
                    {
                        return ABIType.DYNAMICARRAY;
                    }
                    else if (param.GetType().IsArray)
                    {
                        foreach(var item in paramEnumerable)
                        {
                            if (item.GetType() == typeof(System.Byte))
                                return ABIType.BYTES;
                            break;
                        }
                        return ABIType.FIXEDARRAY;
                    }

                }

                return ABIType.NONE;
                
            }
        }

        public static bool IsStaticType(ABIType paramType)
        {
            //Boolean, Integer, Unsigned integer, Address
            if ((paramType == ABIType.BOOLEAN) || (paramType == ABIType.NUMBER) || (paramType == ABIType.ADDRESS))
                return true;
            return false;
        }


        public static bool IsDynamicType(ABIType paramType)
        {
            /*
             Definition: The following types are called “dynamic”:

                bytes

                string

                T[] for any T

                T[k] for any dynamic T and any k >= 0

                (T1,...,Tk) if Ti is dynamic for some 1 <= i <= k
            */

            if ((paramType == ABIType.BYTES) || (paramType == ABIType.STRING) || (paramType == ABIType.DYNAMICARRAY) || (paramType == ABIType.FIXEDARRAY) || (paramType == ABIType.TUPLE))
                return true;
            return false;
        }

    }
}