using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Nethereum.Util;
using Newtonsoft.Json;
using Sequence.EcosystemWallet.Browser;
using Sequence.EcosystemWallet.Primitives;
using Sequence.Utils;
using Sequence.Wallet;
using UnityEngine;

namespace Sequence.EcosystemWallet.Authentication
{
    public class SequenceEcosystemWalletLogin
    {
        public enum SessionType
        {
            Implicit,
            ExplicitOpen,
            ExplicitRestrictive
        }
        
        private Chain _chain;
        private string _walletUrl;
        private string _redirectUrl;
        private string _redirectId;
        private string _emitterAddress;
        private EOAWallet _sessionWallet;
        private SessionStorage _sessionStorage;
        
        public SequenceEcosystemWalletLogin(Chain chain)
        {
            _chain = chain;
            _walletUrl = "https://v3.sequence-dev.app";
            _redirectUrl = "http://localhost:8080";
            _emitterAddress = "0xb7bE532959236170064cf099e1a3395aEf228F44";
            _sessionStorage = new SessionStorage();
        }
        
        public async Task<SequenceEcosystemWallet> SignInWithEmail(string email, SessionType sessionType)
        {
            return await CreateNewSession(GetPermissionsFromSessionType(sessionType),"email", email);
        }
        
        public async Task<SequenceEcosystemWallet> SignInWithGoogle(SessionType sessionType)
        {
            return await CreateNewSession(GetPermissionsFromSessionType(sessionType),"google");
        }
        
        public async Task<SequenceEcosystemWallet> SignInWithApple(SessionType sessionType)
        {
            return await CreateNewSession(GetPermissionsFromSessionType(sessionType),"apple");
        }

        public SequenceEcosystemWallet RecoverSessionFromStorage()
        {
            var walletAddress = _sessionStorage.GetWalletAddress();
            var sessions = _sessionStorage.GetSessions();

            if (string.IsNullOrEmpty(walletAddress) || sessions.Length == 0)
                throw new Exception("No session found in storage.");

            return new SequenceEcosystemWallet(new Address(walletAddress));
        }

        public void SignOut()
        {
            _sessionStorage.Clear();
        }
        
        /// <summary>
        /// Create an implicit- or explicit session based on a given set of permissions.
        /// </summary>
        /// <param name="permissions">Leave it null to create an implicit session. Otherwise, we create an explicit session.</param>
        /// <param name="preferredLoginMethod"></param>
        /// <param name="email"></param>
        private async Task<SequenceEcosystemWallet> CreateNewSession(SessionPermissions permissions, string preferredLoginMethod, string email = null)
        {
            _sessionWallet = new EOAWallet();
            
            var isImplicitSession = permissions == null;
            var payload = new Dictionary<string, object>();
            payload.Add("sessionAddress", _sessionWallet.GetAddress());
            payload.Add("preferredLoginMethod", preferredLoginMethod);
            payload.Add("email", email);
            
            if (isImplicitSession)
                payload.Add("implicitSessionRedirectUrl", _redirectUrl);
            
            if (!isImplicitSession)
                payload.Add("permissions", permissions.ToJson());
            
            var action = isImplicitSession ? "addImplicitSession" : "addExplicitSession";
            var url = ConstructWalletUrl(action, payload, "/request/connect");

            BrowserFactory.CreateBrowser().Show(url);
            
            var editorServer = new EditorServer();
            var response = await editorServer.WaitForResponse("localhost", 8080);
            if (!response.Result)
            {
                throw new Exception("Error during request");
            }
            
            var id = response.QueryString["id"];
            if (id != _redirectId)
            {
                throw new Exception("Incorrect request id");
            }

            if (!response.QueryString.AllKeys.Contains("payload"))
            {
                var errorJson = Encoding.UTF8.GetString(Convert.FromBase64String(response.QueryString["error"]));
                var error = JsonConvert.DeserializeObject<Dictionary<string, string>>(errorJson)["error"];
                
                Debug.LogError($"Error from wallet app: {error}");
                throw new Exception(error);
            }
            
            var encodedResponsePayload = response.QueryString["payload"];
            var responsePayloadJson = Encoding.UTF8.GetString(Convert.FromBase64String(encodedResponsePayload));
            var responsePayload = JsonConvert.DeserializeObject<AuthResponse>(responsePayloadJson);
            
            if (responsePayload.attestation != null)
                Debug.Log($"Attestation approvedSigner: {responsePayload.attestation.approvedSigner}");
            
            if (responsePayload.signature != null)
                Debug.Log($"Signature: {responsePayload.signature}");

            var walletAddress = responsePayload.walletAddress;
            _sessionStorage.StoreWalletAddress(walletAddress);
            _sessionStorage.AddSession(new SessionData(
                _sessionWallet.GetPrivateKeyAsHex(), 
                walletAddress, 
                responsePayload.attestation, 
                responsePayload.signature,
                ChainDictionaries.ChainIdOf[_chain],
                responsePayload.loginMethod,
                responsePayload.email));
            
            return new SequenceEcosystemWallet(walletAddress);
        }
        
