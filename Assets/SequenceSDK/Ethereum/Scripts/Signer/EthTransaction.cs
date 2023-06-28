using Sequence.ABI;
using Sequence.RLP;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Sequence.Extensions;

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
            ValidateParams(to, value, gasPrice, gasLimit, nonce);
            Nonce = nonce;
            GasPrice = gasPrice;
            GasLimit = gasLimit;
            To = to;
            Value = value;
            Data = data;
        }

        public EthTransaction(BigInteger nonce, BigInteger gasPrice, BigInteger gasLimit, string to, BigInteger value, string data, string v, string r, string s)
        {
            ValidateParams(to, value, gasPrice, gasLimit, nonce);
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

        public string RLPEncode()
        {
            List<object> txToEncode = new List<object>();
            txToEncode.Add(SequenceCoder.HexStringToByteArray(Nonce.ToString("x")));
            txToEncode.Add(SequenceCoder.HexStringToByteArray(GasPrice.ToString("x")));
            txToEncode.Add(SequenceCoder.HexStringToByteArray(GasLimit.ToString("x")));
            txToEncode.Add(SequenceCoder.HexStringToByteArray(To));
            txToEncode.Add(SequenceCoder.HexStringToByteArray(Value.ToString("x")));
            txToEncode.Add(SequenceCoder.HexStringToByteArray(Data));

            if (V!=null && R!= null && S!= null)
            {
                txToEncode.Add(SequenceCoder.HexStringToByteArray(V));
                txToEncode.Add(SequenceCoder.HexStringToByteArray(R));
                txToEncode.Add(SequenceCoder.HexStringToByteArray(S));
            }

            byte[] encodedList = RLP.RLP.Encode(txToEncode);
            return SequenceCoder.ByteArrayToHexString(encodedList).EnsureHexPrefix();
        }

        public static string RLPEncode(BigInteger nonce, BigInteger gasPrice, BigInteger gasLimit, string to, BigInteger value, string data)
        {
            ValidateParams(to, value, gasPrice, gasLimit, nonce);

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
            ValidateParams(to, value, gasPrice, gasLimit, nonce);

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

        /// <summary>
        /// Throws an ArgumentOutOfRangeException if a transaction is supplied invalid inputs
        /// </summary>
        /// <param name="toAddress"></param>
        /// <param name="value"></param>
        /// <param name="gasPrice"></param>
        /// <param name="gasLimit"></param>
        /// <param name="nonce"></param>
        public static void ValidateParams(
            string toAddress,
            BigInteger value,
            BigInteger gasPrice,
            BigInteger gasLimit,
            BigInteger nonce)
        {
            if (!toAddress.IsAddress())
            {
                throw new ArgumentOutOfRangeException(nameof(toAddress));
            }
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }
            if (gasPrice <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(gasPrice));
            }
            if (gasLimit <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(gasLimit));
            }
            if (nonce < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(nonce));
            }
        }

        public string SignAndEncodeTransaction(EthWallet wallet)
        {
            string encoded_signing = this.RLPEncode();
            string signingHash = SequenceCoder.KeccakHash(encoded_signing).EnsureHexPrefix();
            (string v, string r, string s) = wallet.SignTransaction(SequenceCoder.HexStringToByteArray(signingHash));
            this.V = v;
            this.R = r;
            this.S = s;
            string tx = this.RLPEncode();
            return tx;
        }
    }
}
