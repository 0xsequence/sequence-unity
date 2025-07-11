using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace Sequence.EcosystemWallet.Browser
{
    public class LocalhostRedirectHandler : IRedirectHandler
    {
        private const string Listener = "http://localhost:8080";
        
        public async Task<(bool Result, NameValueCollection QueryString)> WaitForResponse(string url, string action, Dictionary<string, object> payload)
        {
            var redirectId = $"sequence:{Guid.NewGuid().ToString()}";
            var encodedPayload = Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(payload)));
            var finalUrl = $"{url}?action={action}&payload={encodedPayload}&id={redirectId}&redirectUrl={Listener}&mode=redirect";
            
            Application.OpenURL(finalUrl);
            
            try
            {
                var listener = new HttpListener();
                listener.Prefixes.Add(Listener);
                listener.Start();
                
                var context = await listener.GetContextAsync();
                listener.Stop();

                var queryString = context.Request.QueryString;
                foreach (var key in queryString.AllKeys)
                    Debug.Log($"Key: {key}, Value: {queryString[key]}");
                
                var id = queryString["id"];
                if (id != redirectId)
                    throw new Exception("Incorrect request id");

                return (true, queryString);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                return (false, null);
            }
        }
    }
}