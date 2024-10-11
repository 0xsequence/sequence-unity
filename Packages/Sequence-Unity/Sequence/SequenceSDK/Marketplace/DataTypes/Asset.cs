using System;
using System.Numerics;

namespace Sequence.Marketplace
{
    [Serializable]
    public class Asset
    {
        public BigInteger id;
        public BigInteger collectionId;
        public string tokenId;
        public string url;
        public string metadataField;
        public string name;
        public BigInteger filesize;
        public string mimeType;
        public BigInteger width;
        public BigInteger height;
        public string updatedAt;

        public Asset(BigInteger id, BigInteger collectionId, string tokenId, string metadataField, string url = null, string name = null, BigInteger filesize = default, string mimeType = null, BigInteger width = default, BigInteger height = default, string updatedAt = null)
        {
            this.id = id;
            this.collectionId = collectionId;
            this.tokenId = tokenId;
            this.url = url;
            this.metadataField = metadataField;
            this.name = name;
            this.filesize = filesize;
            this.mimeType = mimeType;
            this.width = width;
            this.height = height;
            this.updatedAt = updatedAt;
        }
    }
}