using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Numerics;
using NBitcoin.Secp256k1;

namespace Sequence.Wallet
{
    internal static class NonceService
    {
        static Dictionary<ECPubKey, BigInteger> walletNonces = new Dictionary<ECPubKey, BigInteger>();

        public static BigInteger GetNonce(EthWallet wallet)
        {
            if (!walletNonces.ContainsKey(wallet.pubKey))
            {
                walletNonces[wallet.pubKey] = 0;
            }
            return walletNonces[wallet.pubKey];
        }

        public static void IncrementNonce(EthWallet wallet)
        {
            if (!walletNonces.ContainsKey(wallet.pubKey))
            {
                walletNonces[wallet.pubKey] = 0;
            }
            walletNonces[wallet.pubKey]++;
        }
    }
}
