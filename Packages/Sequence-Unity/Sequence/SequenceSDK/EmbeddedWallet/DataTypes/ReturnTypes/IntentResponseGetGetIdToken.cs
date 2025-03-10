using System;
using UnityEngine.Scripting;

namespace Sequence.EmbeddedWallet
{
    [UnityEngine.Scripting.Preserve]
    [Serializable]
    public class IntentResponseGetIdToken
    {
        public string IdToken;
        public int ExpiresIn;

        [UnityEngine.Scripting.Preserve]
        public IntentResponseGetIdToken(string idToken, int expiresIn)
        {
            this.IdToken = idToken;
            this.ExpiresIn = expiresIn;
        }
    }
}