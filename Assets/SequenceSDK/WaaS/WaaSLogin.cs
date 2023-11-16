using System;
using System.Threading.Tasks;
using Amazon.CognitoIdentity.Model;
using Sequence.Authentication;
using Sequence.Extensions;
using Sequence.Utils;
using Sequence.Wallet;
using UnityEngine;

namespace Sequence.WaaS
{
    public class WaaSLogin : ILogin 
    {
        public static readonly string WaaSLoginUrl = "https://d14tu8valot5m0.cloudfront.net/rpc/WaasAuthenticator";

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
            
            Debug.LogError($"Credentials fetched from AWS:\n{credentials.PrettyPrint()}");

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

            string loginPayload = AssembleLoginPayloadJson(idToken, sessionWallet);
            string payloadCiphertext = await PrepareEncryptedPayload(sessionWallet, idToken, dataKey, loginPayload);
            string signedPayload = await sessionWallet.SignMessage(loginPayload);

            try
            {
                (string sessionId, string dataWallet) = await RegisterSession(dataKey.Ciphertext.ByteArrayToHexStringWithPrefix(), payloadCiphertext, signedPayload);
                Debug.LogError($"Session ID: {sessionId} | Data Wallet: {dataWallet}");
            }
            catch (Exception e)
            {
                OnLoginFailed?.Invoke("Error registering session: " + e.Message);
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

        private async Task<(string, string)> RegisterSession(string encryptedPayloadKey, string payloadCiphertext, string signedPayload) 
        {
            HttpClient client = new HttpClient(WaaSLoginUrl);
            RegisterSessionPayload payload = new RegisterSessionPayload(encryptedPayloadKey, payloadCiphertext, signedPayload);
            (string sessionId, string dataWallet) = await client.SendRequest<RegisterSessionPayload, (string, string)>("RegisterSession", payload);
            return (sessionId, dataWallet);
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
    
    [Serializable]
    public class RegisterSessionPayload
    {
        public string encryptedPayloadKey;
        public string payloadCiphertext;
        public string payloadSig;

        public RegisterSessionPayload(string encryptedPayloadKey, string payloadCiphertext, string payloadSig)
        {
            this.encryptedPayloadKey = encryptedPayloadKey;
            this.payloadCiphertext = payloadCiphertext;
            this.payloadSig = payloadSig;
        }
    }
}