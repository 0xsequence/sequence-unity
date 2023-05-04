using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;


namespace Sequence.ABI
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
    public class ABI
    {
        static TupleCoder _tupleCoder = new TupleCoder();

       
        public static string Pack(string method, params object[] parameters)
        {
            UnityEngine.Debug.Log("method name: " + method);
            string methodNameEncoded = FunctionSelector(method);
            UnityEngine.Debug.Log("methodNameEncoded : " + methodNameEncoded);
            string parameterEncoded = _tupleCoder.EncodeToString(parameters);
            UnityEngine.Debug.Log("parametersEncoded : " + parameterEncoded);
            return (methodNameEncoded + parameterEncoded);
        }

        public static List<object> UnpackParameters(byte[] data)
        {
            throw new System.NotImplementedException();
        }

        public static string FunctionSelector(string functionSignature)
        {
            string hashed = SequenceCoder.KeccakHashASCII(functionSignature);
            hashed = hashed.Substring(0, 8);
            return "0x" + hashed;
        }

        public static ABIType GetParameterType(object param)
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
            else if (param.GetType() == typeof(string))
            {
                //TODO: make address a custom type  
                if (((string)param).Length == 42 && ((string)param).StartsWith("0x"))
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
            return "";
        }

    }
}
