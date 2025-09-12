using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Scripting;

namespace Sequence.Utils
{
    [Preserve]
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
                    enumValues[i] = ParseEnum<T>(array[i].ToString());
                }
                return enumValues;
            }
            else
            {
                var enumString = JToken.Load(reader).ToString();
                return ParseEnum<T>(enumString);
            }
        }
    
        private T ParseEnum<T>(string enumString) where T : struct
        {
            if (!Enum.TryParse<T>(enumString, ignoreCase: true, out var parsed))
            {
                SequenceLog.Warning($"Unknown enum value '{enumString}' for type {typeof(T).Name}. Returning default.");
                return default(T);
            }

            return parsed;
        }
    }

}