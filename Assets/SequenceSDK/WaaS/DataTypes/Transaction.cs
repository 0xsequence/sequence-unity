using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Sequence.Transactions;
using UnityEngine;
using UnityEngine.Serialization;
using StringExtensions = Sequence.Utils.StringExtensions;

namespace Sequence.WaaS
{
    [System.Serializable]
    public class Transaction
    {
        public static readonly string WaaSZeroAddress = "0x0000000000000000000000000000000000000000";
        public uint chainId;
        [FormerlySerializedAs("from")] public string fromAddress;
        [FormerlySerializedAs("to")] public string toAddress;
        public string autoGas;
        public BigInteger? nonce;
        public string value;
        public string calldata;
        public string tokenAddress;
        public string tokenAmount;
        public string[] tokenIds;
        public string[] tokenAmounts;

        public Transaction(uint chainId, string fromAddress, string toAddress, string autoGas = null, BigInteger? nonce = null, string value = null, string calldata = null, string tokenAddress = null, string tokenAmount = null, string[] tokenIds = null, string[] tokenAmounts = null)
        {
            this.chainId = chainId;
            this.fromAddress = fromAddress;
            if (toAddress == StringExtensions.ZeroAddress)
            {
                toAddress = WaaSZeroAddress;
            }
            this.toAddress = toAddress;
            this.autoGas = autoGas;
            this.nonce = nonce;
            this.value = value;
            this.calldata = calldata;
            this.tokenAddress = tokenAddress;
            this.tokenAmount = tokenAmount;
            this.tokenIds = tokenIds;
            this.tokenAmounts = tokenAmounts;
        }
        
        public Transaction(Chain chainId, string fromAddress, string toAddress, string autoGas = null, BigInteger? nonce = null, string value = null, string calldata = null, string tokenAddress = null, string tokenAmount = null, string[] tokenIds = null, string[] tokenAmounts = null)
        {
            this.chainId = (uint)chainId;
            this.fromAddress = fromAddress;
            if (toAddress == StringExtensions.ZeroAddress)
            {
                toAddress = WaaSZeroAddress;
            }
            this.toAddress = toAddress;
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
