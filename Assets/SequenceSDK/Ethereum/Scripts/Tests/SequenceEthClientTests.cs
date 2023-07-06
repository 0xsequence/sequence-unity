using System;
using System.Collections;
using System.Numerics;
using System.Threading.Tasks;
using NUnit.Framework;
using Sequence;
using Sequence.ABI;
using Sequence.Mocks;
using Sequence.Provider;
using Sequence.Wallet;
using UnityEngine;
using UnityEngine.TestTools;

public class SequenceEthClientTests
{
    string clientUrl = "http://localhost:8545/";
    float polygonBlockTimeInSeconds = 2f;

    [Test]
    public async Task TestChainId() {
        try {
            string expected = "0x7a69";
            var client = new SequenceEthClient(clientUrl);
            string chainId = await client.ChainID();
            Assert.AreEqual(expected, chainId);
        }
        catch (Exception ex) {
            Assert.Fail("Expected no exception, but got: " + ex.Message);
        }
    }

    [Test]
    public async Task TestNetworkId() {
        try {
            string expected = "31337";
            var client = new SequenceEthClient(clientUrl);
            string networkId = await client.NetworkId();
            Assert.AreEqual(expected, networkId);
        }
        catch (Exception ex) {
            Assert.Fail("Expected no exception, but got: " + ex.Message);
        }
    }

    [Test]
    public async Task TestBlockNumber() {
        try {
            var client = new SequenceEthClient(clientUrl);
            var blockNumber = await client.BlockNumber();
            await Task.Delay((int)(polygonBlockTimeInSeconds * 1000 * 3)); // Wait for more than the block time just in case
            var blockNumber2 = await client.BlockNumber();
            Assert.Less(blockNumber, blockNumber2);
        }
        catch (Exception ex) {
            Assert.Fail("Expected no exception, but got: " + ex.Message);
        }
    }


    [Test]
    public async Task TestWaitForTransactionReceipt()
    {
        try
        {
            EthWallet wallet = new EthWallet("0xabc0000000000000000000000000000000000000000000000000000000000001");
            EthWallet wallet2 = new EthWallet("0xabc0000000000000000000000000000000000000000000000000000000000002");
            SequenceEthClient client = new SequenceEthClient("http://localhost:8545/");
            BigInteger nonce = await wallet.GetNonce(client);
            string encoded_signing = EthTransaction.RLPEncode(nonce, 100, 30000000, wallet2.GetAddress(), 1, "");
            Assert.IsNotNull(encoded_signing);
            string signingHash = "0x" + SequenceCoder.KeccakHash(encoded_signing);
            Assert.IsNotNull(signingHash);
            Assert.IsNotNull(wallet);
            (string v, string r, string s) = wallet.SignTransaction(SequenceCoder.HexStringToByteArray(signingHash));
            Assert.IsNotNull(v);
            Assert.IsNotNull(r);
            Assert.IsNotNull(s);
            string tx = EthTransaction.RLPEncode(nonce, 100, 30000000, wallet2.GetAddress(), 1, "", v, r, s);

            string result = await wallet.SendRawTransaction(client, tx);

            Debug.Log("result: " + result);

            TransactionReceipt receipt = await client.WaitForTransactionReceipt(result);
            Assert.IsNotNull(receipt);
        }
        catch (Exception ex)
        {
            Assert.Fail("Expected no exception, but got: " + ex.Message);
        }
    }
}
