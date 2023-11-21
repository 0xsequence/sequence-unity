using System;
using UnityEngine;

namespace SequenceSDK.WaaS
{
    [Serializable]
    public class SendERC721 : SequenceSDK.WaaS.Transaction
    {
        public string data { get; private set; }
        public string id { get; private set; }
        public bool safe { get; private set; }
        public string to { get; private set; }
        public string token { get; private set; }
        public string type { get; private set; } = "erc721send";
        
        public SendERC721(string tokenAddress, string to, string tokenId, bool safe = true, string data = null)
        {
            this.token = tokenAddress;
            this.to = to;
            this.id = tokenId;
            this.safe = safe;
            if (!safe && data != null)
            {
                Debug.LogError($"Error creating {GetType().Name}: {nameof(data)} can only be set when {nameof(safe)} is true.\nUsing null data.");
                data = null;
            }
            this.data = data;
        }
    }
}