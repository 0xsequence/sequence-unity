using System;
using System.Threading.Tasks;
using Sequence.Utils;

namespace Sequence.Relayer
{
    public class Relayer : IRelayer
    {
        private readonly IHttpClient _httpClient;

        public Relayer(Chain chain)
        {
            var name = chain switch
            {
                Chain.ArbitrumOne => "v3-arbitrum",
                _ => throw new NotSupportedException($"Chain {chain} not supported.")
            };

            var relayerUrl = $"https://{name}-relayer.sequence.app/rpc/Relayer";
            _httpClient = new HttpClient(relayerUrl);
        }
        
        public async Task<FeeOptionsReturn> GetFeeOptions(FeeOptionsArgs args)
        {
            return await _httpClient.SendPostRequest<FeeOptionsArgs, FeeOptionsReturn>("FeeOptions", args);
        }
    }
}