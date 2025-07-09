using System;
using System.Net.Http;
using System.Numerics;
using System.Text;
using System.Web;
using Nethereum.Util;
using Newtonsoft.Json;
using Sequence.EcosystemWallet.Primitives;
using Sequence.Utils;
using Sequence.Wallet;
using UnityEngine;

namespace Sequence.EcosystemWallet.Authentication
{
    public class SequenceEcosystemWalletLogin
    {
        private Chain _chain;
        private string _walletUrl;
        private string _redirectUrl;
        private string _redirectId;
        private string _emitterAddress;
        private EOAWallet _sessionWallet;
        
        public SequenceEcosystemWalletLogin(Chain chain)
        {
            _chain = chain;
            _walletUrl = "https://v3.sequence-dev.app";
            _redirectUrl = "http://localhost:8080";
            _emitterAddress = "0xb7bE532959236170064cf099e1a3395aEf228F44";
        }
        
        public void SignInWithEmail(string email)
        {
            CreateNewSession(GetOpenPermissions(),"email", email);
        }
        
        /// <summary>
        /// Create an implicit- or explicit session based on a given set of permissions.
        /// </summary>
        /// <param name="permissions">Leave it null to create an implicit session. Otherwise, we create an explicit session.</param>
        /// <param name="preferredLoginMethod"></param>
        /// <param name="email"></param>
        private void CreateNewSession(SessionPermissions permissions, string preferredLoginMethod, string email)
        {
            Debug.Log($"{JsonConvert.SerializeObject(new {test = BigInteger.Parse("123455551234555512345555123455551234555512345555")})}");
            
            _sessionWallet = new EOAWallet();
            var payload = new AuthPayload
            {
                sessionAddress = _sessionWallet.GetAddress(),
                permissions = permissions.ToJson(),
                preferredLoginMethod = preferredLoginMethod,
                email = email,
            };

            var url = ConstructWalletUrl("addExplicitSession", payload, "/request/connect");
            
            var editorServer = new EditorServer();
            editorServer.StartServer("localhost", 8080);
            
            Application.OpenURL(url);
        }

        private string ConstructWalletUrl(string action, AuthPayload payload, string path)
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
                                value = HashFunctionSelector("explicitEmit()").HexStringToByteArray(32),
                                offset = new BigInteger(0),
                                mask = ParameterRule.SelectorMask
                            },
                            new ParameterRule
                            {
                                cumulative = true,
                                operation = ParameterOperation.greaterThanOrEqual,
                                value = "0x1234567890123456789012345678901234567890".HexStringToByteArray(32),
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