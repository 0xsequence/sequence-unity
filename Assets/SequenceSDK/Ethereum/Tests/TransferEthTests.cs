using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using NUnit.Framework;
using Sequence;
using Sequence.Provider;
using Sequence.Wallet;
using UnityEngine;
using Sequence.Transactions;

namespace Sequence.Ethereum.Tests
{
    public class TransferEthTests
    {
        EOAWallet wallet1 = new EOAWallet("0xabc0000000000000000000000000000000000000000000000000000000000001");
        EOAWallet wallet2 = new EOAWallet("0xabc0000000000000000000000000000000000000000000000000000000000002");
        SequenceEthClient client = new SequenceEthClient("http://localhost:8545/");

        [Test]
        public async Task TestTransferEth()
        {
            try
            {
                BigInteger preBalance1 = await wallet1.GetBalance(client);
                BigInteger preBalance2 = await wallet2.GetBalance(client);

                BigInteger nonce = await wallet1.GetNonce(client);
                TransferEth transfer = new TransferEth(client, wallet1, wallet2.GetAddress(), 1000000, 100, 30000000, nonce);
                TransactionReceipt receipt = await transfer.SendAndWaitForReceipt();

                BigInteger postBalance1 = await wallet1.GetBalance(client);
                BigInteger postBalance2 = await wallet2.GetBalance(client);

                Assert.Greater(preBalance1, postBalance1);
                Assert.Less(preBalance2, postBalance2);
            }
            catch (Exception ex)
            {
                Assert.Fail("Expected no exception, but got: " + ex.Message);
            }
        }

        [Test]
        public void TestInvalidTransferParams()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => {
                TransferEth transfer = new TransferEth(null, wallet1, wallet2.GetAddress(), 1, 1, 1, 1);
            });
            Assert.Throws<ArgumentOutOfRangeException>(() => {
                TransferEth transfer = new TransferEth(client, null, wallet2.GetAddress(), 1, 1, 1, 1);
            });
            Assert.Throws<ArgumentOutOfRangeException>(() => {
                TransferEth transfer = new TransferEth(client, wallet1, null, 1, 1, 1, 1);
            });
            Assert.Throws<ArgumentOutOfRangeException>(() => {
                TransferEth transfer = new TransferEth(client, wallet1, "", 1, 1, 1, 1);
            });
            Assert.Throws<ArgumentOutOfRangeException>(() => {
                TransferEth transfer = new TransferEth(client, wallet1, wallet2.GetAddress(), 0, 1, 1, 1);
            });
            Assert.Throws<ArgumentOutOfRangeException>(() => {
                TransferEth transfer = new TransferEth(client, wallet1, wallet2.GetAddress(), 1, -1, 1, 1);
            });
            Assert.Throws<ArgumentOutOfRangeException>(() => {
                TransferEth transfer = new TransferEth(client, wallet1, wallet2.GetAddress(), 1, 1, 0, 1);
            });
            Assert.Throws<ArgumentOutOfRangeException>(() => {
                TransferEth transfer = new TransferEth(client, wallet1, wallet2.GetAddress(), 1, 1, 1, -10);
            });
            Assert.DoesNotThrow(() => {
                TransferEth transfer = new TransferEth(client, wallet1, wallet2.GetAddress(), 1, 1, 1, 0);
            });
        }
    }
}
