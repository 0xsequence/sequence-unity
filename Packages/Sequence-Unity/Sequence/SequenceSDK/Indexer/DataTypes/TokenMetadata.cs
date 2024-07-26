namespace Sequence
{
    using System.Collections.Generic;
    using System.Numerics;

    [System.Serializable]
    public class TokenMetadata
    {
        public BigInteger tokenId;
        public string contractAddress;
        public string name;
        public string description;
        public string image;
        public float decimals;
        public Dictionary<string, object> properties;
        public string video;
        public string audio;
        public string image_data;
        public string external_url;
        public string background_color;
        public string animation_url;
        public Dictionary<string, object>[] attributes;
    }
}