using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Sequence.Transactions;
using UnityEngine;

namespace Sequence.WaaS
{
    [System.Serializable]
    public class Transaction
    {
        public uint chainId;
        public string from;
        public string to;
        public string autoGas;
        public BigInteger? nonce;
        public string value;
        public string calldata;
        public string tokenAddress;
        public string tokenAmount;
        public string[] tokenIds;
        public string[] tokenAmounts;

        public Transaction(uint chainId, string from, string to, string autoGas = null, BigInteger? nonce = null, string value = null, string calldata = null, string tokenAddress = null, string tokenAmount = null, string[] tokenIds = null, string[] tokenAmounts = null)
        {
            this.chainId = chainId;
            this.from = from;
            this.to = to;
            this.autoGas = autoGas;
            this.nonce = nonce;
            this.value = value;
            this.calldata = calldata;
            this.tokenAddress = tokenAddress;
            this.tokenAmount = tokenAmount;
            this.tokenIds = tokenIds;
            this.tokenAmounts = tokenAmounts;
        }
    }
}
