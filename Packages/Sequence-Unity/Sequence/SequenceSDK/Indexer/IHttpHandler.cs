using System.Threading.Tasks;

namespace Sequence
{
    public interface IHttpHandler
    {
        /// <summary>
        /// Makes an HTTP Post Request with content-type set to application/json
        /// </summary>
        /// <returns></returns>
        public Task<string> HttpPost(string chainID, string endPoint, object args, int retries = 0);
        public void HttpStream<T>(string chainID, string endPoint, object args, WebRPCStreamOptions<T> options, int retries = 0);
        public void AbortStreams();
    }
}