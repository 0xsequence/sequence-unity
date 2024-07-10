using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Sequence.WaaS
{
    [Serializable]
    public class IntentPayload
    {
        public JObject data { get; private set; }
        public uint expiresAt { get; private set; }
        public uint issuedAt { get; private set; }
        public string name { get; private set; }
        public Signature[] signatures { get; private set; }
        public string version { get; private set; }

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
            {IntentType.FeeOptions, "feeOptions"},
            {IntentType.SessionAuthProof, "sessionAuthProof"},
            {IntentType.InitiateAuth, "initiateAuth"},
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
        FeeOptions,
        SessionAuthProof,
        InitiateAuth,
        None
    }
}