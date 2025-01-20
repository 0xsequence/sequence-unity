using System;
using UnityEngine.Scripting;

namespace Sequence.Integrations.Sardine
{
    [Serializable]
    internal class GetOrderStatusRequest
    {
        public string orderId;

        [Preserve]
        public GetOrderStatusRequest(string orderId)
        {
            this.orderId = orderId;
        }
    }
}