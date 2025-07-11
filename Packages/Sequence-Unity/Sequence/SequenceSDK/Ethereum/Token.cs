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
        public string Symbol;
        public string Name;
        public BigInteger Decimals;
        public BigInteger Price;
        public decimal PriceUsd;
        public string LogoUri;

        public Token(Chain chain, Address contract, string tokenId = null, string symbol = null, string name = null,
            BigInteger decimals = default, decimal priceUsd = default, string logoUri = null, BigInteger price = default)
        {
            Chain = chain;
            Contract = contract;
            TokenId = tokenId;
            Symbol = symbol;
            Name = name;
            Decimals = decimals;
            PriceUsd = priceUsd;
            LogoUri = logoUri;
            Price = price;
        }

        [Preserve]
        [JsonConstructor]
        public Token(BigInteger chainId, string contractAddress, string tokenId = null, string symbol = null, 
            string name = null, BigInteger decimals = default, decimal priceUsd = default, string logoUri = null, BigInteger price = default)
        {
            Chain = ChainDictionaries.ChainById[chainId.ToString()];
            if (!string.IsNullOrWhiteSpace(contractAddress))
            {
                Contract = new Address(contractAddress);
            }
            TokenId = tokenId;
            Symbol = symbol;
            Name = name;
            Decimals = decimals;
            PriceUsd = priceUsd;
            LogoUri = logoUri;
            Price = price;
        }
    }

    [Preserve]
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
            if (value.Symbol != null)
            {
                jsonObject["symbol"] = value.Symbol;
            }
            if (value.Name != null)
            {
                jsonObject["name"] = value.Name;
            }
            if (value.Decimals != default)
            {
                jsonObject["decimals"] = value.Decimals.ToString();
            }
            if (value.PriceUsd != default)
            {
                jsonObject["priceUsd"] = value.PriceUsd;
            }
            if (value.LogoUri != null)
            {
                jsonObject["logoUri"] = value.LogoUri;
            }
            if (value.Price != default)
            {
                jsonObject["price"] = value.Price.ToString();
            }

            jsonObject.WriteTo(writer);
        }

        public override Token ReadJson(JsonReader reader, Type objectType, Token existingValue,
            bool hasExistingValue, JsonSerializer serializer)
        {
            var jsonObject = JObject.Load(reader);

            BigInteger chainId = jsonObject["chainId"]?.Value<ulong>() ?? 0;
            string contractAddress = jsonObject["contractAddress"]?.Value<string>();
            if (string.IsNullOrWhiteSpace(contractAddress))
            {
                contractAddress = jsonObject["address"]?.Value<string>();
            }
            string tokenId = jsonObject["tokenId"]?.Value<string>();
            string symbol = jsonObject["symbol"]?.Value<string>();
            string name = jsonObject["name"]?.Value<string>();
            BigInteger decimals = (jsonObject["decimals"]?.Value<ulong>() ?? 18);
            decimal priceUsd = jsonObject["priceUsd"]?.Value<decimal>() ?? default;
            string logoUri = jsonObject["logoUri"]?.Value<string>();
            BigInteger price = 0;
            JToken priceToken = jsonObject["price"];
            if (priceToken != null && priceToken.Type != JTokenType.Null)
            {
                string priceStr = priceToken.Value<string>();
                if (!string.IsNullOrWhiteSpace(priceStr) && BigInteger.TryParse(priceStr, out var parsedPrice))
                {
                    price = parsedPrice;
                }
            }


            return new Token(chainId, contractAddress, tokenId, symbol, name, decimals, priceUsd, logoUri, price);
        }
    }

    public static class TokenExtensions
    {
        public static bool ContainsToken(this Token[] tokens, Token token)
        {
            if (tokens == null || token == null)
            {
                return false;
            }
            foreach (var t in tokens)
            {
                if (t.Chain == token.Chain && t.Contract.Equals(token.Contract) && t.TokenId == token.TokenId)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool ContainsToken(this Token[] tokens, Address token)
        {
            if (tokens == null || token == null)
            {
                return false;
            }
            foreach (var t in tokens)
            {
                if (t.Contract.Equals(token))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
