using System;
using Newtonsoft.Json;
using UnityEngine;

namespace SequenceSDK.WaaS
{
    [Serializable]
    public class SendERC721 : SequenceSDK.WaaS.Transaction
    {
        public const string TypeIdentifier = "erc721send";
        public string data { get; private set; }
        public string id { get; private set; }
        public bool safe { get; private set; }
        public string to { get; private set; }
        public string tokenAddress { get; private set; }
        public string type { get; private set; } = TypeIdentifier;
        
        public SendERC721(string tokenAddress, string to, string tokenId, bool safe = true, string data = null)
        {
            this.tokenAddress = tokenAddress;
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

        [JsonConstructor]
        public SendERC721(string data, string id, bool safe, string to, string tokenAddress, string type)
        {
            this.data = data;
            this.id = id;
            this.safe = safe;
            this.to = to;
            this.tokenAddress = tokenAddress;
            this.type = type;
        }
    }
}