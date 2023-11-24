using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SequenceSDK.WaaS
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
            DelayedEncodeData delayedEncodeData = new DelayedEncodeData(abi, args, func);

            return delayedEncodeData;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            DelayedEncodeData delayedEncodeData = (DelayedEncodeData)value;

            JObject jsonObject = new JObject();
            jsonObject.Add("abi", JToken.FromObject(delayedEncodeData.abi));
            jsonObject.Add("args", JArray.FromObject(delayedEncodeData.args, serializer));
            jsonObject.Add("func", JToken.FromObject(delayedEncodeData.func));

            jsonObject.WriteTo(writer);
        }
    }
}