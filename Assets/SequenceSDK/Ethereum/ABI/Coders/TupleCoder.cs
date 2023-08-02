using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System;


namespace Sequence.ABI
{
    /// <summary>
    /// (T1,...,Tk) for k >= 0 and any types T1, ?, Tk
    ///enc(X) = head(X(1)) ... head(X(k)) tail(X(1)) ... tail(X(k))
    ///where X = (X(1), ..., X(k)) and head and tail are defined for Ti as follows:
    ///if Ti is static:
    ///head(X(i)) = enc(X(i)) and tail(X(i)) = "" (the empty string)
    ///otherwise, i.e. if Ti is dynamic:
    ///head(X(i)) = enc(len(head(X(1)) ... head(X(k)) tail(X(1)) ... tail(X(i-1)) )) tail(X(i)) = enc(X(i))

    ///Note that in the dynamic case, head(X(i)) is well-defined since the lengths of the head parts only depend on the types and not the values.The value of head(X(i)) is the offset of the beginning of tail(X(i)) relative to the start of enc(X).
    /// 
    /// </summary>
    public class TupleCoder : ICoder
    {
        
        AddressCoder _addressCoder = new AddressCoder();        
        BooleanCoder _booleanCoder = new BooleanCoder();
        FixedBytesCoder _fixedBytesCoder = new FixedBytesCoder();
        StaticBytesCoder _staticBytesCoder = new StaticBytesCoder();
        //BytesCoder _bytesCoder = new BytesCoder();
        NumberCoder _numberCoder = new NumberCoder();
        StringCoder _stringCoder = new StringCoder();

        public object Decode(byte[] encoded)
        {
            throw new System.NotImplementedException();
        }



        public byte[] Encode(object value)
        {
            string encodedStr = EncodeToString(value);

            return SequenceCoder.HexStringToByteArray(encodedStr);
        }



        public string EncodeToString(object value, List<ABIType> types = null)
        {
            //List<object> valueTuple = new List<object>();
            IList valueTuple = (IList) value;
            /*if (value.GetType().IsArray)
            {
                UnityEngine.Debug.Log("input tuple is array, fixed size T[K]");
                 valueTuple = ((object[])value).Cast<object>().ToList();
            }
            else
            {
                UnityEngine.Debug.Log("input tuple is list, T[]");
                valueTuple = (IList)value;// (List<object>)value;
            }*/
            int tupleLength = (valueTuple).Count;
            int headerTotalByteLength = tupleLength * 32;
            List<string> headList = new List<string>();
            List<string> tailList = new List<string>();
            int tailLength = 0;
            for (int i = 0; i < tupleLength; i++)
            {
                string head_i = "", tail_i = "";
                ABIType type;
                if (types == null)
                {
                    type = ABI.GetParameterType(valueTuple[i]);
                }else
                {
                    type = types[i];
                    ABIType temp = ABI.GetParameterType(valueTuple[i]);
                    if (temp != type && type != ABIType.FIXEDARRAY && type != ABIType.DYNAMICARRAY) // If it is a non-array data type, a mismatch will cause encoding issues - with arrays, a mismatch may cause encoding issues but it is difficult to predict
                    {
                        throw new ArgumentException($"Argument type is not as expected. Expected: {type} Received: {temp}");
                    }
                }


                switch (type)
                {
                    //Statics: head(X(i)) = enc(X(i) and tail(X(i)) = "" (the empty string)
                    case ABIType.BOOLEAN:
                        UnityEngine.Debug.Log("object in tuple array: boolean");
                        head_i = _booleanCoder.EncodeToString(valueTuple[i]);
                        break;
                    case ABIType.NUMBER:
                        UnityEngine.Debug.Log("object in tuple array: number");
                        head_i = _numberCoder.EncodeToString(valueTuple[i]);
                        break;
                    case ABIType.ADDRESS:
                        UnityEngine.Debug.Log("object in tuple array: address");
                        head_i = _addressCoder.EncodeToString(valueTuple[i]);
                        break;
                    case ABIType.FIXEDBYTES:
                        UnityEngine.Debug.Log("object in tuple array: static bytes");
                        head_i = _staticBytesCoder.EncodeToString(valueTuple[i]);
                        break;
                    //Dynamics: head(X(i)) = enc(len( head(X(1)) ... head(X(k)) tail(X(1)) ... tail(X(i-1)) )) tail(X(i)) = enc(X(i))
                    case ABIType.BYTES:
                        UnityEngine.Debug.Log("object in tuple array: bytes");
                        head_i = _numberCoder.EncodeToString((object)(headerTotalByteLength + tailLength));
                        tail_i = _fixedBytesCoder.EncodeToString(valueTuple[i]);
                        break;
                    case ABIType.STRING:
                        UnityEngine.Debug.Log("object in tuple array: string");
                        Encoding utf8 = Encoding.UTF8;
                        head_i = _numberCoder.EncodeToString((object)(headerTotalByteLength + tailLength));
                        tail_i = _fixedBytesCoder.EncodeToString(utf8.GetBytes((string)valueTuple[i]));
                        break;
                    case ABIType.DYNAMICARRAY:
                        UnityEngine.Debug.Log("object in tuple array: dynamic array");
                        head_i = _numberCoder.EncodeToString((object)(headerTotalByteLength + tailLength));
                        UnityEngine.Debug.Log("dynamic array head: " + head_i);
                        //intList.Cast<object>().ToList();
                        int numberCount = ((IList)valueTuple[i]).Count;
                        UnityEngine.Debug.Log("number count:" + numberCount);

                        string numberCountEncoded = _numberCoder.EncodeToString(numberCount);
                        UnityEngine.Debug.Log("dynamic array tail number count: " + numberCountEncoded);
                        tail_i = numberCountEncoded + EncodeToString(valueTuple[i]);

                        UnityEngine.Debug.Log("dynamic array tail: " + tail_i);
                        break;
                    case ABIType.FIXEDARRAY:
                        UnityEngine.Debug.Log("object in tuple array: fixed array");
                        head_i = _numberCoder.EncodeToString((object)(headerTotalByteLength + tailLength));
                        tail_i = EncodeToString(valueTuple[i]);
                        break;
                    case ABIType.TUPLE:
                        UnityEngine.Debug.Log("object in tuple array: tuple");
                        head_i = _numberCoder.EncodeToString((object)(headerTotalByteLength + tailLength));
                        tail_i = EncodeToString(valueTuple[i]);
                        break;
                    default:
                        break;
                }
                tailLength += tail_i.Length / 2; // 64 hex str-> 32 bytes

                headList.Add(head_i);
                tailList.Add(tail_i);


            }

            //concat head list and tail list
            string encoded = "";
            foreach (string headstr in headList)
            {
                encoded += headstr;
            }
            foreach (string tailstr in tailList)
            {
                encoded += tailstr;
            }

            return encoded;
        }
    }
}