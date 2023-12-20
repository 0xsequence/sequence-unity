using System.Threading.Tasks;

namespace Sequence.Authentication
{
    public class MockLogin : ILogin
    {
        public event ILogin.OnLoginSuccessHandler OnLoginSuccess;
        public event ILogin.OnLoginFailedHandler OnLoginFailed;
        public event ILogin.OnMFAEmailSentHandler OnMFAEmailSent;
        public event ILogin.OnMFAEmailFailedToSendHandler OnMFAEmailFailedToSend;

        private IValidator _validator = new MockValidator();
        
        public async Task Login(string email)
        {
            if (!_validator.ValidateEmail(email))
            {
                OnMFAEmailFailedToSend?.Invoke(email, $"Invalid email: {email}");
            }
            await Task.Delay(1000);
            if (email == "failSend")
            {
                OnMFAEmailFailedToSend?.Invoke(email, $"Failed to send email to {email}");
            }
            else
            {
                OnMFAEmailSent?.Invoke(email);
            }
        }

        public async Task Login(string email, string code)
        {
            if (!_validator.ValidateEmail(email))
            {
                OnLoginFailed?.Invoke("Login failed because of invalid email");
            }
            await Task.Delay(3000);
            if (email == "failLogin")
            {
                OnLoginFailed?.Invoke("Login failed for some reason");
            }
            else
            {
                OnLoginSuccess?.Invoke("1234567890", "0xc683a014955b75F5ECF991d4502427c8fa1Aa249");
            }
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
    }
}