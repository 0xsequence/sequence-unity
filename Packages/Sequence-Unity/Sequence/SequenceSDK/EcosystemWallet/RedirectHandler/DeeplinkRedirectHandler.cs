using System;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Sequence.Utils;
using UnityEngine;

namespace Sequence.EcosystemWallet.Browser
{
    internal class DeeplinkRedirectHandler : RedirectHandler
    {
        private NativeReceiver _receiver;

        public override async Task<(bool Result, TResponse Data)> WaitForResponse<TPayload, TResponse>(string url, string action, TPayload payload)
        {
            var go = new GameObject("SequenceNativeReceiver");
            _receiver = go.AddComponent<NativeReceiver>();
            
            var response = await _receiver.WaitForResponse(ConstructUrl(url, action, payload), RedirectUrl);
            
            GameObject.Destroy(_receiver.gameObject);

            if (!response.Contains(RedirectUrl))
                throw new Exception(response);
            
            var data = response.ExtractQueryAndHashParameters();
            
            var id = data["id"];
            if (id != Id)
                throw new Exception("Invalid request id");

            if (data.TryGetValue("error", out var error))
                throw new Exception(error);
            
            var responsePayloadJson = Encoding.UTF8.GetString(Convert.FromBase64String(Uri.UnescapeDataString(data["payload"])));
            var responsePayload = JsonConvert.DeserializeObject<TResponse>(responsePayloadJson);

            return (true, responsePayload);
        }
    }
}