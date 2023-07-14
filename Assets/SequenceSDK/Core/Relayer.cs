using System;
using Sequence.Transactions;
using Sequence.Core.Wallet;
using System.Numerics;

namespace Sequence.Core
{
    public interface Relayer
    {
        RPCProvider GetProvider();

        EthTransaction[] EstimateGasLimits(IWalletConfig walletConfig, WalletContext walletContext, params EthTransaction[] transactions);

        BigInteger GetNonce(IWalletConfig walletConfig, WalletContext walletContext, BigInteger space, BigInteger blockNumber);

        //(string, EthTransaction, TransactionReceipt, Exception) Relay(SignedTransactions signedTransactions);

        TransactionReceipt Wait(string transactionID, float maxWaitTime = -1);
    }
}
