using System;
using System.Threading.Tasks;

namespace Sequence.Marketplace.Mocks
{
    internal class MockClientReturnsGiven : IHttpClient
    {
        private ListCollectiblesReturn _response;
        
        public MockClientReturnsGiven(ListCollectiblesReturn response)
        {
            _response = response;
        }
        
        public Task<ReturnType> SendRequest<ReturnType>(Chain chain, string url)
        {
            throw new System.NotImplementedException();
        }

        public Task<ReturnType> SendRequest<ArgType, ReturnType>(Chain chain, string endpoint, ArgType args)
        {
            if (typeof(ReturnType) == typeof(ListCollectiblesReturn))
            {
                return Task.FromResult((ReturnType)(object)_response);
            }
            else
            {
                throw new Exception($"Unexpected return type: given {typeof(ReturnType)}, expected {typeof(ListCollectiblesReturn)}");
            }
        }

        public Task<ReturnType> SendRequest<ArgType, ReturnType>(string url, ArgType args)
        {
            throw new System.NotImplementedException();
        }
    }
}