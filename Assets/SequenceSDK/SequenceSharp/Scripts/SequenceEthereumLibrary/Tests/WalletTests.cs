using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using SequenceSharp.WALLET;
using SequenceSharp.ABI;
using SequenceSharp.RPC;
using System.Text;
using NBitcoin.Secp256k1;
using System;
using System.Linq;

public class WalletTests
{
    const string privKey0 = "0xabc0000000000000000000000000000000000000000000000000000000000001";
    const string privKey1 = "0xabc0000000000000000000000000000000000000000000000000000000000002";
    const string privKey2 = "0xabc0000000000000000000000000000000000000000000000000000000000003";
    const string privKey3 = "0xabc0000000000000000000000000000000000000000000000000000000000004";
    const string privKey4 = "0xabc0000000000000000000000000000000000000000000000000000000000005";
    const string privKey5 = "0xabc0000000000000000000000000000000000000000000000000000000000006";
    [Test]
    public void TestChain_AddressesTests()
    {
        string address_0_expected = "0xc683a014955b75F5ECF991d4502427c8fa1Aa249";
        Wallet wallet0 = new Wallet(privKey0);
        string address_0 = wallet0.Address();
        Debug.Log("address 0 from wallet: " + address_0);
        CollectionAssert.AreEqual(address_0_expected, address_0);


        string address_1_expected = "0x1099542D7dFaF6757527146C0aB9E70A967f71C0";
        Wallet wallet1 = new Wallet(privKey1);
        string address_1 = wallet1.Address();
        Debug.Log("address 1 from wallet: " + address_1);
        CollectionAssert.AreEqual(address_1_expected, address_1);

        string address_2_expected = "0x606e6d28e9150D8A3C070AEfB751a2D0C5DB19fa";
        Wallet wallet2 = new Wallet(privKey2);
        string address_2 = wallet2.Address();
        Debug.Log("address 2 from wallet: " + address_2);
        CollectionAssert.AreEqual(address_2_expected, address_2);


        string address_3_expected = "0xb396CbD9b745Ffc4a9C9A6D43D7957b1350Be153";
        Wallet wallet3 = new Wallet(privKey3);
        string address_3 = wallet3.Address();
        Debug.Log("address 3 from wallet: " + address_3);
        CollectionAssert.AreEqual(address_3_expected, address_3);


        string address_4_expected = "0x6F5Ddb00e3cb99Dfd9A07885Ea91303629D1DA94";
        Wallet wallet4 = new Wallet(privKey4);
        string address_4 = wallet4.Address();
        Debug.Log("address 4 from wallet: " + address_4);
        CollectionAssert.AreEqual(address_4_expected, address_4);

        string address_5_expected = "0x3F96a0D6697e5E7ACEC56A21681195dC6262b06C";
        Wallet wallet5 = new Wallet(privKey5);
        string address_5 = wallet5.Address();
        Debug.Log("address 5 from wallet: " + address_5);
        CollectionAssert.AreEqual(address_5_expected, address_5);


    }
    

    [Test]
    public async void TestChain_TransactionTests()
    {
        //{ from: account0Address, to: account1Address, value: "12300000000000000000", gasLimit: 100000, gasPrice: 100 } 
        string encoded_signing = EthTransaction.RLPEncode(0, 100, 100000, "0x1099542D7dFaF6757527146C0aB9E70A967f71C0", 12300000000000000000, "");
        Debug.Log("encoded signing: " + encoded_signing);
        string sigining_hash = "0x" + SequenceCoder.KeccakHash(encoded_signing);
        Debug.Log("signing hash: " + sigining_hash);
        Wallet wallet = new Wallet("0xabc0000000000000000000000000000000000000000000000000000000000001");
        (string v, string r, string s) = wallet.SignTx(SequenceCoder.HexStringToByteArray(sigining_hash));
        string tx = EthTransaction.RLPEncode(0, 100, 100000, "0x1099542D7dFaF6757527146C0aB9E70A967f71C0", 12300000000000000000, "", v, r, s);
        Debug.Log("tx: " + tx);
        SequenceEthClient client = new SequenceEthClient("http://localhost:8545/");
        string result = await client.SendRawTransaction(tx);
        Debug.Log(result);
    }

    [Test]
    public void EIP155SigningTxTest()
    {
        //Example from https://eips.ethereum.org/EIPS/eip-155#parameters
        //Consider a transaction with nonce = 9, gasprice = 20 * 10**9, startgas = 21000, to = 0x3535353535353535353535353535353535353535, value = 10**18, data='' (empty).
        //1. Signing data
        string expected_signing_data = "0xec098504a817c800825208943535353535353535353535353535353535353535880de0b6b3a764000080018080";
        string encoded_signing_data = EthTransaction.RLPEncode(9, 20000000000, 21000, "0x3535353535353535353535353535353535353535", 1000000000000000000, "","1","0","0");
        CollectionAssert.AreEqual(expected_signing_data,encoded_signing_data);

        //signing hash
        string expected_signing_hash = "0xdaf5a779ae972f972197303d7b574746c7ef83eadac0f2791ad23db92e4c8e53";
        string sigining_hash =  "0x" + SequenceCoder.KeccakHash(expected_signing_data);
        CollectionAssert.AreEqual(expected_signing_hash, sigining_hash);

        //the use of 37 instead of 27. The signed tx would become:

        string expected_signed_transaction = "0xf86c098504a817c800825208943535353535353535353535353535353535353535880de0b6b3a76400008025a028ef61340bd939bc2195fe537567866003e1a15d3c71ff63e1590620aa636276a067cbe9d8997f761aecb703304b3800ccf555c9f3dc64214b297fb1966a3b6d83";
        Wallet wallet = new Wallet("0x4646464646464646464646464646464646464646464646464646464646464646");
        (string v, string r, string s) = wallet.SignTx(SequenceCoder.HexStringToByteArray(expected_signing_hash), 1);
        string encoded_signed_transaction = EthTransaction.RLPEncode(9, 20000000000, 21000, "0x3535353535353535353535353535353535353535", 1000000000000000000,"", v,r, s);
        CollectionAssert.AreEqual(expected_signed_transaction, encoded_signed_transaction);


    }

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
