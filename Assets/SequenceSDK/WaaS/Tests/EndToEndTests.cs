using System.Numerics;
using System.Threading.Tasks;
using NUnit.Framework;
using Sequence.Provider;
using Sequence.Transactions;
using UnityEngine;

namespace Sequence.WaaS.Tests
{
    public class EndToEndTests
    {
        // Note: this test uses a real testnet. If this test fails, double check the RPC is active and that the sending account has funds
        // https://mumbai.polygonscan.com/address/0x660250734f31644681ae32d05bd7e8e29fea29e1
        [Test]
        public async Task TestTransferOnTestnet()
        {
            Wallet.IWallet wallet = await WaaSToWalletAdapter.CreateAsync(new WaaSWallet(
                "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJwYXJ0bmVyX2lkIjoyLCJ3YWxsZXQiOiIweDY2MDI1MDczNGYzMTY0NDY4MWFlMzJkMDViZDdlOGUyOWZlYTI5ZTEifQ.FC8WmaC_hW4svdrs4rxyKcvoekfVYFkFFvGwUOXzcHA"),
                new uint[]{0});

            IEthClient client = new SequenceEthClient("https://polygon-mumbai-bor.publicnode.com");
            EthTransaction transaction = await TransferEth.CreateTransaction(client, wallet, "0x9766bf76b2E3e7BCB8c61410A3fC873f1e89b43f",  1);

            BigInteger startingBalance = await client.BalanceAt("0x9766bf76b2E3e7BCB8c61410A3fC873f1e89b43f");
            BigInteger startingBalance2 = await client.BalanceAt(wallet.GetAddress().Value);

            TransactionReceipt receipt = await wallet.SendTransactionAndWaitForReceipt(client, transaction);
            
            BigInteger endingBalance = await client.BalanceAt("0x9766bf76b2E3e7BCB8c61410A3fC873f1e89b43f");
            BigInteger endingBalance2 = await client.BalanceAt(wallet.GetAddress().Value);
            
            Debug.Log($"starting balance {startingBalance} ending balance {endingBalance}");
            Debug.Log($"starting balance 2 {startingBalance2} ending balance 2 {endingBalance2}");
            Assert.Greater(endingBalance, startingBalance);
            Assert.Less(endingBalance2, startingBalance2);
        }
    }
}