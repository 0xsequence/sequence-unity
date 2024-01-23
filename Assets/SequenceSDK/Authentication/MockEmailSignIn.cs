using System;
using System.Threading.Tasks;

namespace Sequence.Authentication
{
    public class MockEmailSignIn : IEmailSignIn
    {
        private int calls = 0;
        private string _returnValue;
        private string _secondReturnValue;
        
        public MockEmailSignIn(string returnValue, string secondReturnValue = "")
        {
            _returnValue = returnValue;
            _secondReturnValue = secondReturnValue;
        }
        
        public async Task<string> SignIn(string email)
        {
            string returnValue = _returnValue;
            if (calls == 1)
            {
                returnValue = _secondReturnValue;
            }
            calls++;
            return returnValue;
        }

        public async Task<string> Login(string challengeSession, string email, string code)
        {
            throw new NotImplementedException();
        }

        public async Task SignUp(string email)
        {
            return;
        }
    }
}