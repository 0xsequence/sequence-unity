using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using UnityEngine;
using Sequence.Utils;

namespace Sequence.ABI
{
    /// <summary>
    /// Represents the types defined in the Solidity ABI specification.
    /// Reference: https://docs.soliditylang.org/en/v0.8.13/abi-spec.html
    /// </summary>
    public enum ABIType
    {
        TUPLE,
        FIXEDARRAY,
        DYNAMICARRAY,
        BYTES,
        FIXEDBYTES,
        STRING,
        NUMBER,
        ADDRESS,
        BOOLEAN,
        NONE
    }

    public static class ABI
    {
        static TupleCoder _tupleCoder = new TupleCoder();

        /// <summary>
        /// Packs the method name and parameters into a string representation of the encoded data.
        /// </summary>
        /// <param name="method">The method name.</param>
        /// <param name="parameters">The parameters to encode.</param>
        /// <returns>The encoded data string.</returns>
        public static string Pack(string method, params object[] parameters)
        {
            try
            {
                method = method.Replace(" ",
                    ""); // Whitespace will mess with the function signature encoding and is easily left in by mistake
                string methodNameEncoded = FunctionSelector(method);
                string[] evmTypes = GetTupleTypes(method.Substring(method.IndexOf('(')));
                string parameterEncoded = _tupleCoder.EncodeToString(parameters, evmTypes);
                return (methodNameEncoded + parameterEncoded);
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Error packing data: {ex.Message}");
            }
        }

        public static ABIType GetTypeFromEvmName(string typeName)
        {
            if (typeName.EndsWith("[]"))
            {
                return ABIType.DYNAMICARRAY;
            }
            
            if (typeName.StartsWith("("))
            {
                return ABIType.TUPLE;
            }

            if (IsFixedArray(typeName))
            {
                return ABIType.FIXEDARRAY;
            }


            if (typeName.StartsWith("bytes"))
            {
                if (typeName == "bytes")
                {
                    return ABIType.BYTES;
                }

                return ABIType.FIXEDBYTES;
            }

            if (typeName == "address")
            {
                return ABIType.ADDRESS;
            }

            if (typeName == "bool")
            {
                return ABIType.BOOLEAN;
            }

            if (typeName.StartsWith("uint") || typeName.StartsWith("int"))
            {
                return ABIType.NUMBER;
            }

            if (typeName == "string")
            {
                return ABIType.STRING;
            }

            return ABIType.NONE;
        }

        /// <summary>
        /// Determines if a string is of type ABIType.FIXEDARRAY
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static bool IsFixedArray(string value)
        {
            int start = value.IndexOf('[');
            if (start <= 0)
            {
                return false;
            }

            return GetInnerValue(value) > 0;
        }

        /// <summary>
        /// Returns the int value within square brackets '[' and ']' in a string
        /// Returns -1 if there is no int value within square brackets
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int GetInnerValue(string value)
        {
            int start = value.IndexOf('[');
            if (start <= 0)
            {
                return -1;
            }

            int end = value.IndexOf(']');
            if (end <= start)
            {
                throw new ArgumentException($"Invalid string provided: {value}");
            }

            string inner = value.Substring(start + 1, end - start - 1);
            bool isNumber = int.TryParse(inner, out int number);
            if (isNumber)
            {
                return number;
            }
            else
            {
                return -1;
            }
        }

        /// <summary>
        /// Decodes an ABI string into a Dictionary<string, List<(string[], string)>> where the key is the function name and the value is a list of tuple (function argument types, function return type)
        /// Returns a FunctionAbi, a wrapper class for Dictionary<string, List<(string[], string)>> that provides a number of utility functions
        /// </summary>
        /// <param name="abi"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static FunctionAbi DecodeAbi(string abi)
        {
            try
            {
                JArray abiArray = JArray.Parse(abi);
                Dictionary<string, List<(string[], string)>> decodedAbi =
                    new Dictionary<string, List<(string[], string)>>();

                int abiArrayLength = abiArray.Count;
                for (int i = 0; i < abiArrayLength; i++)
                {
                    JObject element = abiArray[i] as JObject;
                    if (element["type"].ToString() != "function")
                    {
                        continue; // Skip everything that isn't a function (e.g. events) for now
                    }

                    string functionName = element["name"].ToString();

                    JArray inputsArray = element["inputs"] as JArray;
                    string[] argumentTypes = ExtractTypes(inputsArray);

                    JArray outputsArray = element["outputs"] as JArray;
                    string returnType = null;
                    if (outputsArray != null && outputsArray.Count > 0)
                    {
                        string[] outputTypes = ExtractTypes(outputsArray);
                        returnType = $"({string.Join(", ", outputTypes)})";
                    }

                    decodedAbi = AddToDictionary(decodedAbi, functionName, argumentTypes, returnType);
                }

                return new FunctionAbi(decodedAbi);
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Invalid ABI: {ex.Message}");
            }
        }

