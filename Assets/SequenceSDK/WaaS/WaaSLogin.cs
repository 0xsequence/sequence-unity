using System;
using System.Threading.Tasks;
using Amazon.CognitoIdentity.Model;
using Sequence.Authentication;
using Sequence.Wallet;
using UnityEngine;

namespace Sequence.WaaS
{
    public class WaaSLogin : ILogin 
    {
        public static readonly string WaaSLoginUrl = "https://d14tu8valot5m0.cloudfront.net/rpc/WaaSAuthenticator/";

        private AWSConfig _awsConfig;
        private string _waasProjectId;
        private string _waasVersion;
        private OpenIdAuthenticator _authenticator;

        public WaaSLogin(AWSConfig awsConfig, string waasProjectId, string waasVersion)
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
            Debug.LogError("Google login");
            _authenticator.GoogleSignIn();
        }

        private void OnSocialLogin(OpenIdAuthenticationResult result)
        {
            Debug.LogError("Google Id token: " + result.IdToken);
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
            
            Debug.LogError("Credentials fetched from AWS");

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
            
            Debug.LogError("Data key generated from AWS");

            EthWallet sessionWallet = new EthWallet();

            string signedPayled = await PrepareSignedPayload(sessionWallet, idToken, dataKey);

            // Todo register session with waas rpc
            Debug.LogError("Signed payload: " + signedPayled);
        }

        private async Task<string> PrepareSignedPayload(Wallet.IWallet sessionWallet, string idToken, DataKey dataKey)
        {
            string loginPayload = AssembleLoginPayloadJson(idToken, sessionWallet);
            byte[] encryptedPayload = Encryptor.AES256CBCEncryption(dataKey.Plaintext, loginPayload);
            string signedPayled = await sessionWallet.SignMessage(encryptedPayload);
            return signedPayled;
        }

        private string AssembleLoginPayloadJson(string idToken, Wallet.IWallet sessionWallet)
        {
            WaaSLoginIntent intent = new WaaSLoginIntent(_waasVersion, WaaSLoginIntent.Packet.OpenSessionCode,
                sessionWallet.GetAddress(), idToken);
            string intentJson = JsonUtility.ToJson(intent);
            WaaSLoginPayload payload = new WaaSLoginPayload(_waasProjectId, idToken, sessionWallet.GetAddress(),
                "UserWallet", intentJson);
            string payloadJson = JsonUtility.ToJson(payload);
            return payloadJson;
        }
    }

    [Serializable]
    public class WaaSLoginIntent
    {
        public string version;
        public Packet packet;

        [Serializable]
        public class Packet
        {
            public static string OpenSessionCode = "openSession";
            
            public string code;
            public string session;
            public Proof proof;

            [Serializable]
            public class Proof
            {
                public string idToken;
            }
        }

        public WaaSLoginIntent(string version, string code, string session, string idToken)
        {
            this.version = version;
            this.packet = new Packet()
            {
                code = code,
                session = session,
                proof = new Packet.Proof()
                {
                    idToken = idToken
                }
            };
        }
    }

    [Serializable]
    public class WaaSLoginPayload
    {
        public string projectId;
        public string idToken;
        public string sessionAddress;
        public string friendlyName;
        public string intentJSON;

        public WaaSLoginPayload(string projectId, string idToken, string sessionAddress, string friendlyName, string intentJson)
        {
            this.projectId = projectId;
            this.idToken = idToken;
            this.sessionAddress = sessionAddress;
            this.friendlyName = friendlyName;
            intentJSON = intentJson;
        }
    }
}