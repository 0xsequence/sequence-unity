using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SequenceSDK.WaaS
{
    [Serializable]
    public class IntentPayload
    {
        public JObject data;
        public uint expiresAt;
        public uint issuedAt;
        public string name;
        public Signature[] signatures;
        public string version;

        [JsonConstructor]
        public IntentPayload(string version, string name, uint expiresAt, uint issuedAt, JObject data, Signature[] signatures)
        {
            this.version = version;
            this.name = name;
            this.expiresAt = expiresAt;
            this.issuedAt = issuedAt;
            this.data = data;
            this.signatures = signatures;
        }
        
        public IntentPayload(string version, IntentType name, uint expiresAt, uint issuedAt, JObject data, Signature[] signatures)
        {
            this.version = version;
            this.name = IntentNames[name];
            this.expiresAt = expiresAt;
            this.issuedAt = issuedAt;
            this.data = data;
            this.signatures = signatures;
        }

        public IntentPayload(string version, IntentType type, JObject data, Signature[] signatures, uint timeBeforeExpiryInSeconds = 30)
        {
            this.version = version;
            this.name = IntentNames[type];
            this.data = data;
            this.signatures = signatures;
            this.issuedAt = (uint)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            this.expiresAt = this.issuedAt + timeBeforeExpiryInSeconds;
        }

        private static readonly Dictionary<IntentType, string> IntentNames = new Dictionary<IntentType, string>
        {
            {IntentType.OpenSession, "openSession"},
            {IntentType.CloseSession, "closeSession"},
            {IntentType.ValidateSession, "validateSession"},
            {IntentType.FinishValidateSession, "finishValidateSession"},
            {IntentType.ListSessions, "listSessions"},
            {IntentType.GetSession, "getSession"},
            {IntentType.SignMessage, "signMessage"},
            {IntentType.SendTransaction, "sendTransaction"},
            {IntentType.GetTransactionReceipt, "getTransactionReceipt"},
        };
    }

    public enum IntentType
    {
        OpenSession,
        CloseSession,
        ValidateSession,
        FinishValidateSession,
        ListSessions,
        GetSession,
        SignMessage,
        SendTransaction,
        GetTransactionReceipt,
        None
    }
}