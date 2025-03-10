using System;
using Newtonsoft.Json;
using Sequence.Utils;
using UnityEngine.Scripting;

namespace Sequence.EmbeddedWallet
{
    [UnityEngine.Scripting.Preserve]
    [Serializable]
    public class IntentDataOpenSession
    {
        public string answer;
        public bool forceCreateAccount;
        public string identityType;
        public string sessionId;
        public string verifier;

        [UnityEngine.Scripting.Preserve]
        [JsonConstructor]
        public IntentDataOpenSession(string answer, bool forceCreateAccount, string identityType, string sessionId, string verifier)
        {
            this.answer = answer;
            this.forceCreateAccount = forceCreateAccount;
            this.identityType = identityType;
            this.sessionId = sessionId;
            this.verifier = verifier;
        }

        public IntentDataOpenSession(Address sessionWallet, IdentityType identityType, string verifier = "",
            string answer = "", bool forceCreateAccount = false)
        {
            this.sessionId = CreateSessionId(sessionWallet);
            this.identityType = identityType.ToString();
            this.verifier = verifier;
            this.answer = answer;
            this.forceCreateAccount = forceCreateAccount;
        }

        public IntentDataOpenSession(string sessionWalletAddress, IdentityType identityType, string verifier = "",
            string answer = "", bool forceCreateAccount = false)
        {
            this.sessionId = CreateSessionId(new Address(sessionWalletAddress));
            this.identityType = identityType.ToString();
            this.verifier = verifier;
            this.answer = answer;
            this.forceCreateAccount = forceCreateAccount;
        }

        public IntentDataOpenSession(IntentDataInitiateAuth initiateAuth, string answer)
        {
            this.sessionId = initiateAuth.sessionId;
            this.identityType = initiateAuth.identityType;
            this.verifier = initiateAuth.verifier;
            this.answer = answer;
        }

        public static string CreateSessionId(Address sessionWallet)
        {
            return $"0x00{sessionWallet.Value.ToLower().WithoutHexPrefix()}";
        }
    }
}