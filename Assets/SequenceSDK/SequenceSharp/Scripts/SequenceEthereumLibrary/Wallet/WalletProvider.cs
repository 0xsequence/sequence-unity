using SequenceSharp.RPC;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

namespace SequenceSharp.WALLET
{
    public class WalletProvider
    {
        Wallet wallet;
        Provider provider;

        public BigInteger GetEtherBalanceAt() { throw new System.NotImplementedException(); }
        public BigInteger GetTransactionCount() { throw new System.NotImplementedException(); }
    }
}

