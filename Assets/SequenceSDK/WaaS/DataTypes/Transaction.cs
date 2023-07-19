using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using JetBrains.Annotations;
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
        [CanBeNull] public string value;
        [CanBeNull] public string calldata;
        [CanBeNull] public string tokenAddress;
        [CanBeNull] public string tokenAmount;
        [CanBeNull] public string[] tokenIds;
        [CanBeNull] public string[] tokenAmounts;

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
