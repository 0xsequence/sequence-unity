using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Sequence.Provider;
using UnityEngine;

namespace Sequence.Core
{
    public struct ProviderOption
    {

    }

    public struct Breaker
    {

    }

    //JsonRPC Provider at the moment
    public class RPCProvider
    {
       // Logger logger;
        string nodeURL;
        IEthClient client;


        public RPCProvider(string _nodeURL)
        {
            nodeURL = _nodeURL;
        }

        public RPCProvider(string _nodeURL, ProviderOption options)
        {
            nodeURL = _nodeURL;
            client = new SequenceEthClient(_nodeURL);
        }

        public void SetHTTPClient(IEthClient client)
        {
            this.client = client;
        }

        public async Task<BigInteger> ChainID()
        {
            string chainId = await client.ChainID();
            return BigInteger.Parse(chainId);
        }

    }
}
