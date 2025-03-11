using System;
using Sequence.Utils;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Sequence.EmbeddedWallet
{
    [Preserve]
    [Serializable]
    [JsonConverter(typeof(AbiDataConverter))]
    public class AbiData
    {
        public string abi;
        public object[] args;

        public AbiData(string abi, object[] args)
        {
            this.abi = abi;
            this.args = args;
        }
    }

    public class AbiDataConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(AbiData);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jsonObject = JObject.Load(reader);

            string abi = jsonObject["abi"].ToObject<string>();

            JArray argsArray = jsonObject["args"].ToObject<JArray>();
            object[] args = argsArray.ToObject<object[]>(serializer);
            return new AbiData(abi, args);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            AbiData abiData = (AbiData)value;

            JObject jsonObject = new JObject
            {
                { "abi", JToken.FromObject(abiData.abi) },
                { "args", SerializeArgsWithSortedFields(abiData.args, serializer) },
            };

            jsonObject.WriteTo(writer);
        }

        private JArray SerializeArgsWithSortedFields(object[] args, JsonSerializer serializer)
        {
            JArray sortedArgsArray = new JArray();

            foreach (var arg in args)
            {
                JToken serializedArg = SerializeAlphabetically(arg, serializer);
                sortedArgsArray.Add(serializedArg);
            }

            return sortedArgsArray;
        }

        private JToken SerializeAlphabetically(object arg, JsonSerializer serializer)
        {
            JToken token = JToken.FromObject(arg, serializer);

            if (token.Type == JTokenType.Array)
            {
                JArray array = new JArray();
                foreach (var element in token.Children())
                {
                    array.Add(SerializeAlphabetically(element, serializer));
                }
                return array;
            }

            if (token.Type == JTokenType.Object)
            {
                JObject jsonObject = (JObject)token;
                JObject sortedObject = new JObject(
                    jsonObject.Properties()
                        .OrderBy(p => p.Name)
                        .Select(p => new JProperty(
                            p.Name, SerializeAlphabetically(p.Value, serializer))) 
                );
                return sortedObject;
            }

            return token;
        }

    }
}
