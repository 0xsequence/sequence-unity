using Sequence.Provider;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

namespace Sequence.Core.Wallet
{
    public class WalletProvider
    {
        Wallet wallet;
        RPCProvider provider;

        public BigInteger GetEtherBalanceAt() { throw new System.NotImplementedException(); }
        public BigInteger GetTransactionCount() { throw new System.NotImplementedException(); }
    }
}

