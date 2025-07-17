using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sequence.EcosystemWallet.Primitives;
using UnityEngine.Scripting;

namespace Sequence.EcosystemWallet.Authentication
{
    [Preserve]
    [JsonConverter(typeof(SessionDataConverter))]
    internal class SessionData
    {
        public string privateKey;
        public Address address;
        public Attestation attestation;
        public RSY signature;
        public string chainId;
        public string loginMethod;
        public string email;
        
        public SessionData(string privateKey, Address address, Attestation attestation, RSY signature, 
            string chainId, string loginMethod, string email)
        {
            this.privateKey = privateKey;
            this.address = address;
            this.attestation = attestation;
            this.signature = signature;
            this.chainId = chainId;
            this.loginMethod = loginMethod;
            this.email = email;
        }
    }
    
    [Preserve]
    internal class SessionDataConverter : JsonConverter<SessionData>
    {
        public override void WriteJson(JsonWriter writer, SessionData value, JsonSerializer serializer)
        {
            writer.WriteStartObject();

            writer.WritePropertyName("privateKey");
            writer.WriteValue(value.privateKey);

            writer.WritePropertyName("address");
            serializer.Serialize(writer, value.address);

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
            
            if (value.email != null)
            {
                writer.WritePropertyName("email");
                serializer.Serialize(writer, value.email);
            }

            writer.WriteEndObject();
        }

        public override SessionData ReadJson(JsonReader reader, Type objectType, SessionData existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JObject obj = JObject.Load(reader);

            string privateKey = obj["privateKey"]?.ToString();
            var address = obj["address"]?.ToObject<Address>(serializer);
            var attestation = obj["attestation"]?.ToObject<Attestation>(serializer);
            var signature = obj["signature"]?.ToObject<RSY>(serializer);

            string chainId = obj["chainId"]?.ToString();
            string loginMethod = obj["loginMethod"]?.ToString();
            string email = obj["email"]?.ToString();

            return new SessionData(privateKey, address, attestation, signature, chainId, loginMethod, email);
        }
    }
}