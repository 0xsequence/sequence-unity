using System;
using System.Numerics;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sequence.Config;
using Sequence.Utils;
using UnityEngine.Scripting;

namespace Sequence
{
    public class PriceFeed
    {
        [Serializable]
        [JsonConverter(typeof(TokenConverter))]
        public class Token
        {
            public Chain Chain;
            public Address Contract;

            public Token(Chain chain, Address contract)
            {
                Chain = chain;
                Contract = contract;
            }

            [Preserve]
            [JsonConstructor]
            public Token(BigInteger chainId, string contractAddress)
            {
                Chain = ChainDictionaries.ChainById[chainId.ToString()];
                Contract = new Address(contractAddress);
            }
        }

        private class TokenConverter : JsonConverter<Token>
        {
            public override void WriteJson(JsonWriter writer, Token value, JsonSerializer serializer)
            {
                var jsonObject = new JObject
                {
                    ["chainId"] = ulong.Parse(ChainDictionaries.ChainIdOf[value.Chain]),
                    ["contractAddress"] = value.Contract.ToString(),
                };
                jsonObject.WriteTo(writer);
            }

            public override Token ReadJson(JsonReader reader, Type objectType, Token existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                var jsonObject = JObject.Load(reader);

                BigInteger chainId = jsonObject["chainId"]?.Value<ulong>() ?? 0;
                string contractAddress = jsonObject["contractAddress"]?.Value<string>();

                return new Token(chainId, contractAddress);
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