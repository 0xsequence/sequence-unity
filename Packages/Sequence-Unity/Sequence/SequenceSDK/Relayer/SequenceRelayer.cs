using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Sequence.Config;
using Sequence.Utils;

namespace Sequence.Relayer
{
    public class SequenceRelayer : IRelayer
    {
        private readonly IHttpClient _httpClient;
        private readonly Dictionary<string, string> _headers = new();

        public SequenceRelayer(Chain chain)
        {
            var path = ChainDictionaries.PathOf[chain];
            var relayerUrl = $"https://dev-{path}-relayer.sequence.app/rpc/Relayer";
            _httpClient = new HttpClient(relayerUrl);
        }

        public async Task<string> Relay(Address to, string data, string quote = null, IntentPrecondition[] preconditions = null)
        {
            var response = await SendMetaTxn(new SendMetaTxnArgs(new MetaTxn(to, to.Value, data), quote, -1, preconditions));
            return response.txnHash;
        }
        
        public async Task<FeeOptionsReturn> GetFeeOptions(FeeOptionsArgs args)
        {
            return await _httpClient.SendPostRequest<FeeOptionsArgs, FeeOptionsReturn>("FeeOptions", args, _headers);
        }

        public async Task<GetMetaTxnReceiptReturn> GetMetaTxnReceipt(string metaTxID)
        {
            var args = new GetMetaTxnReceiptArgs { metaTxID = metaTxID.EnsureHexPrefix() };
            return await _httpClient.SendPostRequest<GetMetaTxnReceiptArgs, GetMetaTxnReceiptReturn>("GetMetaTxnReceipt", args, _headers);
        }

        public async Task<SendMetaTxnReturn> SendMetaTxn(SendMetaTxnArgs args)
        {
            return await _httpClient.SendPostRequest<SendMetaTxnArgs, SendMetaTxnReturn>("SendMetaTxn", args, _headers);
        }
    }
}