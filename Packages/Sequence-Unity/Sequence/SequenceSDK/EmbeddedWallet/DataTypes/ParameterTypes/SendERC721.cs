using System;
using Newtonsoft.Json;

namespace Sequence.EmbeddedWallet
{
    [Preserve]
    [Serializable]
    public class SendERC721 : Transaction
    {
        public const string TypeIdentifier = "erc721send";
        public string data;
        public string id;
        public bool safe;
        public string to;
        public string tokenAddress;
        public string type = TypeIdentifier;
        
        public SendERC721(string tokenAddress, string to, string tokenId, bool safe = true, string data = null)
        {
            this.tokenAddress = tokenAddress;
            this.to = to;
            this.id = tokenId;
            this.safe = safe;
            if (!safe && data != null)
            {
                LogHandler.Error($"Error creating {GetType().Name}: {nameof(data)} can only be set when {nameof(safe)} is true.\nUsing null data.");
                data = null;
            }
            this.data = data;
        }

        [Preserve]
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