using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Sequence.Utils
{
    public static class UnityWebRequestExtensions
    {
        public static TaskAwaiter GetAwaiter(this AsyncOperation asyncOp)
        {
            var tcs = new TaskCompletionSource<object>();
            asyncOp.completed += obj => { tcs.SetResult(null); };
            return ((Task)tcs.Task).GetAwaiter();
        }

        public static TaskAwaiter GetAwaiter(this UnityWebRequestAsyncOperation webReqOp)
        {
            var tcs = new TaskCompletionSource<object>();
            webReqOp.completed += obj =>
            {
                {
                    if (webReqOp.webRequest.responseCode >= 300)
                    {
                        tcs.SetException(new FileLoadException(webReqOp.webRequest.error));
                    }
                    else
                    {
                        tcs.SetResult(null);
                    }
                }
            };
            return ((Task)tcs.Task).GetAwaiter();
        }
    }
}