#define HAS_SPAN
using UnityEngine;
using System.Linq;
using System;
using SequenceSharp.WALLET;
using System.Text;
using NBitcoin.Secp256k1;
using SequenceSharp.ABI;

public class SignerTest : MonoBehaviour
{

    private void Start()
    {
        Wallet wallet = new Wallet("b3c503217dbb0fae8950dadf73e2f500e968abddb95e22306ba95bbc7301cc01");

        //test
        Debug.Log("keccak empty: " + SequenceCoder.KeccakHash(""));

        byte[] publickeyBytes = wallet.publicKey.ToBytes(false);
        byte[] publicKeyBytes64 = new byte[64];
        Array.Copy(publickeyBytes, 1, publicKeyBytes64, 0, 64);
        string hashed = SequenceCoder.ByteArrayToHexString(SequenceCoder.KeccakHash(publicKeyBytes64));
        Debug.Log("hashed public key: " + hashed);

        string address = wallet.Address();
        Debug.Log("address: " + address);


        byte[] testMessage = Encoding.ASCII.GetBytes("this is a test");
        SecpECDSASignature signature;
        bool signed = wallet.SignMessage(testMessage, out signature);
        Debug.Log("signed: " + signed);

        byte[] message191 = Encoding.UTF8.GetBytes(@"\x19Ethereum Signed Message:\n");





        //verify

        byte[] message32 = new byte[32];
        int len = testMessage.Length;
        byte[] messageLen = Encoding.UTF8.GetBytes(len.ToString());
        byte[] message = (message191.Concat(messageLen).ToArray()).Concat(testMessage).ToArray(); 
        message32 = SequenceCoder.KeccakHash(message);

        bool verified = wallet.publicKey.SigVerify(signature, message32);
        Debug.Log("verified: " + verified);

    }

 
}
