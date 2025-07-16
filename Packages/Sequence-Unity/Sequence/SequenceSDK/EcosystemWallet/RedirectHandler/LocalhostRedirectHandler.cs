using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Sequence.EcosystemWallet.Authentication;
using UnityEngine;

namespace Sequence.EcosystemWallet.Browser
{
    public class LocalhostRedirectHandler : IRedirectHandler
    {
        public async Task<(bool Result, TResponse Data)> WaitForResponse<TPayload, TResponse>(string url, string action, TPayload payload)
        {
            var redirectId = $"sequence:{Guid.NewGuid().ToString()}";
            var serializedPayload = JsonConvert.SerializeObject(payload);
            var encodedPayload = Convert.ToBase64String(Encoding.UTF8.GetBytes(serializedPayload));
            var finalUrl = $"{url}?action={action}&payload={encodedPayload}&id={redirectId}&redirectUrl={RedirectOrigin.DefaultOrigin}&mode=redirect";
            
            Application.OpenURL(finalUrl);
            
            try
            {
                var listener = new HttpListener();
                listener.Prefixes.Add(RedirectOrigin.DefaultOrigin);
                listener.Start();
                
                var context = await listener.GetContextAsync();
                listener.Stop();

                var queryString = context.Request.QueryString;
                
                var id = queryString["id"];
                if (id != redirectId)
                    throw new Exception("Incorrect request id");

                if (queryString["error"] != null)
                    throw new Exception($"Error during request: {queryString["error"]}");
                
                var responsePayloadJson = Encoding.UTF8.GetString(Convert.FromBase64String(queryString["payload"]));
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