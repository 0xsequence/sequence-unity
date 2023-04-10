using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using SequenceSharp.WALLET;
using SequenceSharp.ABI;
using System.Text;
using NBitcoin.Secp256k1;
using System;
using System.Linq;

public class WalletTests
{


    [Test]
    public void TestWalletRandom()
    {
        Wallet wallet = new Wallet();
        Assert.NotNull(wallet);
        
    }

    [Test]
    public void TestWalletSignMessage()
    {
        Wallet wallet = new Wallet();

        string address = wallet.Address();
        Assert.NotNull(address);

        string sig = wallet.SignMessage("hi");
        Assert.NotNull(sig);

        bool valid = wallet.IsValidSignature(sig, "hi");
        Assert.IsTrue(valid);
    }

    [Test]
    public void TestWalletSignMessageExistingPrefix()
    {
        Wallet wallet = new Wallet("b3c503217dbb0fae8950dadf73e2f500e968abddb95e22306ba95bbc7301cc01");
        CollectionAssert.AreEqual(SequenceCoder.HexStringToByteArray("b3c503217dbb0fae8950dadf73e2f500e968abddb95e22306ba95bbc7301cc01"), wallet.privKey.sec.ToBytes());

        string address = wallet.Address();
        CollectionAssert.AreEqual("0x2AD3Df4A43445545e486a5c62F98Cee22d500bdf", address);

        byte[] _19 = SequenceCoder.HexStringToByteArray("19");
        byte[] testMessage = Encoding.ASCII.GetBytes("Ethereum Signed Message:\n" +"this is a test".Length + "this is a test");
        testMessage = _19.Concat(testMessage).ToArray();
        string sig = wallet.SignMessage(testMessage);

        Assert.AreEqual("0x45c666ac1fc5faae5639014d2c163c1ac4863fb78a4bd23c3785f7db99cf553666191da4cad5968d018287e784ceabc7f5565b5375a4b7e35cba897d0b666f0f1b", sig);
    }

    [Test]
    public void TestWalletSignMessageFromPrivateKey()
    {

         Wallet wallet = new Wallet("b3c503217dbb0fae8950dadf73e2f500e968abddb95e22306ba95bbc7301cc01");
         CollectionAssert.AreEqual(  SequenceCoder.HexStringToByteArray("b3c503217dbb0fae8950dadf73e2f500e968abddb95e22306ba95bbc7301cc01"), wallet.privKey.sec.ToBytes());

        string address = wallet.Address();
        CollectionAssert.AreEqual("0x2AD3Df4A43445545e486a5c62F98Cee22d500bdf", address);


        byte[] testMessage = Encoding.ASCII.GetBytes("this is a test");

        string sig = wallet.SignMessage(testMessage);

        Assert.AreEqual("0x45c666ac1fc5faae5639014d2c163c1ac4863fb78a4bd23c3785f7db99cf553666191da4cad5968d018287e784ceabc7f5565b5375a4b7e35cba897d0b666f0f1b", sig);
       
    }

    [Test]
    public void TestWalletSignAndRecover()
    {
        Wallet wallet = new Wallet("b3c503217dbb0fae8950dadf73e2f500e968abddb95e22306ba95bbc7301cc01");
        CollectionAssert.AreEqual(SequenceCoder.HexStringToByteArray("b3c503217dbb0fae8950dadf73e2f500e968abddb95e22306ba95bbc7301cc01"), wallet.privKey.sec.ToBytes());

        string address = wallet.Address();
        CollectionAssert.AreEqual("0x2AD3Df4A43445545e486a5c62F98Cee22d500bdf", address);


        byte[] testMessage = Encoding.ASCII.GetBytes("this is a test");

        string sig = wallet.SignMessage(testMessage);

        string recoveredAddr = wallet.Recover("this is a test", sig);

        Assert.AreEqual(address, recoveredAddr);
    }



}
