using System;
using Newtonsoft.Json;
using Sequence.Utils;
using Sequence.Utils.SecureStorage;
using UnityEngine;

namespace Sequence.EcosystemWallet
{
    internal static class SessionStorage
    {
        private static readonly string WalletAddressKey = $"sequence-wallet-address-{Application.companyName}-{Application.productName}";
        private static readonly string SessionsKey = $"sequence-sessions-{Application.companyName}-{Application.productName}";
        
        private static readonly ISecureStorage Storage = SecureStorageFactory.CreateSecureStorage();

        public static void Clear()
        {
            Storage.StoreString(WalletAddressKey, string.Empty);
            Storage.StoreString(SessionsKey, string.Empty);
        }

        public static void AddSession(SessionCredentials session)
        {
            var sessions = GetSessions();
            var newSessions = sessions.AddToArray(session);
            StoreSessions(newSessions);
        }
        
        public static SessionCredentials[] GetSessions()
        {
            var json = Storage.RetrieveString(SessionsKey);
            return string.IsNullOrEmpty(json) ? 
                Array.Empty<SessionCredentials>() : 
                JsonConvert.DeserializeObject<SessionCredentials[]>(json);
        }
        
        private static void StoreSessions(SessionCredentials[] sessions)
        {
            var json = JsonConvert.SerializeObject(sessions);
            Storage.StoreString(SessionsKey, json);
        }
    }
}