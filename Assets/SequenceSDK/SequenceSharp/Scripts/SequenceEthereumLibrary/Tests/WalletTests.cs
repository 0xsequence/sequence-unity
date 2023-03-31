using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using SequenceSharp.WALLET;
using SequenceSharp.ABI;
using System.Text;

public class WalletTests
{
    [Test]
    public void TestWalletRandom()
    {
        Wallet wallet = new Wallet();
        Assert.NotNull(wallet);
        //Todo: Check no exceptions been thrown
    }

    [Test]
    public void TestWalletSignMessage()
    {

    }

    [Test]
    public void TestWalletSignMessageExistingPrefix()
    {
        
    }

    [Test]
    public void TestWalletSignMessageFromPrivateKey()
    {
        //Test with random pair
        //http://kjur.github.io/jsrsasign/sample/sample-ecdsa.html
        //

        Wallet wallet = new Wallet("b3c503217dbb0fae8950dadf73e2f500e968abddb95e22306ba95bbc7301cc01");
        string pu = wallet.publicKey;
        string pr = wallet.privateKey;
        Debug.Log("public: " + pu);
        Debug.Log("private:" +pr);
        Assert.AreEqual(pr, "b3c503217dbb0fae8950dadf73e2f500e968abddb95e22306ba95bbc7301cc01");
        string address = wallet.Address();
        Debug.Log("address: " + address);
        //Assert.AreEqual(address, "");

        byte[] hiMessage = Encoding.ASCII.GetBytes("hi");
        byte[] signature = wallet.SignMessage(hiMessage);

        string sigHash = "0x" + SequenceCoder.ByteArrayToHexString(signature);

        bool verified = wallet.IsValidSignature(pu, sigHash, "hi");
        Assert.IsTrue(verified);
        //string expected = "0x30440220545b07b54f734265832cc134818502c86210d8d4ff26fd22311daa3f94ab65d702200fb7b65e16a622d29a70cd988cc2959a10f2be5dbd17bbdec0a1cd31f7edd3cf";
        Debug.Log("sigHash: " + sigHash);

        //Assert.AreEqual(sigHash, expected);
        //TOOO: Validate Ethereum Signature

        //TODO: Is Valid 191 Signature
    }

    [Test]
    public void TestWalletSignAndRecover()
    {

    }



}
