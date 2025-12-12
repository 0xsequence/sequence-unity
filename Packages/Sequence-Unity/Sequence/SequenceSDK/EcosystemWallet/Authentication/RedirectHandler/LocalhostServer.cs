using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Sequence.Utils;
using UnityEngine;

namespace Sequence.EcosystemWallet.Browser
{
    public class LocalhostServer : MonoBehaviour
    {
        private static HttpListener _listener;

        public async Task<NameValueCollection> Run(string url)
        {
            try
            {
                if (_listener == null)
                {
                    _listener = new HttpListener
                    {
                        Prefixes = { url.AppendTrailingSlashIfNeeded() },
                    };

                    _listener.Start();
                }


                HttpListenerContext context = null;
                while (context == null || !context.Request.RawUrl.Contains("?id"))
                    context = await _listener.GetContextAsync();

                var responseString = "{}";
                var buffer = Encoding.UTF8.GetBytes(responseString);

                context.Response.StatusCode = 200;
                context.Response.ContentType = "application/json";
                context.Response.ContentLength64 = buffer.Length;
                context.Response.KeepAlive = true;

                await context.Response.OutputStream.WriteAsync(buffer, 0, buffer.Length);

                context.Response.OutputStream.Close();
                context.Response.Close();

                return context.Request.QueryString;
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
                return null;
            }
        }
    }
}