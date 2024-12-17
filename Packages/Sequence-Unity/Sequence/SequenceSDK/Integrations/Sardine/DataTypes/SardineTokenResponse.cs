using System;
using UnityEngine.Scripting;

namespace Sequence.Integrations.Sardine
{
    [Serializable]
    public class SardineTokenResponse
    {
        public string token;

        [Preserve]
        public SardineTokenResponse(string token)
        {
            this.token = token;
        }
    }
}