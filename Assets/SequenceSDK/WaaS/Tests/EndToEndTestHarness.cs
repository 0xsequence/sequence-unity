using System;
using System.Threading.Tasks;
using Sequence.Authentication;
using Sequence.EmbeddedWallet;

namespace Sequence.WaaS.Tests
{
    public class EndToEndTestHarness
    {
        private SequenceLogin _login;
        
        public EndToEndTestHarness(SequenceLogin login = null)
        {
            if (login == null)
            {
                _login = new SequenceLogin();
            }
            else
            {
                _login = login;
            }
        }
        
        public async Task Login(Action<SequenceWallet> OnLogin, ILogin.OnLoginFailedHandler OnFailedLogin)
        {
            _login.OnLoginFailed += OnFailedLogin;
            SequenceWallet.OnWalletCreated += OnLogin;
            await _login.ConnectToWaaSAsGuest();
            _login.OnLoginFailed -= OnFailedLogin;
            SequenceWallet.OnWalletCreated -= OnLogin;
        }
    }
}