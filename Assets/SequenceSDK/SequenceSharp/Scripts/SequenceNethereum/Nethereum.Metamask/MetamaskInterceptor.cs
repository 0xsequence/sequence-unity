﻿using System;
using System.Threading.Tasks;
using Nethereum.JsonRpc.Client;
using Nethereum.JsonRpc.Client.RpcMessages;
using Nethereum.RPC;
using Nethereum.RPC.Eth.DTOs;
using UnityEngine;

namespace SequenceSharp
{
    public class MetamaskInterceptor : RequestInterceptor
    {
        public readonly Metamask _metamask;

        public MetamaskInterceptor(Metamask metamask)
        {
            _metamask = metamask;
        }

        public override async Task<object> InterceptSendRequestAsync<T>(
            Func<RpcRequest, string, Task<T>> interceptedSendRequestAsync, RpcRequest request,
            string route = null)
        {
            Debug.Log("[Sequence] Metamask Interceptor Request: (method)" + request.Method);
            if (request.Method == ApiMethods.eth_sendTransaction.ToString())
            {
                var transaction = (TransactionInput)request.RawParameters[0];
                Debug.Log(transaction);
                /*transaction.From = _metamaskHostProvider.SelectedAccount;
                request.RawParameters[0] = transaction;

                var response = await _metamaskInterop.SendAsync(new MetamaskRpcRequestMessage(request.Id, request.Method, GetSelectedAccount(),
                    request.RawParameters)).ConfigureAwait(false);
                return ConvertResponse<T>(response);*/
                throw new NotImplementedException();

            }
            else if (request.Method == ApiMethods.eth_estimateGas.ToString() || request.Method == ApiMethods.eth_call.ToString())
            {
                /*var callinput = (CallInput)request.RawParameters[0];
                if (callinput.From == null)
                {
                    callinput.From ??= _metamaskHostProvider.SelectedAccount;
                    request.RawParameters[0] = callinput;
                }
                var response = await _metamaskInterop.SendAsync(new RpcRequestMessage(request.Id,
                    request.Method,
                    request.RawParameters)).ConfigureAwait(false);
                return ConvertResponse<T>(response);*/
                throw new NotImplementedException();
                //return null;
            }
            else if (request.Method == ApiMethods.eth_signTypedData_v4.ToString())
            {
                Debug.Log(request.RawParameters[0]);
                /*var account = GetSelectedAccount();
                var parameters = new object[] { account, request.RawParameters[0] };
                var response = await _metamaskInterop.SendAsync(new MetamaskRpcRequestMessage(request.Id, request.Method, GetSelectedAccount(),
                   parameters)).ConfigureAwait(false);
                return ConvertResponse<T>(response);*/
                throw new NotImplementedException();
            }
            else if (request.Method == ApiMethods.personal_sign.ToString())
            {
                /* var account = GetSelectedAccount();
                 var parameters = new object[] { request.RawParameters[0], account };
                 var response = await _metamaskInterop.SendAsync(new MetamaskRpcRequestMessage(request.Id, request.Method, GetSelectedAccount(),
                    parameters)).ConfigureAwait(false);
                 return ConvertResponse<T>(response);*/
                // return null;
                throw new NotImplementedException();
            }
            else
            {
                Debug.Log("Request Param" + request.RawParameters[0]);
                /*var response = await _metamaskInterop.SendAsync(new RpcRequestMessage(request.Id,
                    request.Method,
                    request.RawParameters)).ConfigureAwait(false);
                return ConvertResponse<T>(response);*/
                //return null;
                throw new NotImplementedException();
            }

        }





        protected void HandleRpcError(RpcResponseMessage response)
        {
            if (response.HasError)
                throw new RpcResponseException(new Nethereum.JsonRpc.Client.RpcError(response.Error.Code, response.Error.Message,
                    response.Error.Data));
        }

        private T ConvertResponse<T>(RpcResponseMessage response,
            string route = null)
        {
            HandleRpcError(response);
            try
            {
                return response.GetResult<T>();
            }
            catch (FormatException formatException)
            {
                throw new RpcResponseFormatException("Invalid format found in RPC response", formatException);
            }
        }

    }
}