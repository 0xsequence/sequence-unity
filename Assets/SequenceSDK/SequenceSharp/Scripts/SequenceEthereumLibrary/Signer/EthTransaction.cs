using Sequence.ABI;
using Sequence.RLP;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

namespace Sequence.Wallet
{
    public class EthTransaction
    {
        BigInteger Nonce{ get; set; }
        BigInteger GasPrice { get; set; }
        BigInteger GasLimit { get; set; }
        string To { get; set; }
        BigInteger Value { get; set; }
        string Data { get; set; }

        string V { get; set; }
        string R { get; set; }
        string S { get; set; }

        public EthTransaction(BigInteger nonce, BigInteger gasPrice, BigInteger gasLimit, string to, BigInteger value, string data)
        {
            Nonce = nonce;
            GasPrice = gasPrice;
            GasLimit = gasLimit;
            To = to;
            Value = value;
            Data = data;
        }

        public EthTransaction(BigInteger nonce, BigInteger gasPrice, BigInteger gasLimit, string to, BigInteger value, string data, string v, string r, string s)
        {
            Nonce = nonce;
            GasPrice = gasPrice;
            GasLimit = gasLimit;
            To = to;
            Value = value;
            Data = data;
            V = v;
            R = r;
            S = s;
        }

        public byte[] RLPEncode()
        {
            List<object> txToEncode = new List<object>();
            txToEncode.Add(SequenceCoder.HexStringToByteArray(Nonce.ToString("x")));
            txToEncode.Add(SequenceCoder.HexStringToByteArray(GasPrice.ToString("x")));
            txToEncode.Add(SequenceCoder.HexStringToByteArray(GasLimit.ToString("x")));
            txToEncode.Add(SequenceCoder.HexStringToByteArray(To));
            txToEncode.Add(SequenceCoder.HexStringToByteArray(Value.ToString("x")));
            txToEncode.Add(new byte[] { });

            if (V!=null && R!= null && S!= null)
            {
                txToEncode.Add(SequenceCoder.HexStringToByteArray(V));
                txToEncode.Add(SequenceCoder.HexStringToByteArray(R));
                txToEncode.Add(SequenceCoder.HexStringToByteArray(S));
            }

            byte[] encodedList = RLP.RLP.Encode(txToEncode);
            return encodedList;
        }

        public static string RLPEncode(BigInteger nonce, BigInteger gasPrice, BigInteger gasLimit, string to, BigInteger value, string data)
        {
            List<object> txToEncode = new List<object>();

            txToEncode.Add(nonce.ToByteArray(true, true));
            txToEncode.Add(gasPrice.ToByteArray(true, true));
            txToEncode.Add(gasLimit.ToByteArray(true, true));
            txToEncode.Add(SequenceCoder.HexStringToByteArray(to));
            txToEncode.Add(value.ToByteArray(true, true));
            txToEncode.Add(SequenceCoder.HexStringToByteArray(data));


            byte[] encodedList = RLP.RLP.Encode(txToEncode);
            return "0x" + SequenceCoder.ByteArrayToHexString(encodedList);
        }

        

        public static string RLPEncode(BigInteger nonce, BigInteger gasPrice, BigInteger gasLimit, string to, BigInteger value, string data, string v, string r, string s)
        {
            List<object> txToEncode = new List<object>();
            txToEncode.Add(nonce.ToByteArray(true, true));
            txToEncode.Add(gasPrice.ToByteArray(true, true));
            txToEncode.Add(gasLimit.ToByteArray(true, true));
            txToEncode.Add(SequenceCoder.HexStringToByteArray(to));
            txToEncode.Add(value.ToByteArray(true, true));
            txToEncode.Add(SequenceCoder.HexStringToByteArray(data));


            txToEncode.Add(SequenceCoder.HexStringToByteArray(v));
            txToEncode.Add(SequenceCoder.HexStringToByteArray(r));
            txToEncode.Add(SequenceCoder.HexStringToByteArray(s));


            byte[] encodedList = RLP.RLP.Encode(txToEncode);
            return "0x" + SequenceCoder.ByteArrayToHexString(encodedList);
        }

    }
}