        private static string[] ExtractTypes(JArray array)
        {
            int length = array.Count;
            List<string> result = new List<string>();
            for (int i = 0; i < length; i++)
            {
                JObject item = array[i] as JObject;
                result[i] = item["type"].ToString();
                if (result[i].Contains("tuple"))
                {
                    StringBuilder tupleType = new StringBuilder();
                    tupleType.Append("(");
                    JArray components = item["components"] as JArray;
                    int componentsLength = components.Count;
                    for (int j = 0; j < componentsLength; j++)
                    {
                        JObject component = components[j] as JObject;
                        tupleType.Append(component["type"].ToString());
                        if (j < componentsLength - 1)
                        {
                            tupleType.Append(",");
                        }
                    }
                    tupleType.Append(")");
                    if (result[i].EndsWith("[]"))
                    {
                        tupleType.Append("[]");
                    }
                    result[i] = tupleType.ToString();
                }
            }

            return result.ToArray();
        }

        private static Dictionary<string, List<(string[], string)>> AddToDictionary(
            Dictionary<string, List<(string[], string)>> dict, string key, string[] value1, string value2)
        {
            if (dict.ContainsKey(key))
            {
                dict[key].Add((value1, value2));
            }
            else
            {
                dict[key] = new List<(string[], string)>();
                dict[key].Add((value1, value2));
            }

            return dict;
        }

        /// <summary>
        /// Generates the function selector for a given function signature.
        /// </summary>
        /// <param name="functionSignature">The function signature.</param>
        /// <returns>The function selector.</returns>
        public static string FunctionSelector(string functionSignature)
        {
            try
            {
                string hashed = SequenceCoder.KeccakHashASCII(functionSignature);
                hashed = hashed.Substring(0, 8);
                return "0x" + hashed;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error generating function selector: {ex.Message}");
                return string.Empty;
            }
        }

