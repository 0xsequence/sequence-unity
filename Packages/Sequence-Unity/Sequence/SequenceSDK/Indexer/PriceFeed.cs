using System;
using System.Numerics;
using System.Threading.Tasks;
using Sequence.Config;
using Sequence.Utils;

namespace Sequence
{
    public class PriceFeed
    {
        [Obsolete("Use the Token class in the Sequence namespace instead.")]
        public class Token : Sequence.Token
        {
            public Token(Chain chain, Address contract, string tokenId = null) : base(chain, contract, tokenId)
            {
            }

            public Token(BigInteger chainId, string contractAddress, string tokenId = null) : base(chainId, contractAddress, tokenId)
            {
            }
        }
        
        private const string _devUrl = "https://dev-api.sequence.app/rpc/API";
        private const string _prodUrl = "https://api.sequence.app/rpc/API";
        
        private IHttpHandler _httpHandler;
        private string _baseUrl;
        
        public PriceFeed(IHttpHandler httpHandler = null)
        {
            _httpHandler = httpHandler;
            if (_httpHandler == null)
            {
                _httpHandler = new HttpHandler(SequenceConfig.GetConfig(SequenceService.Stack).BuilderAPIKey);
            }
            
#if SEQUENCE_DEV_STACK || SEQUENCE_DEV
            _baseUrl = _devUrl;
#else
            _baseUrl = _prodUrl;
#endif
        }

        public async Task<TokenPrice[]> GetCoinPrices(params Token[] tokens)
        {
            GetTokenPricesArgs args = new GetTokenPricesArgs(tokens);
            GetTokenPricesReturn response = await _httpHandler.HttpPost<GetTokenPricesReturn>(_baseUrl.AppendTrailingSlashIfNeeded() + "GetCoinPrices", args);
            return response.tokenPrices;
        }
    }
}