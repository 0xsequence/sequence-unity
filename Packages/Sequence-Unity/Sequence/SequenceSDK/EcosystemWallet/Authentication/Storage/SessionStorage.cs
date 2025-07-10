using System;
using System.Linq;
using Newtonsoft.Json;
using Sequence.Utils;
using Sequence.Utils.SecureStorage;
using UnityEngine;

namespace Sequence.EcosystemWallet.Authentication
{
    internal class SessionStorage
    {
        private static readonly string WalletAddressKey = $"sequence-wallet-address-{Application.companyName}-{Application.productName}";
        private static readonly string SessionsKey = $"sequence-sessions-{Application.companyName}-{Application.productName}";
        
        private readonly ISecureStorage _storage = SecureStorageFactory.CreateSecureStorage();

        public void Clear()
        {
            _storage.StoreString(WalletAddressKey, string.Empty);
            _storage.StoreString(SessionsKey, string.Empty);
        }

        public void StoreWalletAddress(string walletAddress)
        {
            _storage.StoreString(WalletAddressKey, walletAddress);
        }

        public string GetWalletAddress()
        {
            return _storage.RetrieveString(WalletAddressKey);
        }

        public void AddSession(SessionData session)
        {
            var sessions = GetSessions();
            var newSessions = sessions.AddToArray(session);
            StoreSessions(newSessions);
        }
        
        public void StoreSessions(SessionData[] sessions)
        {
            var json = JsonConvert.SerializeObject(sessions);
            _storage.StoreString(SessionsKey, json);
        }
        
        public SessionData[] GetSessions()
        {
            var json = _storage.RetrieveString(SessionsKey);
            return string.IsNullOrEmpty(json) ? 
                Array.Empty<SessionData>() : 
                JsonConvert.DeserializeObject<SessionData[]>(json);
        }
    }
}