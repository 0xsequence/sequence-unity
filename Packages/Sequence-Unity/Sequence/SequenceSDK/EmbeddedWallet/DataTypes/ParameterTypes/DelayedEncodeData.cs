using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Sequence.EmbeddedWallet
{
    [Serializable]
    [JsonConverter(typeof(DelayedEncodeDataConverter))]
    public class DelayedEncodeData
    {
        public string abi;
        public object[] args;
        public string func;

        public DelayedEncodeData(string abi, object[] args, string func)
        {
            this.abi = abi;
            this.args = args;
            this.func = func;
        }
    }

    public class DelayedEncodeDataConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(DelayedEncodeData);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jsonObject = JObject.Load(reader);

            string abi = jsonObject["abi"].ToObject<string>();
            string func = jsonObject["func"].ToObject<string>();

            JArray argsArray = jsonObject["args"].ToObject<JArray>();
            object[] args = argsArray.ToObject<object[]>(serializer);
            return new DelayedEncodeData(abi, args, func);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            DelayedEncodeData delayedEncodeData = (DelayedEncodeData)value;

            JObject jsonObject = new JObject
            {
                { "abi", JToken.FromObject(delayedEncodeData.abi) },
                { "args", SerializeArgsWithSortedFields(delayedEncodeData.args, serializer) },
                { "func", JToken.FromObject(delayedEncodeData.func) }
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
            try
            {
                JObject jsonArg = JObject.FromObject(arg, serializer);
                
                JObject sortedJsonArg = new JObject(
                    jsonArg.Properties().OrderBy(p => p.Name)
                );

                return sortedJsonArg;
            }
            catch (Exception)
            {
                return JToken.FromObject(arg, serializer);
            }
        }
    }
}
