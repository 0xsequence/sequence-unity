
using UnityEngine;
using Sequence.Provider;
using Sequence.ABI;
using System.Numerics;
using Org.BouncyCastle.Crypto;
using System.Globalization;
using System.Text;
using System.Collections;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine.Networking;
using Sequence.RLP;
using Sequence.Wallet;
using Nethereum.Signer;

public class ClientTest : MonoBehaviour
{

    // Start is called before the first frame update
    async void Start()
    {
        {
            //Example from https://eips.ethereum.org/EIPS/eip-155#parameters

            string nonce = SequenceCoder.ByteArrayToHexString(RLP.Encode(SequenceCoder.HexStringToByteArray(9.ToString("x"))));

            Debug.Log("nonce: " + nonce);

            string gasPrice = SequenceCoder.ByteArrayToHexString(RLP.Encode(SequenceCoder.HexStringToByteArray(20000000000.ToString("x"))));
            Debug.Log("gas price: " + gasPrice);

            string startgas = SequenceCoder.ByteArrayToHexString(RLP.Encode(SequenceCoder.HexStringToByteArray(21000.ToString("x"))));
            Debug.Log("start gas: " + startgas);

            string to = SequenceCoder.ByteArrayToHexString(RLP.Encode(SequenceCoder.HexStringToByteArray("0x3535353535353535353535353535353535353535")));
            Debug.Log("to: " + to);

            string value = SequenceCoder.ByteArrayToHexString(RLP.Encode(SequenceCoder.HexStringToByteArray(1000000000000000000.ToString("x"))));
            Debug.Log("value : " + value);

            string data = SequenceCoder.ByteArrayToHexString(RLP.Encode(new byte[] { }));
            Debug.Log("data : " + data);

            string _param = "0x" + nonce + gasPrice + startgas + to + value + data;
            Debug.Log("param: " + _param);

            //List:
            List<object> txToEncode = new List<object>();
            txToEncode.Add(SequenceCoder.HexStringToByteArray(9.ToString("x")));
            txToEncode.Add(SequenceCoder.HexStringToByteArray(20000000000.ToString("x")));
            txToEncode.Add(SequenceCoder.HexStringToByteArray(21000.ToString("x")));
            txToEncode.Add(SequenceCoder.HexStringToByteArray("0x3535353535353535353535353535353535353535"));
            txToEncode.Add(SequenceCoder.HexStringToByteArray(1000000000000000000.ToString("x")));
            txToEncode.Add(new byte[] { });

            txToEncode.Add(SequenceCoder.HexStringToByteArray(1.ToString("x")));
            txToEncode.Add(SequenceCoder.HexStringToByteArray(0.ToString("x")));
            txToEncode.Add(SequenceCoder.HexStringToByteArray(0.ToString("x")));

            byte[] encodedList = RLP.Encode(txToEncode);

            Debug.Log("Encoded List: " + SequenceCoder.ByteArrayToHexString(encodedList));

            //Hash:
            byte[] hashedParam = SequenceCoder.KeccakHash(encodedList);
            Debug.Log("hashed param length: " + hashedParam.Length);
            Debug.Log("hashed param: " + SequenceCoder.ByteArrayToHexString(hashedParam));

            EthWallet wallet = new EthWallet("0x4646464646464646464646464646464646464646464646464646464646464646");
            (string _v, string _r, string _s) = wallet.SignTransaction(hashedParam,1);

            Debug.Log("v: " + _v);
            Debug.Log("r: " + _r);
            Debug.Log("s: " + _s);

            List<object> paramsList = new List<object>();
            paramsList.Add(SequenceCoder.HexStringToByteArray(9.ToString("x")));
            paramsList.Add(SequenceCoder.HexStringToByteArray(20000000000.ToString("x")));
            paramsList.Add(SequenceCoder.HexStringToByteArray(21000.ToString("x")));
            paramsList.Add(SequenceCoder.HexStringToByteArray("0x3535353535353535353535353535353535353535"));
            paramsList.Add(SequenceCoder.HexStringToByteArray(1000000000000000000.ToString("x")));
            paramsList.Add(new byte[] { });

            paramsList.Add(SequenceCoder.HexStringToByteArray(_v));
            paramsList.Add(SequenceCoder.HexStringToByteArray(_r));
            paramsList.Add(SequenceCoder.HexStringToByteArray(_s));

            byte[] encodedParam = RLP.Encode(paramsList);

            Debug.Log("0x" + SequenceCoder.ByteArrayToHexString(encodedParam));

        }
        {

            //{ from: account0Address, to: account1Address, value: "12300000000000000000", gasLimit: 100000, gasPrice: 100 } 



            //TODO: Hard code 0, will call dynamically 
            string nonce = SequenceCoder.ByteArrayToHexString(RLP.Encode(SequenceCoder.HexStringToByteArray(0.ToString("x"))));

            Debug.Log("nonce: " + nonce);

            string gasPrice = SequenceCoder.ByteArrayToHexString(RLP.Encode(SequenceCoder.HexStringToByteArray(100.ToString("x"))));
            Debug.Log("gas price: " + gasPrice);

            string gasLimit = SequenceCoder.ByteArrayToHexString(RLP.Encode(SequenceCoder.HexStringToByteArray(100000.ToString("x"))));
            Debug.Log("start gas/gas limit: " + gasLimit);

            string to = SequenceCoder.ByteArrayToHexString(RLP.Encode(SequenceCoder.HexStringToByteArray("0x606e6d28e9150D8A3C070AEfB751a2D0C5DB19fa")));
            Debug.Log("to: " + to);

            string value = SequenceCoder.ByteArrayToHexString(RLP.Encode(SequenceCoder.HexStringToByteArray(12300000000000000000.ToString("x"))));
            Debug.Log("value : " + value);

            string data = SequenceCoder.ByteArrayToHexString(RLP.Encode(new byte[] { }));
            Debug.Log("data : " + data);

            string v = SequenceCoder.ByteArrayToHexString(RLP.Encode(SequenceCoder.HexStringToByteArray(1.ToString("x"))));
            string r = SequenceCoder.ByteArrayToHexString(RLP.Encode(SequenceCoder.HexStringToByteArray(0.ToString("x"))));
            string s = SequenceCoder.ByteArrayToHexString(RLP.Encode(SequenceCoder.HexStringToByteArray(0.ToString("x"))));

            string _param = "0x" + nonce + gasPrice + gasLimit + to + value + data + v + r + s;
            Debug.Log("param: " + _param);


            List<object> txToEncode = new List<object>();
            txToEncode.Add(SequenceCoder.HexStringToByteArray(0.ToString("x")));
            txToEncode.Add(SequenceCoder.HexStringToByteArray(100.ToString("x")));
            txToEncode.Add(SequenceCoder.HexStringToByteArray(100000.ToString("x")));
            txToEncode.Add(SequenceCoder.HexStringToByteArray("0x1099542D7dFaF6757527146C0aB9E70A967f71C0"));
            txToEncode.Add(SequenceCoder.HexStringToByteArray(12300000000000000000.ToString("x")));
            txToEncode.Add(new byte[] { });

 /*           txToEncode.Add(SequenceCoder.HexStringToByteArray(1.ToString("x")));
            txToEncode.Add(SequenceCoder.HexStringToByteArray(0.ToString("x")));
            txToEncode.Add(SequenceCoder.HexStringToByteArray(0.ToString("x")));*/

            byte[] encodedList = RLP.Encode(txToEncode);
            
            Debug.Log("Encoded List: " + SequenceCoder.ByteArrayToHexString(encodedList));
            byte[] hashedParam = SequenceCoder.KeccakHash(encodedList);
            Debug.Log("hashed Encoded List: " + SequenceCoder.ByteArrayToHexString(hashedParam));
            //Sign:
            EthWallet wallet = new EthWallet("0xabc0000000000000000000000000000000000000000000000000000000000001");
            Debug.Log("addr " + wallet.Address());

            (string _v, string _r, string _s) = wallet.SignTransaction(hashedParam);

            Debug.Log("v: "+ _v);
            Debug.Log("r: "+ _r);
            Debug.Log("s: "+ _s);
/*            //Compare with Nethereum:
            var signerNethereum = new EthereumMessageSigner();
            var signatureNetheruem = signerNethereum.Sign(encodedList, new EthECKey("0xabc0000000000000000000000000000000000000000000000000000000000001"));
            Debug.Log("signed with Nethereum: " + signatureNetheruem);*/

            List<object> paramsList = new List<object>();
            paramsList.Add(SequenceCoder.HexStringToByteArray(0.ToString("x")));
            paramsList.Add(SequenceCoder.HexStringToByteArray(100.ToString("x")));
            Debug.Log("to string x: 100   " + 100.ToString("x"));
            paramsList.Add(SequenceCoder.HexStringToByteArray(100000.ToString("x")));
            paramsList.Add(SequenceCoder.HexStringToByteArray("0x1099542D7dFaF6757527146C0aB9E70A967f71C0"));
            paramsList.Add(SequenceCoder.HexStringToByteArray(12300000000000000000.ToString("x")));
            paramsList.Add(new byte[] { });

            paramsList.Add(SequenceCoder.HexStringToByteArray(_v));
            paramsList.Add(SequenceCoder.HexStringToByteArray(_r));
            paramsList.Add(SequenceCoder.HexStringToByteArray(_s));

            //byte[] nonEncodedParam = (paramsList.Cast<byte[]>()).SelectMany(a => a).ToArray();

            byte[] encodedParam = RLP.Encode(paramsList);
            
            List<string> encodedParamList = new List<string>();
            encodedParamList.Add("0x" + SequenceCoder.ByteArrayToHexString(encodedParam));
            Debug.Log("encoded final:\n" +"0x"+ SequenceCoder.ByteArrayToHexString(encodedParam));

            var request = new
            {

                id = 1,
                jsonrpc = "2.0",
                method = "eth_sendRawTransaction",
                @params = encodedParamList

            };

            /*List<string> paraml = new List<string>();
            paraml.Add("0xc683a014955b75F5ECF991d4502427c8fa1Aa249");
            paraml.Add("latest");
            var request = new
            {

                id = 1,
                jsonrpc = "2.0",
                method = "eth_getBalance",
                @params = paraml

            };*/

            var rpcRequestJson = JsonConvert.SerializeObject(request);

            Debug.Log("RPC Request Json: " + rpcRequestJson);

            var unityRequest = UnityWebRequest.Put("http://localhost:8545/", rpcRequestJson);

            unityRequest.SetRequestHeader("Content-Type", "application/json");
            unityRequest.SetRequestHeader("Accept", "application/json");
            unityRequest.method = UnityWebRequest.kHttpVerbPOST;
            await unityRequest.SendWebRequest();

            if (unityRequest.error != null)
            {
                Debug.Log("RequestError: " + unityRequest.error);
                unityRequest.Dispose();

            }
            else
            {
                byte[] results = unityRequest.downloadHandler.data;
                var responseJson = Encoding.UTF8.GetString(results);
                Debug.Log("before casting:" + responseJson);

                /*RpcResponse result = JsonConvert.DeserializeObject<RpcResponse>(responseJson);
                Debug.Log("result: " + result);*/
                unityRequest.Dispose();

            }
        }
    }




}
