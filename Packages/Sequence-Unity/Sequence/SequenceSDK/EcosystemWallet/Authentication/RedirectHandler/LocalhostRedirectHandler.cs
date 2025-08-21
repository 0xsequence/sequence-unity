using System;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Sequence.Utils;
using UnityEngine;

namespace Sequence.EcosystemWallet.Browser
{
    internal class LocalhostRedirectHandler : RedirectHandler
    {
        public override async Task<(bool Result, TResponse Data)> WaitForResponse<TPayload, TResponse>(string url, string action, TPayload payload)
        {
            Application.OpenURL(ConstructUrl(url, action, payload));
            
            try
            {
                var server = GameObject.FindObjectOfType<LocalhostServer>();
                if (!server)
                {
                    server = new GameObject("LocalhostServer").AddComponent<LocalhostServer>();
                }
                
                var queryString = await server.Run(RedirectUrl);

                var id = queryString["id"];
                if (id != Id)
                    throw new Exception($"Incoming request id '{id}' does not match id '{Id}'");

                if (queryString["error"] != null)
                    throw new Exception($"Error during request: {queryString["error"]}");
                
                var responsePayloadJson = Encoding.UTF8.GetString(Convert.FromBase64String(queryString["payload"]));
                var responsePayload = JsonConvert.DeserializeObject<TResponse>(responsePayloadJson);
                
                SequenceLog.Info(responsePayloadJson);
                
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