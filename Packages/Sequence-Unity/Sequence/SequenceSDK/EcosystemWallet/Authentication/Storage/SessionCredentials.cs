using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sequence.EcosystemWallet.Primitives;
using Sequence.Wallet;
using UnityEngine.Scripting;

namespace Sequence.EcosystemWallet
{
    [Preserve]
    [JsonConverter(typeof(SessionCredentialsConverter))]
    internal class SessionCredentials
    {
        public bool isExplicit;
        public string privateKey;
        public Address address;
        public Address sessionAddress;
        public Attestation attestation;
        public RSY signature;
        public string walletUrl;
        public string chainId;
        public string loginMethod;
        public string userEmail;
        
        public SessionCredentials(bool isExplicit, string privateKey, Address address, Attestation attestation, RSY signature, 
            string walletUrl, string chainId, string loginMethod, string userEmail)
        {
            this.isExplicit = isExplicit;
            this.privateKey = privateKey;
            this.address = address;
            this.attestation = attestation;
            this.signature = signature;
            this.walletUrl = walletUrl;
            this.chainId = chainId;
            this.loginMethod = loginMethod;
            this.userEmail = userEmail;
            
            sessionAddress = new EOAWallet(privateKey).GetAddress();
        }
    }
    
    [Preserve]
    internal class SessionCredentialsConverter : JsonConverter<SessionCredentials>
    {
        public override void WriteJson(JsonWriter writer, SessionCredentials value, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            
            writer.WritePropertyName("isExplicit");
            writer.WriteValue(value.isExplicit);

            writer.WritePropertyName("privateKey");
            writer.WriteValue(value.privateKey);

            writer.WritePropertyName("address");
            serializer.Serialize(writer, value.address);
            
            writer.WritePropertyName("walletUrl");
            writer.WriteValue(value.walletUrl);

            writer.WritePropertyName("chainId");
            writer.WriteValue(value.chainId.ToString());
            
            if (value.attestation != null)
            {
                writer.WritePropertyName("attestation");
                serializer.Serialize(writer, value.attestation);
            }
            
            if (value.signature != null)
            {
                writer.WritePropertyName("signature");
                serializer.Serialize(writer, value.signature);
            }

            if (value.loginMethod != null)
            {
                writer.WritePropertyName("loginMethod");
                serializer.Serialize(writer, value.loginMethod);
            }
            
            if (value.userEmail != null)
            {
                writer.WritePropertyName("userEmail");
                serializer.Serialize(writer, value.userEmail);
            }

            writer.WriteEndObject();
        }

        public override SessionCredentials ReadJson(JsonReader reader, Type objectType, SessionCredentials existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JObject obj = JObject.Load(reader);

            var isExplicit = obj["isExplicit"]?.ToObject<bool>() ?? false;
            var privateKey = obj["privateKey"]?.ToString();
            var address = obj["address"]?.ToObject<Address>(serializer);
            var attestation = obj["attestation"]?.ToObject<Attestation>(serializer);
            var signature = obj["signature"]?.ToObject<RSY>(serializer);

            var walletUrl = obj["walletUrl"]?.ToString();
            var chainId = obj["chainId"]?.ToString();
            var loginMethod = obj["loginMethod"]?.ToString();
            var userEmail = obj["userEmail"]?.ToString();

            return new SessionCredentials(isExplicit, privateKey, address, attestation, signature, walletUrl, chainId, loginMethod, userEmail);
        }
    }
}