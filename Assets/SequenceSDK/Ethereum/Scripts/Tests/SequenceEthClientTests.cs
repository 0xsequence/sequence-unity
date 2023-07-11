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
using Sequence.Transactions;

public class SequenceEthClientTests
{
    string clientUrl = "http://localhost:8545/";
    float polygonBlockTimeInSeconds = 2f;
    IRpcClient failingClient = new FailingRpcClient();
    const string validAddress = "0x3F96a0D6697e5E7ACEC56A21681195dC6262b06C";

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
            string chainId = await client.ChainID();
            string encoded_signing = EthTransaction.RLPEncode(nonce, 100, 30000000, wallet2.GetAddress(), 1, "", chainId);
            Assert.IsNotNull(encoded_signing);
            string signingHash = "0x" + SequenceCoder.KeccakHash(encoded_signing);
            Assert.IsNotNull(signingHash);
            Assert.IsNotNull(wallet);
            (string v, string r, string s) = wallet.SignTransaction(SequenceCoder.HexStringToByteArray(signingHash), chainId);
            Assert.IsNotNull(v);
            Assert.IsNotNull(r);
            Assert.IsNotNull(s);
            string tx = EthTransaction.RLPEncode(nonce, 100, 30000000, wallet2.GetAddress(), 1, "", chainId, v, r, s);

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

    // Note: for methods with optional parameters, we must still specify a value for each parameter
    // This is because the tests are created reflexively and would otherwise throw an exception
    private static object[] errorCases = {
        new object[] { nameof(SequenceEthClient.BalanceAt), new object[] { validAddress, "latest" } },
        new object[] { nameof(SequenceEthClient.BlockByHash), new object[] { "some hash" } },
        new object[] { nameof(SequenceEthClient.BlockByNumber), new object[] { "latest" } },
        new object[] { nameof(SequenceEthClient.BlockNumber), null},
        new object[] { nameof(SequenceEthClient.BlockRange), new object[] { "earliest", "latest", false} },
        new object[] { nameof(SequenceEthClient.CallContract), new object[] { new object[] { "latest" } } },
        new object[] { nameof(SequenceEthClient.CallContract), new object[] { new object[] { "latest", BigInteger.One, "random stuff" } } },
        new object[] { nameof(SequenceEthClient.CallContract), new object[] { null } },
        new object[] { nameof(SequenceEthClient.ChainID), null},
        new object[] { nameof(SequenceEthClient.CodeAt), new object[] { validAddress, "latest" } },
        new object[] { nameof(SequenceEthClient.EstimateGas), new object[] { new TransactionCall() } },
        new object[] { nameof(SequenceEthClient.FeeHistory), new object[] { "latest", "latest", 1 } },
        new object[] { nameof(SequenceEthClient.HeaderByHash), new object[] { "some hash" } },
        new object[] { nameof(SequenceEthClient.HeaderByNumber), new object[] { "latest" } },
        new object[] { nameof(SequenceEthClient.NetworkId), null},
        new object[] { nameof(SequenceEthClient.NonceAt), new object[] { validAddress, "latest" } },
        new object[] { nameof(SequenceEthClient.SendRawTransaction), new object[] { "transaction data" } },
        new object[] { nameof(SequenceEthClient.SuggestGasPrice), null},
        new object[] { nameof(SequenceEthClient.SuggestGasTipCap), null},
        new object[] { nameof(SequenceEthClient.TransactionByHash), new object[] { "some hash" } },
        new object[] { nameof(SequenceEthClient.TransactionCount), new object[] { "some hash" } },
        new object[] { nameof(SequenceEthClient.TransactionReceipt), new object[] { "some hash" } },
        new object[] { nameof(SequenceEthClient.WaitForTransactionReceipt), new object[] { "some hash", 1, 1 } },
    };

    [TestCaseSource("errorCases")]
    public async Task TestErrorResponse(string methodName, params object[] parameters) {
        try {
            var client = new SequenceEthClient(failingClient);
            var method = typeof(SequenceEthClient).GetMethod(methodName);
            var task = (Task)method.Invoke(client, parameters);
            await task.ConfigureAwait(false);
            Assert.Fail("Expected an exception but none was thrown");
        }
        catch (Exception ex) {
            Assert.AreEqual(FailingRpcClient.ErrorMessage, ex.Message);
        }
    }
}
