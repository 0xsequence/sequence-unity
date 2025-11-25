using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace Sequence.EcosystemWallet.Browser
{
    internal class BrowserRedirectHandler : RedirectHandler
    {
        private class NativeReceiveArgs
        {
            public string url;
            public string action;
            public object payload;
        }
        
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
        
        public override async Task<(bool Result, TResponse Data)> WaitForResponse<TPayload, TResponse>(string url, string action, TPayload payload)
        {
            try
            {
                var go = new GameObject("SequenceNativeReceiver");
                _receiver = go.AddComponent<NativeReceiver>();
                
                var response = await _receiver.WaitForResponse(JsonConvert.SerializeObject(new NativeReceiveArgs
                {
                    url = url, 
                    action = action, 
                    payload = payload
                }));
                
                GameObject.Destroy(_receiver.gameObject);
                
                var data = JsonConvert.DeserializeObject<ResponseData>(response);
                if (data.error != null)
                    throw new Exception(data.error.error);

                var responsePayloadJson = Encoding.UTF8.GetString(Convert.FromBase64String(data.payload));
                var responsePayload = JsonConvert.DeserializeObject<TResponse>(responsePayloadJson);

                return (true, responsePayload);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                return (false, default);
            }
        }
    }
}