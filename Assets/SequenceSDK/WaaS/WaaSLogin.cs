using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.CognitoIdentity.Model;
using Newtonsoft.Json;
using Sequence.ABI;
using Sequence.Authentication;
using Sequence.Config;
using Sequence.Extensions;
using Sequence.Utils;
using Sequence.WaaS.Authentication;
using Sequence.Wallet;
using SequenceSDK.WaaS;
using UnityEngine;
using UnityEngine.Serialization;

namespace Sequence.WaaS
{
    public class WaaSLogin : ILogin 
    {
        public static string WaaSWithAuthUrl { get; private set; }
        public const string WaaSLoginMethod = "WaaSLoginMethod";

        private int _waasProjectId;
        private string _waasVersion;
        private OpenIdAuthenticator _authenticator;
        private IValidator _validator;
        private IEmailSignIn _emailSignIn;
        private string _challengeSession;
        private int retries = 0;
        private EthWallet _sessionWallet;
        private string _sessionId;

        public WaaSLogin(IValidator validator = null)
        {
            SequenceConfig config = SequenceConfig.GetConfig();
            string waasVersion = config.WaaSVersion;
            if (string.IsNullOrWhiteSpace(waasVersion))
            {
                throw SequenceConfig.MissingConfigError("WaaS Version");
            }
            _waasVersion = waasVersion;

            ConfigJwt configJwt = SequenceConfig.GetConfigJwt();

            string rpcUrl = configJwt.rpcServer;
            if (string.IsNullOrWhiteSpace(rpcUrl))
            {
                throw SequenceConfig.MissingConfigError("RPC Server");
            }
            WaaSWithAuthUrl = $"{rpcUrl.AppendTrailingSlashIfNeeded()}rpc/WaasAuthenticator";
            
            int projectId = configJwt.projectId;
            if (string.IsNullOrWhiteSpace(projectId.ToString()))
            {
                throw SequenceConfig.MissingConfigError("Project ID");
            }
            _waasProjectId = projectId;
            
            _sessionWallet = new EthWallet();
            _sessionId = IntentDataOpenSession.CreateSessionId(_sessionWallet.GetAddress());
            
            _authenticator = new OpenIdAuthenticator(SequenceCoder.KeccakHashASCII(_sessionId).EnsureHexPrefix());
            _authenticator.PlatformSpecificSetup();
            Application.deepLinkActivated += _authenticator.HandleDeepLink;
            _authenticator.SignedIn += OnSocialLogin;

            if (validator == null)
            {
                validator = new Validator();
            }
            _validator = validator;

            try
            {
                _emailSignIn = new AWSEmailSignIn(configJwt.emailRegion, configJwt.emailClientId);
            }
            catch (Exception e)
            {
                Debug.LogWarning("AWS config not found in config key. Email sign in will not work. Please contact Sequence support for more information.");
            }
        }
        
        public void InjectEmailSignIn(IEmailSignIn emailSignIn)
        {
            _emailSignIn = emailSignIn;
        }
        
        public event ILogin.OnLoginSuccessHandler OnLoginSuccess;
        public event ILogin.OnLoginFailedHandler OnLoginFailed;
        public event ILogin.OnMFAEmailSentHandler OnMFAEmailSent;
        public event ILogin.OnMFAEmailFailedToSendHandler OnMFAEmailFailedToSend;

        public async Task Login(string email)
        {
            if (!_validator.ValidateEmail(email))
            {
                OnMFAEmailFailedToSend?.Invoke(email, $"Invalid email: {email}");
                return;
            }
            
            try
            {
                _challengeSession = await _emailSignIn.SignIn(email);
                if (string.IsNullOrEmpty(_challengeSession))
                {
                    OnMFAEmailFailedToSend?.Invoke(email, "Unknown error establishing AWS session");
                    return;
                }

                if (_challengeSession.Contains("user not found"))
                {
                    if (retries > 1)
                    {
                        OnMFAEmailFailedToSend?.Invoke(email, _challengeSession);
                        return;
                    }
                    await SignUpThenRetryLogin(email);
                    return;
                }
                
                if (_challengeSession.StartsWith("Error"))
                {
                    OnMFAEmailFailedToSend?.Invoke(email, _challengeSession);
                    return;
                }
                
                OnMFAEmailSent?.Invoke(email);
            }
            catch (Exception e)
            {
                OnMFAEmailFailedToSend?.Invoke(email, e.Message);
            }
        }

