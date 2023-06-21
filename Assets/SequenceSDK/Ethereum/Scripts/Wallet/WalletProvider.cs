using Sequence.RPC;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

namespace Sequence.Wallet
{
    public class WalletProvider
    {
        Wallet wallet;
        Provider provider;

        public BigInteger GetEtherBalanceAt() { throw new System.NotImplementedException(); }
        public BigInteger GetTransactionCount() { throw new System.NotImplementedException(); }
    }
}

