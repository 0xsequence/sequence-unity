using Sequence.ABI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Sequence.Utils;
using UnityEngine;

namespace Sequence.RLP
{
    public class RLP
    {
        /// <summary>
        /// Encodes the input object using Recursive Length Prefix (RLP) encoding.
        /// </summary>
        /// <param name="input">The input object to be encoded. If you have a string of characters and you need to represent it in UTF8 byte array</param>
        /// <returns>The RLP-encoded byte array.</returns>
        public static byte[] Encode(object input)
        {
            if(input is Boolean boolInput)
            {
                if(boolInput) return new byte[] { 0x01 };
                return new byte[] { 0x80 };
            }
            else if (input is byte[] byteArrayInput)
            {
                if(byteArrayInput.Length == 0)
                {
                    //Empty string
                    return new byte[] { 0x80 };
                }
                if (byteArrayInput.Length == 1 && byteArrayInput[0] < 128)
                {
                    //Integer 0
                    if(byteArrayInput[0] == 0) return new byte[] { 0x80 };

                    //For a single byte whose value is in the [0x00, 0x7f] (decimal [0, 127]) range, that byte is its own RLP encoding.
                    return byteArrayInput;
                }
                else
                {
                    byte[] head = SequenceCoder.HexStringToByteArray((EncodeLength(byteArrayInput.Length, 128)));
                    return ByteArrayExtensions.ConcatenateByteArrays(head, byteArrayInput);
                }
            }
            else if (input is List<object> listInput)
            {
                if(listInput.Count == 0)
                {
                    //empty list
                    return new byte[] { 0xc0 };
                }
                byte[] output = { };
                foreach (var item in listInput)
                {
                    output = ByteArrayExtensions.ConcatenateByteArrays(output, Encode(item));
                }
                byte[] head = SequenceCoder.HexStringToByteArray(EncodeLength(output.Length, 192));
                return ByteArrayExtensions.ConcatenateByteArrays(head, output);
            }
            return null;
        }

        /// <summary>
        /// Helper function, encodes the length of a data element in RLP format.
        /// </summary>
        /// <param name="length">The length of the data element.</param>
        /// <param name="offset">The offset value based on the encoding type.</param>
        /// <returns>The encoded length as a hexadecimal string.</returns>
        private static string EncodeLength(int length, int offset)
        {
            if (length < 56)
            {
                // if a string is 0-55 bytes long, the RLP encoding consists of a single byte with value 0x80 (dec. 128) plus the length of the string followed by the string. The range of the first byte is thus [0x80, 0xb7] (dec. [128, 183]).
                //If the total payload of a list (i.e. the combined length of all its items being RLP encoded) is 0-55 bytes long, the RLP encoding consists of a single byte with value 0xc0 plus the length of the list followed by the concatenation of the RLP encodings of the items.
                return (length + offset).ToString("x");
            }
            else if (length < Math.Pow(256, 8))
            {
                //If a string is more than 55 bytes long, the RLP encoding consists of a single byte with value 0xb7 (dec. 183) plus the length in bytes of the length of the string in binary form, followed by the length of the string, followed by the string. 
                //If the total payload of a list is more than 55 bytes long, the RLP encoding consists of a single byte with value 0xf7 plus the length in bytes of the length of the payload in binary form, followed by the length of the payload, followed by the concatenation of the RLP encodings of the items. 
                string lenString = length.ToString("x");
                if(lenString.Length % 2 !=0)
                {
                    //add zero 
                    lenString = '0' + lenString;

                }
                return (((lenString.Length)/2 + offset + 55)).ToString("x") + lenString;
            }
            else
                throw new Exception("Input too long");
        }

        /// <summary>
        /// Decodes the RLP-encoded byte array and returns the decoded object.
        /// </summary>
        /// <param name="input">The RLP-encoded byte array to be decoded.</param>
        /// <returns>The decoded object.</returns>
        public static object Decode(byte[] input)
        {
            if (input.Length == 0)
            {
                return null;
            }

            (int offset, int dataLen, Type type) = DecodeLength(input);

            string output = "";
            List<object> outputList = new List<object>();

