using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using UnityEngine;

namespace Sequence.RPC
{
    public struct ProviderOption
    {

    }

    public struct Breaker
    {

    }

    //JsonRPC Provider at the moment
    public class Provider
    {
       // Logger logger;
        string nodeURL;
        HttpRpcClient httpClient;


        public Provider(string _nodeURL)
        {
            nodeURL = _nodeURL;
        }

        public Provider(string _nodeURL, ProviderOption options)
        {
            nodeURL = _nodeURL;
            httpClient = new HttpRpcClient(_nodeURL);
        }

        public void SetHTTPClient(HttpRpcClient _httpClient)
        {
            httpClient = _httpClient;
        }
        public async Task Send(string payload)
        {
            //?
            await httpClient.SendRequest(payload);
        }

    }
}
