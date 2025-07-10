using System;
using System.Collections.Specialized;
using System.Threading.Tasks;
using UnityEngine;
using System.Net;

namespace Sequence.EcosystemWallet.Authentication
{
    public class EditorServer
    {
        private HttpListener _listener;
        
        public void StopListening()
        {
            _listener.Stop();
            Debug.Log($"Stopped listening");
        }

        public async Task<(bool Result, NameValueCollection QueryString)> WaitForResponse(string host, int port)
        {
            try
            {
                _listener = new HttpListener();
                _listener.Prefixes.Add($"http://{host}:{port}/");
                _listener.Start();
                
                var context = await _listener.GetContextAsync();
                StopListening();

                foreach (var key in context.Request.QueryString.AllKeys)
                    Debug.Log($"Key: {key}, Value: {context.Request.QueryString[key]}");

                return (true, context.Request.QueryString);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                return (false, null);
            }
        }
    }
}