        public async Task SignUpThenRetryLogin(string email)
        {
            try
            {
                await _emailSignIn.SignUp(email);
                retries++;
                await Login(email);
            }
            catch (Exception e)
            {
                OnMFAEmailFailedToSend?.Invoke(email, e.Message);
            }
        }

        public async Task Login(string email, string code)
        {
            if (!_validator.ValidateCode(code))
            {
                OnLoginFailed?.Invoke($"Invalid code: {code}");
                return;
            }
            
            try
            {
                string idToken = await _emailSignIn.Login(_challengeSession, email, code, _sessionWallet.GetAddress());
                if (string.IsNullOrEmpty(idToken))
                {
                    OnLoginFailed?.Invoke("Unknown error establishing AWS session");
                    return;
                }

                if (idToken.StartsWith("Error"))
                {
                    OnLoginFailed?.Invoke(idToken);
                    return;
                }
                
                await ConnectToWaaS(idToken, LoginMethod.Email);
            }
            catch (Exception e)
            {
                OnLoginFailed?.Invoke(e.Message);
            }
        }

        public void GoogleLogin()
        {
            _authenticator.GoogleSignIn();
        }

        public void DiscordLogin()
        {
            _authenticator.DiscordSignIn();
        }

        public void FacebookLogin()
        {
            _authenticator.FacebookSignIn();
        }
        
        public void AppleLogin()
        {
            _authenticator.AppleSignIn();
        }

        private void OnSocialLogin(OpenIdAuthenticationResult result)
        {
            ConnectToWaaS(result.IdToken, result.Method);
        }

        public async Task ConnectToWaaS(string idToken, LoginMethod method)
        {

            IntentSender sender = new IntentSender(
                new HttpClient(WaaSWithAuthUrl),
                _sessionWallet,
                _sessionId,
                _waasProjectId,
                _waasVersion);
            IntentDataOpenSession loginIntent = AssembleLoginIntent(idToken, _sessionWallet);

            try
            {
                IntentResponseSessionOpened registerSessionResponse = await sender.SendIntent<IntentResponseSessionOpened, IntentDataOpenSession>(loginIntent, IntentType.OpenSession);
                string sessionId = registerSessionResponse.sessionId;
                string walletAddress = registerSessionResponse.wallet;
                OnLoginSuccess?.Invoke(sessionId, walletAddress);
                WaaSWallet wallet = new WaaSWallet(new Address(walletAddress), sessionId, new IntentSender(new HttpClient(WaaSLogin.WaaSWithAuthUrl), _sessionWallet, sessionId, _waasProjectId, _waasVersion));
                WaaSWallet.OnWaaSWalletCreated?.Invoke(wallet);
                string email = Sequence.Authentication.JwtHelper.GetIdTokenJwtPayload(idToken).email;
                PlayerPrefs.SetInt(WaaSLoginMethod, (int)method);
                PlayerPrefs.SetString(OpenIdAuthenticator.LoginEmail, email);
                PlayerPrefs.Save();
            }
            catch (Exception e)
            {
                OnLoginFailed?.Invoke("Error registering waaSSession: " + e.Message);
                return;
            }
        }

        private IntentDataOpenSession AssembleLoginIntent(string idToken, Wallet.IWallet sessionWallet)
        {
            IntentDataOpenSession intent = new IntentDataOpenSession(sessionWallet.GetAddress(), null, idToken);
            return intent;
        }
    }
}