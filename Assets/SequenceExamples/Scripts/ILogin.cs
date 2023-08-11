using System.Threading.Tasks;

namespace Sequence.Demo
{
    public interface ILogin
    {
        public delegate void OnLoginSuccessHandler(string userId);
        public event OnLoginSuccessHandler OnLoginSuccess;

        public delegate void OnLoginFailedHandler(string error);
        public event OnLoginFailedHandler OnLoginFailed;

        /// <summary>
        /// Attempt to log the user in
        /// Emits an OnLoginSuccess event when successful
        /// Emits an OnLoginFailed event when unsuccessful
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public Task Login(string email);
    }
}