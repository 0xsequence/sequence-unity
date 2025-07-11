using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;

namespace Sequence.EcosystemWallet.Browser
{
    public interface IRedirectHandler
    {
        Task<(bool Result, NameValueCollection QueryString)> WaitForResponse(string url, string action, Dictionary<string, object> payload);
    }
}