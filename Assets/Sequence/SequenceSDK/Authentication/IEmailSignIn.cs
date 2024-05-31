using System.Threading.Tasks;

namespace Sequence.Authentication
{
    public interface IEmailSignIn
    {
        /// <summary>
        /// Initiate the email sign in process
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public Task<string> SignIn(string email);
        
        /// <summary>
        /// Finish the email sign in process, providing the OTP `code`
        /// </summary>
        /// <param name="challengeSession"></param>
        /// <param name="email"></param>
        /// <param name="code"></param>
        /// <param name="sessionWalletAddress"></param>
        /// <returns></returns>
        public Task<string> Login(string challengeSession, string email, string code, string sessionWalletAddress = "");
        
        /// <summary>
        /// Sign up the user with the email auth provider so that they can sign in via email + OTP
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public Task SignUp(string email);
    }
}