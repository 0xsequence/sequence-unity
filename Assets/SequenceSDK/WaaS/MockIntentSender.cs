using System;
using System.Threading.Tasks;
using Sequence.WaaS.Authentication;
using SequenceSDK.WaaS;

namespace Sequence.WaaS
{
    public class MockIntentSender : IIntentSender
    {
        private object _returnObject;
        private Exception _exception;
        
        public MockIntentSender(object returnObject)
        {
            _returnObject = returnObject;
        }

        public MockIntentSender(Exception e)
        {
            _exception = e;
        }
        
        public async Task<T> SendIntent<T, T2>(T2 args, IntentType type = IntentType.None, uint timeBeforeExpiryInSeconds = 30)
        {
            if (_exception != null)
            {
                throw _exception;
            }
            
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