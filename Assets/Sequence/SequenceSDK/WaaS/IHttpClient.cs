using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Sequence.WaaS
{
    public interface IHttpClient
    {
        public Task<T2> SendRequest<T, T2>(string path, T args,
            [CanBeNull] Dictionary<string, string> headers = null, string overrideUrl = null);
    }
}