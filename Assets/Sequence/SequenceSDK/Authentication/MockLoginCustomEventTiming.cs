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
            OnLoginFailed?.Invoke("");
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

        public void SetupAuthenticator(IValidator validator = null)
        {
            
        }
    }
}