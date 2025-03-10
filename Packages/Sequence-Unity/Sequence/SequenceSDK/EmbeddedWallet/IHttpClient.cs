using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Sequence.EmbeddedWallet
{
    public interface  IHttpClient
    {
        public Task<T2> SendRequest<T, T2>(string path, T args,
            [CanBeNull] Dictionary<string, string> headers = null, string overrideUrl = null);

        public Task<TimeSpan> GetTimeShift();
    }
}