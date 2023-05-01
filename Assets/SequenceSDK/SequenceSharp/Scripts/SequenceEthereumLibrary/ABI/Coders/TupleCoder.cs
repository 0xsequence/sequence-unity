using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;

namespace Sequence.ABI
{
    /// <summary>
    /// (T1,...,Tk) for k >= 0 and any types T1, …, Tk
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
        //TODO: mmmmmmm,  will refactor these coders, maybe put them in sequencecoders and make some static methods to call
        AddressCoder _addressCoder = new AddressCoder();
        //ArrayCoder _arrayCoder = new ArrayCoder();
        BooleanCoder _booleanCoder = new BooleanCoder();
        FixedBytesCoder _fixedBytesCoder = new FixedBytesCoder();
        BytesCoder _bytesCoder = new BytesCoder();
        NumberCoder _numberCoder = new NumberCoder();
        StringCoder _stringCoder = new StringCoder();

        public object Decode(byte[] encoded)
        {
            throw new System.NotImplementedException();
        }

        public T DefaultValue<T>()
        {
            throw new System.NotImplementedException();
        }


        public byte[] Encode(object value)
        {
            string encodedStr = EncodeToString(value);

            return SequenceCoder.HexStringToByteArray(encodedStr);

        }



        public string EncodeToString(object value)
        {

            List<object> valueTuple = (List<object>)value;

            int tupleLength = valueTuple.Count;
            int headerTotalByteLength = tupleLength * 32;
            List<string> headList = new List<string>();
            List<string> tailList = new List<string>();
            int tailLength = 0;
            for (int i = 0; i < tupleLength; i++)
            {
                string head_i = "", tail_i = "";
                ABIType type = SequenceCoder.GetParameterType(valueTuple[i]);


                switch (type)
                {
                    //Statics: head(X(i)) = enc(X(i) and tail(X(i)) = "" (the empty string)
                    case ABIType.BOOLEAN:
                        head_i = _booleanCoder.EncodeToString(valueTuple[i]);
                        break;
                    case ABIType.NUMBER:
                        head_i = _numberCoder.EncodeToString(valueTuple[i]);
                        break;
                    case ABIType.ADDRESS:
                        head_i = _addressCoder.EncodeToString(valueTuple[i]);
                        break;
                    //Dynamics: head(X(i)) = enc(len( head(X(1)) ... head(X(k)) tail(X(1)) ... tail(X(i-1)) )) tail(X(i)) = enc(X(i))
                    case ABIType.BYTES:
                        head_i = _numberCoder.EncodeToString((object)(headerTotalByteLength + tailLength));
                        tail_i = _fixedBytesCoder.EncodeToString(valueTuple[i]);
                        break;
                    case ABIType.STRING:
                        Encoding utf8 = Encoding.UTF8;
                        head_i = _numberCoder.EncodeToString((object)(headerTotalByteLength + tailLength));
                        tail_i = _fixedBytesCoder.EncodeToString(utf8.GetBytes((string)valueTuple[i]));
                        break;
                    case ABIType.DYNAMICARRAY:
                        head_i = _numberCoder.EncodeToString((object)(headerTotalByteLength + tailLength));
                        tail_i = _numberCoder.EncodeToString(((List<object>)(valueTuple[i])).Count) + EncodeToString(valueTuple[i]);
                        break;
                    case ABIType.FIXEDARRAY:
                        head_i = _numberCoder.EncodeToString((object)(headerTotalByteLength + tailLength));
                        tail_i = EncodeToString(valueTuple[i]);
                        break;
                    case ABIType.TUPLE:
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



        public List<object> DecodeFromString(string encodedString, List<object> types)
        {
            List<object> decodedList = new List<object>();
            List<int> offsetList = new List<int>();
            int index = 0;
            foreach (object type in types)
            {
                //For offsets 
                switch (type)
                {
                    //Statics: head(X(i)) = enc(X(i) and tail(X(i)) = "" (the empty string)
                    case ABIType.BOOLEAN:
                        bool decodedBool = _booleanCoder.DecodeFromString(encodedString.Substring(index * 64, 64));
                        decodedList.Add(decodedBool);
                        offsetList.Add(0);
                        break;
                    case ABIType.NUMBER:
                        BigInteger decodedNumber = (BigInteger)_numberCoder.DecodeFromString(encodedString.Substring(index * 64, 64));
                        decodedList.Add(decodedNumber);
                        offsetList.Add(0);
                        break;
                    case ABIType.ADDRESS:
                        string decodedAddress = _addressCoder.DecodeFromString(encodedString.Substring(index * 64, 64));
                        decodedList.Add(decodedAddress);
                        offsetList.Add(0);
                        break;
                    //Dynamics: head(X(i)) = enc(len( head(X(1)) ... head(X(k)) tail(X(1)) ... tail(X(i-1)) )) tail(X(i)) = enc(X(i))
                    case ABIType.BYTES:
                        int byteOffset = (int)_numberCoder.DecodeFromString(encodedString.Substring(index * 64, 64));
                        offsetList.Add(byteOffset);
                        break;
                    case ABIType.STRING:
                        int stringOffset = (int)_numberCoder.DecodeFromString(encodedString.Substring(index * 64, 64));
                        offsetList.Add(stringOffset);
                        break;
                    case ABIType.DYNAMICARRAY:
                    case List<object>:
                        BigInteger _dArrayOffset = (BigInteger)(_numberCoder.DecodeFromString(encodedString.Substring(index * 64, 64)));
                        int dArrayOffset = (int)_dArrayOffset;
                        offsetList.Add(dArrayOffset);
                        break;
                    case ABIType.TUPLE:
                        int tupleOffset = (int)_numberCoder.DecodeFromString(encodedString.Substring(index * 64, 64));
                        offsetList.Add(tupleOffset);
                        break;
                    default:
                        break;
                }
                index++;
            }
            int offsetIndex = 0;
            foreach (object type in types)
            {

                switch (type)
                {
                    //Dynamics: head(X(i)) = enc(len( head(X(1)) ... head(X(k)) tail(X(1)) ... tail(X(i-1)) )) tail(X(i)) = enc(X(i))
                    case ABIType.BYTES:
                        string bytesEncodedString = encodedString.Substring(offsetList[offsetIndex] * 2,  64);
                        decodedList.Add(_fixedBytesCoder.DecodeFromString(bytesEncodedString));
                        offsetIndex++;
                        break;
                    case ABIType.STRING:
                        string stringEncodedString = encodedString.Substring(offsetList[offsetIndex] * 2, 64);
                        decodedList.Add(_stringCoder.DecodeFromString(stringEncodedString));
                        offsetIndex++;
                        break;
                    case ABIType.DYNAMICARRAY:
                    case ABIType.TUPLE:
                    case List<object>:
                        int length = ((List<object>)type).Count;
                        string dArrayEncodedString = encodedString.Substring(offsetList[offsetIndex] * 2+64, (length)* 64);
                        //TODO: Needs to know the type 

                        decodedList.Add(DecodeFromString(dArrayEncodedString, (List<object>)type));
                        offsetIndex++;
                        break;

                    default:
                        break;
                }

            }



            return decodedList;
        }

        public bool IsSupportedType()
        {
            throw new System.NotImplementedException();
        }
    }


}