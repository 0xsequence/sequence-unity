using Sequence.ABI;
using Sequence.RLP;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Sequence.Wallet;
using Sequence.Utils;

namespace Sequence.Transactions
{
    public class EthTransaction
    {
        public BigInteger Nonce{ get; private set; }
        public BigInteger GasPrice { get; private set; }
        public BigInteger GasLimit { get; private set; }
        public string To { get; private set; }
        public BigInteger Value { get; private set; }
        public string Data { get; private set; }
        public string ChainId { get; private set; }

        string V { get; set; }
        string R { get; set; }
        string S { get; set; }

        public EthTransaction(BigInteger nonce, BigInteger gasPrice, BigInteger gasLimit, string to, BigInteger value, string data, string chainId)
        {
            ValidateParams(to, value, gasPrice, gasLimit, nonce, chainId);
            Nonce = nonce;
            GasPrice = gasPrice;
            GasLimit = gasLimit;
            To = to;
            Value = value;
            Data = data;
            ChainId = chainId;
        }

        public EthTransaction(BigInteger nonce, BigInteger gasPrice, BigInteger gasLimit, string to, BigInteger value, string data, string chainId, string v, string r, string s)
        {
            ValidateParams(to, value, gasPrice, gasLimit, nonce, chainId);
            Nonce = nonce;
            GasPrice = gasPrice;
            GasLimit = gasLimit;
            To = to;
            Value = value;
            Data = data;
            ChainId = chainId;
            V = v;
            R = r;
            S = s;
        }

        public string RLPEncode()
        {
            List<object> txToEncode = new List<object>();
            txToEncode.Add(Nonce.ToByteArray(true, true));
            txToEncode.Add(GasPrice.ToByteArray(true, true));
            txToEncode.Add(GasLimit.ToByteArray(true, true));
            txToEncode.Add(SequenceCoder.HexStringToByteArray(To));
            txToEncode.Add(Value.ToByteArray(true, true));
            txToEncode.Add(SequenceCoder.HexStringToByteArray(Data));
            if (V!=null && R!= null && S!= null)
            {
                txToEncode.Add(SequenceCoder.HexStringToByteArray(V));
                txToEncode.Add(SequenceCoder.HexStringToByteArray(R));
                txToEncode.Add(SequenceCoder.HexStringToByteArray(S));
            }else
            {
                txToEncode.Add(SequenceCoder.HexStringToByteArray(ChainId));
                txToEncode.Add(BigInteger.Zero.ToByteArray(true, true));
                txToEncode.Add(BigInteger.Zero.ToByteArray(true, true));
            }

            byte[] encodedList = RLP.RLP.Encode(txToEncode);
            return SequenceCoder.ByteArrayToHexString(encodedList).EnsureHexPrefix();
        }

        public static string RLPEncode(BigInteger nonce, BigInteger gasPrice, BigInteger gasLimit, string to, BigInteger value, string data, string chainId)
        {
            EthTransaction transaction = new EthTransaction(nonce, gasPrice, gasLimit, to, value, data, chainId);
            return transaction.RLPEncode();
        }

        

        public static string RLPEncode(BigInteger nonce, BigInteger gasPrice, BigInteger gasLimit, string to, BigInteger value, string data, string chainId, string v, string r, string s)
        {
            EthTransaction transaction = new EthTransaction(nonce, gasPrice, gasLimit, to, value, data, chainId, v, r, s);
            return transaction.RLPEncode();
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
            BigInteger nonce,
            string chainId = "valid")
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
            if (chainId == null || chainId == "")
            {
                throw new ArgumentNullException(nameof(chainId));
            }
        }

        public string SignAndEncodeTransaction(EthWallet wallet)
        {
            string encoded_signing = this.RLPEncode();
            string signingHash = SequenceCoder.KeccakHash(encoded_signing).EnsureHexPrefix();
            (string v, string r, string s) = wallet.SignTransaction(SequenceCoder.HexStringToByteArray(signingHash), ChainId);
            this.V = v;
            this.R = r;
            this.S = s;
            string tx = this.RLPEncode();
            return tx;
        }
    }
}
