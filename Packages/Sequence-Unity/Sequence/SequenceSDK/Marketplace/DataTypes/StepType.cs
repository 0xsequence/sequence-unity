using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Sequence.Marketplace
{
    [JsonConverter(typeof(StepTypeConverter))]
    public enum StepType
    {
        unknown,
        tokenApproval,
        buy,
        sell,
        createListing,
        createOffer,
        signEIP712,
        signEIP191,
    }
    
    public static class StepTypeExtensions
    {
        public static string AsString(this StepType kind)
        {
            return kind.ToString();
        }
    }
    
    public class StepTypeConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(StepType) || objectType == typeof(StepType[]);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is StepType[] paymentMethods)
            {
                writer.WriteStartArray();
                foreach (var method in paymentMethods)
                {
                    writer.WriteValue(method.AsString());
                }
                writer.WriteEndArray();
            }
            else if (value is StepType paymentMethod)
            {
                writer.WriteValue(paymentMethod.AsString());
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (objectType == typeof(StepType[]))
            {
                var array = JArray.Load(reader);
                var methods = new StepType[array.Count];
                for (int i = 0; i < array.Count; i++)
                {
                    methods[i] = Enum.Parse<StepType>(array[i].ToString());
                }
                return methods;
            }
            else
            {
                var method = JToken.Load(reader).ToString();
                return Enum.Parse<StepType>(method);
            }
        }
    }
}