using System;

namespace Sequence.Integrations.Sardine
{
    [Serializable]
    public class SardineTokenResponse
    {
        public string token;

        public SardineTokenResponse(string token)
        {
            this.token = token;
        }
    }
}