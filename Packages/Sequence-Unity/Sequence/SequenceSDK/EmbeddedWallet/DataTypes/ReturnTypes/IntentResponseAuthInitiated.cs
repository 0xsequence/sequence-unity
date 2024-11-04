using System;
using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace Sequence.EmbeddedWallet
{
    [Preserve]
    public class IntentResponseAuthInitiated
    {
        public string challenge;
        public int expiresIn;
        public string identityType;
        public string sessionId;

        [Preserve]
        [JsonConstructor]
        public IntentResponseAuthInitiated(string challenge, int expiresIn, string identityType, string sessionId)
        {
            this.challenge = challenge;
            this.expiresIn = expiresIn;
            this.identityType = identityType;
            this.sessionId = sessionId;
        }

        public bool ValidateChallenge()
        {
            IdentityType type = identityType.GetIdentityType();
            switch (type)
            {
                case IdentityType.Email:
                    return !string.IsNullOrWhiteSpace(challenge);
                case IdentityType.Guest:
                    return !string.IsNullOrWhiteSpace(challenge);
                case IdentityType.PlayFab:
                    return true;
                case IdentityType.OIDC:
                    return true;
            }
            throw new Exception($"Encountered unexpected identity type: {type} with name {identityType}");
        }
    }
}