using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

namespace Sequence.Marketplace
{
    [Serializable]
    public class TokenMetadata
    {
        public string tokenId;
        public string name;
        public string description;
        public string image;
        public string video;
        public string audio;
        public Dictionary<string, object> properties;
        public Dictionary<string, object>[] attributes;
        public string image_data;
        public string external_url;
        public string background_color;
        public string animation_url;
        public uint decimals;
        public string updatedAt;
        public Asset[] assets;

        [Preserve]
        public TokenMetadata(string tokenId, string name, Dictionary<string, object> properties = null, Dictionary<string, object>[] attributes = null, string description = null, string image = null, string video = null, string audio = null, string imageData = null, string externalURL = null, string backgroundColor = null, string animationURL = null, uint decimals = default, string updatedAt = null, Asset[] assets = null)
        {
            this.tokenId = tokenId;
            this.name = name;
            this.description = description;
            this.image = image;
            this.video = video;
            this.audio = audio;
            this.properties = properties;
            this.attributes = attributes;
            image_data = imageData;
            external_url = externalURL;
            background_color = backgroundColor;
            animation_url = animationURL;
            this.decimals = decimals;
            this.updatedAt = updatedAt;
            this.assets = assets;
        }
    }
}