        /// <summary>
        /// Determines the ABIType of a given parameter object.
        /// </summary>
        /// <param name="param">The parameter object.</param>
        /// <returns>The ABIType of the parameter.</returns>
        public static ABIType GetParameterType(object param)
        {
            try
            {
                //Start with static type
                //Boolean, Integer, Unsigned integer, Address
                if (param.GetType() == typeof(bool))
                {
                    return ABIType.BOOLEAN;
                }
                else if (param.GetType() == typeof(BigInteger) || param.GetType() == typeof(int) ||
                         param.GetType() == typeof(uint))
                {
                    return ABIType.NUMBER;
                }
                else if (param.GetType() == typeof(Address))
                {
                    return ABIType.ADDRESS;
                }
                else if (param.GetType() == typeof(string))
                {
                    if (((string)param).IsAddress())
                    {
                        return ABIType.ADDRESS;
                    }

                    return ABIType.STRING;
                }
                else if (param is System.Runtime.CompilerServices.ITuple)
                {
                    return ABIType.TUPLE;
                }
                else if ((param.GetType() == typeof(FixedByte)))
                {
                    return ABIType.FIXEDBYTES;
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
                            foreach (var item in paramEnumerable)
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
            catch (Exception ex)
            {
                Debug.LogError($"Error determining parameter type: {ex.Message}");
            }

            return ABIType.NONE;
        }


        /// <summary>
        /// Determines if a given ABIType is a static type.
        /// </summary>
        /// <param name="paramType">The ABIType to check.</param>
        /// <returns>True if the ABIType is static; otherwise, false.</returns>
        public static bool IsStaticType(ABIType paramType)
        {
            return ((paramType == ABIType.BOOLEAN) || (paramType == ABIType.NUMBER) || (paramType == ABIType.ADDRESS));
        }

        /// <summary>
        /// Determines if a given ABIType is a dynamic type.
        /// </summary>
        /// <param name="paramType">The ABIType to check.</param>
        /// <returns>True if the ABIType is dynamic; otherwise, false.</returns>
        public static bool IsDynamicType(ABIType paramType)
        {
            return ((paramType == ABIType.BYTES) || (paramType == ABIType.STRING) ||
                    (paramType == ABIType.DYNAMICARRAY) || (paramType == ABIType.FIXEDARRAY) ||
                    (paramType == ABIType.TUPLE));
        }

        public static T Decode<T>(string value, string evmType)
        {
            ABIType type = GetTypeFromEvmName(evmType);
            value = value.WithoutHexPrefix();
            switch (type)
            {
                case ABIType.STRING:
                    if (typeof(T) != typeof(string))
                    {
                        ThrowDecodeException<T>(evmType, typeof(string).ToString());
                    }

                    return (T)(object)StringCoderExtensions.DecodeFromString(value);
                case ABIType.ADDRESS:
                    if (typeof(T) == typeof(Address))
                    {
                        string address = AddressCoderExtensions.Decode(value);
                        return (T)(object)new Address(address);
                    }

                    if (typeof(T) == typeof(string))
                    {
                        string address = AddressCoderExtensions.Decode(value);
                        return (T)(object)address;
                    }

                    ThrowDecodeException<T>(evmType, typeof(Address).ToString(), typeof(string).ToString());
                    break;
                case ABIType.NUMBER:
                    if (typeof(T) == typeof(BigInteger))
                    {
                        return (T)(object)value.HexStringToBigInteger();
                    }

                    if (typeof(T) == typeof(int))
                    {
                        return (T)(object)value.HexStringToInt();
                    }
                    
                    if (typeof(T) == typeof(uint))
                    {
                        return (T)(object)(uint)value.HexStringToBigInteger();
                    }

                    ThrowDecodeException<T>(evmType, typeof(BigInteger).ToString(), typeof(int).ToString(), typeof(uint).ToString());
                    break;
                case ABIType.BOOLEAN:
                    if (typeof(T) != typeof(bool))
                    {
                        ThrowDecodeException<T>(evmType, typeof(bool).ToString());
                    }

                    return (T)(object)value.HexStringToBool();
                case ABIType.BYTES:
                    if (typeof(T) != typeof(byte[]))
                    {
                        ThrowDecodeException<T>(evmType, typeof(byte[]).ToString());
                    }

                    return (T)(object)BytesCoderExtensions.Decode(value);
                case ABIType.FIXEDBYTES:
                    if (typeof(T) == typeof(byte[]))
                    {
                        return (T)(object)StaticBytesCoderExtensions.Decode(value);
                    }

                    if (typeof(T) == typeof(FixedByte))
                    {
                        byte[] bytes = StaticBytesCoderExtensions.Decode(value);
                        int length = bytes.Length;
                        return (T)(object)new FixedByte(length, bytes);
                    }

                    ThrowDecodeException<T>(evmType, typeof(byte[]).ToString(), typeof(FixedByte).ToString());
                    break;
                case ABIType.FIXEDARRAY:
                    if (!CollectionUtils.IsCollection<T>())
                    {
                        ThrowDecodeException<T>(evmType, typeof(IEnumerable).ToString());
                    }

                    return DecodeFixedArray<T>(value, evmType);
                case ABIType.DYNAMICARRAY:
                    if (!CollectionUtils.IsCollection<T>())
                    {
                        ThrowDecodeException<T>(evmType, typeof(IEnumerable).ToString());
                    }

                    BigInteger size = (BigInteger)DecodeAsObject(value.Substring(0, 64), "uint");
                    if (size <= 0)
                    {
                        Type instanceType = CollectionUtils.GetUnderlyingType<T>();
                        return (T)(object)Array.CreateInstance(instanceType, 0);
                    }

                    return Decode<T>(value.Substring(64), evmType.WithFixedSize(size));
                case ABIType.TUPLE:
                    string[] internalTypes = GetTupleTypes(evmType);
                    int count = internalTypes.Length;
                    if (count > 1)
                    {
                        if (typeof(T) != typeof(object[]))
                        {
                            ThrowDecodeException<T>(evmType, typeof(object[]).ToString());
                        }

                        return (T)(object)DecodeTuple(value, internalTypes, count);
                    }
                    else
                    {
                        ABIType internalType = GetTypeFromEvmName(internalTypes[0]);
                        if (IsDynamicType(internalType))
                        {
                            string chunk = value.Substring(0, 64);
                            int offset = GetOffset(chunk, internalType) * 2;
                            return Decode<T>(value.Substring(offset, value.Length - offset), internalTypes[0]);
                        }
                        else
                        {
                            return Decode<T>(value, internalTypes[0]);
                        }
                    }
            }

            throw new ArgumentException($"EVM type \'{evmType}\' is unsupported");
        }

        private static T DecodeFixedArray<T>(string value, string evmType)
        {
            ABIType underlying = GetUnderlyingCollectionType(evmType);
            string typeNonCollection = GetUnderlyingCollectionTypeName(evmType);
            int instanceCount = GetInnerValue(evmType);
            // Sanity check - condition should never be met
            if (instanceCount == -1)
            {
                throw new Exception(
                    $"Unexpected exception. System state is unexpected. Received {nameof(ABIType.FIXEDARRAY)} from {evmType} with invalid instance count");
            }

            Type instanceType = CollectionUtils.GetUnderlyingType<T>();
            if (instanceType == typeof(object))
            {
                instanceType = TryAndInferType(typeNonCollection);
            }

            if (CollectionUtils.IsCollection(instanceType))
            {
                instanceType = typeof(object[]);
            }

            var returnValue = Array.CreateInstance(instanceType, instanceCount);
            if (IsStaticType(underlying))
            {
                for (int i = 0; i < instanceCount; i++)
                {
                    string nextChunk = value.Substring(i * 64, 64);
                    object nextChunkValue = DecodeAsObject(nextChunk, typeNonCollection);
                    returnValue.SetValue(Convert.ChangeType(nextChunkValue, instanceType), i);
                }

                if (typeof(T) == typeof(object[]))
                {
                    return (T)(object)ConvertToObjectArray(returnValue);
                }

                return (T)(object)returnValue;
            }
            else
            {
                Queue<int> offsets = new Queue<int>();
                for (int i = 0; i < instanceCount; i++)
                {
                    string chunk = value.Substring(i * 64, 64);
                    offsets.Enqueue(GetOffset(chunk, underlying));
                }

                for (int i = 0; i < instanceCount; i++)
                {
                    string nextChunk = value.Substring(offsets.Dequeue() * 2);
                    object nextChunkValue = DecodeAsObject(nextChunk, typeNonCollection);
                    if (instanceType != typeof(object))
                    {
                        returnValue.SetValue(Convert.ChangeType(nextChunkValue, instanceType), i);
                    }
                    else
                    {
                        returnValue.SetValue(nextChunkValue, i);
                    }
                }

                if (typeof(T) == typeof(object[]))
                {
                    return (T)(object)ConvertToObjectArray(returnValue);
                }

                return (T)(object)returnValue;
            }
        }

        private static object[] DecodeTuple(string value, string[] internalTypes, int count)
        {
            Queue<int> offsets = new Queue<int>();
            object[] returnValue = new object[count];
            for (int i = 0; i < count; i++)
            {
                string chunk = value.Substring(i * 64, 64);
                ABIType internalType = GetTypeFromEvmName(internalTypes[i]);
                if (IsDynamicType(internalType))
                {
                    offsets.Enqueue(GetOffset(chunk, internalType));
                }
                else
                {
                    returnValue[i] = DecodeAsObject(chunk, internalTypes[i]);
                }
            }

            for (int i = 0; i < count; i++)
            {
                ABIType internalType = GetTypeFromEvmName(internalTypes[i]);
                if (IsDynamicType(internalType))
                {
                    string nextChunk = value.Substring(offsets.Dequeue() * 2);
                    returnValue[i] = DecodeAsObject(nextChunk, internalTypes[i]);
                }
            }

            return returnValue;
        }

        private static int GetOffset(string value, ABIType type)
        {
            if (!IsDynamicType(type))
            {
                throw new ArgumentException("Invalid method use. Please only use on dynamic ABITypes");
            }

            return value.Substring(0, 64).HexStringToInt();
        }

        private static object DecodeAsObject(string value, string evmType)
        {
            ABIType type = GetTypeFromEvmName(evmType);
            switch (type)
            {
                case ABIType.STRING:
                    return Decode<string>(value, evmType);
                case ABIType.ADDRESS:
                    return Decode<Address>(value, evmType);
                case ABIType.NUMBER:
                    return Decode<BigInteger>(value, evmType);
                case ABIType.BOOLEAN:
                    return Decode<bool>(value, evmType);
                case ABIType.BYTES:
                    return Decode<byte[]>(value, evmType);
                case ABIType.FIXEDBYTES:
                    return Decode<byte[]>(value, evmType);
                case ABIType.FIXEDARRAY:
                    return Decode<object[]>(value, evmType);
                case ABIType.DYNAMICARRAY:
                    return Decode<object[]>(value, evmType);
                case ABIType.TUPLE:
                    return Decode<object[]>(value, evmType);
            }

            throw new ArgumentException($"ABIType {type} is not supported");
        }

        private static ABIType GetUnderlyingCollectionType(string evmType)
        {
            string typeNonCollection = GetUnderlyingCollectionTypeName(evmType);
            return GetTypeFromEvmName(typeNonCollection);
        }

        public static string GetUnderlyingCollectionTypeName(string evmType)
        {
            string prefix = evmType.Substring(0, evmType.IndexOf('['));
            string postfix = "";
            int postfixIndex = evmType.IndexOf(']');
            if (postfixIndex + 1 < evmType.Length)
            {
                postfix = evmType.Substring(postfixIndex + 1);
            }

            string underlying = prefix + postfix;
            return underlying;
        }

        /// <summary>
        /// WithFixedSize will modify value such that
        /// if value contains an instance of '[' immediately followed by an instance of ']', it will replace the first
        /// occurrence of this phenomenon with $"[{size}]" and return the result
        /// 
        /// Examples:
        /// value = uint[5][][couch] , size = 11 => uint[5][11][couch]
        /// value = banana[house] , size = 2 => banana[house]
        /// </summary>
        /// <param name="value"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        private static string WithFixedSize(this string value, BigInteger size)
        {
            int valueLength = value.Length;
            int startIndex = -2;
            for (int i = 0; i < valueLength; i++)
            {
                if (startIndex + 1 == i)
                {
                    if (value[i] != ']')
                    {
                        startIndex = -2;
                    }
                    else
                    {
                        break;
                    }
                }

                if (value[i] == '[')
                {
                    startIndex = i;
                }
            }

            if (startIndex == -2)
            {
                return value;
            }

            string prefix = value.Substring(0, startIndex + 1);
            string postfix = value.Substring(startIndex + 1);
            return prefix + size + postfix;
        }

        private static Type TryAndInferType(string evmType)
        {
            ABIType type = GetTypeFromEvmName(evmType);
            switch (type)
            {
                case ABIType.STRING:
                    return typeof(string);
                case ABIType.ADDRESS:
                    return typeof(Address);
                case ABIType.NUMBER:
                    return typeof(BigInteger);
                case ABIType.BOOLEAN:
                    return typeof(bool);
                case ABIType.BYTES:
                    return typeof(byte[]);
                case ABIType.FIXEDBYTES:
                    return typeof(byte[]);
            }

            return typeof(object);
        }

        private static void ThrowDecodeException<T>(string evmType, params string[] supportedTypes)
        {
            throw new ArgumentException(
                $"Unable to decode to type \'{typeof(T)}\' when ABI expects to decode to type \'{evmType}\'. Supported types: {string.Join(", ", supportedTypes)}");
        }

        public static string[] GetTupleTypes(string evmType)
        {
            if (GetTypeFromEvmName(evmType) != ABIType.TUPLE)
            {
                throw new ArgumentException("Invalid method use. Expects a tuple evm type");
            }

            string withoutParenthesis = evmType.Substring(1, evmType.Length - 2).Replace(" ", "");

            if (string.IsNullOrEmpty(withoutParenthesis))
            {
                return Array.Empty<string>();
            }

            List<string> types = new List<string>();
            int start = 0;
            int nestedLevel = 0;

            for (int i = 0; i < withoutParenthesis.Length; i++)
            {
                char currentChar = withoutParenthesis[i];

                if (currentChar == '(')
                {
                    nestedLevel++;
                }
                else if (currentChar == ')')
                {
                    nestedLevel--;
                }
                else if (currentChar == ',' && nestedLevel == 0)
                {
                    types.Add(withoutParenthesis.Substring(start, i - start));
                    start = i + 1;
                }
            }

            types.Add(withoutParenthesis.Substring(start));

            return types.ToArray();
        }

        private static object[] ConvertToObjectArray<T>(this T value)
        {
            return value.ConvertToTArray<object, T>();
        }
    }
}