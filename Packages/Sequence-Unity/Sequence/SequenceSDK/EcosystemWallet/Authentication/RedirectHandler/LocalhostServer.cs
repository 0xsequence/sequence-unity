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

                var result = await _listener.GetContextAsync();

                var responseString = "{}";
                var buffer = Encoding.UTF8.GetBytes(responseString);

                result.Response.StatusCode = 200;
                result.Response.ContentType = "application/json";
                result.Response.ContentLength64 = buffer.Length;
                result.Response.KeepAlive = true;

                await result.Response.OutputStream.WriteAsync(buffer, 0, buffer.Length);

                result.Response.OutputStream.Close();
                result.Response.Close();

                return result.Request.QueryString;
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
                return null;
            }
        }
    }
}