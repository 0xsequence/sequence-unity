using System;
using UnityEngine.Scripting;

namespace Sequence.Pay.Sardine
{
    [Serializable]
    internal class SardineOrderResponse
    {
        public SardineOrder resp;

        [Preserve]
        public SardineOrderResponse(SardineOrder resp)
        {
            this.resp = resp;
        }
    }
}