        private string ConstructWalletUrl(string action, Dictionary<string, object> payload, string path)
        {
            _redirectId = $"sequence:{Guid.NewGuid().ToString()}";
            var encodedPayload = Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(payload)));
                
            var uriBuilder = new UriBuilder($"{_walletUrl}{path}");
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);

            query["action"] = action;
            query["payload"] = encodedPayload;
            query["id"] = _redirectId;
            query["redirectUrl"] = _redirectUrl;
            query["mode"] = "redirect";

            uriBuilder.Query = query.ToString();
            return uriBuilder.ToString();
        }

        private SessionPermissions GetPermissionsFromSessionType(SessionType sessionType)
        {
            return sessionType switch
            {
                SessionType.Implicit => null,
                SessionType.ExplicitOpen => GetOpenPermissions(),
                SessionType.ExplicitRestrictive => GetRestrictivePermissions(),
                _ => throw new Exception("Unsupported session type")
            };
        }

        private SessionPermissions GetOpenPermissions()
        {
            return new SessionPermissions
            {
                chainId = new BigInteger((int)_chain),
                signer = new Address(_emitterAddress),
                valueLimit = new BigInteger(0),
                deadline = new BigInteger(DateTime.UtcNow.ToUnixTimestamp() * 1000 + 1000 * 60 * 5000),
                permissions = new []
                {
                    new Permission
                    {
                        target = new Address("0x8F6066bA491b019bAc33407255f3bc5cC684A5a4"),
                        rules = Array.Empty<ParameterRule>()
                    }
                }
            };
        }

        private SessionPermissions GetRestrictivePermissions()
        {
            return new SessionPermissions
            {
                chainId = new BigInteger((int)_chain),
                signer = new Address(_emitterAddress),
                valueLimit = new BigInteger(0),
                deadline = new BigInteger(DateTime.UtcNow.ToUnixTimestamp() * 1000 + 1000 * 60 * 5000),
                permissions = new []
                {
                    new Permission
                    {
                        target = new Address("0x8F6066bA491b019bAc33407255f3bc5cC684A5a4"),
                        rules = new []
                        {
                            new ParameterRule
                            {
                                cumulative = false,
                                operation = ParameterOperation.equal,
                                value = HashFunctionSelector("explicitEmit()").HexStringToByteArray().PadRight(32),
                                offset = new BigInteger(0),
                                mask = ParameterRule.SelectorMask
                            },
                            new ParameterRule
                            {
                                cumulative = true,
                                operation = ParameterOperation.greaterThanOrEqual,
                                value = "0x1234567890123456789012345678901234567890".HexStringToByteArray().PadRight(32),
                                offset = new BigInteger(4),
                                mask = ParameterRule.Uint256Mask
                            }
                        }
                    }
                }
            };
        }

        private string HashFunctionSelector(string function)
        {
            var sha3 = new Sha3Keccack();
            var hash = sha3.CalculateHash(function);
            return "0x" + hash.Substring(0, 8);
        }
    }
}