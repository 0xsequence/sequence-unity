using System;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Sequence.EcosystemWallet.Authentication;

namespace Sequence.EcosystemWallet.Browser
{
    internal abstract class RedirectHandler
    {
        protected string Id = $"sequence:{Guid.NewGuid().ToString()}";

        public abstract Task<(bool Result, TResponse Data)> WaitForResponse<TPayload, TResponse>(string url, string action, TPayload payload);
        
        protected string ConstructUrl<TPayload>(string url, string action, TPayload payload)
        {
            var serializedPayload = JsonConvert.SerializeObject(payload);
            var encodedPayload = Convert.ToBase64String(Encoding.UTF8.GetBytes(serializedPayload));
            return $"{url}?action={action}&payload={encodedPayload}&id={Id}&redirectUrl={RedirectOrigin.GetOriginString()}&mode=redirect";
        }
    }
}