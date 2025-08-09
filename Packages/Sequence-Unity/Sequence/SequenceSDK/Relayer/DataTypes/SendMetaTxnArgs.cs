using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine.Scripting;

namespace Sequence.Relayer
{
    [JsonConverter(typeof(SessionCredentialsConverter))]
    public class SendMetaTxnArgs
    {
        public MetaTxn call;
        public string quote;
        public int projectID;
        public IntentPrecondition[] preconditions;

        public SendMetaTxnArgs(MetaTxn call, string quote = null, int projectID = -1, IntentPrecondition[] preconditions = null)
        {
            this.call = call;
            this.quote = quote;
            this.projectID = projectID;
            this.preconditions = preconditions;
        }
    }
    
    [Preserve]
    internal class SessionCredentialsConverter : JsonConverter<SendMetaTxnArgs>
    {
        public override void WriteJson(JsonWriter writer, SendMetaTxnArgs value, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            
            writer.WritePropertyName("call");
            serializer.Serialize(writer, value.call);

            if (!string.IsNullOrEmpty(value.quote))
            {
                writer.WritePropertyName("quote");
                writer.WriteValue(value.quote);
            }

            if (value.projectID != -1)
            {
                writer.WritePropertyName("projectID");
                writer.WriteValue(value.quote);
            }
            
            if (value.preconditions != null)
            {
                writer.WritePropertyName("preconditions");
                serializer.Serialize(writer, value.preconditions);
            }

            writer.WriteEndObject();
        }

        public override SendMetaTxnArgs ReadJson(JsonReader reader, Type objectType, SendMetaTxnArgs existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            return null;
        }
    }
}