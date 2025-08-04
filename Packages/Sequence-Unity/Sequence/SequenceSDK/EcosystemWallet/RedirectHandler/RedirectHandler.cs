using System;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Sequence.EcosystemWallet.Browser
{
    public abstract class RedirectHandler
    {
        protected string Id = $"sequence-{Guid.NewGuid().ToString()}";
        protected string RedirectUrl;

        public abstract Task<(bool Result, TResponse Data)> WaitForResponse<TPayload, TResponse>(string url, string action, TPayload payload);

        public void SetRedirectUrl(string redirectUrl)
        {
            RedirectUrl = redirectUrl;
        }
        
        protected string ConstructUrl<TPayload>(string url, string action, TPayload payload)
        {
            var serializedPayload = JsonConvert.SerializeObject(payload);
            var encodedPayload = Convert.ToBase64String(Encoding.UTF8.GetBytes(serializedPayload));
            var finalUrl = $"{url}?action={action}&payload={encodedPayload}&id={Id}&mode=redirect";
            
            if (!string.IsNullOrEmpty(RedirectUrl))
                finalUrl += $"&redirectUrl={RedirectUrl}";
            
            return finalUrl;
        }
    }
}