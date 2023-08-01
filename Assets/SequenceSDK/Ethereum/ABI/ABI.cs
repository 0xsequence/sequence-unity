using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using UnityEngine;
using System.Text.RegularExpressions;
using Org.BouncyCastle.Asn1.Esf;
using Org.BouncyCastle.Utilities;
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
                method = method.Replace(" ", ""); // Whitespace will mess with the function signature encoding and is easily left in by mistake
                string methodNameEncoded = FunctionSelector(method);
                List<ABIType> parameterTypes = GetParameterTypes(method);
                string parameterEncoded = _tupleCoder.EncodeToString(parameters, parameterTypes);
                return (methodNameEncoded + parameterEncoded);
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Error packing data: {ex.Message}");
            }
        }

        public static List<ABIType> GetParameterTypes(string functionSignature)
        {
            functionSignature = functionSignature.Substring(functionSignature.IndexOf('(') + 1);
            functionSignature = functionSignature.TrimEnd(')');
            string[] types = functionSignature.Split(',');
            int count = types.Length;
            List<ABIType> abiTypes = new List<ABIType>();

            for (int i = 0; i < count; i++)
            {
                string signature = types[i];
                ABIType type = GetTypeFromEvmName(signature);
                if (type == ABIType.FIXEDBYTES)
                {
                    int instanceCount = GetInnerValue(signature);
                    if (instanceCount > 0)
                    {
                        for (int j = 0; j < instanceCount; j++)
                        {
                            abiTypes.Add(ABIType.FIXEDBYTES);
                        }
                    }else
                    {
                        abiTypes.Add(ABIType.FIXEDBYTES);
                    }
                }
                else
                {
                    abiTypes.Add(type);
                }
            }
            return abiTypes;
        }

        private static ABIType GetTypeFromEvmName(string typeName)
        {
            if (typeName.StartsWith("("))
            {
                return ABIType.TUPLE;
            }
            if (typeName.StartsWith("bytes"))
            {
                if (typeName == "bytes")
                {
                    return ABIType.BYTES;
                }
                return ABIType.FIXEDBYTES;
            }
            if (IsFixedArray(typeName))
            {
                return ABIType.FIXEDARRAY;
            }
            if (typeName.EndsWith("[]"))
            {
                return ABIType.DYNAMICARRAY;
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
        private static int GetInnerValue(string value)
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
                Dictionary<string, List<(string[], string)>> decodedAbi = new Dictionary<string, List<(string[], string)>>();

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
                    int inputsArrayLength = inputsArray.Count;
                    string[] argumentTypes = new string[inputsArrayLength];
                    for (int j = 0; j < inputsArrayLength; j++)
                    {
                        JObject inputItem = inputsArray[j] as JObject;
                        argumentTypes[j] = inputItem["type"].ToString();
                    }
                    
                    JArray outputsArray = element["outputs"] as JArray;
                    string returnType = null;
                    if (outputsArray != null && outputsArray.Count > 0)
                    {
                        int outputsArrayLength = outputsArray.Count;
                        string[] outputTypes = new string[outputsArrayLength];
                        for (int j = 0; j < outputsArrayLength; j++)
                        {
                            JObject outputItem = outputsArray[j] as JObject;
                            outputTypes[j] = outputItem["type"].ToString();
                        }
                        returnType = $"({string.Join(", ", outputTypes)})";
                    }

                    if (decodedAbi.ContainsKey(functionName))
                    {
                        decodedAbi[functionName].Add((argumentTypes, returnType));
                    }
                    else
                    {
                        decodedAbi[functionName] = new List<(string[], string)>();
                        decodedAbi[functionName].Add((argumentTypes, returnType));
                    }
                }

                return new FunctionAbi(decodedAbi);
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Invalid ABI: {ex.Message}");
            }
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
                else if (param.GetType() == typeof(BigInteger) || param.GetType() == typeof(int) || param.GetType() == typeof(uint))
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
            /*
             Definition: The following types are called ?dynamic?:

                bytes

                string

                T[] for any T

                T[k] for any dynamic T and any k >= 0

                (T1,...,Tk) if Ti is dynamic for some 1 <= i <= k
            */
            return ((paramType == ABIType.BYTES) || (paramType == ABIType.STRING) ||
                    (paramType == ABIType.DYNAMICARRAY) || (paramType == ABIType.FIXEDARRAY) ||
                    (paramType == ABIType.TUPLE));
        }

        /// <summary>
        /// Retrieves the parameter types from an ABI JSON string.
        /// </summary>
        /// <param name="abi">The ABI JSON string.</param>
        /// <returns>A list of parameter types.</returns>
        public static List<object> GetParameterTypesFromABI(string abi)
        {
            try
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
            catch (Exception ex)
            {
                Debug.LogError($"Error retrieving parameter types from ABI: {ex.Message}");
            }
            return new List<object>();
        }

        /// <summary>
        /// Retrieves the parameter type based on its name.
        /// </summary>
        /// <param name="paramName">The name of the parameter type.</param>
        /// <returns>The corresponding ABIType.</returns>
        private static object GetParameterTypeByName(string paramName)
        {
            try
            {
                switch (paramName)
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

                if (paramName.Contains("[]"))
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
                
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error retrieving parameter type by name: {ex.Message}");
            }

            return string.Empty;
        }
    
        public static T Decode<T>(string value, string evmType)
        {
            ABIType type = GetTypeFromEvmName(evmType);
            value = value.WithoutHexPrefix();
            Queue<int> offsets = new Queue<int>();
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
                    ThrowDecodeException<T>(evmType, typeof(BigInteger).ToString(), typeof(int).ToString());
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
                    return (T)(object)Encoding.UTF8.GetBytes(value);
                case ABIType.FIXEDBYTES:
                    if (typeof(T) == typeof(byte[]))
                    {
                        return (T)(object)FixedBytesCoderExtensions.Decode(value);
                    }
                    if (typeof(T) == typeof(FixedByte))
                    {
                        byte[] bytes = FixedBytesCoderExtensions.Decode(value);
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

                    ABIType underlying = GetUnderlyingCollectionType(evmType);
                    string typeNonCollection = GetUnderlyingCollectionTypeName(evmType);
                    int instanceCount = GetInnerValue(evmType);
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
                        
                    // Sanity check - condition should never be met
                    if (instanceCount == -1)
                    {
                        throw new Exception(
                             $"Unexpected exception. System state is unexpected. Received {nameof(ABIType.FIXEDARRAY)} from {evmType} with instance count of -1");
                    }
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
                case ABIType.DYNAMICARRAY:
                    if (!CollectionUtils.IsCollection<T>())
                    {
                        ThrowDecodeException<T>(evmType, typeof(IEnumerable).ToString());
                    }

                    BigInteger size = (BigInteger)DecodeAsObject(value.Substring(0, 64), "uint");
                    if (size <= 0)
                    {
                        instanceType = CollectionUtils.GetUnderlyingType<T>();
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
                        object[] retValue = new object[count];
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
                                retValue[i] = DecodeAsObject(chunk, internalTypes[i]);
                            }
                        }
                        
                        for (int i = 0; i < count; i++)
                        {
                            ABIType internalType = GetTypeFromEvmName(internalTypes[i]);
                            if (IsDynamicType(internalType))
                            {
                                string nextChunk = value.Substring(offsets.Dequeue() * 2);
                                retValue[i] = DecodeAsObject(nextChunk, internalTypes[i]);
                            }
                        }

                        return (T)(object)retValue;
                    }
                    else
                    {
                        ABIType internalType = GetTypeFromEvmName(internalTypes[0]);
                        if (IsDynamicType(internalType))
                        {
                            string chunk = value.Substring(0, 64);
                            offsets.Enqueue(GetOffset(chunk, internalType));
                        }
                        else
                        {
                            return Decode<T>(value, internalTypes[0]);
                        }

                        int offset = offsets.Dequeue() * 2;
                        return Decode<T>(value.Substring(offset, value.Length - offset), internalTypes[0]);
                    }
            }
            throw new ArgumentException($"EVM type \'{evmType}\' is unsupported");
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
        
        private static string GetUnderlyingCollectionTypeName(string evmType)
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

        private static string[] GetTupleTypes(string evmType)
        {
            if (GetTypeFromEvmName(evmType) != ABIType.TUPLE)
            {
                throw new ArgumentException("Invalid method use. Expects a tuple evm type");
            }

            string withoutParenthesis = evmType.Substring(1, evmType.Length - 2);
            string[] types = withoutParenthesis.Split(", ");
            return types;
        }

        private static object[] ConvertToObjectArray<T>(this T value)
        {
            return value.ConvertToTArray<object,T>();
        }

        public static T[] ConvertToTArray<T,T2>(this T2 value)
        {
            if (value is Array array)
            {
                int length = array.Length;
                T[] converted = new T[length];

                for (int i = 0; i < length; i++)
                {
                    converted[i] = (T)array.GetValue(i);
                }

                return converted;
            }

            throw new ArgumentException(
                $"Value {value} with type {value.GetType()} is not an array as expected.");
        }
    }
}
