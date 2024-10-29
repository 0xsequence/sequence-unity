using System;

namespace Sequence.EmbeddedWallet
{
    [Serializable]
    public class IntentResponseGetIdToken
    {
        public string IdToken;
        public int ExpiresIn;

        public IntentResponseGetIdToken(string idToken, int expiresIn)
        {
            this.IdToken = idToken;
            this.ExpiresIn = expiresIn;
        }
    }
}