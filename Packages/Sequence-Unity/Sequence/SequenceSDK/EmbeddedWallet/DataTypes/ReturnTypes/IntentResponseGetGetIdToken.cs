using System;

namespace Sequence.EmbeddedWallet
{
    [Serializable]
    public class IntentResponseGetIdToken
    {
        public string IdToken { get; private set; }
        public int ExpiresIn { get; private set; }

        public IntentResponseGetIdToken(string idToken, int expiresIn)
        {
            this.IdToken = idToken;
            this.ExpiresIn = expiresIn;
        }
    }
}