            if (type == typeof(string))
            {
                output = Encoding.UTF8.GetString(input).Substring(offset, dataLen);
                UnityEngine.Debug.Log("output: " + output);
                return output;
            }
            else if (type == typeof(List<object>))
            {
                UnityEngine.Debug.Log("input length: " + input.Length);
                while (offset<input.Length)
                {
                    UnityEngine.Debug.Log("Decode offset: " + offset);
                    UnityEngine.Debug.Log("Decode dataLen: " + dataLen);
                    //TODO: Optimize 
                    object rtn = new List<object>(); 
                    int _offset= 0, _dataLen = 1;
                    Type _type;
                    if (dataLen > 0)                   
                    {
                        byte[] restInput = new byte[dataLen];
                        Array.Copy(input, offset, restInput, 0, dataLen);
                        ( _offset, _dataLen,  _type) = DecodeLength(restInput);
                        UnityEngine.Debug.Log("Decode _offset: " + _offset);
                        UnityEngine.Debug.Log("Decode _dataLen: " + _dataLen);
                        UnityEngine.Debug.Log("Decode _type: " + _type);
                        rtn = Decode(restInput);
                        UnityEngine.Debug.Log("rtn: " + rtn);

                       // outputList.Add(rtn);
                        /*offset += _dataLen;
                        dataLen -= _dataLen;*/
                        offset += _offset + _dataLen;
                        dataLen -= _offset + _dataLen;
                    }
                    else
                    {
                        UnityEngine.Debug.Log("rtn: " + ((List<object>)rtn).Count);
                        //outputList.Add(rtn);
                        return outputList;
                        

                    }
                    outputList.Add(rtn);
                    UnityEngine.Debug.Log("Decode offset!: " + offset);
                    UnityEngine.Debug.Log("Decode dataLen!: " + dataLen);

                }
                return outputList;
            }
            else
            {
                throw new Exception("Invalid type");
            }

        }

        /// <summary>
        /// Decodes the length of a data element in RLP format.
        /// </summary>
        /// <param name="input">The RLP-encoded byte array.</param>
        /// <returns>A tuple containing the offset, length of the actual data, and data type.</returns>
        private static (int, int, Type) DecodeLength(byte[] input)
        {
            int length = input.Length;
            UnityEngine.Debug.Log("Decode input length: " + length);
            if (length == 0)
            {
                throw new Exception("Input is null");
            }

            int firstByte = input[0];
            UnityEngine.Debug.Log("Decode firstbyte: " + firstByte);
            if (firstByte <= 127)
            {
                
                //the data is a string if the range of the first byte (i.e. prefix) is [0x00, 0x7f], and the string is the first byte itself exactly;
                return (0, 1, typeof(string));
            }            
            else if (firstByte <= 183 && length > (firstByte - 128))
            {
                //the data is a string if the range of the first byte is [0x80, 0xb7], and the string whose length is equal to the first byte minus 0x80 follows the first byte;
                int strLen = firstByte - 0x80;
                return (1, strLen, typeof(string));
            }
            else if (firstByte <= 191 && length > (firstByte - 183))
            {
                //the data is a string if the range of the first byte is [0xb8, 0xbf], and the length of the string whose length in bytes is equal to the first byte minus 0xb7 follows the first byte, and the string follows the length of the string;
                int lenOfStrLen = firstByte - 183;
                byte[] lenArray = new byte[lenOfStrLen];
                Array.Copy(input, 1, lenArray, 0, lenOfStrLen);
                string hexString = SequenceCoder.ByteArrayToHexString(lenArray);
                int strLen = int.Parse(hexString, System.Globalization.NumberStyles.HexNumber); 
                return (1 + lenOfStrLen, strLen, typeof(string));
            }
            else if (firstByte <= 247 && length > (firstByte - 192))
            {
                //the data is a list if the range of the first byte is [0xc0, 0xf7], and the concatenation of the RLP encodings of all items of the list which the total payload is equal to the first byte minus 0xc0 follows the first byte;
                int listLen = firstByte - 192;
                return (1, listLen, typeof(List<object>));
            }
            else if (firstByte <= 255 && length > (firstByte - 247 ))
            {
                //the data is a list if the range of the first byte is [0xf8, 0xff], and the total payload of the list whose length is equal to the first byte minus 0xf7 follows the first byte, and the concatenation of the RLP encodings of all items of the list follows the total payload of the list;
                int lenOfListLen = firstByte - 247;
                byte[] lenArray = new byte[lenOfListLen];
                Array.Copy(input, 1, lenArray, 0, lenOfListLen);
                string hexString = SequenceCoder.ByteArrayToHexString(lenArray);
                int listLen = int.Parse(hexString, System.Globalization.NumberStyles.HexNumber);
                return (1 + lenOfListLen, listLen, typeof(List<object>));
            }
            else
            {
                throw new Exception("Input doesn't conform to RLP encoding form");
            }
        }

    }
}