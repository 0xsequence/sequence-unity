using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Sequence.Marketplace
{
    
    [JsonConverter(typeof(ContractTypeConverter))]
    public enum ContractType
    {
        UNKNOWN,
        ERC20,
        ERC721,
        ERC1155
    }
    
    public static class ContractTypeExtensions
    {
        public static string AsString(this ContractType kind)
        {
            return kind.ToString();
        }
    }
    
    public class ContractTypeConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(ContractType) || objectType == typeof(ContractType[]);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is ContractType[] paymentMethods)
            {
                writer.WriteStartArray();
                foreach (var method in paymentMethods)
                {
                    writer.WriteValue(method.AsString());
                }
                writer.WriteEndArray();
            }
            else if (value is ContractType paymentMethod)
            {
                writer.WriteValue(paymentMethod.AsString());
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (objectType == typeof(ContractType[]))
            {
                var array = JArray.Load(reader);
                var methods = new ContractType[array.Count];
                for (int i = 0; i < array.Count; i++)
                {
                    methods[i] = Enum.Parse<ContractType>(array[i].ToString());
                }
                return methods;
            }
            else
            {
                var method = JToken.Load(reader).ToString();
                return Enum.Parse<ContractType>(method);
            }
        }
    }
}