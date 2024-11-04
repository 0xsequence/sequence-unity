using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Sequence.Utils
{
    public class EnumConverter<T> : JsonConverter where T : struct, Enum
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(T) || objectType == typeof(T[]);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is T[] enumArray)
            {
                writer.WriteStartArray();
                foreach (var enumValue in enumArray)
                {
                    writer.WriteValue(enumValue.ToString());
                }
                writer.WriteEndArray();
            }
            else if (value is T enumValue)
            {
                writer.WriteValue(enumValue.ToString());
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (objectType == typeof(T[]))
            {
                var array = JArray.Load(reader);
                var enumValues = new T[array.Count];
                for (int i = 0; i < array.Count; i++)
                {
                    enumValues[i] = Enum.Parse<T>(array[i].ToString());
                }
                return enumValues;
            }
            else
            {
                var enumString = JToken.Load(reader).ToString();
                return Enum.Parse<T>(enumString);
            }
        }
    }

}