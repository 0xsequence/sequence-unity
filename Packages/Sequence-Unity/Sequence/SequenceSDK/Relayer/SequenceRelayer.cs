using System;
using System.Numerics;
using System.Threading.Tasks;
using Sequence.Utils;

namespace Sequence.Relayer
{
    public class SequenceRelayer : IRelayer
    {
        private readonly IHttpClient _httpClient;

        public SequenceRelayer(Chain chain)
        {
            var name = chain switch
            {
                Chain.ArbitrumOne => "v3-arbitrum",
                Chain.TestnetArbitrumSepolia => "v3-arbitrum-sepolia",
                _ => throw new NotSupportedException($"Chain {chain} not supported.")
            };

            var relayerUrl = $"https://{name}-relayer.sequence.app/rpc/Relayer";
            _httpClient = new HttpClient(relayerUrl);
        }

        public async Task<string> Relay(Address to, string data, string quote = null, IntentPrecondition[] preconditions = null)
        {
            var response = await SendMetaTxn(new SendMetaTxnArgs(new MetaTxn(to, to.Value, data), quote, -1, preconditions));
            return response.txnHash;
        }
        
        public async Task<FeeOptionsReturn> GetFeeOptions(FeeOptionsArgs args)
        {
            return await _httpClient.SendPostRequest<FeeOptionsArgs, FeeOptionsReturn>("FeeOptions", args);
        }

        public async Task<GetMetaTxnReceiptReturn> GetMetaTxnReceipt(string metaTxnID)
        {
            var args = new GetMetaTxnReceiptArgs { metaTxnID = metaTxnID };
            return await _httpClient.SendPostRequest<GetMetaTxnReceiptArgs, GetMetaTxnReceiptReturn>("GetMetaTxnReceipt", args);
        }

        public async Task<SendMetaTxnReturn> SendMetaTxn(SendMetaTxnArgs args)
        {
            return await _httpClient.SendPostRequest<SendMetaTxnArgs, SendMetaTxnReturn>("SendMetaTxn", args);
        }
    }
}