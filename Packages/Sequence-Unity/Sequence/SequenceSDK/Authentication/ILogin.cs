using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sequence.Authentication
{
    public interface ILogin
    {
        public delegate void OnLoginSuccessHandler(string sessionId, string walletAddress);
        public event OnLoginSuccessHandler OnLoginSuccess;

        /// <summary>
        /// Includes:
        /// string error received when attempting to log in
        /// LoginMethod method used to attempt to log in
        /// string email the user attempted to log in with if available
        /// List<LoginMethod> loginMethods associated with the email the user attempted to login with - included if received an email already in use error
        /// </summary>
        public delegate void OnLoginFailedHandler(string error, LoginMethod method, string email = "", List<LoginMethod> loginMethods = default);
        public event OnLoginFailedHandler OnLoginFailed;

        public delegate void OnMFAEmailSentHandler(string email);

        public event OnMFAEmailSentHandler OnMFAEmailSent;

        public delegate void OnMFAEmailFailedToSendHandler(string email, string error);

        public event OnMFAEmailFailedToSendHandler OnMFAEmailFailedToSend;

        /// <summary>
        /// Attempt to send the user an MFA email
        /// Emits an OnMFAEmailSent event when successful
        /// Emits an OnMFAEmailFailedToSend event when unsuccessful
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public Task Login(string email);

        /// <summary>
        /// Attempt to log the user in
        /// Emits an OnLoginSuccess event when successful
        /// Emits an OnLoginFailed event when unsuccessful
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public Task Login(string email, string code);

        /// <summary>
        /// Attempt to log the user in using Google login
        /// Emits an OnLoginSuccess event when successful
        /// Emits an OnLoginFailed event when unsuccessful
        /// Social login may also emit relevant events
        /// </summary>
        public void GoogleLogin();

        /// <summary>
        /// Attempt to log the user in using Discord login
        /// Emits an OnLoginSuccess event when successful
        /// Emits an OnLoginFailed event when unsuccessful
        /// Social login may also emit relevant events
        /// </summary>
        public void DiscordLogin();

        /// <summary>
        /// Attempt to log the user in using Facebook login
        /// Emits an OnLoginSuccess event when successful
        /// Emits an OnLoginFailed event when unsuccessful
        /// Social login may also emit relevant events
        /// </summary>
        public void FacebookLogin();

        /// <summary>
        /// Attempt to log the user in using Apple login
        /// Emits an OnLoginSuccess event when successful
        /// Emits an OnLoginFailed event when unsuccessful
        /// Social login may also emit relevant events
        /// </summary>
        public void AppleLogin();

        /// <summary>
        /// Returns true if there is an authentication attempt in process
        /// </summary>
        /// <returns></returns>
        public bool IsLoggingIn();

        /// <summary>
        /// Used to setup or reset the authenticator for the ILogin
        /// </summary>
        public void SetupAuthenticator(IValidator validator = null, IAuthenticator authenticator = null);
        
        /// <summary>
        /// Using securely saved credentials, try to restore the session
        /// </summary>
        public void TryToRestoreSession();

        /// <summary>
        /// Login as a guest
        /// </summary>
        public Task GuestLogin();

        /// <summary>
        /// Login with PlayFab
        /// </summary>
        public void PlayFabLogin(string titleId, string sessionTicket, string email);
        
        /// <summary>
        /// Retry authentication, forcing a new account to be created. This will delete and override the account with the cached email if it exists.
        /// </summary>
        public void ForceCreateAccount();
    }
}