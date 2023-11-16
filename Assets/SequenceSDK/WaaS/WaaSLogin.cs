using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.CognitoIdentity.Model;
using Sequence.Authentication;
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
        public static readonly string WaaSLoginUrl = "https://d14tu8valot5m0.cloudfront.net/rpc/WaasAuthenticator";

        private AWSConfig _awsConfig;
        private int _waasProjectId;
        private string _waasVersion;
        private OpenIdAuthenticator _authenticator;

        public WaaSLogin(AWSConfig awsConfig, int waasProjectId, string waasVersion)
        {
            _awsConfig = awsConfig;
            _waasProjectId = waasProjectId;
            _waasVersion = waasVersion;
            _authenticator = new OpenIdAuthenticator();
            _authenticator.PlatformSpecificSetup();
            Application.deepLinkActivated += _authenticator.HandleDeepLink;
            _authenticator.SignedIn += OnSocialLogin;
        }
        public event ILogin.OnLoginSuccessHandler OnLoginSuccess;
        public event ILogin.OnLoginFailedHandler OnLoginFailed;
        public event ILogin.OnMFAEmailSentHandler OnMFAEmailSent;
        public event ILogin.OnMFAEmailFailedToSendHandler OnMFAEmailFailedToSend;
        public async Task Login(string email)
        {
            Debug.LogError("Not Implemented... mocking for now");
            await new MockLogin().Login(email);
        }

        public async Task Login(string email, string code)
        {
            Debug.LogError("Not Implemented... mocking for now");
            await new MockLogin().Login(email, code);
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
            ConnectToWaaS(result.IdToken);
        }

        public async Task ConnectToWaaS(string idToken)
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

            string loginPayload = AssembleLoginPayloadJson(idToken, sessionWallet);
            string payloadCiphertext = await PrepareEncryptedPayload(sessionWallet, idToken, dataKey, loginPayload);
            string signedPayload = await sessionWallet.SignMessage(loginPayload);

            try
            {
                WaaSSessionData sessionData = await RegisterSession(dataKey.Ciphertext.ByteArrayToHexStringWithPrefix(), payloadCiphertext, signedPayload);
                Debug.LogError($"WaaSSession ID: {sessionData.sessionId} | Wallet: {sessionData.wallet}");
            }
            catch (Exception e)
            {
                OnLoginFailed?.Invoke("Error registering waaSSession: " + e.Message);
                return;
            }
        }

        private async Task<string> PrepareEncryptedPayload(Wallet.IWallet sessionWallet, string idToken, DataKey dataKey, string loginPayload)
        {
            byte[] encryptedPayload = Encryptor.AES256CBCEncryption(dataKey.Plaintext, loginPayload);
            return encryptedPayload.ByteArrayToHexStringWithPrefix();
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

        private async Task<WaaSSessionData> RegisterSession(string encryptedPayloadKey, string payloadCiphertext, string signedPayload) 
        {
            HttpClient client = new HttpClient(WaaSLoginUrl);
            RegisterSessionPayload payload = new RegisterSessionPayload(encryptedPayloadKey, payloadCiphertext, signedPayload);
            RegisterSessionResponse response = await client.SendRequest<RegisterSessionPayload, RegisterSessionResponse>("RegisterSession", payload, new Dictionary<string, string>()
            {
                {"X-Sequence-Tenant", "9"},
            });
            return response.data;
        }
    }
}