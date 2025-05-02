using System;
using UnityEngine.Scripting;

namespace Sequence.Pay.Sardine
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