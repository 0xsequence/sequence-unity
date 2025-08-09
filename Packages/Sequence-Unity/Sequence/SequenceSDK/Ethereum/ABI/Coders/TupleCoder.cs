using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System;
using Sequence.Utils;
using UnityEngine;


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
            throw new System.NotImplementedException();
        }
        
        public string EncodeToString(object value, string[] evmTypes)
        {
            if (evmTypes == null)
            {
                throw new ArgumentException($"{nameof(evmTypes)} must not be null");
            }

            if (value == null)
            {
                return new string('0', 32);
            }

            if (value.IsTuple())
            {
                value = value.ToObjectList();
            }

            IList valueTuple = (IList)value;
            if (evmTypes.Length == 1 && (ABI.GetTypeFromEvmName(evmTypes[0]) == ABIType.FIXEDARRAY ||
                                         ABI.GetTypeFromEvmName(evmTypes[0]) == ABIType.DYNAMICARRAY) &&
                valueTuple.Count > 1)
            {
                valueTuple = (IList)new object[] { value };
            }
            int tupleLength = (valueTuple).Count;
            if (evmTypes.Length != tupleLength)
            {
                throw new ArgumentException($"{nameof(evmTypes)} must have as many elements as {nameof(value)}. {nameof(evmTypes)} has {evmTypes.Length} elements while {nameof(value)} has {tupleLength} elements");
            }
            int headerTotalByteLength = tupleLength * 32;
            List<string> headList = new List<string>();
            List<string> tailList = new List<string>();
            int tailLength = 0;
            for (int i = 0; i < tupleLength; i++)
            {
                string head_i = "", tail_i = "";
                ABIType type = ABI.GetTypeFromEvmName(evmTypes[i]);
                ABIType temp = type;
                if (valueTuple[i] != null)
                {
                    temp = ABI.GetParameterType(valueTuple[i]);
                }
                if (temp != type && type != ABIType.FIXEDARRAY && type != ABIType.DYNAMICARRAY) // If it is a non-array data type, a mismatch will cause encoding issues - with arrays, a mismatch may cause encoding issues but it is difficult to predict
                {
                    throw new ArgumentException($"Argument type is not as expected. Expected: {type} Received: {temp}");
                }

                switch (type)
                {
                    //Statics: head(X(i)) = enc(X(i) and tail(X(i)) = "" (the empty string)
                    case ABIType.BOOLEAN:
                        SequenceLog.Info("object in tuple array: boolean");
                        head_i = _booleanCoder.EncodeToString(valueTuple[i]);
                        break;
                    case ABIType.NUMBER:
                        SequenceLog.Info("object in tuple array: number");
                        head_i = _numberCoder.EncodeToString(valueTuple[i]);
                        break;
                    case ABIType.ADDRESS:
                        SequenceLog.Info("object in tuple array: address");
                        head_i = _addressCoder.EncodeToString(valueTuple[i]);
                        break;
                    case ABIType.FIXEDBYTES:
                        SequenceLog.Info("object in tuple array: static bytes");
                        head_i = _staticBytesCoder.EncodeToString(valueTuple[i]);
                        break;
                    //Dynamics: head(X(i)) = enc(len( head(X(1)) ... head(X(k)) tail(X(1)) ... tail(X(i-1)) )) tail(X(i)) = enc(X(i))
                    case ABIType.BYTES:
                        head_i = _numberCoder.EncodeToString((object)(headerTotalByteLength + tailLength));
                        tail_i = _fixedBytesCoder.EncodeToString(valueTuple[i]);
                        break;
                    case ABIType.STRING:
                        SequenceLog.Info("object in tuple array: string");
                        Encoding utf8 = Encoding.UTF8;
                        head_i = _numberCoder.EncodeToString((object)(headerTotalByteLength + tailLength));
                        tail_i = _fixedBytesCoder.EncodeToString(utf8.GetBytes((string)valueTuple[i]));
                        break;
                    case ABIType.DYNAMICARRAY:
                        SequenceLog.Info("object in tuple array: dynamic array");
                        head_i = _numberCoder.EncodeToString((object)(headerTotalByteLength + tailLength));
                        SequenceLog.Info("dynamic array head: " + head_i);
                        //intList.Cast<object>().ToList();
                        int numberCount = 0;
                        if (valueTuple[i] != null)
                        {
                            numberCount = ((IList)valueTuple[i]).Count;
                        }
                        SequenceLog.Info("number count:" + numberCount);

                        string numberCountEncoded = _numberCoder.EncodeToString(numberCount);
                        SequenceLog.Info("dynamic array tail number count: " + numberCountEncoded);
                        tail_i = numberCountEncoded + EncodeToString(valueTuple[i], 
                            ArrayUtils.BuildArrayWithRepeatedValue(ABI.GetUnderlyingCollectionTypeName(evmTypes[i]), numberCount));

                        SequenceLog.Info("dynamic array tail: " + tail_i);
                        break;
                    case ABIType.FIXEDARRAY:
                        SequenceLog.Info("object in tuple array: fixed array");
                        numberCount = ABI.GetInnerValue(evmTypes[i]);
                        head_i = EncodeToString(valueTuple[i], 
                            ArrayUtils.BuildArrayWithRepeatedValue(ABI.GetUnderlyingCollectionTypeName(evmTypes[i]), numberCount));
                        break;
                    case ABIType.TUPLE:
                        SequenceLog.Info("object in tuple array: tuple");
                        head_i = _numberCoder.EncodeToString((object)(headerTotalByteLength + tailLength));
                        tail_i = EncodeToString(valueTuple[i], ABI.GetTupleTypes(evmTypes[i]));
                        break;
                    case ABIType.NONE:
                        throw new ArgumentException(
                            $"Must be a valid ABIType not {nameof(ABIType.NONE)}, given {valueTuple[i]}");
                }
                tailLength += tail_i.Length / 2; // 64 hex str-> 32 bytes

                headList.Add(head_i);
                tailList.Add(tail_i);
            }

            return ConcatenateHeadsAndTails(headList, tailList);
        }

        private string ConcatenateHeadsAndTails(List<string> head, List<string> tail)
        {
            StringBuilder stringBuilder = new StringBuilder();
            int length = head.Count;
            if (tail.Count != length)
            {
                throw new ArgumentException("Head and Tail lists must be the same length");
            }

            for (int i = 0; i < length; i++)
            {
                stringBuilder.Append(head[i]);
            }

            for (int i = 0; i < length; i++)
            {
                stringBuilder.Append(tail[i]);
            }

            return stringBuilder.ToString();
        }
    }
}