using System;
using System.Numerics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine.Scripting;

namespace Sequence
{
    [Serializable]
    [JsonConverter(typeof(TokenConverter))]
    public class Token
    {
        public Chain Chain;
        public Address Contract;
        public string TokenId;

        public Token(Chain chain, Address contract, string tokenId = null)
        {
            Chain = chain;
            Contract = contract;
            TokenId = tokenId;
        }

        [Preserve]
        [JsonConstructor]
        public Token(BigInteger chainId, string contractAddress, string tokenId = null)
        {
            Chain = ChainDictionaries.ChainById[chainId.ToString()];
            Contract = new Address(contractAddress);
            TokenId = tokenId;
        }
    }

    internal class TokenConverter : JsonConverter<Token>
    {
        public override void WriteJson(JsonWriter writer, Token value, JsonSerializer serializer)
        {
            var jsonObject = new JObject
            {
                ["chainId"] = ulong.Parse(ChainDictionaries.ChainIdOf[value.Chain]),
                ["contractAddress"] = value.Contract.ToString(),
            };
            if (value.TokenId != null)
            {
                jsonObject["tokenId"] = value.TokenId;
            }

            jsonObject.WriteTo(writer);
        }

        public override Token ReadJson(JsonReader reader, Type objectType, Token existingValue,
            bool hasExistingValue, JsonSerializer serializer)
        {
            var jsonObject = JObject.Load(reader);

            BigInteger chainId = jsonObject["chainId"]?.Value<ulong>() ?? 0;
            string contractAddress = jsonObject["contractAddress"]?.Value<string>();
            string tokenId = jsonObject["tokenId"]?.Value<string>();

            return new Token(chainId, contractAddress, tokenId);
        }
    }
}