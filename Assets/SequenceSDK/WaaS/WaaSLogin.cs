using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.CognitoIdentity.Model;
using Sequence.Authentication;
using Sequence.Config;
using Sequence.Extensions;
using Sequence.Utils;
using Sequence.WaaS.Authentication;
using Sequence.Wallet;
using UnityEngine;
using UnityEngine.Serialization;

namespace Sequence.WaaS
{
    public class WaaSLogin : ILogin 
    {
        public static string WaaSWithAuthUrl { get; private set; }
        public const string WaaSLoginMethod = "WaaSLoginMethod";

        private AWSConfig _awsConfig;
        private int _waasProjectId;
        private string _waasVersion;
        private OpenIdAuthenticator _authenticator;
        private IValidator _validator;
        private string _challengeSession;

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
            AWSConfig awsConfig = new AWSConfig(configJwt.idpRegion, configJwt.identityPoolId, configJwt.keyId, configJwt.emailClientId);
            _awsConfig = awsConfig;

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
            
            _authenticator = new OpenIdAuthenticator();
            _authenticator.PlatformSpecificSetup();
            Application.deepLinkActivated += _authenticator.HandleDeepLink;
            _authenticator.SignedIn += OnSocialLogin;

            if (validator == null)
            {
                validator = new Validator();
            }
            _validator = validator;
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
            
            AWSEmailSignIn signIn = new AWSEmailSignIn(_awsConfig.IdentityPoolId, _awsConfig.Region, _awsConfig.CognitoClientId);
            try
            {
                _challengeSession = await signIn.SignIn(email);
                if (string.IsNullOrEmpty(_challengeSession))
                {
                    OnMFAEmailFailedToSend?.Invoke(email, "Unknown error establishing AWS session");
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

        public async Task Login(string email, string code)
        {
            if (!_validator.ValidateCode(code))
            {
                OnLoginFailed?.Invoke($"Invalid code: {code}");
                return;
            }
            
            AWSEmailSignIn signIn = new AWSEmailSignIn(_awsConfig.IdentityPoolId, _awsConfig.Region, _awsConfig.CognitoClientId);
            try
            {
                string idToken = await signIn.Login(_challengeSession, email, code);
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
                
                ConnectToWaaS(idToken, LoginMethod.Email);
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
            Credentials credentials;
            DataKey dataKey;

            try
            {
                AWSCredentialsFetcher fetcher = new AWSCredentialsFetcher(idToken, _awsConfig.IdentityPoolId, _awsConfig.Region);
                credentials = await fetcher.GetCredentials();
            }
            catch (Exception e)
            {
                OnLoginFailed?.Invoke("Error fetching credentials from AWS: " + e.Message);
                return;
            }

            try
            {
                AWSDataKeyGenerator generator =
                    new AWSDataKeyGenerator(credentials, _awsConfig.Region, _awsConfig.KMSEncryptionKeyId);
                dataKey = await generator.GenerateDataKey();
            }
            catch (Exception e)
            {
                OnLoginFailed?.Invoke("Error generating data key from AWS: " + e.Message);
                return;
            }

            EthWallet sessionWallet = new EthWallet();

            IntentSender sender = new IntentSender(
                new HttpClient(WaaSWithAuthUrl),
                dataKey,
                sessionWallet,
                "Unknown",
                _waasProjectId,
                _waasVersion);
            string loginPayload = AssembleLoginPayloadJson(idToken, sessionWallet);

            try
            {
                RegisterSessionResponse registerSessionResponse = await sender.PostIntent<RegisterSessionResponse>(loginPayload, "RegisterSession");
                string sessionId = registerSessionResponse.session.id;
                string walletAddress = registerSessionResponse.data.wallet;
                OnLoginSuccess?.Invoke(sessionId, walletAddress);
                WaaSWallet wallet = new WaaSWallet(new Address(walletAddress), sessionId, new IntentSender(new HttpClient(WaaSLogin.WaaSWithAuthUrl), dataKey, sessionWallet, sessionId, _waasProjectId, _waasVersion));
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

        private string AssembleLoginPayloadJson(string idToken, Wallet.IWallet sessionWallet)
        {
            WaaSLoginIntent intent = new WaaSLoginIntent(_waasVersion, WaaSLoginIntent.Packet.OpenSessionCode,
                sessionWallet.GetAddress(), idToken);
            string intentJson = JsonUtility.ToJson(intent);
            WaaSLoginPayload payload = new WaaSLoginPayload(_waasProjectId, idToken, sessionWallet.GetAddress(),
                "FRIENDLY SESSION WALLET", intentJson);
            string payloadJson = JsonUtility.ToJson(payload);
            return payloadJson;
        }
    }
}