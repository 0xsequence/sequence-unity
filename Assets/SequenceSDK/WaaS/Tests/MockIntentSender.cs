using System;
using System.Threading.Tasks;
using Sequence.EmbeddedWallet;

namespace Sequence.EmbeddedWallet
{
    public class MockIntentSender : IIntentSender
    {
        private object[] _returnObjects;
        private Exception _exception;
        private int _numberOfCalls = 0;
        
        public MockIntentSender(params object[] returnObjects)
        {
            _returnObjects = returnObjects;
        }

        public MockIntentSender(Exception e)
        {
            _exception = e;
        }

        public void InjectException(Exception e)
        {
            _exception = e;
        }
        
        public async Task<T> SendIntent<T, T2>(T2 args, IntentType type = IntentType.None, uint timeBeforeExpiryInSeconds = 30)
        {
            if (_exception != null && !(_numberOfCalls == 0 && _returnObjects != null && _returnObjects.Length > 1))
            {
                throw _exception;
            }

            _numberOfCalls++;
            return (T)_returnObjects[_numberOfCalls - 1];
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

        public async Task<SuccessfulTransactionReturn> GetTransactionReceipt(SuccessfulTransactionReturn response)
        {
            if (_exception != null)
            {
                throw _exception;
            }

            _numberOfCalls++;
            return (SuccessfulTransactionReturn)_returnObjects[_numberOfCalls - 1];
        }
    }
}