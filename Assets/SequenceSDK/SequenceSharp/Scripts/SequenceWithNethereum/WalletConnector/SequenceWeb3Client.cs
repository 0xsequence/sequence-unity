using UnityEngine;
using System;
using System.Threading.Tasks;
using Nethereum.JsonRpc.Client.RpcMessages;
using Nethereum.RPC;
using Nethereum.RPC.Eth.DTOs;
using Newtonsoft.Json;
using Nethereum.Hex.HexTypes;
using System.Numerics;
using Nethereum.Web3;
using Nethereum.JsonRpc.Client;

namespace Sequence
{
    public static class Web3Extensions
    {
        public static async Task<string> GetAddress(this Web3 web3)
        {
            var rpcReq = await web3.Eth.Accounts.SendRequestAsync();
            // One day, we'll need to replace this to use the current account, once Sequence has multi account support.
            var address = (rpcReq)[0];
            return address;
        }
    }

    public struct EstimatedGas
    {
        public string type;
        public string hex;
    }

    public class SequenceWeb3Client : IClient
    {
        public BigInteger chainID;
        //private readonly Wallet _wallet;

        // TODO THIS IS WRONG! we need to get tx receipts from specific TXs.
        private TransactionReceipt transactionReceipt = new TransactionReceipt();

        public RequestInterceptor OverridingRequestInterceptor { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public SequenceWeb3Client(BigInteger chainID)
        {
           // _wallet = wallet;
            this.chainID = chainID;
        }

        public async Task<T> SendRequestAsync<T>(
                  Nethereum.JsonRpc.Client.RpcRequest request, string route = null
              )
        {
            return (T)await _SendRequestAsync(request, route);
        }


        public Task<RpcRequestResponseBatch> SendBatchRequestAsync(RpcRequestResponseBatch rpcRequestResponseBatch)
        {
            throw new NotImplementedException();
        }

        public async Task<T> SendRequestAsync<T>(string method, string route = null, params object[] paramList)
        {
            return (T)await _SendRequestAsync(new Nethereum.JsonRpc.Client.RpcRequest ("1234", method, paramList));
        }

        public async Task SendRequestAsync(Nethereum.JsonRpc.Client.RpcRequest request, string route = null)
        {
            await _SendRequestAsync(request, route);
        }

        public async Task SendRequestAsync(string method, string route = null, params object[] paramList)
        {
            await _SendRequestAsync(new Nethereum.JsonRpc.Client.RpcRequest("1234", method, paramList));
        }
        private async Task<object> _SendRequestAsync(
            Nethereum.JsonRpc.Client.RpcRequest request, string route = null
        )
        {
            if (request.Method == ApiMethods.eth_sendTransaction.ToString())
            {
                TransactionInput transactionInput = (TransactionInput)request.RawParameters[0];

                string rpcResponse = "";
                Debug.Log("rpc response: " + rpcResponse);
                RpcResponseMessage rpcResponseMessage =
                    JsonConvert.DeserializeObject<RpcResponseMessage>(rpcResponse);

                transactionReceipt = ConvertResponse<TransactionReceipt>(rpcResponseMessage);

                return transactionReceipt.ToString();
            }
            else if (request.Method == ApiMethods.eth_getTransactionReceipt.ToString())
            {
                return transactionReceipt;
            }
            else if (request.Method == ApiMethods.eth_estimateGas.ToString())
            {
                CallInput callInput = (CallInput)request.RawParameters[0];

                string estimatedGas = "";
                EstimatedGas gas = JsonConvert.DeserializeObject<EstimatedGas>(estimatedGas);

                return new HexBigInteger(gas.hex);
            }
            else if (request.Method == ApiMethods.eth_call.ToString())
            {
                var callInput = (CallInput)request.RawParameters[0];

                if (callInput.From == null)
                {
                    var address = "";
                    string rpcResponse = "";

                    RpcResponseMessage rpcResponseMessage =
                        JsonConvert.DeserializeObject<RpcResponseMessage>(rpcResponse);
                    var response = ConvertResponse<string>(rpcResponseMessage);

                    return response;
                }
                else if (request.Method == ApiMethods.eth_signTypedData_v4.ToString())
                {
                    // TODO
                    throw new NotImplementedException();
                }
                else if (request.Method == ApiMethods.eth_sign.ToString())
                {
                    string rpcResponse = "";

                    RpcResponseMessage rpcResponseMessage =
                        JsonConvert.DeserializeObject<RpcResponseMessage>(rpcResponse);
                    var response = ConvertResponse<string>(rpcResponseMessage);
                    Debug.Log(response);
                    return response;
                }
                else if (request.Method == ApiMethods.eth_getBalance.ToString())
                {
                    var accountAddress = "";// await _wallet.GetAddress();
                    var ethBalance = await Indexer.GetEtherBalance(chainID, accountAddress);
                    return ethBalance;
                }
                else if (request.Method == ApiMethods.eth_chainId.ToString())
                {
                    return new HexBigInteger(chainID);
                }
                else if (request.Method == ApiMethods.wallet_switchEthereumChain.ToString())
                {
                    this.chainID = BigInteger.Parse((string)request.RawParameters[0]);
                    return null; // TODO should throw 4902 if it's not valid
                }
                else if (request.Method == ApiMethods.eth_accounts.ToString())
                {
                    var accountAddress = "";// await _wallet.GetAddress();
                    return new string[] { accountAddress };
                }
                else
                {
                    Debug.Log("Non-intercepted Sequence call: " + request.Method);
                    throw new NotImplementedException();
                }
                
            }
            return null;
        }

        protected void HandleRpcError(RpcResponseMessage response)
        {
            if (response.HasError)
                throw new RpcResponseException(
                    new Nethereum.JsonRpc.Client.RpcError(
                        response.Error.Code,
                        response.Error.Message,
                        response.Error.Data
                    )
                );
        }

        private T ConvertResponse<T>(RpcResponseMessage response, string route = null)
        {
            HandleRpcError(response);
            try
            {
                return response.GetResult<T>();
            }
            catch (FormatException formatException)
            {
                throw new RpcResponseFormatException(
                    "Invalid format found in RPC response",
                    formatException
                );
            }
        }

    }
}
