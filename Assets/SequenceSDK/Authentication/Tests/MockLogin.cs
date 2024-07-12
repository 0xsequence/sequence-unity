using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sequence.Authentication
{
    public class MockLogin : ILogin
    {
        public event ILogin.OnLoginSuccessHandler OnLoginSuccess;
        public event ILogin.OnLoginFailedHandler OnLoginFailed;
        public event ILogin.OnMFAEmailSentHandler OnMFAEmailSent;
        public event ILogin.OnMFAEmailFailedToSendHandler OnMFAEmailFailedToSend;

        private IValidator _validator = new Validator();
        
        public async Task Login(string email)
        {
            if (!_validator.ValidateEmail(email))
            {
                OnMFAEmailFailedToSend?.Invoke(email, $"Invalid email: {email}");
                return;
            }
            await Task.Delay(1000);
            if (email == "failSend@fakeDomain.com")
            {
                OnMFAEmailFailedToSend?.Invoke(email, $"Failed to send email to {email}");
                return;
            }
            
            OnMFAEmailSent?.Invoke(email);
        }

        public async Task Login(string email, string code)
        {
            if (!_validator.ValidateEmail(email))
            {
                OnLoginFailed?.Invoke("Login failed because of invalid email", LoginMethod.Email);
                return;
            }
            if (!_validator.ValidateCode(code))
            {
                OnLoginFailed?.Invoke("Login failed because of invalid code", LoginMethod.Email);
                return;
            }
            await Task.Delay(1000);
            if (email == "failLogin@noReason.net")
            {
                OnLoginFailed?.Invoke("Login failed for some reason", LoginMethod.Email);
                return;
            }
            
            OnLoginSuccess?.Invoke("1234567890", "0xc683a014955b75F5ECF991d4502427c8fa1Aa249");
        }

        public void GoogleLogin()
        {
            OnLoginSuccess?.Invoke("1234567890", "0xc683a014955b75F5ECF991d4502427c8fa1Aa249");
        }

        public void DiscordLogin()
        {
            OnLoginSuccess?.Invoke("1234567890", "0xc683a014955b75F5ECF991d4502427c8fa1Aa249");
        }

        public void FacebookLogin()
        {
            OnLoginSuccess?.Invoke("1234567890", "0xc683a014955b75F5ECF991d4502427c8fa1Aa249");
        }

        public void AppleLogin()
        {
            OnLoginSuccess?.Invoke("1234567890", "0xc683a014955b75F5ECF991d4502427c8fa1Aa249");
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

        public void OverrideAccount()
        {
            throw new System.NotImplementedException();
        }

        public List<LoginMethod> GetLoginMethodsAssociatedWithEmail(string email)
        {
            throw new System.NotImplementedException();
        }
    }
}