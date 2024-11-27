using System.Threading.Tasks;
using NUnit.Framework;

namespace Sequence.Marketplace.Mocks
{
    internal class MockClientAssertsExpectedFilter : IHttpClient
    {
        private CollectiblesFilter _filter;
        
        public MockClientAssertsExpectedFilter(CollectiblesFilter filter)
        {
            _filter = filter;
        }
        
        public Task<ReturnType> SendRequest<ReturnType>(Chain chain, string url)
        {
            throw new System.NotImplementedException();
        }

        public Task<ReturnType> SendRequest<ArgType, ReturnType>(Chain chain, string endpoint, ArgType args)
        {
            if (args is ListCollectiblesArgs myArgs)
            {
                Assert.AreEqual(_filter, myArgs.filter);
            }
            else
            {
                Assert.Fail($"Unexpected argument type: given {args.GetType()}, expected {typeof(ListCollectiblesArgs)}");
            }

            return null;
        }

        public Task<ReturnType> SendRequest<ArgType, ReturnType>(string url, ArgType args)
        {
            throw new System.NotImplementedException();
        }
    }
}