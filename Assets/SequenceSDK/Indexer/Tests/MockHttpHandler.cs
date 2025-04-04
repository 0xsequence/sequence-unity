using System;
using System.Threading.Tasks;

namespace Sequence
{
    public class MockHttpHandler : IHttpHandler
    {
        private string _response;
        private Exception _exception = null;

        public MockHttpHandler(string response)
        {
            _response = response;
        }

        public MockHttpHandler(Exception e)
        {
            _exception = e;
        }

        public Task<string> HttpPost(string chainID, string endPoint, object args, int retries = 0)
        {
            if (_exception != null)
            {
                throw _exception;
            }
            return Task.FromResult(_response);
        }

        public Task<T> HttpPost<T>(string url, object args)
        {
            throw new NotImplementedException();
        }

        public void HttpStream<T>(string chainID, string endPoint, object args, WebRPCStreamOptions<T> options)
        {
            throw new NotImplementedException();
        }

        public void AbortStreams()
        {
            throw new NotImplementedException();
        }
    }
}