using System.Collections.Generic;
using Newtonsoft.Json;

namespace Sequence.EcosystemWallet.IntegrationTests
{
    public static class IntegrationTestParamsUtils
    {
        public static T[] GetArray<T>(this Dictionary<string, object> @params, string key)
        {
            if (!@params.TryGetValue(key, out var inputObj)) 
                return null;
            
            var inputJson = inputObj.ToString();
            return JsonConvert.DeserializeObject<T[]>(inputJson);
        }
        
        public static Dictionary<string, object> GetNestedObjects(this Dictionary<string, object> dict, string key)
        {
            if (!dict.TryGetValue(key, out var inputObj)) 
                return null;
            
            var inputJson = inputObj.ToString();
            return JsonConvert.DeserializeObject<Dictionary<string, object>>(inputJson);
        }
    }
}