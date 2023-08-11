using System.Threading.Tasks;

namespace Sequence.Demo
{
    public class MockLogin : ILogin
    {
        public event ILogin.OnLoginSuccessHandler OnLoginSuccess;
        public event ILogin.OnLoginFailedHandler OnLoginFailed;
        private IValidator _validator = new MockValidator();

        public async Task Login(string email)
        {
            if (!_validator.ValidateEmail(email))
            {
                OnLoginFailed?.Invoke("Login failed because of invalid email");
            }
            await Task.Delay(3000);
            if (email == "fail")
            {
                OnLoginFailed?.Invoke("Login failed for some reason");
            }
            else
            {
                OnLoginSuccess?.Invoke("1234567890");
            }
        }
    }
}