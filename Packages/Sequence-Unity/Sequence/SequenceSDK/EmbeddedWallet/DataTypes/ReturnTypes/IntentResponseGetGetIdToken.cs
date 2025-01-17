using System;
using UnityEngine.Scripting;

namespace Sequence.EmbeddedWallet
{
    [Preserve]
    [Serializable]
    public class IntentResponseGetIdToken
    {
        public string IdToken;
        public int ExpiresIn;

        [Preserve]
        public IntentResponseGetIdToken(string idToken, int expiresIn)
        {
            this.IdToken = idToken;
            this.ExpiresIn = expiresIn;
        }
    }
}