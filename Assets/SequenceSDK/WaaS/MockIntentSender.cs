using System.Threading.Tasks;
using Sequence.WaaS.Authentication;

namespace Sequence.WaaS
{
    public class MockIntentSender : IIntentSender
    {
        private object _returnObject;
        
        public MockIntentSender(object returnObject)
        {
            _returnObject = returnObject;
        }
        
        public async Task<T> SendIntent<T, T2>(T2 args)
        {
            return (T)_returnObject;
        }

        public Task<bool> DropSession(string dropSessionId)
        {
            throw new System.NotImplementedException();
        }

        public Task<T> PostIntent<T>(string payload, string path)
        {
            throw new System.NotImplementedException();
        }

        public Task<WaaSSession[]> ListSessions()
        {
            throw new System.NotImplementedException();
        }
    }
}