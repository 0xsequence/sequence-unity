using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.TestTools;
using NUnit.Framework;
using Sequence;
using System;
using Sequence.Wallet;
using Sequence.ABI;
using Sequence.Provider;

public class EthClientTests 
{
    [Test]
    public async Task TestWaitForTransactionReceipt()
    {
        try
        {
            EthWallet wallet = new EthWallet("0xabc0000000000000000000000000000000000000000000000000000000000001");
            EthWallet wallet2 = new EthWallet("0xabc0000000000000000000000000000000000000000000000000000000000002");
            string encoded_signing = EthTransaction.RLPEncode(wallet.GetNonce(), 100, 30000000, wallet2.GetAddress(), 1, "");
            Assert.IsNotNull(encoded_signing);
            string signingHash = "0x" + SequenceCoder.KeccakHash(encoded_signing);
            Assert.IsNotNull(signingHash);
            Assert.IsNotNull(wallet);
            (string v, string r, string s) = wallet.SignTransaction(SequenceCoder.HexStringToByteArray(signingHash));
            Assert.IsNotNull(v);
            Assert.IsNotNull(r);
            Assert.IsNotNull(s);
            string tx = EthTransaction.RLPEncode(wallet.GetNonce(), 100, 30000000, wallet2.GetAddress(), 1, "", v, r, s);

            SequenceEthClient client = new SequenceEthClient("http://localhost:8545/");
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
