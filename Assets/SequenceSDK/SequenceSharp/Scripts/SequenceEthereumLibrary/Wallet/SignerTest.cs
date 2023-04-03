using UnityEngine;
using System.Linq;
using System;
using SequenceSharp.WALLET;
using System.Text;
using NBitcoin.Secp256k1;

public class SignerTest : MonoBehaviour
{
   
    private void Start()
    {
        Wallet wallet = new Wallet("b3c503217dbb0fae8950dadf73e2f500e968abddb95e22306ba95bbc7301cc01");

        // string address = wallet.Address();
        // Debug.Log("address: " + address);
        //Assert.AreEqual(address, "");

        byte[] hiMessage = Encoding.ASCII.GetBytes("hi");
        SecpECDSASignature signature;
        bool signed = wallet.SignMessage(hiMessage, out signature);

        Debug.Log("signature: " + signature);
        Debug.Log("signed: " + signed);
        bool verified = wallet.publicKey.SigVerify(signature, hiMessage);

    }







}
