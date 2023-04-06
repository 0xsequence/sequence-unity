#define HAS_SPAN
using UnityEngine;
using System.Linq;
using System;
using SequenceSharp.WALLET;
using System.Text;
using NBitcoin.Secp256k1;
using SequenceSharp.ABI;


using Nethereum.Web3;
using Nethereum.Util;
using System.Collections.Generic;
using Nethereum.Signer;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.ABI.Encoders;
using SequenceSharp.Signer;

public class SignerTest : MonoBehaviour
{

    private void Start()
    {
        Wallet wallet = new Wallet("b3c503217dbb0fae8950dadf73e2f500e968abddb95e22306ba95bbc7301cc01");

        //test
        Debug.Log("keccak empty: " + SequenceCoder.KeccakHash(""));
        //test hash message
        string strToTest = "Hello World";
        byte[] toHash = SequenceCoder.HexStringToByteArray("19").Concat(Encoding.ASCII.GetBytes("Ethereum Signed Message:\n" + strToTest.Length + strToTest)).ToArray();
        Debug.Log("hashMessage 'Hello World': " + SequenceCoder.ByteArrayToHexString( SequenceCoder.KeccakHash(toHash)));

        byte[] publickeyBytes = wallet.pubKey.ToBytes(false);
        byte[] publicKeyBytes64 = new byte[64];
        Array.Copy(publickeyBytes, 1, publicKeyBytes64, 0, 64);
        string hashed = SequenceCoder.ByteArrayToHexString(SequenceCoder.KeccakHash(publicKeyBytes64));
        Debug.Log("hashed public key: " + hashed);

        string address = wallet.Address();
        Debug.Log("address: " + address);


        byte[] testMessage = Encoding.ASCII.GetBytes("this is a test");
        string sig = wallet.SignMessage(testMessage);
        Debug.Log("signature: " + sig);

        bool valid = wallet.IsValidSignature(sig, "this is a test");
        Debug.Log("isValid? :" + valid);

        //Sign with Nethereum
        var signer1 = new EthereumMessageSigner();
        var signature1 = signer1.EncodeUTF8AndSign("this is a test", new EthECKey("b3c503217dbb0fae8950dadf73e2f500e968abddb95e22306ba95bbc7301cc01"));
        Debug.Log("nethereum prefix hash: " + SequenceCoder.ByteArrayToHexString(HashPrefixedMessage(Encoding.UTF8.GetBytes("this is a test"))));
        Debug.Log("signature from nethereum:" + signature1);

        //Recover with Nethereum
        var addressRec1 = signer1.EncodeUTF8AndEcRecover("this is a test", signature1);
        Debug.Log("nethereum recovered Address:" + addressRec1);

        
        /*bool verified = wallet.publicKey.SigVerify(signature, message32);
        Debug.Log("verified: " + verified);*/

    }

    public byte[] HashPrefixedMessage(byte[] message)
    {
        var byteList = new List<byte>();
        var bytePrefix = "0x19".HexToByteArray();
        var textBytePrefix = Encoding.UTF8.GetBytes("Ethereum Signed Message:\n" + message.Length);

        byteList.AddRange(bytePrefix);
        byteList.AddRange(textBytePrefix);
        byteList.AddRange(message);
        return SequenceCoder.KeccakHash(byteList.ToArray());
    }


}
