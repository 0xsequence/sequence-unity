using System;
using System.Net;
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
                var listener = new HttpListener();
                listener.Prefixes.Add(RedirectUrl.AppendTrailingSlashIfNeeded());
                listener.Start();
                
                var context = await listener.GetContextAsync();
                listener.Stop();

                var queryString = context.Request.QueryString;
                
                var id = queryString["id"];
                if (id != Id)
                    throw new Exception("Incorrect request id");

                if (queryString["error"] != null)
                    throw new Exception($"Error during request: {queryString["error"]}");
                
                var responsePayloadJson = Encoding.UTF8.GetString(Convert.FromBase64String(queryString["payload"]));
                var responsePayload = JsonConvert.DeserializeObject<TResponse>(responsePayloadJson);
                
                Debug.Log($"{responsePayloadJson}");
                
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