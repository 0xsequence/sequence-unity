using System;
using System.Threading.Tasks;
using PlayFab;
using PlayFab.ClientModels;
using Sequence.Authentication;
using Sequence.EmbeddedWallet;
using Sequence.EmbeddedWallet.Tests;

namespace Sequence.EmbeddedWallet.Tests
{
    public class EndToEndTestHarness
    {
        private SequenceLogin _login;
        
        public EndToEndTestHarness(SequenceLogin login = null)
        {
            if (login == null)
            {
                _login = SequenceLogin.GetInstance();
            }
            else
            {
                _login = login;
            }
            _login.ResetLoginAfterTest();
        }
        
        public async Task Login(Action<SequenceWallet> OnLogin, ILogin.OnLoginFailedHandler OnFailedLogin)
        {
            _login.OnLoginFailed += OnFailedLogin;
            SequenceWallet.OnWalletCreated += OnLogin;
            await _login.ConnectToWaaSAsGuest();
            _login.OnLoginFailed -= OnFailedLogin;
            SequenceWallet.OnWalletCreated -= OnLogin;
        }

        public async Task LoginWithPlayFab(LoginResult result, string email, Action<SequenceWallet> OnLogin,
            ILogin.OnLoginFailedHandler OnFailedLogin)
        {
            _login.OnLoginFailed += OnFailedLogin;
            SequenceWallet.OnWalletCreated += OnLogin;
            await _login.ConnectToWaaSViaPlayFab(PlayFabSettings.staticSettings.TitleId, result.SessionTicket, email);
            _login.OnLoginFailed -= OnFailedLogin;
            SequenceWallet.OnWalletCreated -= OnLogin;
        }
    }
}