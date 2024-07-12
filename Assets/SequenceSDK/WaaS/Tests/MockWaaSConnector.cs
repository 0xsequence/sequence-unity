using System;
using System.Threading.Tasks;
using Sequence.Authentication;
using SequenceSDK.WaaS;

namespace Sequence.WaaS.Tests
{
    public class MockWaaSConnector : IWaaSConnector
    {
        private int calls = 0;
        private string _returnValue;
        private string _secondReturnValue;
        private Exception _exception;

        public MockWaaSConnector(string returnValue, string secondReturnValue = "")
        {
            _returnValue = returnValue;
            _secondReturnValue = secondReturnValue;
        }
        
        public MockWaaSConnector(Exception exception)
        {
            _exception = exception;
        }
        
        public Task<string> InitiateAuth(IntentDataInitiateAuth initiateAuthIntent, LoginMethod method)
        {
            if (_exception != null)
            {
                throw _exception;
            }
            if (calls == 1)
            {
                return Task.FromResult(_secondReturnValue);
            }
            calls++;
            return Task.FromResult(_returnValue);
        }

        public Task ConnectToWaaS(IntentDataOpenSession loginIntent, LoginMethod method, string email = "")
        {
            throw new NotImplementedException();
        }
    }
}