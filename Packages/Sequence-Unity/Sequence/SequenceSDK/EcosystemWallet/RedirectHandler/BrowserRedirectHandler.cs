using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace Sequence.EcosystemWallet.Browser
{
    public class BrowserRedirectHandler : IRedirectHandler
    {
        private class ResponseErrorData
        {
            public string error;
        }
        
        private class ResponseData
        {
            public string id;
            public string type;
            public string payload;
            public ResponseErrorData error;
        }
        
        private NativeReceiver _receiver;
        
        public async Task<(bool Result, NameValueCollection QueryString)> WaitForResponse(string url, string action, Dictionary<string, object> payload)
        {
            try
            {
                var go = new GameObject("SequenceNativeReceiver");
                _receiver = go.AddComponent<NativeReceiver>();
                
                var response = await _receiver.WaitForResponse(JsonConvert.SerializeObject(new {url, action, payload}));
                
                GameObject.Destroy(_receiver.gameObject);
                
                var data = JsonConvert.DeserializeObject<ResponseData>(response);
                if (data.error != null)
                    throw new Exception(data.error.error);

                var query = new NameValueCollection();
                query.Add("payload", data.payload);

                return (true, query);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                return (false, null);
            }
        }
    }
}