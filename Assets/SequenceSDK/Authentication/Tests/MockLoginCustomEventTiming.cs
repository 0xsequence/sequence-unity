using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sequence.Authentication
{
    public class MockLoginCustomEventTiming : ILogin
    {
        public event ILogin.OnLoginSuccessHandler OnLoginSuccess;
        public event ILogin.OnLoginFailedHandler OnLoginFailed;
        public event ILogin.OnMFAEmailSentHandler OnMFAEmailSent;
        public event ILogin.OnMFAEmailFailedToSendHandler OnMFAEmailFailedToSend;

        public void FireOnLoginSuccessEvent()
        {
            OnLoginSuccess?.Invoke("","");
        }
        
        public void FireOnLoginFailedEvent()
        {
            OnLoginFailed?.Invoke("", LoginMethod.None);
        }
        
        public void FireOnMFAEmailSentEvent()
        {
            OnMFAEmailSent?.Invoke("");
        }
        
        public void FireOnMFAEmailFailedToSendEvent()
        {
            OnMFAEmailFailedToSend?.Invoke("","");
        }
        
        public Task Login(string email)
        {
            return null;
        }

        public Task Login(string email, string code)
        {
            return null;
        }

        public void GoogleLogin()
        {
            
        }

        public void DiscordLogin()
        {
            
        }

        public void FacebookLogin()
        {
            
        }

        public void AppleLogin()
        {
            
        }

        public bool IsLoggingIn()
        {
            throw new System.NotImplementedException();
        }

        public void SetupAuthenticator(IValidator validator = null, IAuthenticator authenticator = null)
        {
            
        }

        public void SetupAuthenticator(IValidator validator = null)
        {
            
        }

        public void TryToRestoreSession()
        {
            
        }

        public void GuestLogin()
        {
            throw new System.NotImplementedException();
        }

        public void PlayFabLogin(string titleId, string sessionTicket, string email)
        {
            throw new System.NotImplementedException();
        }

        public void ForceCreateAccount()
        {
            throw new System.NotImplementedException();
        }

        public List<LoginMethod> GetLoginMethodsAssociatedWithEmail(string email)
        {
            throw new System.NotImplementedException();
        }
    }
}